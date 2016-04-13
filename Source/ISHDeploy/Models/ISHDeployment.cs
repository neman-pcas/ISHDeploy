﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ISHDeploy.Models
{
    /// <summary>
    /// Represents the installed Content Manager deployment.
    /// </summary>
    public class ISHDeployment
    {
        /// <summary>
        /// The HTTPS prefix
        /// </summary>
        private const string HttpsPrefix = "https://";

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeployment"/> class.
        /// </summary>
        /// <param name="parameters">The dictionary with all parameters from inputparameter.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeployment(Dictionary<string, string> parameters, Version softwareVersion)
        {
            OriginalParameters = parameters;
            SoftwareVersion = softwareVersion;
        }

        /// <summary>
        /// Gets the all parameters from inputparameter.xml file.
        /// </summary>
        public Dictionary<string, string> OriginalParameters { get; }

        /// <summary>
        /// Gets the deployment version.
        /// </summary>
        public Version SoftwareVersion { get; }

        /// <summary>
        /// Gets the deployment suffix in user-friendly format.
        /// </summary>
        public string Name => $"InfoShare{GetSuffix()}";

        /// <summary>
        /// Gets the application path.
        /// </summary>
        public string AppPath => OriginalParameters["apppath"];

        /// <summary>
        /// Gets the web path.
        /// </summary>
        public string WebPath => OriginalParameters["webpath"];

        /// <summary>
        /// Gets the data path.
        /// </summary>
        public string DataPath => OriginalParameters["datapath"];

        /// <summary>
        /// Gets the DB connection string.
        /// </summary>
        public string ConnectString => OriginalParameters["connectstring"];

        /// <summary>
        /// Gets the DB type.
        /// </summary>
        public string DatabaseType => OriginalParameters["databasetype"];

        /// <summary>
        /// Gets the path to the Author folder.
        /// </summary>
        public string WebNameCM => Path.Combine(GetAuthorFolderPath(), "Author");

        /// <summary>
        /// Gets the path to the InfoShareWS folder.
        /// </summary>
        public string WebNameWS => Path.Combine(GetAuthorFolderPath(), "InfoShareWS");

        /// <summary>
        /// Gets the path to the InfoShareSTS folder.
        /// </summary>
        public string WebNameSTS => Path.Combine(GetAuthorFolderPath(), "InfoShareSTS");

        /// <summary>
        /// Gets the name of the access host.
        /// </summary>
        public string AccessHostName => OriginalParameters["baseurl"].Substring(HttpsPrefix.Length);

        /// <summary>
        /// Gets the path to the Web+Suffix Author folder.
        /// </summary>
        public string GetAuthorFolderPath() => Path.Combine(WebPath, $"Web{GetSuffix()}");

        /// <summary>
        /// Gets the path to the App+Suffix Author folder.
        /// </summary>
        public string GetAppFolderPath() => Path.Combine(AppPath, $"App{GetSuffix()}");

        /// <summary>
        /// Gets the path to the Data+Suffix Author folder.
        /// </summary>
        public string GetDataFolderPath() => Path.Combine(AppPath, $"Data{GetSuffix()}");

        /// <summary>
        /// Gets the deployment suffix.
        /// </summary>
        public string GetSuffix() => OriginalParameters["projectsuffix"];
    }
}