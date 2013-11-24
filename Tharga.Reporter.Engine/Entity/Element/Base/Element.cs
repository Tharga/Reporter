using System;
using System.Collections.Generic;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class Element
    {
        private UnitValue? _left;
        private UnitValue? _top;
        private string _name;
        private bool? _isBackground;

        public virtual UnitValue? Top { get { return _top ?? "0"; } set { _top = value; } }
        public virtual UnitValue? Left { get { return _left ?? "0"; } set { _left = value; } }
        public virtual string Name { get { return _name ?? string.Empty; } set { _name = value; } }
        public virtual bool IsBackground { get { return _isBackground ?? false; } set { _isBackground = value; } }

        internal virtual XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            if (_name != null)
                xme.SetAttribute("Name", _name);

            if (_isBackground != null)
                xme.SetAttribute("IsBackground", _isBackground.ToString());

            return xme;
        }

        protected void AppendData(XmlElement xme)
        {
            _name = GetString(xme, "Name");
            _isBackground = GetBool(xme, "IsBackground");
        }

        protected abstract XRect GetBounds(XRect parentBounds);

        protected string GetString(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return val.Value;
            return null;
        }

        protected bool? GetBool(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return bool.Parse(val.Value);
            return null;
        }

        protected UnitValue? GetValue(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return UnitValue.Parse(val.Value);
            return null;
        }

        protected IEnumerable<Element> GetElements(XmlElement xme)
        {
            foreach (XmlElement xmlElement in xme.ChildNodes)
            {
                Element element;
                switch (xmlElement.Name)
                {
                    case "Image":
                        element = Image.Load(xmlElement);
                        break;
                    case "Line":
                        element = Line.Load(xmlElement);
                        break;
                    case "Rectangle":
                        element = Rectangle.Load(xmlElement);
                        break;
                    case "Table":
                        element = Table.Load(xmlElement);
                        break;
                    case "Text":
                        element = Text.Load(xmlElement);
                        break;
                    case "TextBox":
                        element = TextBox.Load(xmlElement);
                        break;
                    case "ReferencePoint":
                        element = ReferencePoint.Load(xmlElement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Cannot parse element {0} as a subelement of pane.", xmlElement.Name));
                }
                yield return element;
            }
        }

    }
}