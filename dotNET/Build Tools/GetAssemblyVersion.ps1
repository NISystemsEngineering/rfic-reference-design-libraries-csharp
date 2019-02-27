 param (
    [Parameter(Mandatory=$true)][string]$path
 )
$result = Select-String -Path $path -Pattern 'AssemblyFileVersion\(\"(.*)\"\)'
$result.Matches.Groups[1].Value