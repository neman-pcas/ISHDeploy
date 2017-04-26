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

using System.Collections.Generic;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using System.Linq;
using ISHDeploy.Data.Actions.COMPlus;
using Microsoft.Web.Administration;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
	/// <summary>
	/// Operation to revert changes to Vanilla state
	/// </summary>
    public class UndoISHDeploymentOperation : BaseOperationPaths, IOperation
	{
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Gets or sets a value indicating whether skip recycle or not. For integration test perspective only.
        /// Please, see https://jira.sdl.com/browse/TS-11329
        /// </summary>
        /// <value>
        ///   <c>true if [true] then skip recycle; otherwise do recycle</c>.
        /// </value>
        public static bool SkipRecycle { get; set; } = false;

        /// <summary>
        /// Reverts all changes to the vanilla
        /// </summary>
        /// <param name="logger">Logger object.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
		{
            _invoker = new ActionInvoker(logger, "Reverting of changes to Vanilla state");
            _fileManager = ObjectFactory.GetInstance<IFileManager>();


            // For version 12.X.X only
            DeleteExtensionsLoaderFile();

            // Remove redundant files from BIN
            _invoker.AddAction(new DirectoryBinReturnToVanila(
                logger, 
                BackupFolderPath, 
                "vanilla.web.author.asp.bin.xml",
                $@"{WebFolderPath}\Author\ASP\bin"));

            // Disable internal STS login (remove directory) 
            _invoker.AddAction(new DirectoryRemoveAction(Logger, InternalSTSFolderToChange));

            if (!SkipRecycle)
            {
                // Stop Application pools before undo
                _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.WSAppPoolName));
                _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));
                _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.CMAppPoolName));
                // Cleaning up STS App_Data folder
                _invoker.AddAction(new FileCleanDirectoryAction(logger, WebNameSTSAppData));
            }

            // Disable Windows Authentication for STS web site
            _invoker.AddAction(new WindowsAuthenticationSwitcherAction(Logger, InputParameters.STSWebAppName, false));

            // Restore InputParameters.xml
            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            Models.InputParameters inputParameters;
            bool isInputParameterBackupFileExist = false;
            (new FileExistsAction(logger, InputParametersFilePath.VanillaPath, returnResult => isInputParameterBackupFileExist = returnResult)).Execute();
            if (isInputParameterBackupFileExist)
            {
                var dictionary = xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.VanillaPath);
                inputParameters = new Models.InputParameters(InputParametersFilePath.VanillaPath, dictionary);
                _invoker.AddAction(new FileCopyAction(logger, InputParametersFilePath.VanillaPath, InputParametersFilePath.AbsolutePath,
                    true));
            }
            else
            {
                var dictionary = xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.AbsolutePath);
                inputParameters = new Models.InputParameters(InputParametersFilePath.AbsolutePath, dictionary);
            }

            // Stop and delete excess ISH windows services
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationBuilder, ISHWindowsServiceType.TranslationOrganizer).ToList();
            foreach (var service in services)
            {
                _invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                _invoker.AddAction(new SetWindowsServiceCredentialsAction(Logger, service, inputParameters.OSUser, inputParameters.OSPassword));
            }

            // Rolling back credentials for COM+ component
            _invoker.AddAction(new SetCOMPlusCredentialsAction(Logger, "Trisoft-InfoShare-Author", inputParameters.OSUser, inputParameters.OSPassword));

            var servicesForDeleting = services.Where(serv => serv.Sequence > 1);
            foreach (var service in servicesForDeleting)
            {
                _invoker.AddAction(new RemoveWindowsServiceAction(Logger, service));
            }


            // Rolling back changes for Web folder
            _invoker.AddAction(new FileCopyDirectoryAction(logger, BackupWebFolderPath, WebFolderPath));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, BackupDataFolderPath, DataFolderPath));
            
			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, BackupAppFolderPath, AppFolderPath));

            // Removing licenses
            _invoker.AddAction(new FileCleanDirectoryAction(logger, LicenceFolderPath.AbsolutePath));


            // WS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.password,
                inputParameters.OSPassword));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                inputParameters.OSPassword));

            // STS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.password,
                inputParameters.OSPassword));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                inputParameters.OSPassword));

            // CM
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.password,
                inputParameters.OSPassword));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                inputParameters.OSUser));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                inputParameters.OSPassword));

            if (!SkipRecycle)
            {
                // Recycling Application pools after undo
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName, true));
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName, true));

                // Waiting until files becomes unlocked
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareAuthorWebConfigPath));
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSWebConfigPath));
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareWSWebConfigPath));
            }

			// Removing Backup folder
			_invoker.AddAction(new DirectoryRemoveAction(logger, ISHDeploymentProgramDataFolderPath));

            // Remove Author\ASP\Custom
            _invoker.AddAction(new DirectoryRemoveAction(logger, $@"{WebFolderPath}\Author\ASP\Custom"));
        }

        /// <summary>
        /// Deletes ~\Web\Author\ASP\UI\Helpers\ExtensionsLoader.js file if file exists.
        /// </summary>
        public void DeleteExtensionsLoaderFile()
        {
            if (_fileManager.FileExists(ExtensionsLoaderFilePath.AbsolutePath))
            {
                Logger.WriteDebug("Delete file", ExtensionsLoaderFilePath.RelativePath);

                _invoker.AddAction(new FileDeleteAction(Logger, ExtensionsLoaderFilePath.AbsolutePath));
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
	}
}
