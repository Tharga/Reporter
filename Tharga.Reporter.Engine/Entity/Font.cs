using System.Drawing;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity
{
    public class Font
    {
        private readonly Color _defaultColor = Color.Black;

        private string _fontName;
        private double? _size;
        private Color? _color;

        public string FontName { get { return _fontName ?? string.Empty; } set { _fontName = value; } }
        public double Size { get { return _size ?? 10; } set { _size = value; } }
        public Color Color { get { return _color ?? _defaultColor; } set { _color = value; } }

        internal XmlElement ToXme(string elementName = null)
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(elementName ?? GetType().ToShortTypeName());

            if (_color != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));

            if (_fontName != null)
                xme.SetAttribute("FontName", _fontName);

            if (_size != null)
                xme.SetAttribute("Size", _size.ToString());

            return xme;
        }

        internal static Font Load(XmlElement xme)
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
    }
}