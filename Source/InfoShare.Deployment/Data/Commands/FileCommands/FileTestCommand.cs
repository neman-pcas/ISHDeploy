﻿using System;
using System.Reflection;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.FileCommands
{
    public class FileTestCommand : ICommand
    {
		private bool _returnResult;
		public FileTestCommand(ILogger logger, string filePath)
        {
			//_xmlConfigManager = new XmlConfigManager(logger, filePath);
			//_returnResult = returnResult;
		}

		public void Execute()
		{
			//_returnResult = true;
		}
	}
}