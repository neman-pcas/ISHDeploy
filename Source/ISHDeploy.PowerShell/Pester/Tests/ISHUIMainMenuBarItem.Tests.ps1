﻿param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables

# Generating file pathes to remote PC files
$xmlPath = Join-Path $webPath "Author\ASP\XSL"
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$testLabelName = "test"
$invalidLabel = "Invalid"
#endregion

#region Script Blocks 

# Function reads target files and their content, searches for specified nodes in xm
function compareArray([string[]]$firstArray, [string[]]$secondArray){
    if ($firstArray.Length -eq $secondArray.Length){
        
        $compareArrayResult = 0
        for ($i=0; $i -le $firstArray.Length - 1;$i++){
            if ($firstArray[$i] -ne $secondArray[$i]) {$compareArrayResult++}
        } 

        return $compareArrayResult 
    }
    else{
        return $false
    }
}

function getMainMenuButton{
    param([string]$Label)

    [System.Xml.XmlDocument]$xmlMainMenuBar = new-object System.Xml.XmlDocument
    $xmlMainMenuBar.load("$xmlPath\MainMenuBar.xml")
    $menuitemXml = $xmlMainMenuBar.SelectSingleNode("mainmenubar/menuitem[@label='$Label']")
    
    if ($menuitemXml)
    {
        $menuitem = @{Label = $menuitemXml.label; Action = $menuitemXml.action; Id = $menuitemXml.id; Userrole = @{}}
    
        $i = 0
        $menuitemXml.userrole | ForEach-Object { $menuitem.Userrole[$i] = $_; $i = $i + 1; }
    
        return $menuitem
    }
    else
    {
        return $null 
    }
}

function getCountMainMenuButton{
    param([string]$Label)

    [System.Xml.XmlDocument]$xmlMainMenuBar = new-object System.Xml.XmlDocument
    $xmlMainMenuBar.load("$xmlPath\MainMenuBar.xml")
    $menuitemXml = $xmlMainMenuBar.SelectNodes("mainmenubar/menuitem[@label='$Label']")
    
    return $menuitemXml.Count
}

function checkAmountOFUserroles{
    param( [string]$Label)

    [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
      
    $textMainMenuBar = $XmlMainMenuBar.menubar.menuitem | ? {($_.label -eq $Label)}
    Return  $textMainMenuBar.userrole.Count
  
}

$scriptBlockSetMainMenuButton = {
    param (
        $ishDeployName,
        $parametersHash
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHUIMainMenuBarItem -ISHDeployment $ishDeploy @parametersHash
}

$scriptBlockMoveMainMenuButton = {
    param (
        $ishDeployName,
        $parametersHash,
        $switchState
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($switchState -eq "First"){
        Move-ISHUIMainMenuBarItem -ISHDeployment $ishDeploy @parametersHash -First
    }
    elseif($switchState -eq "Last"){
        Move-ISHUIMainMenuBarItem -ISHDeployment $ishDeploy @parametersHash -Last
    }
    else{
        Move-ISHUIMainMenuBarItem -ISHDeployment $ishDeploy @parametersHash
    }
}

$scriptBlockRemoveMainMenuBar= {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $label
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHUIMainMenuBarItem -ISHDeployment $ishDeploy -Label $label
}

Describe "Testing ISHUIMainMenuBarItem"{
    BeforeEach {
		ArtifactCleaner -filePath $xmlPath -fileName "MainMenuBar.xml"
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set main menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        $item = getMainMenuButton -Label $params.Label
        $item.Label | Should be $params.Label
        $item.Action | Should be $params.Action
        $item.UserRole[0] | Should Match $params.UserRole[0]
        $item.UserRole[1] | Should Match $params.UserRole[1]
    }

    It "Remove main menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        getCountMainMenuButton -Label $params.Label | Should be 1
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        #Assert
        $item = getMainMenuButton -Label $params.Label
        $item | Should be $null
        getCountMainMenuButton -Label $params.Label | Should be 0
    }
       
    It "Sets main menu button with no XML"{
        #Arrange
        Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Could not find file" 
        #Rollback
        Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
    }    
      
    It "Sets main menu button with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
        New-Item "$xmlPath\MainMenuBar.xml" -type file |Out-Null
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\MainMenuBar.xml"
        Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
    }
  
    It "Remove main menu button with no XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
        New-Item "$xmlPath\MainMenuBar.xml" -type file |Out-Null
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\MainMenuBar.xml"
        Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
    }

    It "Set existing main menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw
        getCountMainMenuButton -Label $params.Label | Should be 1
    }

    It "Remove unexisting main menu button"{
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} | Should Not Throw
    }

    It "Move After"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[1] = $moveblelabel
        for ($i=2; $i -le $arrayLength - 1;$i++){
            $labelArray[$i] = $tempArray[$i-1]
        }
        $params = @{label = $moveblelabel; After = $tempArray[0]}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params

        #read the updated xml file
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $thirdArray =$XmlMainMenuBar.mainmenubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0
    
        #Assert
        $checkResult | Should Be "True"
    }

    It "Move Last"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[0]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[$arrayLength-1] = $moveblelabel
        for ($i=0; $i -le $arrayLength - 2;$i++){
            $labelArray[$i] = $tempArray[$i+1]
        }
        $params = @{label = $moveblelabel;}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "Last"

        #read the updated xml file
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $thirdArray =$XmlMainMenuBar.mainmenubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0 
        #Assert
        $checkResult | Should Be "True"
    }

    It "Move After itsels"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $compareArray =$XmlMainMenuBar.mainmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $invalidLabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $compareArray =$XmlMainMenuBar.mainmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $invalidLabel ; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $compareArray =$XmlMainMenuBar.mainmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0)| Should Be "True"
    }

    It "Move First"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlMainMenuBar.mainmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[0] = $moveblelabel
        for ($i=1; $i -le $arrayLength - 1;$i++){
            $labelArray[$i] = $tempArray[$i-1]
        }
        $params = @{label = $moveblelabel}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "First"

        #read the updated xml file
        [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
        $thirdArray =$XmlMainMenuBar.mainmenubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0 
        $CheckResult | Should Be "True"
     }

	It "Update existing item"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        $item = getMainMenuButton -Label $params.Label
        $item.UserRole.Count | Should Be 2
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator", "Reviewer"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        $item = getMainMenuButton -Label $params.Label
        $item.UserRole.Count | Should Be 3
    }
}