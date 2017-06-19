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
﻿using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    ///	<para type="description">Represents the installed Content Manager deployment.</para>
    /// </summary>
    public class ISHDeployment
    {
        /// <summary>
        /// Gets the deployment version.
        /// </summary>
        public Version SoftwareVersion { get; set; }

        /// <summary>
        /// Gets the deployment suffix in user-friendly format.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// Gets the web path.
        /// </summary>
        public string WebPath { get; set; }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// Gets the DB type.
        /// </summary>
        public string DatabaseType { get; set; }

        /// <summary>
        /// Gets the name of the access host.
        /// </summary>
        public string AccessHostName { get; set; }
        
        /// <summary>
        /// Gets the name of the CM main url folder.
        /// </summary>
        public string WebAppNameCM { get; set; }

        /// <summary>
        /// Gets the name of the WS main url folder.
        /// </summary>
        public string WebAppNameWS { get; set; }

        /// <summary>
        /// Gets the name of the STS main url folder.
        /// </summary>
        public string WebAppNameSTS { get; set; }
        
        /// <summary>
        /// Gets the web site name.
        /// </summary>
        public string WebSiteName { get; set; }

        /// <summary>
        /// Gets the status of Deployment.
        /// </summary>
        public ISHDeploymentStatus Status { get; set; }
    }
}