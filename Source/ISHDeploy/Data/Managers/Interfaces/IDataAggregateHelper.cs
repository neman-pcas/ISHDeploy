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

using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Aggregates data from different places.
    /// </summary>
    public interface IDataAggregateHelper
    {
        /// <summary>
        /// Returns input parameters
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>InputParameters containing all parameters from InputParameters.xml file for specified deployment</returns>
        InputParameters GetInputParameters(string deploymentName);

        /// <summary>
        /// Returns all components of deployment
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>The collection of components for specified deployment</returns>
        ISHComponentsCollection GetComponents(string deploymentName);

        /// <summary>
        /// Save all components of deployment
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <param name="collection">The collection of components for specified deployment</param>
        void SaveComponents(string filePath, ISHComponentsCollection collection);

        /// <summary>
        /// Returns all components of deployment which were saved in a file 
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <returns>The collection of components readed from file</returns>
        ISHComponentsCollection ReadComponentsFromFile(string filePath);

        /// <summary>
        /// Returns all windows services with all properties needed for their recreation
        /// </summary>
        /// <param name="deploymentName">The name of deployment.</param>
        /// <returns>The collection of windows services with all properties needed for their recreation</returns>
        ISHWindowsServiceBackupCollection GetISHWindowsServiceBackupCollection(string deploymentName);
    }
}
