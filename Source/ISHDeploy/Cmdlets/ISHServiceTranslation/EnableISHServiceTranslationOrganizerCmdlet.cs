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
﻿using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHServiceTranslation;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Enables translation organizer windows service.</para>
    /// <para type="description">The Enable-ISHServiceTranslationOrganizer cmdlet enables translation organizer windows service.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHServiceTranslationOrganizer -ISHDeployment $deployment</code>
    /// <para>This command enables the translation organizer windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHServiceTranslationOrganizer")]
    public sealed class EnableISHServiceTranslationOrganizerCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHServiceTranslationOrganizerOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}