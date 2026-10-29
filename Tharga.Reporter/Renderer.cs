using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Text;
using PdfSharp;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Fonts;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Section = Tharga.Reporter.Entity.Section;

namespace Tharga.Reporter;

public class Renderer
{
    private readonly IDebugData _debugData;
    private readonly DocumentData _documentData;
    private readonly DocumentProperties _documentProperties;
    private readonly IGraphicsFactory _graphicsFactory;
    private readonly bool _includeBackgroundObjects;
    private readonly RenderContext _context = new();

    private readonly Template _template;
    private bool _preRendered;

    public Renderer(Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
        : this(new MyGraphicsFactory(), template, documentData, includeBackgroundObjects, documentProperties, debug)
    {
    }

    internal Renderer(IGraphicsFactory graphicsFactory, Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ReporterFontResolver.EnsureInstalled();

        _template = template;
        _documentData = documentData;
        _includeBackgroundObjects = includeBackgroundObjects;
        _documentProperties = documentProperties;
        if (debug)
        {
            _debugData = new DebugData();
        }

        _graphicsFactory = graphicsFactory;
    }

    private void RenderPdfDocument(PdfDocument pdfDocument, bool preRender, PageFormat pageFormat)
    {
        if (_preRendered && preRender)
        {
            throw new InvalidOperationException("Prerender has already been performed.");
        }

        var pageCount = GetPageCount(preRender);

        for (var ii = 0; ii < pageCount; ii++)
        {
            var page = pdfDocument.AddPage();

            if (pageFormat.PageSize.HasValue)
            {
                page.Size = pageFormat.PageSize.Value;
            }
            else if (pageFormat.CustomSize.HasValue)
            {
                page.Width = XUnit.FromPoint(pageFormat.CustomSize.Value.Width);
                page.Height = XUnit.FromPoint(pageFormat.CustomSize.Value.Height);
            }
            else
            {
                throw new InvalidOperationException($"The {nameof(pageFormat)} does not have {pageFormat.PageSize} or {pageFormat.CustomSize}.");
            }

            var gfx = _graphicsFactory.PrepareGraphics(page);

            DoRenderStuff(gfx, new XRect(0, 0, page.Width.Point, page.Height.Point), preRender, ii, _template.SectionList.Sum(x => _context.GetPageCount(x)));
        }

        if (preRender)
        {
            _preRendered = true;
        }
    }

    private PdfDocument CreatePdfDocument()
    {
        var pdfDocument = new PdfDocument();
        if (_documentProperties != null)
        {
            pdfDocument.Info.Author = _documentProperties.Author;
            pdfDocument.Info.Subject = _documentProperties.Subject;
            pdfDocument.Info.Title = _documentProperties.Title;
            pdfDocument.Info.Creator = _documentProperties.Creator;
        }

        // Encryption, owner password and permissions are fixed for now. See #32.
        pdfDocument.SecurityHandler.SetEncryptionToV2With128Bits();
        pdfDocument.SecuritySettings.OwnerPassword = "qwerty12";
        pdfDocument.SecuritySettings.PermitAnnotations = false;
        pdfDocument.SecuritySettings.PermitAssembleDocument = false;
        pdfDocument.SecuritySettings.PermitExtractContent = true;
        pdfDocument.SecuritySettings.PermitFormsFill = false;
        pdfDocument.SecuritySettings.PermitFullQualityPrint = true;
        pdfDocument.SecuritySettings.PermitModifyDocument = false;
        pdfDocument.SecuritySettings.PermitPrint = true;
        return pdfDocument;
    }

    private void PreRender(PageFormat pageFormat)
    {
        if (!_preRendered)
        {
            var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
            if (hasMultiPageElements)
            {
                var pdfDocument = CreatePdfDocument();
                RenderPdfDocument(pdfDocument, true, pageFormat);
            }
        }
    }

    private void DoRenderStuff(IGraphics gfx, XRect size, bool preRender, int page, int? totalPages)
    {
        var postRendering = new List<Action>();

        var pageNumberInfo = new PageNumberInfo(page + 1, totalPages);

        var section = GetSection(preRender, page);

        var sectionBounds = new XRect(section.Margin.GetLeft(size.Width), section.Margin.GetTop(size.Height), section.Margin.GetWidht(size.Width), section.Margin.GetHeight(size.Height));

        if (_debugData != null)
        {
            var sectionName = string.IsNullOrEmpty(section.Name) ? "Unnamed section" : section.Name;
            var textSize = gfx.MeasureString(sectionName, _debugData.Font);
            gfx.DrawString(sectionName, _debugData.Font, _debugData.Brush, new XPoint(0, textSize.Height));

            //Left margin
            gfx.DrawLine(_debugData.Pen, sectionBounds.Left, 0, sectionBounds.Left, size.Height);

            //Right margin
            gfx.DrawLine(_debugData.Pen, sectionBounds.Right, 0, sectionBounds.Right, size.Height);

            //Top margin
            gfx.DrawLine(_debugData.Pen, 0, sectionBounds.Top, size.Width, sectionBounds.Top);

            //Bottom margin
            gfx.DrawLine(_debugData.Pen, 0, sectionBounds.Bottom, size.Width, sectionBounds.Bottom);
        }

        var headerHeight = section.Header.Height.GetXUnitValue(sectionBounds.Height);
        var footerHeight = section.Footer.Height.GetXUnitValue(sectionBounds.Height);
        var paneBounds = new XRect(sectionBounds.Left, sectionBounds.Top + headerHeight, sectionBounds.Width, sectionBounds.Height - headerHeight - footerHeight);

        var renderData = new RenderData(gfx, paneBounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects, _context);

        if (preRender)
        {
            var pageCount = section.Pane.PreRender(renderData);
            _context.SetPageCount(section, pageCount);
        }
        else
        {
            section.Pane.Render(renderData, page);

            //Header
            if (section.Header != null)
            {
                var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects, _context);
                postRendering.Add(() => section.Header.Render(renderDataHeader, page));

                if (_debugData != null)
                {
                    gfx.DrawLine(_debugData.Pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                }
            }

            //Footer
            if (section.Footer != null)
            {
                var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                var renderDataFooter = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects, _context);
                postRendering.Add(() => section.Footer.Render(renderDataFooter, page));

                if (_debugData != null)
                {
                    gfx.DrawLine(_debugData.Pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                }
            }
        }

        foreach (var action in postRendering)
        {
            action();
        }
    }

    private Section GetSection(bool preRender, int page)
    {
        var section = _template.SectionList.First();
        if (page > 0)
        {
            if (preRender)
            {
                section = _template.SectionList.ToArray()[page];
            }
            else
            {
                var tot = 0;
                foreach (var s in _template.SectionList)
                {
                    tot += _context.GetPageCount(s);
                    if (tot >= page + 1)
                    {
                        section = s;
                        _context.SetPageOffset(section, tot - _context.GetPageCount(s));
                        break;
                    }
                }
            }
        }

        return section;
    }

    private int GetPageCount(bool preRender)
    {
        return preRender
            ? _template.SectionList.Count
            : _template.SectionList.Sum(x => _context.GetPageCount(x));
    }

    public byte[] GetPdfBinary(PageFormat pageFormat)
    {
        pageFormat ??= PageSize.A4;

        PreRender(pageFormat);

        var pdfDocument = CreatePdfDocument();
        RenderPdfDocument(pdfDocument, false, pageFormat);

        var memStream = new MemoryStream();
        pdfDocument.Save(memStream);
        return memStream.ToArray();
    }

}