/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.WindowsServices
{
	/// <summary>
	/// Class to do single file manipulations
	/// </summary>
    public class WindowsServiceVanillaBackUpAction : BaseAction
	{
        /// <summary>
        /// Path to back up file
        /// </summary>
        private readonly string _backupFilePath;

        /// <summary>
        /// The name of deployment
        /// </summary>
        private readonly string _deploymentName;

        /// <summary>
        /// File Manager instance
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private readonly IDataAggregateHelper _dataAggregateHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceVanillaBackUpAction"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="backupFilePath">Path to vanilla backup folder</param>
        /// <param name="deploymentName">The name of deployment.</param>
        public WindowsServiceVanillaBackUpAction(ILogger logger, string backupFilePath, string deploymentName)
			: base(logger)
        {
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            _backupFilePath = backupFilePath;
            _deploymentName = deploymentName;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            if (!_fileManager.FileExists(_backupFilePath))
            {
                Logger.WriteDebug($"Create back up of all windows services of `{_deploymentName}`");

                var backup = _dataAggregateHelper.GetISHWindowsServiceBackupCollection(_deploymentName);
                
                _fileManager.EnsureDirectoryExists(Path.GetDirectoryName(_backupFilePath));
                _fileManager.SaveObjectToFile(_backupFilePath, backup);

                Logger.WriteVerbose($"The back up of all windows services of `{_deploymentName}` has been created");
            }
        }
	}
}
