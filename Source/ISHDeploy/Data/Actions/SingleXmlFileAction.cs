﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
    /// Base class for all actions that operate with xml files.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public abstract class SingleXmlFileAction : SingleFileAction
	{
        /// <summary>
        /// The xml configuration manager
        /// </summary>
        protected readonly IXmlConfigManager XmlConfigManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleXmlFileAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishFilePath">Wrapper for file path.</param>
        protected SingleXmlFileAction(ILogger logger, ISHFilePath ishFilePath)
			: base(logger, ishFilePath)
        {
			XmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
		}
	}
}