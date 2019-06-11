# RF ADV Reference Design Libraries - C#


## Overview

The RF ADV Reference Design Libraries (RDLs) are a lightweight, **open-source** wrapper around existing NI instrument drivers providing a higher-level starting point for semiconductor RF Automated Device Validation (ADV) engineers to use for getting start with automating common tests for PA/FEM validation. They are designed for simplicity and ease of integration with a test sequencer, such as NI's TestStand. They have been designed to be language agnostic, so that the same functionality can be accomplished in LabVIEW or C# with minimal difference between code modules. 

Automation examples in TestStand are provided to show how the modules can be combined together in functional test applications. 

To get started using the RDLs, [clone the repository to your computer](https://help.github.com/en/desktop/contributing-to-projects/cloning-a-repository-from-github-to-github-desktop) and build the libraries. Alternatively, [grab the latest release](../../releases/latest) and start using the libraries in your application. Check out the [module examples](/Examples/) to see how to implement the modules in your code or sequencer, or see the [TestStand examples](/Automation/) for application-level examples.

If you would like to extend the RDLs for your application or contribute to the project, please see the [setup guidelines](SETUP.md) and the [contribution guidelines](CONTRIBUTING.md).

## What Are the Reference Design Libraries?

Most drivers that will be used for ADV tasks have examples that demonstrate driver features that look like the following:

![Standard Driver Examples](/images/README/standard_example_code.png)

This is useful for demonstrating a driver feature or function. However, it is often difficult to understand how to take the example functionality that is being demonstrated and implement it in an automated test routine. Moreover, there are many functions that are frequently called in the same order to setup Finally, for RF ADV applications especially, many different drivers are required to be used and combined together in order to make even basic measurements. The RDLs have been designed to meet these need, implementing functionality as seen below:

![RDL Code](/images/README/rdl_code.png)

The RDLs are **example code** implementing common tasks in discrete modules in order to provide a higher-level starting point for ADV applications. Each module is a very simple wrapper around individual driver functions to implement common functions. The modules can be pieced together to rapidly create a full application or automated test routine. 

Since each application is different, the RDLs **are not intended to meet every use case**. Rather, they are intended to address around 80% of common use cases to minimize the effort getting started building an ADV application. Some customization may be required; however, since the modules are designed with simplicity as a primary design consideration, extending and modifying the existing code should be straightforward even with basic programming skills.