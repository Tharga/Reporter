using System.Xml;

namespace Tharga.Reporter.Entity.Area;

public class Footer : Pane
{
    private UnitValue? _height;

    internal Footer()
    {
    }

    public UnitValue Height
    {
        get => _height ?? UnitValue.Parse("0");
        set => _height = value;
    }

    public override XmlElement ToXme()
    {
        var xmd = new XmlDocument();
        var header = xmd.CreateElement("Footer");

        if (_height != null)
        {
            header.SetAttribute("Height", Height.ToString());
        }

        var elms = GetElements(xmd);
        foreach (var elm in elms)
        {
            header.AppendChild(elm);
        }

        return header;
    }

    public new static Footer Load(XmlElement xme)
    {
        var pane = new Footer();

        var xmeHeight = xme.Attributes["Height"];
        if (xmeHeight != null)
        {
            pane.Height = UnitValue.Parse(xmeHeight.Value);
        }

        var elms = GetElements(xme);
        pane.ElementList.AddRange(elms);

        return pane;
    }
}