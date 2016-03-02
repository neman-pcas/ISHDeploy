﻿using System;
using System.Text;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;

namespace InfoShare.Deployment.Cmdlets
{
    /// <summary>
    /// Base cmdlet class that writes cmdlet usage into history info
    /// </summary>
    public abstract class BaseHistoryEntryCmdlet : BaseCmdlet
    {
        private string CurrentDateTime => DateTime.Now.ToString("yyyyMMdd HH:mm");

        /// <summary>
        /// Deployment Suffix
        /// </summary>
        protected abstract ISHPaths IshPaths { get; }
        
        /// <summary>
        /// Overrides ProcessRecord from Cmdlet class
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            AddHistoryEntry();
        }

        /// <summary>
        /// Appends new record to history file
        /// </summary>
        private void AddHistoryEntry()
        {
            if (IshPaths == null)
            {
                throw new ArgumentException($"{nameof(IshPaths)} in {nameof(BaseHistoryEntryCmdlet)} cannot be null.");
            }

            // don't log if cmdlet was executed with WhatIf parameter
            if (MyInvocation.BoundParameters.ContainsKey("WhatIf"))
            {
                return;
            }

            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            var historyEntry = new StringBuilder();
            
            if (!fileManager.Exists(IshPaths.HistoryFilePath))
            {
                historyEntry.AppendLine($"# {CurrentDateTime}");
                historyEntry.Append($"$deployment = Get-ISHDeployment -Deployment '{IshPaths.DeploymentSuffix}'");
            }
            
            historyEntry.AppendLine($"# {CurrentDateTime}");
            historyEntry.Append(InvocationLine);

            fileManager.AppendLine(IshPaths.HistoryFilePath, historyEntry.ToString());
        }

        /// <summary>
        /// Describes which cmdlet was executed with which parameters
        /// </summary>
        protected virtual string InvocationLine
        {
            get
            {
                var strBldr = new StringBuilder(MyInvocation.MyCommand.Name);

                foreach (var boundParameter in MyInvocation.BoundParameters)
                {
                    if (string.Compare(boundParameter.Key, "ISHDeployment", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        strBldr.Append($" -{boundParameter.Key} $deployment");
                        continue;
                    }

                    strBldr.Append($" -{boundParameter.Key} {boundParameter.Value}");
                }

                return strBldr.ToString();
            }
        }
    }
}