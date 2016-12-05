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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Replaces placeholders on appropriate values from input parameters
    /// </summary>
    public class FileCopyAndReplacePlaceholdersAction : BaseAction
    {
        /// <summary>
        /// The source file path
        /// </summary>
        private readonly string _sourcePath;

        /// <summary>
        /// The destination file path
        /// </summary>
        private readonly string _destinationPath;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The dictionary with matches between placeholders and input parameters.
        /// </summary>
        private readonly Dictionary<string, string> _matchesDictionary;

        private const string RegexPlaceHolderPattern = @"#!#(.*?)#!#";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyAndReplacePlaceholdersAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="matchesDictionary">The dictionary with matches between placeholders and input parameters.</param>
        public FileCopyAndReplacePlaceholdersAction(ILogger logger, string sourcePath, string destinationPath, Dictionary<string, string> matchesDictionary) 
			: base(logger)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _matchesDictionary = matchesDictionary;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        public override void Execute()
        {
            Logger.WriteDebug("Reading of file", _sourcePath);
            string content = _fileManager.ReadAllText(_sourcePath);

            Match m = Regex.Match(content, RegexPlaceHolderPattern);
            while (m.Success)
            {
                var placeHolder = m.Value.ToLower();
                var key = placeHolder.Replace("installtool:", string.Empty).Replace("#!#", string.Empty);
                if (_matchesDictionary.ContainsKey(key))
                {
                    var value = _matchesDictionary[key];
                    Logger.WriteDebug("Replace placeholder", m.Value, value);
                    content = content.Replace(m.Value, value);
                    Logger.WriteVerbose($"The placeholder {m.Value} has been replaced on {value}");
                }
                else
                {
                    Logger.WriteWarning($"Could not find the input parameter which correspond to placeholder {m.Value}");
                }

                m = m.NextMatch();
            }

            Logger.WriteDebug("Write content to file", _destinationPath);
            _fileManager.WriteAllText(_destinationPath, content);

            Logger.WriteVerbose($"The file {_destinationPath} has been saved");
        }
    }
}
