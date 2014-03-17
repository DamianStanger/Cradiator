using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Cradiator.Config
{
    public class ViewSettingsParser
    {
        const string ProjectRegex = "project-regex";
        const string CategoryRegex = "category-regex";
        const string ServerRegex = "server-regex";
        const string Url = "url";
        const string Username = "build-agent-username";
        const string Password = "build-agent-password";
        const string Skin = "skin";
        const string ViewName = "name";
        const string ShowOnlyBroken = "showOnlyBroken";
        const string ShowServerName = "showServerName";
        const string ShowOutOfDate = "showOutOfDate";
        const string OutOfDateDifferenceInMinutes = "outOfDateDifferenceInMinutes";


        readonly XDocument _xdoc;

        public ViewSettingsParser(TextReader xml)
        {
            _xdoc = XDocument.Parse(xml.ReadToEnd());
        }

        public static ICollection<ViewSettings> Read(string xmlFile)
        {
            using (var stream = new StreamReader(xmlFile))
            {
                var reader = new ViewSettingsParser(stream);
                return reader.ParseXml();
            }
        }

        public ICollection<ViewSettings> ParseXml()
        {
            var xElements = _xdoc.Elements("configuration").Elements("views").Elements("view");
            var viewSettingses = new Collection<ViewSettings>();
            foreach (var element in xElements)
            {
                var outOfDateDifferenceInMinutes = int.Parse(element.Attribute(OutOfDateDifferenceInMinutes).Value);
                var showOutOfDate = bool.Parse(element.Attribute(ShowOutOfDate).Value);
                var showServerName = bool.Parse(element.Attribute(ShowServerName).Value);
                var showOnlyBroken = bool.Parse(element.Attribute(ShowOnlyBroken).Value);
                var viewName = element.Attribute(ViewName).Value;
                var skinName = element.Attribute(Skin).Value;
                var serverNameRegEx = element.Attribute(ServerRegex).Value;
                var categoryRegEx = element.Attribute(CategoryRegex).Value;
                var projectNameRegEx = element.Attribute(ProjectRegex).Value;
                var url = element.Attribute(Url).Value;
                var username = element.Attribute(Username).Value;
                var password = element.Attribute(Password).Value;
                var viewSettings = new ViewSettings
                {
                    URL = url,
                    BuildAgentUsername = username,
                    BuildAgentPassword = password,
                    ProjectNameRegEx = projectNameRegEx,
                    CategoryRegEx = categoryRegEx,
                    ServerNameRegEx = serverNameRegEx,
                    SkinName = skinName,
                    ViewName = viewName,
                    ShowOnlyBroken = showOnlyBroken,
                    ShowServerName = showServerName,
                    ShowOutOfDate = showOutOfDate,
                    OutOfDateDifferenceInMinutes = outOfDateDifferenceInMinutes
                };
                viewSettingses.Add(viewSettings);
            }

            return new ReadOnlyCollection<ViewSettings>(viewSettingses);
        }

        //-----
        // modify functionality (below) is only for the settings dialog save functionality
        //-----

        public static void Modify(string xmlFile, ViewSettings viewSettings)
        {
            string xmlUpdated;
            using (var stream = new StreamReader(xmlFile))
            {
                var parser = new ViewSettingsParser(stream);
                xmlUpdated = parser.CreateUpdatedXml(viewSettings);
            }
            using (var stream = new StreamWriter(xmlFile))
            {
                stream.Write(xmlUpdated);
            }
        }

        public string CreateUpdatedXml(IViewSettings settings)
        {
            var view1 = _xdoc.Elements("configuration")
                             .Elements("views")
                             .Elements("view").First(); // only used to update a view when there is 1

            view1.Attribute(Url).Value = settings.URL;
            view1.Attribute(Username).Value = settings.BuildAgentUsername;
            view1.Attribute(Password).Value = settings.BuildAgentPassword;
            view1.Attribute(ProjectRegex).Value = settings.ProjectNameRegEx;
            view1.Attribute(CategoryRegex).Value = settings.CategoryRegEx;
            view1.Attribute(ServerRegex).Value = settings.ServerNameRegEx;
            view1.Attribute(Skin).Value = settings.SkinName;
            view1.Attribute(ViewName).Value = settings.ViewName;
            view1.Attribute(ShowOnlyBroken).Value = settings.ShowOnlyBroken.ToString();
            view1.Attribute(ShowServerName).Value = settings.ShowServerName.ToString();
            view1.Attribute(ShowOutOfDate).Value = settings.ShowOutOfDate.ToString();
            view1.Attribute(OutOfDateDifferenceInMinutes).Value = settings.OutOfDateDifferenceInMinutes.ToString();

            var xml = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(xml, new XmlWriterSettings
                                    {
                                        OmitXmlDeclaration = true,
                                        NewLineHandling = NewLineHandling.None,
                                        Indent = true,
                                    }))
            {
                _xdoc.WriteTo(xmlWriter);
            }
            return xml.ToString();
        }
    }
}