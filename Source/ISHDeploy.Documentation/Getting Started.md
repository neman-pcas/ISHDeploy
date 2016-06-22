# Getting started
 
# Requirements

## PowerShell requirements
[{ModuleName}](https://www.powershellgallery.com/packages/{ModuleName}/) requires PowerShell version 4.0 installed in the operating system.

To quickly check your installed PowerShell version execute
```powershell
$PSVersionTable
```

A version 4.0 console would return 
```
Name                           Value                                                                                   
----                           -----                                                                                   
PSVersion                      4.0                                                                                     
WSManStackVersion              3.0                                                                                     
SerializationVersion           1.1.0.1                                                                                 
CLRVersion                     4.0.30319.42000                                                                         
BuildVersion                   6.3.9600.17400                                                                          
PSCompatibleVersions           {1.0, 2.0, 3.0, 4.0}                                                                    
PSRemotingProtocolVersion      2.2                                                                                     
```

If your operating system has a PowerShell version earlier than 4.0 then install it following the instructions on [technet](http://social.technet.microsoft.com/wiki/contents/articles/21016.how-to-install-windows-powershell-4-0.aspx).


## PowerShellGet module

To query and install modules from the PowerShell gallery, requires the **PowerShellGet** module. PowerShell version 5.0 has the **PowerShellGet** module available out of the box.

To install the PowerShellGet module on PowerShell version 4.0, install the [Package Management](https://www.microsoft.com/en-us/download/details.aspx?id=51451) installer. 

# Setting up PowerShellGet

PowerShellGet provides a default repository registration for [PowerShell Gallery](https://powershellgallery.com/), therefore no action is necessary. 
To verify that PowerShell gallery is registered correctly execute:

```powershell
Get-PSRepository
```
and the outcome should be

```powershell

Name                      PackageManagementProvider InstallationPolicy   SourceLocation                                
----                      ------------------------- ------------------   --------------                                
PSGallery                 NuGet                     Untrusted            https://www.powershellgallery.com/api/v2/     
```

To register or unregister repositories use the `Register-PSRepository` and `Unregister-PSRepository` cmdlets respectively.

# Install the module

The module is availabe on powershell gallery [here](https://powershellgallery.com/packages/{ModuleName}/). 

To install the module with administrative rights for all users execute
```powershell
Install-Module {ModuleName}
```

To install only for your user execute
```powershell
Install-Module {ModuleName} -Scope CurrentUser
```

# Upgrading the module

Newer versions of the module will become available on powershell gallery [here](https://powershellgallery.com/packages/{ModuleName}/). 

To install a new version for all users execute
```powershell
# All users. Requires administration rights
Install-Module {ModuleName} -Force
# Current user. No administration rights are required.
Install-Module {ModuleName} -Scope CurrentUser -Force
```

# Verify the module presense
To verify that the module is installed and check the version execute:
```powershell
Get-Module ISHDeploy.12.0.0 -ListAvailable |Format-Table Name,Version
```

if the module is available then the result should be one line such as
```
Name                                                        Version
----                                                        -------
ISHDeploy.12.0.0                                            0.1
```

# Available cmdlets
To retrieve all cmdlets offered by the module then execute:
```powershell
Get-Command -Module ISHDeploy.12.0.0 | Select-Object Name
```

Each cmdlet offers support for the `Get-Help` cmdlet. Each cmdlet provides a full description, parameter syntax and examples. 
To show the help of `Get-ISHDeployment` execute 
```powershell
Get-Help Get-ISHDeployment -Full
``` 

# The documentation portal

Although the module contains help for each cmdlet from within the module, this documentation portal offers also the same content. 
[Get-ISHDeployment](commands\Get-ISHDeployment.md) for example is the online equivelent for the PowerShell command `Get-Help Get-ISHDeployment -Full`.

Additional to the cmdlets help, the documentation portal offers an article base where different subjects are analyzed and sample scripts are provided. 
For example [Work with deployments](articles/Module/Work with deployments.md) explains how to work with the core cmdlets of the module.

# Open source and feedback
The module's code and the content of this portal is availabe on [github](https://github.com/sdl/{ModuleName}/). 