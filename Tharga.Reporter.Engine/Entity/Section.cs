using System;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity
{
    public class Section
    {
        public UnitRectangle Margin { get; set; }
        public Header Header { get; private set; }
        public Footer Footer { get; private set; }
        public Pane Pane { get; private set; }
        public string Name { get; set; }

        public Section()
        {
            Margin = Margins.Create(UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0")); // new UnitRectangle();
            Header = new Header(UnitValue.Parse("0"));
            Pane = new Pane();
            Footer = new Footer(UnitValue.Parse("0"));
        }

        private Section(Margins margin, UnitValue headerSize, UnitValue footerSize)
        {
            Margin = margin;
            Header = new Header(headerSize);
            Pane = new Pane();
            Footer = new Footer(footerSize);
        }

        [Obsolete("Use default constructor and property setters instead.")]
        public static Section Create()
        {
            return new Section(Margins.Create(UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0")),
                UnitValue.Parse("0"), UnitValue.Parse("0"));
        }

        public static Section Create(Margins margin, UnitValue headerSize, UnitValue footerSize)
        {
            return new Section(margin, headerSize, footerSize);
        }

        //internal static Section Create(XmlElement xmlElement)
        //{
        //    var section = new Section();

        //    foreach(XmlElement xmlPane in xmlElement.ChildNodes)
        //    {
        //        switch(xmlPane.Name)
        //        {
        //            case "Header":
        //                var header = new Header(xmlPane);
        //                section.Header = header;
        //                break;
        //            case "Footer":
        //                var footer = new Footer(xmlPane);
        //                section.Footer = footer;
        //                break;
        //            case "Pane":
        //                var pane = new Pane(xmlPane);
        //                section.Pane = pane;
        //                break;
        //        }
        //    }

        //    return section;
        //}

        public void AppendXml(ref XmlElement xmeTemplate)
        {
            if (xmeTemplate == null) throw new ArgumentNullException("xmeTemplate");
            if (xmeTemplate.OwnerDocument == null) throw new ArgumentNullException("xmeTemplate", "xmeTemplate has no owner document.");

            var xmeSection = xmeTemplate.OwnerDocument.CreateElement("Section");
            xmeTemplate.AppendChild(xmeSection);

            Header.AppendXml(ref xmeSection);
            Pane.AppendXml(ref xmeSection);
            Footer.AppendXml(ref xmeSection);
        }
    }
}
