﻿#Service certificate thumbprint
$serviceCertificateThumbprint="20161201.Thumbprint"

# Set service certificate
Set-ISHAPIWCFServiceCertificate -ISHDeployment $deploymentName -Thumbprint $serviceCertificateThumbprint