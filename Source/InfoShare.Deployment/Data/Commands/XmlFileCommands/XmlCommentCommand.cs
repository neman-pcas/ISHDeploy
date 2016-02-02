﻿using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlCommentCommand : ICommand, IRestorable
    {
        private readonly IEnumerable<string> _commentPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlCommentCommand(ILogger logger, string filePath, IEnumerable<string> commentPatterns)
        {
            _commentPatterns = commentPatterns;

            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }

        public XmlCommentCommand(ILogger logger, string filePath, string commentPattern)
            : this(logger, filePath, new[] {commentPattern})
        { }

        public void Backup()
        {
            _xmlConfigManager.Backup();
        }

        public void Execute()
        {
            foreach (var pattern in _commentPatterns)
            {
                _xmlConfigManager.CommentNode(pattern);
            }
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}