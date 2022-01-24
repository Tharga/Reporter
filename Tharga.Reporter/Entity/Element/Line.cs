using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Element.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public sealed class Line : SinglePageAreaElement
{
    private readonly Color _defaultColor = Color.Black;
    private readonly UnitValue _defaultThickness = "0.1px";

    private Color? _color;
    private UnitValue? _thickness;

    public Color Color
    {
        get => _color ?? _defaultColor;
        set => _color = value;
    }

    public UnitValue Thickness
    {
        get => _thickness ?? _defaultThickness;
        set => _thickness = value;
    }

    internal override void Render(IRenderData renderData)
    {
        if (IsNotVisible(renderData)) return;

        renderData.ElementBounds = GetBounds(renderData.ParentBounds);

        if (renderData.IncludeBackground || !IsBackground)
        {
            var borderWidth = UnitValue.Parse(Thickness);
            //TODO: var pen = new XPen(XColor.FromArgb(Color), borderWidth.GetXUnitValue(0));
            var pen = new XPen(XColor.FromKnownColor(XKnownColor.Black));

            renderData.Graphics.DrawLine(pen, renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Right, renderData.ElementBounds.Bottom);
        }
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_color != null)
        {
            xme.SetAttribute("Color", string.Format("{0}{1}{2}", _color.Value.R.ToString("X2"), _color.Value.G.ToString("X2"), _color.Value.B.ToString("X2")));
        }

        if (_thickness != null)
        {
            xme.SetAttribute("Thickness", Thickness.ToString());
        }

        return xme;
    }

    internal static Line Load(XmlElement xme)
    {
        var line = new Line();

        line.AppendData(xme);

        var xmlBorderColor = xme.Attributes["Color"];
        if (xmlBorderColor != null)
        {
            line.Color = xmlBorderColor.Value.ToColor();
        }

        var xmlAttribute = xme.Attributes["IsBackground"];
        if (xmlAttribute != null)
        {
            line.IsBackground = bool.Parse(xmlAttribute.Value);
        }

        var xmlName = xme.Attributes["Name"];
        if (xmlName != null)
        {
            line.Name = xmlName.Value;
        }

        var xmlThickness = xme.Attributes["Thickness"];
        if (xmlThickness != null)
        {
            line.Thickness = xmlThickness.Value;
        }

        return line;
    }
}