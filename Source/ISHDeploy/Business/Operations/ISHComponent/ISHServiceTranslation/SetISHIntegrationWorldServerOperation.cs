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

using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.TranslationOrganizer;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Business.Operations.ISHComponent.ISHServiceTranslation
{
    /// <summary>
    /// Sets configuration of WorldServer.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationWorldServerOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationWorldServerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="worldServerConfiguration">The world server configuration.</param>
        /// <param name="isExternalJobMaxTotalUncompressedSizeBytesSpecified">Is ExternalJobMaxTotalUncompressedSizeBytes specified.</param>
        /// <param name="isRetriesOnTimeoutSpecified">Is RetriesOnTimeout specified.</param>
        /// <param name="exceptionMessage">The error message.</param>
        public SetISHIntegrationWorldServerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement worldServerConfiguration, bool isExternalJobMaxTotalUncompressedSizeBytesSpecified, bool isRetriesOnTimeoutSpecified, string exceptionMessage) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting configuration of WorldServer");
            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, worldServerConfiguration.RelativeFilePath);

            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();

            if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, worldServerConfiguration.XPath) || !xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, TranslationOrganizerConfig.WorldServerNodeXPath))
            {
                if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, worldServerConfiguration.XPath)
                    && (!isExternalJobMaxTotalUncompressedSizeBytesSpecified
                        || !isRetriesOnTimeoutSpecified))
                {
                    if (!isExternalJobMaxTotalUncompressedSizeBytesSpecified)
                    {
                        int currentExternalJobMaxTotalUncompressedSizeBytes =
                            int.Parse(xmlConfigManager.GetValue(filePath.AbsolutePath,
                                $"{worldServerConfiguration.XPath}/@{WorldServerConfigurationSetting.externalJobMaxTotalUncompressedSizeBytes}"));

                        ((WorldServerConfigurationSection)worldServerConfiguration).ExternalJobMaxTotalUncompressedSizeBytes =
                            currentExternalJobMaxTotalUncompressedSizeBytes;
                    }

                    if (!isRetriesOnTimeoutSpecified)
                    {
                        int currentRetriesOnTimeout =
                            int.Parse(xmlConfigManager.GetValue(filePath.AbsolutePath,
                                $"{worldServerConfiguration.XPath}/@{WorldServerConfigurationSetting.retriesOnTimeout}"));

                        ((WorldServerConfigurationSection)worldServerConfiguration).RetriesOnTimeout =
                            currentRetriesOnTimeout;
                    }
                }

                Invoker.AddAction(new SetElementAction(
                    logger,
                    filePath,
                    worldServerConfiguration));
            }
            else
            {
                throw new DocumentAlreadyContainsElementException(exceptionMessage);
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}