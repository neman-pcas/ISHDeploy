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
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;
using ISHDeploy.Data.Actions.WindowsServices;
using System;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Disable components of deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHComponentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="componentsNames">Names of components to be Disabled.</param>
        /// <remarks>Don't use this method for background task services</remarks>
        public DisableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, params ISHComponentName[] componentsNames) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of components");

            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            var components =
                dataAggregateHelper.GetActualStateOfComponents(ishDeployment.Name).Components.Where(x => componentsNames.Contains(x.Name)).ToList();

            InitializeActions(Logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">List with the components to be disabled.</param>
        public DisableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, IEnumerable<Models.ISHComponent> components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of components");
            InitializeActions(Logger, ishDeployment, components.ToList());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="backgroundTaskRole">The role of BackgroundTask component to be Disabled.</param>
        public DisableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string backgroundTaskRole) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Disabling of BackgroundTask component with role `{backgroundTaskRole}`");

            var stopOperation = new StopISHComponentOperation(logger, ishDeployment, backgroundTaskRole);

            Invoker.AddActionsRange(stopOperation.Invoker.GetActions());
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var componentsCollection =
                dataAggregateHelper.GetExpectedStateOfComponents(CurrentISHComponentStatesFilePath.AbsolutePath);

            var component = componentsCollection[ISHComponentName.BackgroundTask, backgroundTaskRole];

            if (component != null)
            {
                Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                var services = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                foreach (
                    var service in
                        services.Where(
                            x => string.Equals(x.Role, component.Role, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, service, ISHWindowsServiceStartupType.Manual));
                }
            }
            else
            {
                throw new ArgumentException($"The BackgroundTask component with role `{backgroundTaskRole}` does not exist");
            }

            if (component != null)
            {
                Invoker.AddAction(
                    new SaveISHComponentAction(
                        Logger,
                        CurrentISHComponentStatesFilePath,
                        component.Role,
                        false));
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }


        #region Private methods
        /// <summary>
        /// Initializes the actions of the <see cref="DisableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">List with the components to be disabled.</param>
        private void InitializeActions(ILogger logger, Models.ISHDeployment ishDeployment, List<Models.ISHComponent> components)
        {
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            if (components.Any(x => x.Name == ISHComponentName.SolrLucene) && !components.Any(x => x.Name == ISHComponentName.Crawler))
            {
                var dependencies = serviceManager.GetDependencies(ishDeployment.Name, ISHWindowsServiceType.Crawler);
                if (dependencies.Count() > 0)
                {
                    // Check if Crawler is already disabled, otherwise add Crawler to the components to disabled
                    var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

                    var enabledCrawlerComponents = dataAggregateHelper.GetActualStateOfComponents(ishDeployment.Name).Components.Where(x => x.Name == ISHComponentName.Crawler && x.IsEnabled).ToArray();
                    components.AddRange(enabledCrawlerComponents);
                }
            }

            // Stop components
            var stopOperation = new StopISHComponentOperation(logger, ishDeployment, components);
            Invoker.AddActionsRange(stopOperation.Invoker.GetActions());

            foreach (var component in components)
            {
                // Disable component
                IEnumerable<Models.ISHWindowsService> services;
                switch (component.Name)
                {
                    case ISHComponentName.COMPlus:
                        // Check if this operation has implications for several Deployments
                        IEnumerable<Models.ISHDeployment> ishDeployments = null;
                        new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();

                        var comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
                        var comPlusComponents = comPlusComponentManager.GetCOMPlusComponents();

                        foreach (var comPlusComponent in comPlusComponents)
                        {
                            if (comPlusComponent.Status == ISHCOMPlusComponentStatus.Enabled)
                            {
                                Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                                    $"The disabling of COM+ component `{comPlusComponent.Name}` has implications across all deployments."));

                                Invoker.AddAction(
                                    new DisableCOMPlusComponentAction(Logger, comPlusComponent.Name));
                            }
                            else
                            {
                                Invoker.AddAction(new WriteVerboseAction(Logger, () => (true),
                                    $"COM+ component `{comPlusComponent.Name}` was already disabled"));
                            }
                        }
                        break;
                    case ISHComponentName.TranslationBuilder:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationBuilder);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, service, ISHWindowsServiceStartupType.Manual));
                        }
                        break;
                    case ISHComponentName.TranslationOrganizer:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationOrganizer);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, service, ISHWindowsServiceStartupType.Manual));
                        }
                        break;
                    case ISHComponentName.Crawler:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, service, ISHWindowsServiceStartupType.Manual));
                        }
                        break;
                    case ISHComponentName.SolrLucene:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.SolrLucene);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, service, ISHWindowsServiceStartupType.Manual));
                        }
                        break;
                    case ISHComponentName.BackgroundTask:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

                        var backgroundTaskServices = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                        foreach (var backgroundTaskService in backgroundTaskServices)
                        {
                            if (backgroundTaskService.Role.Equals(component.Role, StringComparison.InvariantCultureIgnoreCase))
                            {
                                Invoker.AddAction(new SetWindowsServiceStartupTypeAction(Logger, backgroundTaskService, ISHWindowsServiceStartupType.Manual));
                            }
                        }
                        break;
                }

                if (ishDeployment.Status != ISHDeploymentStatus.Starting && ishDeployment.Status != ISHDeploymentStatus.Stopping)
                {
                    // [SCTCM-302] Don't change the 'IsEnabled' flag while the deployment is in the process of stopping (or starting)
                    if (component.Name == ISHComponentName.BackgroundTask)
                    {
                        Invoker.AddAction(
                            new SaveISHComponentAction(
                                Logger,
                                CurrentISHComponentStatesFilePath,
                                component.Role,
                                false));
                    }
                    else
                    {
                        Invoker.AddAction(
                            new SaveISHComponentAction(
                                Logger,
                                CurrentISHComponentStatesFilePath,
                                component.Name,
                                false));
                    }
                }
            }
        }
        #endregion
    }
}
