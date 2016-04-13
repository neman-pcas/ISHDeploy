﻿using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations.ISHUITranslationJob;

namespace ISHDeploy.Cmdlets.ISHUITranslationJob
{
    /// <summary>
    /// <para type="synopsis">Enable translation job for Content Manager deployment.</para>
    /// <para type="description">The Enable-ISHUITranslationJob cmdlet enables translation job for Content Manager deployment.</para>
    /// <para type="link">Disable-ISHUITranslationJob</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHUITranslationJob -ISHDeployment $deployment</code>
    /// <para>This command enables translation job.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHUITranslationJob")]
    public class EnableISHUITranslationJobCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// Cashed value for <see cref="IshPaths"/> property
        /// </summary>
        private ISHPaths _ishPaths;

        /// <summary>
        /// Returns instance of the <see cref="ISHPaths"/>
        /// </summary>
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHUITranslationJobOperation(Logger, IshPaths);

            operation.Run();
        }
    }
}