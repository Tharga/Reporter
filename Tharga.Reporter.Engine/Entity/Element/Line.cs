using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public interface IRenderData
    {
        XRect ParentBounds { get; }
        XRect ElementBounds { get; set; }
        bool IncludeBackground { get; }
        XGraphics Gfx { get; }
    }

    class RenderData : IRenderData
    {
        private readonly XGraphics _gfx;

        public RenderData(XGraphics gfx, XRect parentBounds)
        {
            _gfx = gfx;
            ParentBounds = parentBounds;
        }

        public XRect ParentBounds { get; private set; }
        public XRect ElementBounds { get; set; }
        public bool IncludeBackground { get; private set; }
        public XGraphics Gfx { get { return _gfx; } }
    }

    public sealed class Line : SinglePageAreaElement
    {
        private readonly Color _defaultColor = Color.Black;
        private readonly UnitValue _defaultThickness = "0.1px";

        private Color? _color;
        private UnitValue? _thickness;

        public Color Color { get { return _color ?? _defaultColor; } set { _color = value; } }
        public UnitValue Thickness { get { return _thickness ?? _defaultThickness; } set { _thickness = value; }}

        protected internal override void Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section)
        {
            elementBounds = GetBounds(parentBounds);

            if (includeBackground || !IsBackground)
            {
                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var borderWidth = UnitValue.Parse(Thickness);
                    var pen = new XPen(XColor.FromArgb(Color), borderWidth.GetXUnitValue(0));

                    gfx.DrawLine(pen, elementBounds.Left, elementBounds.Top, elementBounds.Right, elementBounds.Bottom);
                }
            }
        }

        protected internal override void Render(IRenderData renderData)
        {
            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (renderData.IncludeBackground || !IsBackground)
            {
                //using (var gfx = XGraphics.FromPdfPage(page))
                //{
                    var borderWidth = UnitValue.Parse(Thickness);
                    var pen = new XPen(XColor.FromArgb(Color), borderWidth.GetXUnitValue(0));

                    renderData.Gfx.DrawLine(pen, renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Right, renderData.ElementBounds.Bottom);
                //}
            }
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_color != null)
                xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));

            if (_isBackground != null)
                xme.SetAttribute("IsBackground", IsBackground.ToString());

            if (_name != null)
                xme.SetAttribute("Name", Name);

            if (_thickness != null)
                xme.SetAttribute("Thickness", Thickness.ToString());

            return xme;
        }

        internal static Line Load(XmlElement xme)
        {
            var line = new Line();

            line.AppendData(xme);

            var xmlBorderColor = xme.Attributes["Color"];
            if (xmlBorderColor != null)
                line.Color = xmlBorderColor.Value.ToColor();

            var xmlAttribute = xme.Attributes["IsBackground"];
            if (xmlAttribute != null)
                line.IsBackground = bool.Parse(xmlAttribute.Value);

            var xmlName = xme.Attributes["Name"];
            if (xmlName != null)
                line.Name = xmlName.Value;

            var xmlThickness = xme.Attributes["Thickness"];
            if (xmlThickness != null)
                line.Thickness = xmlThickness.Value;

            return line;
        }
    }
}