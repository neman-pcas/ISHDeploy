﻿using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class OperationPaths
    {
        /// <summary>
        /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config file
        /// </summary>
        public static class SynchronizeToLiveContentConfig
        {
            /// <summary>
            /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHFilePath.IshDeploymentType.App,
                @"Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config");

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config where endpoint url should be updated
            /// </summary>
            public const string WSTrustEndpointUrlAttributeName = "wsTrustEndpoint";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config where BindingType should be updated
            /// </summary>
            public const string WSTrustBindingTypeAttributeName = "wsTrustBindingType";
        }
    }
}