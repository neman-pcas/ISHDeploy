﻿using System.Xml.Linq;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Data.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Commands.XmlFileCommands
{
    [TestClass]
    public class XmlUncommentCommandTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void Execute_EnableXOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + testCommentPattern + " END --></BUTTONBAR>");


            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlBlockUncommentCommand(Logger, testFilePath, testCommentPattern).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void Execute_EnableEnrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      testCommentPattern + "--> " +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"*/*[local-name()='javascript'][@src='{testSrc}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );


            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlNodeUncommentCommand(Logger, testFilePath, testCommentPattern).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }
    }
}