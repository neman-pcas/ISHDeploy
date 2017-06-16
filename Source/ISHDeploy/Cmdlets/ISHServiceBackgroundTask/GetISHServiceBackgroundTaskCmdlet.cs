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
using System.Linq;
using System.Management.Automation;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHServiceBackgroundTask
{
    /// <summary>
    /// <para type="synopsis">Gets list of windows services for BackgroundTask.</para>
    /// <para type="description">The Get-ISHServiceBackgroundTask cmdlet gets list of BackgroundTask windows services.</para>
    /// <para type="link">Set-ISHServiceBackgroundTask</para>
    /// <para type="link">Enable-ISHServiceBackgroundTask</para>
    /// <para type="link">Disable-ISHServiceBackgroundTask</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHServiceBackgroundTask -ISHDeployment $deployment</code>
    /// <para>This command shows the BackgroundTask windows services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHServiceBackgroundTask -ISHDeployment $deployment -Role "PublishOnly"</code>
    /// <para>This command shows the BackgroundTask windows services with role "PublishOnly".
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHServiceBackgroundTask")]
    public sealed class GetISHServiceBackgroundTaskCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">The role of BackgroundTask services.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The BackgroundTask role")]
        [ValidateNotNullOrEmpty]
        public string Role { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var services = serviceManager.GetServices(ISHDeployment.Name, ISHWindowsServiceType.BackgroundTask);

            if (string.IsNullOrEmpty(Role))
            {
                ISHWriteOutput(services);
            }
            else
            {
                ISHWriteOutput(services.Where(x => string.Equals(x.Role.ToString(), Role, StringComparison.CurrentCultureIgnoreCase)));
            }
        }
    }
}
