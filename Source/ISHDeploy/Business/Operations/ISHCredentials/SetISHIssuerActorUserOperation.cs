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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.DataBase;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHCredentials
{
    /// <summary>
    /// Sets IssuerActor user credentials.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIssuerActorUserOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIssuerActorUserOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user name.</param>
        public SetISHIssuerActorUserOperation(ILogger logger, Models.ISHDeployment ishDeployment, string userName, string password) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting of new IssuerActor credential.");

            // ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorUserNameXPath, userName));
            Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorPasswordXPath, password));

            // InputParameters
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerActorUserNameXPath, userName));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerActorPasswordXPath, password));

            // ~\Web\InfoShareSTS\Configuration\infoShareSTS.config
            Invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSConfigPath, InfoShareSTSConfig.ActorUsernameAttributeXPath, userName));

            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            bool isDataBaseFileExist = fileManager.FileExists(InfoShareSTSDataBasePath.AbsolutePath);

            // Update UserName in Delegation table of STS database if STS database file exists
            if (isDataBaseFileExist)
            {
                Invoker.AddAction(new SqlCompactExecuteAction(Logger,
                InfoShareSTSDataBaseConnectionString,
                string.Format(InfoShareSTSDataBase.UpdateIssuerActorUserNameInDelegationSQLCommandFormat,
                    userName)));
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