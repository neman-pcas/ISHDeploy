﻿using System;
using System.Management.Automation;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets
    /// </summary>
    public abstract class BaseCmdlet : PSCmdlet
    {
        /// <summary>
        /// Logger
        /// </summary>
        public readonly ILogger Logger;

		/// <summary>
		/// Constructor
		/// </summary>
		protected BaseCmdlet()
        {   
            Logger = CmdletsLogger.Instance();
            ObjectFactory.SetInstance<IFileManager>(new FileManager(Logger));
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
            ObjectFactory.SetInstance<ITextConfigManager>(new TextConfigManager(Logger));
            ObjectFactory.SetInstance<IRegistryManager>(new RegistryManager(Logger));
        }

		/// <summary>
		/// Method to be overridden instead of process record
		/// </summary>
		public abstract void ExecuteCmdlet();

        /// <summary>
        /// Overrides ProcessRecord from base Cmdlet class
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                CmdletsLogger.Initialize(this);
                base.ProcessRecord();

                ExecuteCmdlet();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}