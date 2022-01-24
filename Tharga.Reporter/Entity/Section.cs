using System.Xml;
using Tharga.Reporter.Entity.Area;

namespace Tharga.Reporter.Entity;

public class Section
{
    private readonly Font _globalDefaultFont = new();
    private Font _defaultFont;

    private UnitRectangle _margin;
    private string _name;
    private int? _pageOffset;
    private int? _renderPageCount;

    public Section()
    {
        Header = new Header();
        Pane = new Pane();
        Footer = new Footer();
    }

    public UnitRectangle Margin
    {
        get => _margin ?? Margins.Create(UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0"), UnitValue.Parse("0"));
        set => _margin = value;
    }

    public Header Header { get; private set; }
    public Footer Footer { get; private set; }
    public Pane Pane { get; private set; }

    public string Name
    {
        get => _name ?? string.Empty;
        set => _name = value;
    }

    public Font DefaultFont
    {
        get => _defaultFont ?? _globalDefaultFont;
        set => _defaultFont = value;
    }

    internal XmlElement ToXme()
    {
        var xmd = new XmlDocument();
        var section = xmd.CreateElement("Section");

        var ownerDocument = section.OwnerDocument;
        if (ownerDocument == null) throw new NullReferenceException("ownerDocument");

        if (_name != null)
        {
            section.SetAttribute("Name", Name);
        }

        if (_margin != null)
        {
            var xmeMargin = Margin.ToXme("Margin");
            var importedSection = ownerDocument.ImportNode(xmeMargin, true);
            section.AppendChild(importedSection);
        }

        if (_defaultFont != null)
        {
            var xmeDefaultFont = DefaultFont.ToXme("DefaultFont");
            var importedDefaultFont = ownerDocument.ImportNode(xmeDefaultFont, true);
            section.AppendChild(importedDefaultFont);
        }

        var header = Header.ToXme();
        var importedHeader = ownerDocument.ImportNode(header, true);
        section.AppendChild(importedHeader);

        var pane = Pane.ToXme();
        var importedPane = ownerDocument.ImportNode(pane, true);
        section.AppendChild(importedPane);

        var footer = Footer.ToXme();
        var importedFooter = ownerDocument.ImportNode(footer, true);
        section.AppendChild(importedFooter);

        return section;
    }

    internal static Section Load(XmlElement xmlSection)
    {
        var section = new Section();

        var name = xmlSection.Attributes["Name"];
        if (name != null)
        {
            section.Name = name.Value;
        }

        foreach (XmlElement child in xmlSection)
        {
            switch (child.Name)
            {
                case "Margin":
                    section.Margin = UnitRectangle.Load(child);
                    break;
                case "Header":
                    section.Header = Header.Load(child);
                    break;
                case "Footer":
                    section.Footer = Footer.Load(child);
                    break;
                case "Pane":
                    section.Pane = Pane.Load(child);
                    break;
                case "DefaultFont":
                    section.DefaultFont = Font.Load(child);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to section.", child.Name));
            }
        }

        return section;
    }

    public int GetPageOffset()
    {
        return _pageOffset ?? 0;
    }

    public void SetPageOffset(int pageOffset)
    {
        _pageOffset = pageOffset;
    }

    internal void SetRenderPageCount(int renderPageCount)
    {
        _renderPageCount = renderPageCount;
    }

    internal int GetRenderPageCount()
    {
        return _renderPageCount ?? 1;
    }
}