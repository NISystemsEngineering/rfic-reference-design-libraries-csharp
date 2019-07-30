## Contribution Guidelines

Please review the [setup guidelines](SETUP.md) for more information on the proper workflow to follow for setting up your repository, syncing changes, and submitting changes back to the public repository.



+ [Creating New Instrument Classes](#creating-new-instrument-classes)
    - [Project and File Setup](#project-and-file-setup)
    - [Versioning](#versioning)
+ [Module Design](#module-design)
    - [Structure](#structure)
    - [Data Types](#data-types)
    - [Exception Handling](#exception-handling)
+ [Building and Preparing for Distribution](#building-and-preparing-for-distribution)

----------

### Creating New Instrument Classes

#### Project and File Setup

- Each instrument should be defined as its own class with a basic name *(i.e. Scope, SG, Supply, etc.)*
- Each instrument class has its own project which is added to the **Reference Design Libraries** solution file
- Each class should exist in the `NationalInstruments.ReferenceDesignLibraries` namespace
  - For personalties *(i.e. RFmx WLAN, RFmx LTE)* or specific implementations of protocols *(i.e. RFFE MIPI for digital communication)* the namespace should be a subset of the parent instrument. For example, the full namespace for the RFmx WLAN class is `NationalInstruments.ReferenceDesignLibraries.SA.WLAN`, where `SA` is the parent namespace

#### Versioning

All projects share the **SolutionInfo.cs** file located at the root **Source** directory. This file defines the copyright, trademark, and *AssemblyVersion* parameter for all assemblies. The intention is that when assemblies are built from the solution file, all binaries would share the same *AssemblyVersion* to indicate what build they are from. The *AssemblyFileVersion*, however, is unique to each project. This way, semantic versioning can be properly applied to a single module while not requiring the same for the entire group of assemblies. For example, if a backwards-compatibility breaking change is made in **Module X**, then the version for that assembly should be increased from 1.x to 2.x. However, it would be inconvenient to upgrade the version numbers of the entire package in this way, and so the *AssemblyFileVersion* is allowed to be controlled per module. This

To setup your project to properly reference the shared **SolutionInfo** file, perform the following steps:
1) Add the [**SolutionInfo** file](/Source/SolutionInfo.cs) as a [linked file to the project](https://theburningmonk.com/2010/03/net-tips-use-a-shared-assemblyinfo-cs-for-your-solution/)
2) Remove the parameters from the project's **AssemblyInfo** file that are to be defined instead in the **SolutionInfo** file

Each time that a push is made to the master branch of the trunk, the **AssemblyVersion** should be incremented to reflect the change. This version will be used by the automated build and delivery routines to create versioned releases.

### Module Design

All classes should be implemented as [static classes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members) for simplicity of implementation in automation and parity with the LabVIEW implementation

#### Structure

- The class should have the following sections, defined in the code via the `#region` tag
  - Type Definitions
  - Configuration
  - Results
- The class can also implement an optional utilities class for functions that provide supporting functionality. For example, the Digital library includes the following function in the utilities class: `public static ProjectFiles SearchForProjectFiles(string searchDirectory, bool recursiveSearch)`. This does not act on the instrument or apply any settings, but does provide a helpful interface for using the built-in file functions to locate appropriate NI-Digital files.

#### Data Types

- Structs are utilized to pass configuration parameters to the various functions.
  - For each configuration struct, a static method returning that struct should be created that returns default values. It should be given the name *GetDefault()*. These default values should be set in such a way that they are reasonable starting configurations for a majority of applications, and should not be set for specific hardware.
- See the following example from the [Digital](Source/Digital/Digital.cs) library:

```c#
public struct TriggerConfiguration
{
    public TriggerType BurstTriggerType;
    public DigitalEdge DigitalEdgeType;
    public string DigitalEdgeSource;

    public static TriggerConfiguration GetDefault()
    {
        return new TriggerConfiguration
        {
            BurstTriggerType = TriggerType.StartTrigger,
            DigitalEdgeType = DigitalEdge.Rising,
            DigitalEdgeSource = "PXI_Trig0",
        };
    }
}
```

- A new instance of the TriggerConfiguration struct with its default values set can be created with the following:

```c#
TriggerConfiguration digiTrigConfig = TriggerConfiguration.GetDefault();
```

- Structs should also be used for passing function results back to a function if more than one parameter is returned
- The structs should only contain configuration information; instrument handles should be passed separately

#### Exception Handling

- Generally, it is acceptable to allow the underlying driver to throw exceptions that will be clearly understood by the developer
  - For example, it is not necessary to check for a valid session handle at the beginning of class method, as the driver itself will provide the necessary exception at the first function call. It is also not necessary in most cases to check if parameters are in range as the driver will do so automatically.
- Class methods **should generate exceptions** when input parameters are known to be incorrect such that it will produce a non-intuitive driver error, or an error at some other point later on after the class method. 
  - For example, the [SG](/dotNET/Source/SG/SG.cs) class has a method to generate a packet at a certain duty cycle. If 0% is specified as the requested duty cycle, the driver will throw an error due to errors in the generation script. This will not, however, provide any indication to indicate the source of the error.
    the developer of the actual problem, and so an `ArgumentOutOfRangeException` exception should be generated indicating the issue.

### Building and Preparing for Distribution

- The release build directory for a class should be set as a relative path to the *Builds* directory, on the same level as the *Source* directory. This is very important so that the automatic build and release tools can properly find the referenced DLLs in order to prepare them for distribution.
- Ensure that the [CopyLocal property](https://docs.microsoft.com/en-us/dotnet/api/vslangproj.reference.copylocal?view=visualstudiosdk-2017) is set to **False** for all references in the project to instrument drivers or dependent components of the drivers. This is to ensure that when the project is built, *only* the compiled DLL for the project and any sub-module dependencies will be placed in the build directory. Because use of these libraries requires the proper instrument drivers to be installed on disk, the appropriate DLLs should already be in the GAC and therefore the local copies are unnecessary and add to the size of any releases generated.
- The *AssemblyName* property under **Project Properties » Application » Assembly Name** should match the namespace for the class