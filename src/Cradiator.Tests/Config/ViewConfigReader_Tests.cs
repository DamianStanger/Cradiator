using System.IO;
using System.Linq;
using Cradiator.Config;
using NUnit.Framework;
using Shouldly;

namespace Cradiator.Tests.Config
{
    [TestFixture]
    public class ViewConfigReader_Tests
    {
        ViewSettingsParser _parser;

        const string XML = "<configuration>" +
                                "<views>" +
                                    @"<view url=""http://url1"" " +
                                        @"build-agent-username=""foogo"" " +
                                        @"build-agent-password=""Bargo"" " +
                                        @"skin=""Grid"" " +
                                        @"project-regex=""v5.*"" " +
                                        @"category-regex="".*"" " +
                                        @"server-regex="".*"" " +
                                        @"name=""Scoring"" " +
                                        @"showOnlyBroken=""false"" " +
                                        @"showServerName=""false"" " +
                                        @"showOutOfDate=""false"" " +
                                        @"outOfDateDifferenceInMinutes=""0"" />" +
                                "</views>" +
                            "</configuration>";

        [SetUp]
        public void SetUp()
        {
            _parser = new ViewSettingsParser(new StringReader(XML));
        }

        [Test]
        public void can_read_view_from_xml()
        {
            var views = _parser.ParseXml();
            views.Count().ShouldBe(1);

            var view1 = views.First();

            view1.URL.ShouldBe("http://url1");
            view1.BuildAgentUsername.ShouldBe("foogo");
            view1.BuildAgentPassword.ShouldBe("Bargo");
            view1.SkinName.ShouldBe("Grid");
            view1.ProjectNameRegEx.ShouldBe("v5.*");
            view1.CategoryRegEx.ShouldBe(".*");
        }

        [Test]
        public void can_read_then_write_modified_view_to_xml()
        {
            var views = _parser.ParseXml();
            var xmlModified = _parser.CreateUpdatedXml(new ViewSettings
                              {
                                URL = "http://new",
                                BuildAgentUsername = "go",
                                BuildAgentPassword = "foo",
                                ProjectNameRegEx = "[a-z]",  
                                CategoryRegEx = "[1-9]",  
                                SkinName = "StackPhoto",  
                                ServerNameRegEx = "",
                                OutOfDateDifferenceInMinutes = 1,
                                ShowOnlyBroken = true,
                                ShowOutOfDate = true,
                                ShowServerName = true,
                                ViewName = "foobar"
                              });

            _parser = new ViewSettingsParser(new StringReader(xmlModified));

            views = _parser.ParseXml();
            var view1 = views.First();

            view1.URL.ShouldBe("http://new");
            view1.BuildAgentUsername.ShouldBe("go");
            view1.BuildAgentPassword.ShouldBe("foo");
            view1.SkinName.ShouldBe("StackPhoto");
            view1.ProjectNameRegEx.ShouldBe("[a-z]");
            view1.CategoryRegEx.ShouldBe("[1-9]");
        }
    }
}