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
using System.Runtime.InteropServices;
using ISHDeploy.Business.Operations.ISHComponent.ISHServiceTranslation;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHComponent.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Sets configuration of WorldServer.</para>
    /// <para type="description">The Set-ISHIntegrationWorldServer cmdlet sets configuration of WorldServer.</para>
    /// <para type="link">New-ISHIntegrationWorldServerMapping</para>
    /// <para type="link">Set-ISHServiceTranslationOrganizer</para>
    /// <para type="link">Remove-ISHIntegrationWorldServer</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHIntegrationWorldServer -ISHDeployment $deployment -Name "ws1" -Uri "https:\\ws1.sd.com" -Credential $credential -MaximumJobSize 5242880 -RetriesOnTimeout 3 -Mapping $mapping </code>
    /// <para>This command enables the translation organizer windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// Parameter $credential is a set of security credentials, such as a user name and a password.
    /// Parameter $mapping is a object with pair of properties, where ISHLanguage is InfoShare language identifier retrieved from New-ISHIntegrationWorldServerMapping cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationWorldServer")]
    public sealed class SetISHIntegrationWorldServerCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The name (alias) of WorldServer.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name (alias) of WorldServer")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The Uri to WorldServer.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Uri to WorldServer")]
        [ValidateNotNullOrEmpty]
        public string Uri { get; set; }

        /// <summary>
        /// <para type="description">The credential to get access to WorldServer.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The credential to get access to WorldServer")]
        [ValidateNotNullOrEmpty]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// <para type="description">The max value of total size in bytes of uncompressed external job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The max value of total size in bytes of uncompressed external job")]
        public int MaximumJobSize { get; set; } = 5242880;

        /// <summary>
        /// <para type="description">The number of retries on timeout.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The number of retries on timeout.")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 30)]
        public int RetriesOnTimeout { get; set; } = 3;

        /// <summary>
        /// <para type="description">The mapping between trisoftLanguage and worldServerLocaleId.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The mapping between trisoftLanguage and worldServerLocaleId.")]
        [ValidateNotNullOrEmpty]
        public ISHLanguageToWorldServerLocaleIdMapping[] Mappings { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var worldServerConfiguration = new WorldServerConfigurationSection(
                Name,
                Uri,
                Credential.UserName,
                Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password)),
                MaximumJobSize,
                RetriesOnTimeout,
                Mappings);

            var operation = new SetISHIntegrationWorldServerOperation(Logger,
                ISHDeployment, 
                worldServerConfiguration,
                MyInvocation.BoundParameters.ContainsKey("MaximumJobSize"),
                MyInvocation.BoundParameters.ContainsKey("RetriesOnTimeout"),
                "TranslationOrganizer.exe.config already contains settings for WorldServer. You should remove WorldServer configuration section first. To do this you can use Remove-ISHIntegrationWorldServer cmdlet.");

            operation.Run();
        }
    }
}