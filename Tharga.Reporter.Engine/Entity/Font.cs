using System;
using System.Globalization;
using System.Xml;

namespace Tharga.Reporter.Engine.Entity
{
    public class Font
    {
        private const double SizeEpsilon = 0.01;

        private System.Drawing.Color? _color;

        public string FontName { get; set; }
        public double Size { get; set; }

        internal Font()
        {
            
        }

        internal Font(XmlElement xmeFont)
        {
            if (xmeFont.Attributes.GetNamedItem("FontName") != null)
                FontName = xmeFont.Attributes.GetNamedItem("FontName").Value;

            if (xmeFont.Attributes.GetNamedItem("Size") != null)
                Size = double.Parse(xmeFont.Attributes.GetNamedItem("Size").Value);

            if (xmeFont.Attributes.GetNamedItem("Color") != null)
                _color = System.Drawing.Color.FromArgb(int.Parse(xmeFont.Attributes.GetNamedItem("Color").Value));
        }

        public System.Drawing.Color? Color
        {
            get { return _color == null ? (System.Drawing.Color?)null : _color.Value; }
            set { _color = value; }
        }

        internal string GetRenderName(string defaultClass)
        {
            if (string.IsNullOrEmpty(FontName))
            {
                if (string.IsNullOrEmpty(defaultClass))
                    return "Verdana";

                throw new NotImplementedException();
                //var fontList = Engine._fontClassList.FindAll(itm => string.Compare(itm.ClassName, defaultClass, StringComparison.InvariantCultureIgnoreCase) == 0);
                //if (fontList.Count != 1) throw new InvalidOperationException(String.Format("The template contains {0} definitions of the font class {1}.", fontList.Count, defaultClass));
                //return fontList[0].FontName;
            }
            return FontName;
        }

        internal double GetRenderSize(string defaultClass)
        {
            if (Math.Abs(Size - 0) < SizeEpsilon)
            {
                if (string.IsNullOrEmpty(defaultClass))
                    return 10;
                throw new NotImplementedException();
                //var fontList = Engine._fontClassList.FindAll(itm => string.Compare(itm.ClassName, defaultClass, StringComparison.InvariantCultureIgnoreCase) == 0);
                //if (fontList.Count != 1) throw new InvalidOperationException(String.Format("The template contains {0} definitions of the font class {1}.", fontList.Count, defaultClass));
                //return fontList[0].Size;
            }
            return Size;
        }

        internal System.Drawing.Color GetRenderColor(string defaultClass)
        {
            if (Color == null)
            {
                var result = System.Drawing.Color.Black;
                if (!string.IsNullOrEmpty(defaultClass))
                {
                    throw new NotImplementedException();
                    //var fontList = Engine._fontClassList.FindAll(itm => string.Compare(itm.ClassName, defaultClass, StringComparison.InvariantCultureIgnoreCase) == 0);
                    //if (fontList.Count != 1) throw new InvalidOperationException(String.Format("The template contains {0} definitions of the font class {1}.", fontList.Count, defaultClass));
                    //if (fontList[0].Color != null)
                    //    result = fontList[0].Color.Value;
                }
                return result;
            }
            return Color.Value;
        }

        public XmlNode ToXml()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement("Font");
            xmd.AppendChild(xme);

            if (!string.IsNullOrEmpty(FontName))
                xme.SetAttribute("FontName", FontName);

            if (Math.Abs(Size - 0) > SizeEpsilon)
                xme.SetAttribute("Size", Size.ToString(CultureInfo.InvariantCulture));

            if (_color != null) 
                xme.SetAttribute("Color", _color.Value.ToArgb().ToString(CultureInfo.InvariantCulture));

            return xme;
        }
    }
}