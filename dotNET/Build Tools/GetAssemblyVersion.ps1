 param (
    [Parameter(Mandatory=$true)][string]$path
 )
$result = Select-String -Path $path -Pattern 'AssemblyFileVersion\(\"(.*)\"\)'
$version = $result.Matches.Groups[1].Value
Write-Host "##vso[task.setvariable variable=assemblyVersion]$($version)"
Write-Host "##vso[build.updatebuildnumber]$version"