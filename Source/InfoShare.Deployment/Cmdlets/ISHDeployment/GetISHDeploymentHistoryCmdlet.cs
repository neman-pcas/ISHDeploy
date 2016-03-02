﻿using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Get, "ISHDeploymentHistory")]
    public class GetISHDeploymentHistoryCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        private ISHPaths _ishPaths;
        protected ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment));

        public override void ExecuteCmdlet()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            var historyContent = fileManager.ReadAllText(IshPaths.HistoryFilePath);

            WriteObject(historyContent);
        }
    }
}