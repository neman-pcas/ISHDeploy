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
using System.Linq;
using COMAdmin;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="ICOMPlusComponentManager" />
    public class COMPlusComponentManager : ICOMPlusComponentManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The COMAdminCatalogWrapper.
        /// </summary>
        private readonly COMAdminCatalogWrapperSingleton _comAdminCatalogWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="COMPlusComponentManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="comAdminCatalogWrapper">The COMAdminCatalog wrapper.</param>
        public COMPlusComponentManager(ILogger logger, COMAdminCatalogWrapperSingleton comAdminCatalogWrapper)
        {
            _logger = logger;
            _comAdminCatalogWrapper = comAdminCatalogWrapper;
        }

        /// <summary>
        /// Set COM+ component credentials
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public void SetCOMPlusComponentCredentials(string comPlusComponentName, string userName, string password)
        {
            _logger.WriteDebug("Set COM+ component credentials", comPlusComponentName);

            var applications = _comAdminCatalogWrapper.GetApplications();
            foreach (ICatalogObject applicationInstance in applications)
            {
                if (string.Equals(applicationInstance.Name.ToString(), comPlusComponentName,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    applicationInstance.Value["Identity"] = userName;
                    applicationInstance.Value["Password"] = password;
                }

            }
            applications.SaveChanges();

            _logger.WriteVerbose($"Credentials for the COM+ component `{comPlusComponentName}` has been chenged");
        }

        /// <summary>
        /// Gets Com+ component by name
        /// </summary>
        /// <param name="comPlusComponentName"></param>
        /// <returns>Com+ component as <see cref="ICatalogObject" /></returns>
        private ICatalogObject GetComPlusComponentByName(string comPlusComponentName)
        {
            foreach (ICatalogObject applicationInstance in _comAdminCatalogWrapper.GetApplications())
            {
                if (string.Equals(applicationInstance.Name.ToString(), comPlusComponentName,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    return applicationInstance;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Check COM+ component is enabled or not
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="doOutput">Do output.</param>
        /// <returns>State of COM+ component</returns>
        public bool IsCOMPlusComponentEnabled(string comPlusComponentName, bool doOutput = true)
        {
            if (doOutput)
            {
                _logger.WriteDebug("Check COM+ component is enabled or not", comPlusComponentName);
            }

            var applicationInstance = GetComPlusComponentByName(comPlusComponentName);
            var isEnabled = (bool)applicationInstance.Value["IsEnabled"];

            if (doOutput)
            {
                _logger.WriteVerbose(
                    $"COM+ component `{comPlusComponentName}` is {(isEnabled ? "Enabled" : "Disabled")}");
            }

            return isEnabled;
        }

        /// <summary>
        /// Check is Com+ component running or not
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="doOutput">Do output.</param>
        /// <returns>True if COM+ is running</returns>
        public bool IsComPlusComponentRunning(string comPlusComponentName, bool doOutput = true)
        {
            if (doOutput)
            {
                _logger.WriteDebug("Check COM+ component is running or not", comPlusComponentName);
            }

            var applicationInstance = GetComPlusComponentByName(comPlusComponentName);
            
            var appCollection = _comAdminCatalogWrapper.GetApplicationInstances();

            var isRunning = appCollection.Cast<ICatalogObject>().Any(app => applicationInstance.Key.ToString() == app.Value["Application"].ToString());

            if (doOutput)
            {
                _logger.WriteVerbose(
                    $"COM+ component `{comPlusComponentName}` is {(isRunning ? "Running" : "Stopped")}");
            }

            return isRunning;
        }

        /// <summary>
        /// Enable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void EnableCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Enable COM+ component");

            var isEnabled = IsCOMPlusComponentEnabled(comPlusComponentName);

            if (isEnabled)
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` was already enabled");
            }
            else
            {
                var applications = _comAdminCatalogWrapper.GetApplications();
                foreach (ICatalogObject application in applications)
                {
                    if (string.Equals(application.Name.ToString(), comPlusComponentName,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        application.Value["IsEnabled"] = true;
                    }
                }
                var count = int.Parse(applications.SaveChanges().ToString());

                if (count == 1)
                {
                    _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been enabled");
                }
                
            }
        }

        /// <summary>
        /// Disable COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void DisableCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Disable COM+ component");

            var isEnabled = IsCOMPlusComponentEnabled(comPlusComponentName);

            if (isEnabled)
            {
                var applications = _comAdminCatalogWrapper.GetApplications();
                foreach (ICatalogObject application in applications)
                {
                    if (string.Equals(application.Name.ToString(), comPlusComponentName,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        application.Value["IsEnabled"] = false;
                    }
                }

                var count = int.Parse(applications.SaveChanges().ToString());

                if (count == 1)
                {
                    _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been disabled");
                }
            }
            else
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` was already disabled");
            }
        }

        /// <summary>
        /// Shutdown COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void ShutdownCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Shutdown COM+ component");
            if (IsComPlusComponentRunning(comPlusComponentName))
            {
                _comAdminCatalogWrapper.ShutdownApplication(comPlusComponentName);
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been stopped");
            }
            else
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` was already stopped");
            }
        }

        /// <summary>
        /// Starts COM+ components
        /// </summary>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public void StartCOMPlusComponents(string comPlusComponentName)
        {
            _logger.WriteDebug("Shutdown COM+ component");

            if (IsComPlusComponentRunning(comPlusComponentName))
            {
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` was already started");
            }
            else
            {
                _comAdminCatalogWrapper.StartApplication(comPlusComponentName);
                _logger.WriteVerbose($"COM+ component `{comPlusComponentName}` has been started");
            }
        }

        /// <summary>
        /// Gets all COM+ components.
        /// </summary>
        /// <returns>
        /// The list of COM+ components.
        /// </returns>
        public IEnumerable<ISHCOMPlusComponent> GetCOMPlusComponents()
        {
            _logger.WriteDebug("Get COM+ components");

            var components = new List<ISHCOMPlusComponent>();

            foreach (ICatalogObject applicationInstance in _comAdminCatalogWrapper.GetApplications())
            {
                string componentName = applicationInstance.Name.ToString();
                if (componentName.StartsWith("Trisoft"))
                {
                    components.Add(
                        new ISHCOMPlusComponent
                        {
                            Name = componentName,
                            Status = ((bool) applicationInstance.Value["IsEnabled"])
                                ? ISHCOMPlusComponentStatus.Enabled
                                : ISHCOMPlusComponentStatus.Disabled,
                            ActivationType = (ISHCOMPlusActivationType) applicationInstance.Value["Activation"]
                        });
                }
            }

            return components;
        }
    }
}
