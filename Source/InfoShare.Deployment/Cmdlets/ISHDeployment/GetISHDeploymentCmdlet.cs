﻿using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHDeployment;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Get, "ISHDeployment")]
    public class GetISHDeploymentCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "Suffix of the already deployed Content Manager instance")]
        [Alias("Suffix")]
        public string Deployment { get; set; }

        public override void ExecuteCmdlet()
        {
            var cmdset = new GetISHDeploymentCmdSet(this, Deployment);

            var result = cmdset.Run();

            WriteObject(result);
        }
    }
}