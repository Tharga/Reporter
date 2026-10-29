using System.Xml;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Extensions;

namespace Tharga.Reporter.Entity.Util;

internal class TableColumn
{
    internal TableColumn(string displayName, UnitValue? width, Table.WidthMode widthMode, Table.Alignment align, string hideValue)
    {
        if (width == null && widthMode == Table.WidthMode.Specific) throw new InvalidOperationException("When not assigning a specific value for width the width mode cannot be set to specific.");

        DisplayName = displayName;
        Width = width;
        WidthMode = widthMode;
        Align = align;
        HideValue = hideValue;
    }

    public string DisplayName { get; }
    public UnitValue? Width { get; }
    public Table.WidthMode WidthMode { get; }
    public Table.Alignment Align { get; }
    public string HideValue { get; }

    internal XmlElement ToXme()
    {
        var xmd = new XmlDocument();
        var xme = xmd.CreateElement(GetType().ToShortTypeName());

        xme.SetAttribute("DisplayName", DisplayName);
        xme.SetAttribute("Align", Align.ToString());
        if (HideValue != null)
        {
            xme.SetAttribute("HideValue", HideValue);
        }

        if (Width != null)
        {
            xme.SetAttribute("Width", Width.Value.ToString());
        }

        xme.SetAttribute("WidthMode", WidthMode.ToString());

        return xme;
    }

    internal static TableColumn Load(XmlElement xme)
    {
        var displayName = GetRequiredAttribute(xme, "DisplayName");
        var align = (Table.Alignment)Enum.Parse(typeof(Table.Alignment), GetRequiredAttribute(xme, "Align"));

        string hideValue = null;
        if (xme.Attributes["HideValue"] != null)
        {
            hideValue = xme.Attributes["HideValue"].Value;
        }

        UnitValue? width = null;
        if (xme.Attributes["Width"] != null)
        {
            width = UnitValue.Parse(xme.Attributes["Width"].Value);
        }

        var widthMode = (Table.WidthMode)Enum.Parse(typeof(Table.WidthMode), GetRequiredAttribute(xme, "WidthMode"));

        var tableColumn = new TableColumn(displayName, width, widthMode, align, hideValue);
        return tableColumn;
    }

    private static string GetRequiredAttribute(XmlElement xme, string name)
    {
        return xme.Attributes[name]?.Value ?? throw new InvalidOperationException($"The {xme.Name} element has no {name} attribute.");
    }
}