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
		/// The path to ~\Web\InfoShareSTS\Web.config
		/// </summary>
		public static class InfoShareSTSWebConfig
		{
			/// <summary>
			/// The path to ~\Web\InfoShareSTS\Web.config
			/// </summary>
			public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
				@"InfoShareSTS\Web.config");

			/// <summary>
			/// Node to uncomment to enable infoshare sts to provide identity delegation for tokens issued by other sts
			/// </summary>
			public const string TrustedIssuerBehaviorExtensions = "<add name=\"addActAsTrustedIssuer\"";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@name='{0}']" element in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string ServiceBehaviorsTrustedUserXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@name='{0}']";

            /// <summary>
            /// The xpath of "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@thumbprint='{0}']" element in ~\Web\InfoShareSTS\Web.config file
            /// </summary>
            public const string ServiceBehaviorsTrustedUserPathByThumbprintXPath = "configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/addActAsTrustedIssuer[@thumbprint='{0}']";
		}
	}
}