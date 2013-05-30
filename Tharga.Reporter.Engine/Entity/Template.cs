using System;
using System.Collections.Generic;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity
{
    public class Template
    {
        public List<Section> SectionList = new List<Section>();
        internal List<FontClass> FontClassList = new List<FontClass>();

        private Template()
        {

        }

        public static Template Create(Section section)
        {
            var template = new Template();
            template.SectionList.Add(section);
            return template;
        }

        public static Template Create(XmlDocument xmlDocument)
        {
            var template = new Template();

            foreach (XmlElement element in xmlDocument)
            {
                if (element.Name != "Template") throw new InvalidOperationException(string.Format("Template cannot parse element of type {0}.", element.Name));
                //var version = element.GetAttribute("Version");
                //switch (version)
                //{
                //    case "1":
                        foreach (XmlElement xmlSection in element.ChildNodes)
                        {
                            if (xmlSection.Name != "Section") throw new InvalidOperationException(string.Format("section cannot parse element of type {0}.", xmlSection.Name));
                            var section = Section.Create(xmlSection);
                            template.SectionList.Add(section);
                        }
                //        break;
                //    default:
                //        throw new ArgumentOutOfRangeException(string.Format("Cannot handle templates of version {0}.", version));
                //}
            }

            return template;
        }

        public XmlDocument ToXml()
        {
            var xmd = new XmlDocument();

            var xmeTemplate = xmd.CreateElement("Template");
            //xmeTemplate.SetAttribute("Version", "1");
            xmd.AppendChild(xmeTemplate);

            foreach (var section in SectionList)
                section.AppendXml(ref xmeTemplate);

            return xmd;
        }
    }
}