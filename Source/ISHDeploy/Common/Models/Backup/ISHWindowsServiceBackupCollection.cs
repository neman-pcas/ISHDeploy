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

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ISHDeploy.Common.Models.Backup
{
    /// <summary>
    /// <para type="description">Represents collection of services with backup properties.</para>
    /// </summary>
    [Serializable]
    [XmlRoot("ISHWindowsServices", Namespace = "")]
    public class ISHWindowsServiceBackupCollection
    {
        /// <summary>
        /// List of services
        /// </summary>
        [XmlElement("ISHWindowsServiceBackup", Namespace = "")]
        public List<ISHWindowsServiceBackup> Services { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHWindowsServiceBackupCollection"/> class.
        /// </summary>
        public ISHWindowsServiceBackupCollection()
        {
            Services = new List<ISHWindowsServiceBackup>();
        }
    }
}