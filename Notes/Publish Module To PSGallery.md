# Publishing **ISHDeploy** module to PowerShell Gallery
This article explains how to publish module to PowerShell Gallery.

***

As a build result, versioned NuGet package (e.g. `ISHDeploy.12.0.1.0.1605.0.nupkg`) should be created and uploaded to a NuGet server.

After this action is done, it should be possible to find it in PowerShell Gallery
```powershell
Find-Package -Name ISHDeploy.12.0 -AllVersions
```
This outputs the following

    Name              Version      Source           Summary
    ----              -------      ------           -------
    ISHDeploy.12.0    1.0.0        KievGreenNu...   SDL Trisoft ISHDepl..
    ISHDeploy.12.0    1.0.1605     KievGreenNu...   SDL Trisoft ISHDepl..
    ISHDeploy.12.0    1.0.2750.0   KievGreenNu...   SDL Trisoft ISHDepl..  


## Version plan

All modules starts on their first release with `Major.Minor=1.0`. For more information please see [Maintaining Versions][versions]

 
## Digital and Strong Name Assembly Signing

Before publishing assembly to the gallery, it should be signed with a strong name and digital signature. 

Signing is done via `Trisoft.SignTool.SignFiles` on Jenkins as a step of daily build.

```
$fileToSign = Get-ChildItem "${ENV:WORKSPACE}\Source\ISHDeploy\bin\Release\ISHDeploy*.dll"
&"${ENV:WORKSPACE}\Tools\SignTool\Trisoft.SignTool.SignFiles.exe" "${ENV:WORKSPACE}\Tools\SignTool\InfoShare.snk" "$fileToSign"
```

The above command performs both actions by adding to the assembly a strong name and signing it by digital key. That guarantees the uniqueness of the assembly, confirms SDL ownership and protects against tampering of the asset.


## Generating Manifest file `ISHDeploy.xx.x.psd1`

Instructions how we provide module name and version per branches can be found in [Maintaining Versions][versions].


## Creating temporary NuGet server to host packages

Until we figure out where to publish to real source, we should use temporary created sandbox NuGet server. Information about how to configure NuGet server can be found in article about [Creating Remote Feeds](https://docs.nuget.org/create/hosting-your-own-nuget-feeds#user-content-creating-remote-feeds "Hosting Your Own NuGet Feeds").

We should publish and host packages under IIS on the build server.

## Publishing

To be able to publish as well as to find the package, you should have [`PowerShellGet`](https://technet.microsoft.com/en-us/library/dn807169.aspx "Windows PowerShellGet Module") module configured. See more details at [Get Started with the PowerShell Gallery](https://www.powershellgallery.com/GettingStarted "Get Started with the PowerShell Gallery").

### Registering repository
If repository source is not registered, you should at first register it
```powershell
$sourceName = "TestNuGetServer"
$publishLocation = "http://test-nuget-server.global.sdl.corp:8088/"
$sourceLocation = $publishLocation + "nuget/"

Register-PSRepository -Name $sourceName -SourceLocation $sourceLocation -PublishLocation  $publishLocation -InstallationPolicy Trusted
```

To check if it was registered execute following command
```powershell
Get-PSRepository -Name "KievGreenNuGetServer"
```
The output result should be

    Name            PackageManageme..  InstallationPolicy   SourceLocation
    ----            -----------------  ------------------   --------------
    TestNuGetServer NuGet              Trusted              http://test-nuget-server.glob...

### Verifying Manifest file
To be able to create package module manifest should be found next to the module. If folder where module and its manifest locates has different name than module name, then temporary folder with the module name should be created, and all files related to module should be copied there.

After Publishing is done, remove temporary module directory.

### Publishing Module

To publish module you should provide module path, where manifest file can be found, server API key, and repository name
```powershell
$modulePath = "c:\path-to-module"
$apiKey = "test-api-key"
$repoName = "TestNuGetServer"

Publish-Module -Path $modulePath -NuGetApiKey $apiKey -Repository $repoName
```

[versions]: ./Maintain%20versions.md