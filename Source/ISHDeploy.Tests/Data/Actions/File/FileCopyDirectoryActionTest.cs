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
﻿using ISHDeploy.Data.Actions.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class FileCopyDirectoryActionTest : BaseUnitTest
    {
		[TestMethod]
		[TestCategory("Actions")]
		public void Execute_Copy_files()
		{
			// Arrange
			var souceFolder = "Source";
			var destinationFolder = "Target";

			// Act
			(new FileCopyDirectoryAction(Logger, souceFolder, destinationFolder)).Execute();

			// Assert
			FileManager.Received(1).CopyDirectoryContent(souceFolder, destinationFolder);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}
	}
}