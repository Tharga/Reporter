using System.Globalization;
using System.Xml;
using Tharga.Reporter.Extensions;

namespace Tharga.Reporter.Entity.Util;

public class SkipLine
{
    private UnitValue? _height;
    private int _interval = 3;

    public int Interval
    {
        get => _interval;
        set
        {
            if (value < 1) throw new InvalidOperationException("Interval needs to be larger than zero.");
            _interval = value;
        }
    }

    public UnitValue Height
    {
        get => _height ?? "10px";
        set => _height = value;
    }

    public XmlNode ToXme()
    {
        var xmd = new XmlDocument();
        var xme = xmd.CreateElement(GetType().ToShortTypeName());

        xme.SetAttribute("Interval", Interval.ToString(CultureInfo.InvariantCulture));

        if (_height != null)
        {
            xme.SetAttribute("Height", Height.ToString());
        }

        return xme;
    }

    internal static SkipLine Load(XmlElement xme)
    {
        var skipLine = new SkipLine();

        var xmlInterval = xme.Attributes["Interval"];
        if (xmlInterval != null)
        {
            skipLine.Interval = int.Parse(xmlInterval.Value);
        }

        var xmlHeight = xme.Attributes["Height"];
        if (xmlHeight != null)
        {
            skipLine.Height = xmlHeight.Value;
        }

        return skipLine;
    }
}