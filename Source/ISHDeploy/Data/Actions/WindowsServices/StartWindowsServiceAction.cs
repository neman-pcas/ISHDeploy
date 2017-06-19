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

using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.WindowsServices
{
    /// <summary>
    /// Starts an windows service.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class StartWindowsServiceAction : BaseAction
    {
        /// <summary>
        /// The name of deployment service.
        /// </summary>
        private readonly string _serviceName;

        /// <summary>
        /// The windows service manager
        /// </summary>
        private readonly IWindowsServiceManager _serviceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartWindowsServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">The deployment service.</param>
        public StartWindowsServiceAction(ILogger logger, ISHWindowsService service)
            : base(logger)
        {
            _serviceName = service.Name;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartWindowsServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceName">The name of deployment service.</param>
        public StartWindowsServiceAction(ILogger logger, string serviceName)
            : base(logger)
        {
            _serviceName = serviceName;

            _serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _serviceManager.StartWindowsService(_serviceName);
        }
    }
}