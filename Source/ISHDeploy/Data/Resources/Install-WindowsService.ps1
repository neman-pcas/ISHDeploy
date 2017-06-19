﻿$VerbosePreference="Continue"
$secureString = "$password" | ConvertTo-SecureString -asPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ("$username", $secureString)
New-Service -Name "$name" -DisplayName "$displayName" -Description "$description" -BinaryPathName "$pathToExecutable" -StartupType Manual -Credential $credential