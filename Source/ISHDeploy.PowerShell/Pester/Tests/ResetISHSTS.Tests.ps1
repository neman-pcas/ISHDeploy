﻿param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

# Script block for getting ISH deployment
$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$testCertificate = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCreateCertificate -Session $session

$suffix = GetProjectSuffix($testingDeployment.Name)
$absolutePath = $testingDeployment.WebPath
$dbPath = ("\\$computerName\{0}\Web{1}\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf" -f $testingDeployment.Webpath, $suffix).replace(":", "$")
$computerName = $computerName.split(".")[0]

$scriptBlockGetHistory = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

$scriptBlockResetISHSTS = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Reset-ISHSTS -ISHDeployment $ishDeploy

}

$scriptBlockSetISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $thumbprint,
        $validationMode
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHAPIWCFServiceCertificate -ISHDeployment $ishDeploy -Thumbprint $thumbprint -ValidationMode $validationMode
 
}



$scriptBlockQuerry = {
    param (
        
        $suffix,
        $dbPath,
        $absolutePath,
        $command,
        $stringOutput
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
     #Create System.Data.SqlServerCe.dll path
    $sqlCEAssemblyPath=[System.IO.Path]::Combine("$absolutePath\Web$suffix\InfoShareSTS\bin","System.Data.SqlServerCe.dll")
    
    #Add SQL Server CE Engine
    $var = [Reflection.Assembly]::LoadFile($sqlCEAssemblyPath)

    #Create Connection String

    $connectionString="Data Source=$dbPath;"

    #Prepare Database Connection and Command
	$connection = New-Object "System.Data.SqlServerCe.SqlCeConnection" $connectionString
        
    $existCommand = New-Object "System.Data.SqlServerCe.SqlCeCommand"
	$existCommand.CommandType = [System.Data.CommandType]::Text
	$existCommand.Connection = $connection
	$existCommand.CommandText = "$command"

	#Execute Command
	try
	{
		$connection.Open()
        if ($stringOutput){
		    $result =$existCommand.ExecuteScalar().ToString()
        }
        else{
            $result =$existCommand.ExecuteScalar()
        }

	}
    catch{
        Write-Host $Error
    }
	finally
	{
		$connection.Close()
		$connection.Dispose()
	}
     return $result
}


function remoteQuerryDatabase() {
    param(
        [string]$command,
        $stringOutput
    )
    if($stringOutput){
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockQuerry -Session $session -ArgumentList $suffix, $dbPath, $absolutePath, $command, $stringOutput 
    }
    else{
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockQuerry -Session $session -ArgumentList $suffix, $dbPath, $absolutePath, $command
    }
    return $result
}



Describe "Testing Reset-ISHSTS"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Reset ISH STS"{
        WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        $infosharewswebappname = $testingDeployment.WebAppNameWS
        $dbQuerryCommandSet = "UPDATE RelyingParties SET EncryptingCertificate='testThumbprint' WHERE Realm=`'https://$computerName.$env:USERDNSDOMAIN/$infosharewswebappname/Wcf/API20/Folder.svc`'"
        $dbQuerryCommandSelect = "SELECT EncryptingCertificate FROM RelyingParties WHERE Realm=`'https://$computerName.$env:USERDNSDOMAIN/$infosharewswebappname/Wcf/API20/Folder.svc`'"

        $thumbprint1 = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true
        
        #Change thubmprint value
        remoteQuerryDatabase -command $dbQuerryCommandSet

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockResetISHSTS -Session $session -ArgumentList $testingDeploymentName  
        Start-Sleep -Milliseconds 7000

        If (Test-Path $dbPath) {
            $thumbprint2 = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true
            $thumbprint1 -eq $thumbprint2 | Should be $true
        }
        else {
            Test-Path $dbPath | Should be $False
        }
    }
    
    It "Reset ISH STS when DB not exists"{
        if (Test-Path $dbPath){
            Remove-Item $dbPath
        }
         
        Test-Path $dbPath | Should be $False
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockResetISHSTS -Session $session -ArgumentList $testingDeploymentName} | Should Not Throw 

    }
    
     It "Reset ISH STS writes proper history"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockResetISHSTS -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Reset-ISHSTS -ISHDeployment $deployment')
              
    }
	Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
}
