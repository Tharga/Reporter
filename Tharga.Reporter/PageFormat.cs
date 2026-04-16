using PdfSharp;
using PdfSharp.Drawing;

namespace Tharga.Reporter;

public record PageFormat
{
    private readonly PageSize? _pageFormat;
    private readonly XSize? _size;

    /// <summary>
    /// Page paper size.
    /// </summary>
    /// <param name="pageFormat"></param>
    public PageFormat(PageSize pageFormat)
    {
        _pageFormat = pageFormat;
    }

    private PageFormat(XSize customSize)
    {
        _size = customSize;
    }

    /// <summary>
    /// Custom page size in millimeters.
    /// </summary>
    /// <param name="customWidth">Width in millimeters</param>
    /// <param name="customHeight">Height in millimeters</param>
    public PageFormat(int customWidth, int customHeight)
    {
        var width = new XUnit(customWidth, XGraphicsUnit.Millimeter);
        var height = new XUnit(customHeight, XGraphicsUnit.Millimeter);

        _size = new XSize(width, height);
    }

    public PageSize? PageSize => _pageFormat;
    public XSize? CustomSize => _size;

    public static implicit operator PageFormat(PageSize format) => new(format);
    public static implicit operator PageSize?(PageFormat wrapper) => wrapper._pageFormat;

    /// <summary>
    /// Standard size for plastic cards.
    /// </summary>
    public static PageFormat PlasticCard => new(new XSize(new XUnit(85, XGraphicsUnit.Millimeter), new XUnit(54, XGraphicsUnit.Millimeter)));
}