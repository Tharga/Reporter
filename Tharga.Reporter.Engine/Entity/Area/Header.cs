using System;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class Header : Pane
    {
        public UnitValue Height { get; set; }

        internal Header()
        {
            Height = null;
        }

        //internal Header(XmlElement xmlPane)
        //    :base(xmlPane)
        //{
        //    if (xmlPane.Attributes.GetNamedItem("Height") != null)
        //        Height = UnitValue.Parse(xmlPane.Attributes.GetNamedItem("Height").Value);
        //}

        internal Header(UnitValue height)
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