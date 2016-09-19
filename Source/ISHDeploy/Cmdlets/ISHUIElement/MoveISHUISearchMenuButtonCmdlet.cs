﻿/*
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

using ISHDeploy.Business.Enums;
using ISHDeploy.Business.Operations.ISHUIElement;
using ISHDeploy.Models.UI;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIElement
{

    /// <summary>
    ///		<para type="synopsis">Manipulates with definitions in SearchMenuBar.</para>
    ///		<para type="description">The Move-ISHUISearchMenuButton cmdlet moves Buttons definitions in Content Manager deployment.</para>
    ///		<para type="link">Set-ISHUISearchMenuButton</para>
    ///		<para type="link">Remove-ISHUISearchMenuButton</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Move-ISHUISearchMenuButton -ISHDeployment $deployment -Label "Publish" -First</code>
    ///		<para>Moves definition of the "Publish" to the top.</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Move-ISHUISearchMenuButton -ISHDeployment $deployment -Label "Publish" -Last</code>
    ///		<para>Moves definition of the "Publish" to the bottom.</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Move-ISHUISearchMenuButton -ISHDeployment $deployment -Label "Translation" -After "Publish"</code>
    ///		<para>Moves definition of the "Translation" after "Publish".</para> 
    /// </example>
    /// <para>This command manipulates XML definitions nodes in SearchMenuBar.
    ///		Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// </para>
    [Cmdlet(VerbsCommon.Move, "ISHUISearchMenuButton")]
    public sealed class MoveISHUISearchMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Label of menu item.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }
        
        /// <summary>
		/// <para type="description">Menu item move to the last position.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move to the last position", ParameterSetName = "Last")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Last { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the first position.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Menu item move to the first position", ParameterSetName = "First")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter First { get; set; }

        /// <summary>
        /// <para type="description">Menu item move position.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu item move position", ParameterSetName = "After")]
        [ValidateNotNullOrEmpty]
        public string After { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            MoveElementDirection operationType;
            switch (ParameterSetName)
            {
                case "Last":
                    operationType = MoveElementDirection.Last;
                    break;
                case "First":
                    operationType = MoveElementDirection.First;
                    break;
                case "After":
                    operationType = MoveElementDirection.After;
                    break;
                default:
                    throw new System.ArgumentException($"Operation type in {nameof(MoveISHUISearchMenuButtonCmdlet)} should be defined.");
            }

            var model = new SearchMenuItem(Label);
            var operation = new MoveUIElementOperation(Logger, ISHDeployment, model, operationType, After);
            operation.Run();
        }
    }
}
