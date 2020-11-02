#Runs various tests on the source code to ensure that it completes the requirements for conformity
#as outlined in CONTRIBUTING.md


Write-Host "Check for exclusion of referencd DLLs in builds directory"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<Private>(.+)</Private>' -FilePattern ""*.csproj""" +
    "   -ValidMatchValues ""False"" -RecurseDirectory -ExcludeFilePattern 'Tests','Common','FocusTuner','PowerMeter'"

Invoke-Expression $command

Write-Host "Check for disabling of specific versions for references"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<SpecificVersion>(.+)</SpecificVersion>' -FilePattern ""*.csproj""" +
    "   -ValidMatchValues ""False"" -RecurseDirectory -ExcludeFilePattern 'Tests','Common','FocusTuner','PowerMeter'"

Invoke-Expression $command

Write-Host "`nCheck for SolutionInfo.cs file"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<Compile Include=""(.+)"">[\s\r\n]+<Link>Properties\\SolutionInfo.cs</Link>' -FilePattern ""*.csproj"" " +
    " -ValidMatchValues '..\SolutionInfo.cs','..\..\SolutionInfo.cs' -RecurseDirectory -ExcludeFilePattern 'Tests' -MultiLineSearch"

Invoke-Expression $command

Write-Host "`nCheck for proper build output"
$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern 'Release[\W\w]+<OutputPath>(.+)</OutputPath>' -FilePattern ""*.csproj"" " +
    " -ValidMatchValues '..\..\Builds\','..\..\..\Builds\' -RecurseDirectory -ExcludeFilePattern 'Tests' -MultiLineSearch"

Invoke-Expression $command

Write-Host "`nEnsure the assembly name is properly set to ""NationalInstruments.ReferenceDesignLibraries.<name>"
$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -RecurseDirectory -SearchTextOrPattern '<AssemblyName>NationalInstruments.ReferenceDesignLibraries(.*)</AssemblyName>'" +
    " -FilePattern ""*.csproj""  -ExcludeFilePattern 'Tests'"

Invoke-Expression $command

# Removed 11/2/20 since this was no longer really an appropriate test

#Write-Host "`nEnsure that all classes are implemented as static classes"
#$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -RecurseDirectory -SearchTextOrPattern 'public\s+(.+)\s+class'" +
#    " -ValidMatchValues 'static' -FilePattern ""*.cs""  -ExcludeFilePattern 'Tests','Common','obj','Assembly','Solution','FocusITunerBroker'"

#Invoke-Expression $command
