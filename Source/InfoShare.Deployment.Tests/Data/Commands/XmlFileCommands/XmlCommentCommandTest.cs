﻿using System.Xml.Linq;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Data.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Commands.XmlFileCommands
{
    [TestClass]
    public class XmlCommentCommandTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void Execute_DisableXOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<!-- " + testCommentPattern + " START --><BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON><!-- " + testCommentPattern + " END -->" +
                                    "</BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNull(element, "Uncommented node is null");
                    }
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlBlockCommentCommand(Logger, testFilePath, testCommentPattern).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void Execute_DisableEnrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='" + testSrc + "']";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval=\"false\" phase=\"Xopus\" />" +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, testXPath);
                        Assert.IsNull(element, "Comment command doesn't work");
                    }
                );

            new XmlNodeCommentCommand(Logger, testFilePath, testXPath).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }
    }
}