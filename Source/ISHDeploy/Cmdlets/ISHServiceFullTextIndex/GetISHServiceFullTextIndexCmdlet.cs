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

using System.Management.Automation;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHServiceFullTextIndex
{
    /// <summary>
    /// <para type="synopsis">Gets list of windows services for SolrLucene.</para>
    /// <para type="description">The Get-ISHServiceFullTextIndex cmdlet gets list of SolrLucene windows services.</para>
    /// <para type="link">Set-ISHServiceFullTextIndex</para>
    /// <para type="link">Enable-ISHServiceFullTextIndex</para>
    /// <para type="link">Disable-ISHServiceFullTextIndex</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHServiceFullTextIndex -ISHDeployment $deployment</code>
    /// <para>This command shows the SolrLucene windows services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHServiceFullTextIndex")]
    public sealed class GetISHServiceFullTextIndexCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var services = serviceManager.GetServices(ISHDeployment.Name, ISHWindowsServiceType.SolrLucene);

            ISHWriteOutput(services);
        }
    }
}