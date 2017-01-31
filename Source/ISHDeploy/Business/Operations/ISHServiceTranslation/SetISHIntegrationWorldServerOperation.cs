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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Actions.XmlFile;

namespace ISHDeploy.Business.Operations.ISHServiceTranslation
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
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="worldServerConfiguration">The world server configuration.</param>
        public SetISHIntegrationWorldServerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement worldServerConfiguration) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting configuration of WorldServer (SOAP)");
            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, worldServerConfiguration.RelativeFilePath);

            _invoker.AddAction(new SetElementAction(
                logger,
                filePath,
                worldServerConfiguration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="worldServerConfiguration">The world server configuration.</param>
        /// <param name="httpTimeout">The HTTP timeout (Used for REST client only).</param>
        public SetISHIntegrationWorldServerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement worldServerConfiguration, TimeSpan httpTimeout) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting configuration of WorldServer (REST)");

            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, worldServerConfiguration.RelativeFilePath);

            _invoker.AddAction(new SetElementAction(
                logger,
                filePath,
                worldServerConfiguration));

            _invoker.AddAction(
                    new SetAttributeValueAction(Logger,
                    filePath,
                    $"{worldServerConfiguration.XPath}/@httpTimeout",
                    httpTimeout.ToString(@"hh\:mm\:ss\.fff"),
                    true));
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
