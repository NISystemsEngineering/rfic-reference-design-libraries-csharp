#Runs various tests on the source code to ensure that it completes the requirements for conformity
#as outlined in CONTRIBUTING.md

Write-Host "Check for exclusion of referencd DLLs in builds directory"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<Private>(.+)</Private>' -FilePattern ""*.csproj""" +
    "  -Verbose -ValidMatchValues ""False"" -RecurseDirectory -ExcludeFilePattern 'Tests'"

Invoke-Expression $command

Write-Host "Check for disabling of specific versions for references"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<SpecificVersion>(.+)</SpecificVersion>' -FilePattern ""*.csproj""" +
    "  -Verbose -ValidMatchValues ""False"" -RecurseDirectory -ExcludeFilePattern 'Tests'"

Invoke-Expression $command

Write-Host "`nCheck for SolutionInfo.cs file"

$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern '<Compile Include=""(.+)"">[\s\r\n]+<Link>Properties\\SolutionInfo.cs</Link>' -FilePattern ""*.csproj"" -Verbose" +
    " -ValidMatchValues '..\SolutionInfo.cs','..\..\SolutionInfo.cs' -RecurseDirectory -ExcludeFilePattern 'Tests' -MultiLineSearch"

Invoke-Expression $command

Write-Host "`nCheck for proper build output"
$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -SearchTextOrPattern 'Release[\W\w]+<OutputPath>(.+)</OutputPath>' -FilePattern ""*.csproj"" -Verbose" +
    " -ValidMatchValues '..\..\Builds\','..\..\..\Builds\' -RecurseDirectory -ExcludeFilePattern 'Tests' -MultiLineSearch"

Invoke-Expression $command

Write-Host "`nEnsure the assembly name is properly set to ""NationalInstruments.ReferenceDesignLibraries.<name>"
$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -RecurseDirectory -SearchTextOrPattern '<AssemblyName>NationalInstruments.ReferenceDesignLibraries(.*)</AssemblyName>'" +
    " -FilePattern ""*.csproj"" -Verbose -ExcludeFilePattern 'Tests'"

Invoke-Expression $command

Write-Host "`nEnsure that all classes are implemented as static classes"
$command = ".\MatchFileText.ps1 -FileOrFolderPath ""..\Source\"" -RecurseDirectory -SearchTextOrPattern 'public\s+(.+)\s+class'" +
    " -ValidMatchValues 'static' -FilePattern ""*.cs"" -Verbose -ExcludeFilePattern 'Tests','obj','Assembly','Solution'"

Invoke-Expression $command