﻿using System.Xml.Linq;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Data.Managers;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.XmlFile
{
    [TestClass]
    public class XmlUncommentActionTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Enable_XOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = this.GetIshFilePath("DisabledXOPUS.xml");

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + testCommentPattern + " END --></BUTTONBAR>");


            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath.AbsolutePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );

            new XmlBlockUncommentAction(Logger, testFilePath, testCommentPattern).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Enable_Enrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = this.GetIshFilePath("DisabledEnrich.xml");

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      testCommentPattern + "--> " +
                                      "</config>");

            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath.AbsolutePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"*/*[local-name()='javascript'][@src='{testSrc}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );

            new XmlNodeUncommentAction(Logger, testFilePath, testCommentPattern).Execute();
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }
    }
}