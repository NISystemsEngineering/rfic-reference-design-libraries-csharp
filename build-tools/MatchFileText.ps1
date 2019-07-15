[CmdletBinding()]

param (
    [string]$FileOrFolderPath, # Defines the file or folder path to find files in
    [string]$SearchTextOrPattern, # Defines the desired search text to find or pattern to match in a file or files
    [string]$FilePattern = "*.*", # Defines the file pattern to search for if a folder is specified
    [string[]]$ExcludeFilePattern = "", # An array of regular expressions used to parse files to exclude from the search
    [string[]]$ValidMatchValues = "", # An array of valid strings to compare to any value captured in $SearchTextOrPattern
    [switch]$RecurseDirectory = $false, # Specify whether or not to recurse the search directory, if input
    [switch]$IgnoreFilesWithoutMatch = $false # A flag to specify that files without the match should not produce an error
)

# Determine if the input path is a file or folder
$IsFolder = (Get-Item $FileOrFolderPath) -is [System.IO.DirectoryInfo]

# See comments below on Get-ChildItem calls
$ExcludeMatchString = $ExcludeFilePattern -join "|"

# If folder, we need to get the sub-items
if ($IsFolder) {
    # Recurse the directory and subdirectories to get all matching files 
    if ($RecurseDirectory) {
        # Rather than use the built-in -Exclude input, we manually search the results for the exclude match string
        # This is because -Exclude only matches base file names, not entire search paths
        $files = Get-ChildItem -File -Filter $FilePattern -Path $FileOrFolderPath -Recurse | Where {$_.FullName -notmatch $ExcludeMatchString}
    }
    # Search the directory to get matching files
    else {
        $files = Get-ChildItem -File -Filter $FilePattern -Path $FileOrFolderPath | Where {$_.FullName -notmatch $ExcludeMatchString}
    }
    # We are using relative paths later, so set the working directory to the current top-level folder
    Push-Location $FileOrFolderPath
}
# Not folder, so just get the file object
else {
    $files = Get-Item $FileOrFolderPath
    # We are using relative paths later, so set the working directory to the current top-level folder
    Push-Location $files[0].Directory
}

# Ensure that one or more files are found
if ($files.Count -gt 0) {
    # In some cases the developer may not want to error if the pattern is not found;
    # the developer may only care to check the value if it exists and ortherwise ignore it
    if (!$IgnoreFilesWithoutMatch)
    {
        Foreach ($file in $files) {
            
            # Use Get-Content to get full file text as a single string for multi-line matching
            # Use the Select-String cmdlet to search the file text for the search patttern
            # Quiet indicates it will simply return a boolean value for each file if a match is found
            $simpleResult = $file | Get-Content -Raw | Select-String -Pattern $SearchTextOrPattern -Quiet

            # Relative paths are much easier to understand since this is a part of a build process
            $relativePath = Resolve-Path -Relative $file.FullName

            # Check to see if a match was found; if not, throw an error
            if (!$simpleResult -or $simpleResult -eq $null) {
                Write-Error "File ""$relativePath"" does not include any match for the requested search string." -Category InvalidData
            }
            else {
                # Only visible if the script is called with the -Verbose arguement
                Write-Verbose "File ""$relativePath"" includes one or more matches for the requested search string."
            }
        }
    }

    # Only try to match specific text if the expected value is not null or empty
    if (![string]::IsNullOrEmpty($ValidMatchValues)) {

        Foreach ($file in $files) {
            
            # Use Get-Content to get full file text as a single string for multi-line matching
            $results = $file | Get-Content -Raw | Select-String -Pattern $SearchTextOrPattern -AllMatches

            Foreach ($result in $results) {
                # Relative paths are much easier to understand since this is a part of a build process
                $relativePath = Resolve-Path -Relative $file.FullName

                # Retrieve the captured text which is contained in Group 1, Group 0 being the whole match
                $matchedValue = $result.Matches.Groups[1].Value

                $testStrings = Compare-Object -ReferenceObject $ValidMatchValues -DifferenceObject $matchedValue -IncludeEqual -ExcludeDifferent -PassThru

                if ($testStrings.Count -le 0) {
                    Write-Error "File ""$relativePath"" has value of ""$matchedValue"" instead of ""$($ValidMatchValues)""."
                }
                else {
                    # Only visible if the script is called with the -Verbose arguement
                    Write-Verbose "File ""$relativePath"" matches the expected value of ""$ValidMatchValues""."
                }
            }
        }
    }
    Write-Host "A total of $($files.Count) files were successfully searched."
}
else {
    Write-Warning "No files were found matching the input parameters."
}

# Return the current directory to where it was before
Pop-Location