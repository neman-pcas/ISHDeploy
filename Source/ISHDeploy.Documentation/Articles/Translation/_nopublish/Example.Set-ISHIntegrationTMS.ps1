﻿# Uri and credentials for SDL TMS
$uri="http://tms.example.com/"
$credential=Get-Credential -Message "SDL TMS integration credential" 

# Set the integration with required parameters
Set-ISHIntegrationTMS -ISHDeployment $deploymentName -Name WorldServer -Uri $uri -Credential $credential -Mappings $mappings -Templates $templates

# Set the integration with extra parameters
Set-ISHIntegrationTMS -ISHDeployment $deploymentName -Name WorldServer -Uri $uri -Credential $credential -Mappings $mappings -Templates $templates -RequestMetadata $requestMetadata -GroupMetadata $groupMetadata