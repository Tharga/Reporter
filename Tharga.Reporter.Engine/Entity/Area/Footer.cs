using System;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class Footer : Pane
    {
        public UnitValue Height { get; set; }

        internal Footer()
        {

        }

        internal Footer(XmlElement xmlPane)
            : base(xmlPane)
        {
            if (xmlPane.Attributes.GetNamedItem("Height") != null)
                Height = UnitValue.Parse(xmlPane.Attributes.GetNamedItem("Height").Value);
        }

        internal Footer(UnitValue height)
        {
            Height = height;
        }

        internal override XmlElement AppendXml(ref XmlElement xmeSection)
        {
            var xmePane = base.AppendXml(ref xmeSection);

            if (Height != null && Math.Abs(Height.Value - 0) > HeightEpsilon) 
                xmePane.SetAttribute("Height", Height.ToString());

            return xmePane;
        }

    }
}