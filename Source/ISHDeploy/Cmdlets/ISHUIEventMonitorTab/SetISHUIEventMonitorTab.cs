﻿using System.Collections.Generic;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations.ISHUIEventMonitorTab;
using ISHDeploy.Models;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	///		<para type="synopsis">Update or adda new EventMonitor tab.</para>
	///		<para type="description">The Set-ISHUIEventMonitorTab cmdlet updates or adds new Tab definitions to Content Manager deployment.</para>
	///		<para type="description">If Icon is not specified, the default value '~/UIFramework/events.32x32.png' is taken.</para>
	///		<para type="description">If UserRole is not specified, the default value 'Administrator' is taken.</para>
	///		<para type="description">If ModifiedSinceMinutesFilter is not specified, the default value '1440' is taken.</para>
	///		<para type="description">If SelectedStatusFilter is not specified, the default value 'Recent' is taken.</para>
	///		<para type="link">Move-ISHUIEventMonitorTab</para>
	///		<para type="link">Remove-ISHUIEventMonitorTab</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "All Parameters" -Icon "~/UIFramework/new-tab.job.32x32.png" -EventTypesFilter @("EXPORTFORPUBLICATION", "EXPORTFORPUBLICATIONPDF", "EXPORTFORPUBLICATIONZIP") -SelectedStatusFilter "All" -ModifiedSinceMinutesFilter "3600" -UserRole "Administrator" -Description "Tab using all available parameters"</code>
	///		<para>Sets new tab with all sets of available and provided parameters.</para>
	///		<para>This command sets XML definitions to EventMonitor.
	///			Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	/// <example>
	///		<code>PS C:\>Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Defaults" -Description "Using default parameters"</code>
	///		<para>Sets new tab with default set of provided parameters.</para>
	///		<para>This command sets XML definitions to EventMonitor.
	///			Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>/// 
	[Cmdlet(VerbsCommon.Set, "ISHUIEventMonitorTab")]
    public class SetISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
    {
		/// <summary>
		/// Status filter enum
		/// </summary>
		public enum StatusFilter
		{
			/// <summary>
			/// Show busy tasks
			/// </summary>
			Busy,

			/// <summary>
			/// Show success tasks
			/// </summary>
			Recent,

			/// <summary>
			/// Show tasks with warnings
			/// </summary>
			Warning,

			/// <summary>
			/// Show failed
			/// </summary>
			Failed,

			/// <summary>
			/// Show All
			/// </summary>
			All
		}

		/// <summary>
		/// The status filter desctiptions
		/// </summary>
		private readonly Dictionary<StatusFilter, string> _statusFilterDesctiptions = new Dictionary<StatusFilter, string>
		{
			{ StatusFilter.Recent, "Show Recent" },
			{ StatusFilter.Failed, "Show Failed"},
			{ StatusFilter.Busy, "Show Busy"},
			{ StatusFilter.Warning, "Show Warning"},
			{ StatusFilter.All, "Show All"}
		};

		/// <summary>
		/// Cashed value for <see cref="IshPaths"/> property
		/// </summary>
		private ISHPaths _ishPaths;

		/// <summary>
		/// <para type="description">Specifies the instance of the Content Manager deployment.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
		public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Label of menu item")]
		[ValidateNotNullOrEmpty]
		public string Label { get; set; }

		/// <summary>
		/// <para type="description">Menu item icon representation. Default value is '~/UIFramework/events.32x32.png'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item icon representation")]
		[ValidateNotNullOrEmpty]
		public string Icon { get; set; } = "~/UIFramework/events.32x32.png";

		#region Action parameters

		/// <summary>
		/// <para type="description">Status filter. Null by default</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Status filter")]
		public string[] EventTypesFilter { get; set; } = null;

		/// <summary>
		/// <para type="description">Selected Status filter. Default value is 'Recent'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Selected Status filter")]
		public StatusFilter SelectedStatusFilter { get; set; } = StatusFilter.Recent;

		/// <summary>
		/// <para type="description">Modified since minutes filter value. Default value is '1440'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Modified since minutes filter value")]
		public int ModifiedSinceMinutesFilter { get; set; } = 1440;

		#endregion

		/// <summary>
		/// <para type="description">Action of menu item. Default value is 'Administrator'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
		public string UserRole { get; set; } = "Administrator";

		/// <summary>
		/// <para type="description">User role description.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "User role description")]
		public string Description { get; set; }

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
	        var operation = new SetISHUIEventMonitorTabOperation(Logger, IshPaths, new EventLogMenuItem()
			{
				Label = Label,
				Description = Description,
				Icon = Icon,
				UserRole = UserRole,
				Action = new EventLogMenuItemAction()
				{
					SelectedButtonTitle = _statusFilterDesctiptions[SelectedStatusFilter],
					ModifiedSinceMinutesFilter = ModifiedSinceMinutesFilter,
					SelectedMenuItemTitle = Label,
					StatusFilter = StatusFilter.All.ToString(), // By default 'All' is used
					EventTypesFilter = EventTypesFilter
				}
			});

			operation.Run();
        }
    }
}