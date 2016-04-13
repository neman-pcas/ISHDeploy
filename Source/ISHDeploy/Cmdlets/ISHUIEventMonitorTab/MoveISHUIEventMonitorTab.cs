﻿using System;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations.ISHUIEventMonitorTab;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	///		<para type="synopsis">Manipulates with definitions in EventMonitorTab.</para>
	///		<para type="description">The Move-ISHUIEventMonitorTab cmdlet moves Tabs definitions in Content Manager deployment.</para>
	///		<para type="link">Set-ISHUIEventMonitorTab</para>
	///		<para type="link">Remove-ISHUIEventMonitorTab</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Move-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Publish" -First</code>
	///		<para>Moves definition of the "Publish" to the top.</para>
	/// </example>
	/// <example>
	///		<code>PS C:\>Move-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Publish" -Last</code>
	///		<para>Moves definition of the "Publish" to the bottom.</para>
	/// </example>
	/// <example>
	///		<code>PS C:\>Move-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Translation" -After "Publish"</code>
	///		<para>Moves definition of the "Translation" after "Publish".</para> 
	/// </example>
	/// <para>This command manipulates XML definitions nodes in EventMonitor.
	///		Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	/// </para>
	[Cmdlet(VerbsCommon.Move, "ISHUIEventMonitorTab")]
    public class MoveISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// Cashed value for <see cref="IshPaths"/> property
        /// </summary>
        private ISHPaths _ishPaths;

		/// <summary>
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Label of menu item")]
		[ValidateNotNullOrEmpty]
		public string Label { get; set; }

		/// <summary>
		/// <para type="description">Menu item move to the last position.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move to the last position", ParameterSetName = "Last")]
		[ValidateNotNullOrEmpty]
		public SwitchParameter Last  { get; set; }

		/// <summary>
		/// <para type="description">Menu item move to the first position.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move to the first position", ParameterSetName = "First")]
		[ValidateNotNullOrEmpty]
		public SwitchParameter First  { get; set; }

		/// <summary>
		/// <para type="description">Menu item move position.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Menu item move position", ParameterSetName = "After")]
		[ValidateNotNullOrEmpty]
		public string After { get; set; }

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
	        MoveISHUIEventMonitorTabOperation operation;

			switch (ParameterSetName)
	        {
				case "Last":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertAfter);
					break;
				case "First":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertBefore);
					break;
				case "After":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertAfter, After);
					break;
				default:
					throw new ArgumentException($"Operation type in {nameof(MoveISHUIEventMonitorTabCmdlet)} should be defined.");
	        }

			operation.Run();
        }
    }
}