﻿param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($backupPath.replace(":", "$"))
$pathToAppFolder = Join-Path $testingDeployment.WebPath ("\App{0}" -f $suffix )
$pathToDataFolder = Join-Path $testingDeployment.WebPath ("\Data{0}" -f $suffix )
$pathToWebFolder = Join-Path $testingDeployment.WebPath ("\Web{0}" -f $suffix )
$pathToBackupAppFolder = Join-Path $backupPath ("\App{0}" -f $suffix )
$pathToBackupDataFolder = Join-Path $backupPath ("\Data{0}" -f $suffix )
$pathToBackupWebFolder = Join-Path $backupPath ("\Web{0}" -f $suffix )

#endregion

#region Script Blocks 

$scriptBlockBackupISHDeployment = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName ,
        [Parameter(Mandatory=$true)]
        $paths,
        [Parameter(Mandatory=$true)]
        $switch
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }  
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($switch -eq "Web"){
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -Web
    }
    elseif($switch -eq "App"){
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -App
    }
    elseif($switch -eq "Data") {
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -Data
    }
    else
    {
        Write-Error "Wrong switch parameter"
    }
}

function GetListOfFiles {
    param (
        [Parameter(Mandatory=$true)]
        $path,
        [Parameter(Mandatory=$true)]
        $filter,
        [Parameter(Mandatory=$false)]
        $allFolders = $true
    ) 
    if ($allFolders)
    {
        $listOfFiles = (Get-ChildItem -Path $path –File -Filter $filter -Recurse | sort FullName | ForEach-Object { $_.FullName.Replace($path, "").ToLower() })
    }
    else
    {
        $listOfFiles = (Get-ChildItem -Path $path –File -Filter $filter | sort FullName | ForEach-Object { $_.FullName.Replace($path, "").ToLower() })
    }
    return $listOfFiles
}

Describe "Testing Backup-ISHDeployment"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Backup-ISHDeployment backup all *.dll files in Author\ASP\bin folder and sub folders"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles (Join-Path $pathToWebFolder "Author\ASP\bin") "*.dll"
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\bin\*.dll", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles (Join-Path $pathToBackupWebFolder "Author\ASP\bin") "*.dll"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup all files in Author\ASP\bin folder"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles (Join-Path $pathToWebFolder "Author\ASP\bin") "*.*" $false
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\bin", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles (Join-Path $pathToBackupWebFolder "Author\ASP\bin") "*.*"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup all Data files"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles $pathToDataFolder "*.*"
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "*", "Data"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupDataFolder "*.*"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup all App files"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles $pathToAppFolder "*.*"
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "*", "App"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupAppFolder "*.*"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup only one file"{
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\Web.config", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "Web.config"
        $listOfBackupFiles2 = GetListOfFiles $pathToBackupWebFolder "*.*"

        $listOfBackupFiles.Count | Should be 1
        $listOfBackupFiles2.Count | Should be 1
    }

    It "Backup-ISHDeployment backup few files"{
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, @("Author\ASP\Web.config", "Author\ASP\bin\Trisoft.InfoShare.Web.dll", "Author\ASP\XSL\FolderButtonbar.xml"), "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "*.*"

        $listOfBackupFiles.Count | Should be 3
    }

    It "Backup-ISHDeployment backup the same file twice"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles (Join-Path $pathToWebFolder "Author\ASP") "Web.config" $false
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\Web.config", "Web"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\Web.config", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles (Join-Path $pathToBackupWebFolder "Author\ASP") "Web.config"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup Author\ASP\Images"{
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\Images", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "*.*"

        $listOfBackupFiles.Count | Should be 37
    }

    It "Backup-ISHDeployment backup Author\ASP\Images\*.gif"{
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\Images\*.gif", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "*.gif"

        $listOfBackupFiles.Count | Should be 5
    }

    It "Backup-ISHDeployment backup \Author\ASP\UI\*"{
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Author\ASP\UI\*", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "*.*"

        $listOfBackupFiles.Count | Should be 100
    }
}