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

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides constants related to ~\Web\InfoShareSTS\Configuration\infoShareSTS.config.
        /// </summary>
        protected class InfoShareSTSConfig
        {
            /// <summary>
            /// The xpath of "infoShareSTS/initialize/@certificateThumbprint" element in ~\Web\InfoShareSTS\Configuration\infoShareSTS.config file
            /// </summary>
            public const string CertificateThumbprintAttributeXPath = "infoShareSTS/initialize/@certificateThumbprint";

            /// <summary>
            /// The xpath of "infoShareSTS/initialize/@authenticationType" element in ~\Web\InfoShareSTS\Configuration\infoShareSTS.config file
            /// </summary>
            public const string AuthenticationTypeAttributeXPath = "infoShareSTS/initialize/@authenticationType";

            /// <summary>
            /// The xpath of "infoShareSTS/initialize/@actorUsername" element in ~\Web\InfoShareSTS\Configuration\infoShareSTS.config file
            /// </summary>
            public const string ActorUsernameAttributeXPath = "infoShareSTS/initialize/@actorUsername";
        }
    }
}