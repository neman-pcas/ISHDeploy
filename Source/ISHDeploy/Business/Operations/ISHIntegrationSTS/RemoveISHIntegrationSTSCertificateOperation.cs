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
﻿using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTS
{
    /// <summary>
    /// Removes certificate based on a issuer name
    /// </summary>
    /// <seealso cref="IOperation" />
    public class RemoveISHIntegrationSTSCertificateOperation : BaseOperationPaths, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="issuer">The certificate issuer.</param>
        public RemoveISHIntegrationSTSCertificateOperation(ILogger logger, Models.ISHDeployment ishDeployment, string issuer) :
            base(logger, ishDeployment)
		{
			Invoker = new ActionInvoker(logger, "Remove certificate credentials based on issuer name");

			// Author web Config
			Invoker.AddAction(new RemoveNodesAction(logger, InfoShareAuthorWebConfigPath, 
				String.Format(InfoShareAuthorWebConfig.IdentityTrustedIssuersByNameXPath, issuer)));
        
			// WS web Config
			Invoker.AddAction(new RemoveNodesAction(logger, InfoShareWSWebConfigPath, 
				String.Format(InfoShareWSWebConfig.IdentityTrustedIssuersByNameXPath, issuer)));

            // STS web Config
            Invoker.AddAction(new RemoveNodesAction(logger, InfoShareSTSWebConfigPath,
				String.Format(InfoShareSTSWebConfig.ServiceBehaviorsTrustedUserByNameXPath, issuer)));
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