﻿using System.IdentityModel.Selectors;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations.ISHSTS;
using ISHDeploy.Models.ISHXmlNodes;

namespace ISHDeploy.Cmdlets.ISHSTS
{
	/// <summary>
	///		<para type="synopsis">Sets STS sertificate.</para>
	///		<para type="description">The Set-ISHIntegrationSTSCertificate cmdlet sets Thumbprint and issuers values to configuration.</para>
	/// </summary>
	/// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS"</code>
	///		<para>This command sets STS trusted issuer credentials.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS" -ValidationMode "None" </code>
	///		<para>This command sets STS trusted issuer credentials with no Validation Mode.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	[Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSCertificate")]
	public sealed class SetISHIntegrationSTSCertificate : BaseHistoryEntryCmdlet
	{
		/// <summary>
		/// <para type="description">Specifies the instance of the Content Manager deployment.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
		public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// <para type="description">Action of menu item. Default value is 'Administrator'.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
		public string Thumbprint { get; set; }

		/// <summary>
		/// <para type="description">Action of menu item. Default value is 'Administrator'.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
		public string Issuer { get; set; }
		
		/// <summary>
		/// <para type="description">Selected Status filter. Default value is 'Recent'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Selected Status filter")]
		public X509CertificateValidator ValidationMode { get; set; } = X509CertificateValidator.ChainTrust;

		/// <summary>
		/// Cashed value for <see cref="IshPaths"/> property
		/// </summary>
		private ISHPaths _ishPaths;

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

		/// <summary>
		/// Executes cmdlet
		/// </summary>
		public override void ExecuteCmdlet()
		{
			var operation = new SetISHIntegrationSTSCertificateOperation(Logger, new IssuerThumbprintItem()
			{
				Thumbprint = Thumbprint,
				Issuer = Issuer,
				ValidationMode = ValidationMode
			});

			operation.Run();
		}

	}
}