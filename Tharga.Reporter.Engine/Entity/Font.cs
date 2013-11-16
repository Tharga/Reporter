using System;
using System.Drawing;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity
{
    public class Font : IEquatable<Font>
    {
        private readonly Color _defaultColor = Color.Black;
        private const double SizeEpsilon = 0.01;

        private string _fontName;
        private double? _size;
        private Color? _color;

        public string FontName { get { return _fontName ?? string.Empty; } set { _fontName = value; } }
        public double Size { get { return _size ?? 10; } set { _size = value; } }
        public Color Color { get { return _color ?? _defaultColor; } set { _color = value; } }

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
            if (_color == null)
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
            return Color;
        }

        internal XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            if (_color != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));

            if (_fontName != null)
                xme.SetAttribute("FontName", _fontName);

            if (_size != null)
                xme.SetAttribute("Size", _size.ToString());

            return xme;
        }

        public static Font Load(XmlElement xme)
        {
            var line = new Font();

            var xmlBorderColor = xme.Attributes["Color"];
            if (xmlBorderColor != null)
                line.Color = xmlBorderColor.Value.ToColor();

            var xmlFontName = xme.Attributes["FontName"];
            if (xmlFontName != null)
                line.FontName = xmlFontName.Value;

            var xmlSize = xme.Attributes["Size"];
            if (xmlSize != null)
                line.Size = double.Parse(xmlSize.Value);

            return line;
        }

        public bool Equals(Font other)
        {
            return true;
        }
    }
}