﻿param (
    [Parameter(Mandatory=$true)]
    [string]
    $SourcePath,
    [string]
    $ExportPath
)

$DebugPreference="Continue"
$VerbosePreference="Continue"

Write-Debug "SourcePath=$SourcePath"
Write-Debug "ExportPath=$ExportPath"


try
{
    if ($ExportPath)
    {
	    if(-not (Test-Path $ExportPath))
	    {
		    New-Item $ExportPath -ItemType Directory
	    }
	    Get-ChildItem -Path $ExportPath -Include * | Remove-Item -recurse 

        Copy-Item "$SourcePath\*" $ExportPath -Recurse
    }
    else
    {
        # means that the source path is the same as export path and markdown files should be resolved within one folder
        $ExportPath = $SourcePath;
    }

    $pathsGroupedByContainer=Get-ChildItem $ExportPath -Recurse -Include @("*.ps1","*.md") | Group-Object Directory
    foreach($container in $pathsGroupedByContainer)
    {
        $containerPath=$container.Name
        $files=$container.Group
		#Write-Verbose "Processing ${($files.Count) files in $containerPath"
        $mdFiles=$files|Where-Object {$_.Extension -eq ".md"}
        $copyCodeBlockAndLinkMatchEvaluator = 
        {  
			param($m) 
			$targetFileName=$m.Groups["file"].Value
            $targetFilePath=Join-Path $containerPath $targetFileName
            Write-Verbose "Loading $targetFilePath"
			$codeContent=Get-Content $targetFilePath | Out-String
            $syntaxType=""
            if($targetFilePath.ToLowerInvariant().EndsWith(".ps1"))
            {
                $syntaxType="powershell"
            }
            $newContent="
``````$syntaxType
$codeContent``````
[Download]($targetFileName)
"
            return $newContent

        }
        $copyCodeBlockMatchEvaluator = 
        {  
			param($m) 
			$targetFileName=$m.Groups["file"].Value
            $targetFilePath=Join-Path $containerPath $targetFileName
            Write-Verbose "Loading $targetFilePath"
			$codeContent=Get-Content $targetFilePath | Out-String
            $syntaxType=""
            if($targetFilePath.ToLowerInvariant().EndsWith(".ps1"))
            {
                $syntaxType="powershell"
            }
            $newContent="
``````$syntaxType
$codeContent``````
"
            return $newContent

        }
        foreach($mdFile in $mdFiles)
        {
			Write-Verbose "Processing $($mdFile.Name)"
            $mdContent=$mdFile |Get-Content|Out-String
            $mdContent=[regex]::replace($mdContent,'CopyCodeBlockAndLink\((?<file>.*)\)',$copyCodeBlockAndLinkMatchEvaluator)
            $mdContent=[regex]::replace($mdContent,'CopyCodeBlock\((?<file>.*)\)',$copyCodeBlockMatchEvaluator)
            $mdContent|Out-File $mdFile -Force
        }
    }
}
catch
{
	Write-Error $_.Exception
	exit 1
}




