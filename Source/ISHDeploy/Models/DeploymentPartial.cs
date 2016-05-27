﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Models
{
    class DeploymentPartial
    {
        /// <summary>
        /// This class is used to make partial view to output some parameters from main ISHDeployment class.
        /// </summary>
        /// <param name="iSHDeployment">Main class to get information from.</param>
        public DeploymentPartial(ISHDeployment iSHDeployment)
        {
            SoftwareVersion = iSHDeployment.SoftwareVersion.ToString();
            Name = iSHDeployment.Name;
            AppPath = iSHDeployment.AppPath;
            WebPath = iSHDeployment.WebPath;
            DataPath = iSHDeployment.DataPath;
            ConnectString = iSHDeployment.ConnectString;
            DatabaseType = iSHDeployment.DatabaseType;
            WebAppNameCM = iSHDeployment.GetCMWebAppName();
            WebAppNameWS = iSHDeployment.GetWSWebAppName();
            WebAppNameSTS = iSHDeployment.GetSTSWebAppName();
            AccessHostName = iSHDeployment.AccessHostName;
        }
        public String SoftwareVersion { get; set; }
        public String Name { get; set; }
        public String AppPath { get; set; }
        public String WebPath { get; set; }
        public String DataPath { get; set; }
        public String ConnectString { get; set; }
        public String DatabaseType { get; set; }
        public String WebAppNameCM { get; set; }
        public String WebAppNameWS { get; set; }
        public String WebAppNameSTS { get; set; }
        public String AccessHostName { get; set; }
    }
}
