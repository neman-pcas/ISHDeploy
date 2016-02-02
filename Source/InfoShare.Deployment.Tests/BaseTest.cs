﻿using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Interfaces;
using NSubstitute;

namespace InfoShare.Deployment.Tests
{
    public abstract class  BaseTest
    {
        public const string TestProjectPath = @".\TestData";
        public ILogger Logger = Substitute.For<ILogger>();

        public const string XPathFolderButtonbarCheckOutWithXopusButton = "BUTTONBAR/BUTTON/INPUT[@NAME='CheckOutWithXopus']";
        public const string XPathFolderButtonbarUndoCheckOutButton = "BUTTONBAR/BUTTON/INPUT[@NAME='undoCheckOut']";

        public string GetPathToFile(string relativeFilePath)
        {
            return Path.Combine(TestProjectPath, relativeFilePath);
        }

        public XElement GetXElementByXPath(string filePath, string xpath)
        {
            var doc = XDocument.Load(filePath);
            return doc.XPathSelectElement(xpath);
        }
    }
}
