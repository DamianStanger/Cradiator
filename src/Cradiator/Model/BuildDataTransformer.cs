using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Cradiator.Config;
using Cradiator.Extensions;
using System;
using log4net;

namespace Cradiator.Model
{
    public class BuildDataTransformer : IConfigObserver
    {
        Regex _projectNameRegEx;
        Regex _categoryRegEx;
        Regex _serverNameRegEx;

        public BuildDataTransformer(IConfigSettings configSettings)
        {
            SetLocalValuesFromConfig(configSettings);
            configSettings.AddObserver(this);
        }

        public IEnumerable<ProjectStatus> Transform(string xml)
        {
            if (xml.IsEmpty()) return new List<ProjectStatus>();

            // caters for ccNet v1.5 new xml format for CurrentMessage (messages/message)
            var xElements = XDocument.Parse(xml.Replace('\n', ' ').Trim())
                .Elements("Projects")
                .Elements("Project");

            var projectStatuses = new Collection<ProjectStatus>();
            foreach (var project in xElements)
            {
                var name = project.Attribute("name").GetValue();
                var category = project.Attribute("category").GetValue();
                var serverName = project.Attribute("serverName").GetValue();

                if(!_projectNameRegEx.Match(name).Success ||
                   !_categoryRegEx.Match(category).Success ||
                   !_serverNameRegEx.Match(serverName).Success) continue;

                var currentMessage = project.Attribute("CurrentMessage").GetValue();
                var lastBuildStatus = project.Attribute("lastBuildStatus").GetValue();
                var projectActivity = new ProjectActivity(project.Attribute("activity").GetValue());
                var lastBuildTime = System.Xml.XmlConvert.ToDateTime(project.Attribute("lastBuildTime").GetValue(), System.Xml.XmlDateTimeSerializationMode.Local);

                projectStatuses.Add(new ProjectStatus(name)
                {
                    CurrentMessage = currentMessage,
                    LastBuildStatus = lastBuildStatus,
                    ProjectActivity = projectActivity,
                    ServerName = serverName,
                    LastBuildTime =
                        lastBuildTime
                });
            }

            var query = (from p in projectStatuses
                         
                         join m in
                             (from message in XDocument.Parse(xml)
                               .Elements("Projects")
                               .Elements("Project")
                               .Elements("messages")
                               .Elements("message")
                              where message.Attribute("kind").GetValue() == "Breakers"
                              select new
                              {
                                  Message = message.Attribute("text").GetValue(),
                                  ProjectName = message.Parent.Parent.Attribute("name").GetValue(),
                              }) on p.Name equals m.ProjectName into j
                         
                         from m in j.DefaultIfEmpty()
                         select new ProjectStatus(p.Name)
                         {
                             CurrentMessage = m != null ? m.Message : p.CurrentMessage,
                             LastBuildStatus = p.LastBuildStatus,
                             ProjectActivity = p.ProjectActivity,
                             ServerName = p.ServerName,
                             LastBuildTime = p.LastBuildTime
                         }
                         );


            return query.ToArray();
        }

        public void ConfigUpdated(ConfigSettings newSettings)
        {
            SetLocalValuesFromConfig(newSettings);
        }

        void SetLocalValuesFromConfig(IConfigSettings newSettings)
        {
            _projectNameRegEx = new Regex(newSettings.ProjectNameRegEx);
            _categoryRegEx = new Regex(newSettings.CategoryRegEx);
            _serverNameRegEx = new Regex(newSettings.ServerNameRegEx);
        }
    }
}