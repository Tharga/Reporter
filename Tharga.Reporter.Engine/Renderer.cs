using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;
using Section = Tharga.Reporter.Engine.Entity.Section;

namespace Tharga.Reporter.Engine
{
    public class Renderer
    {
        public enum PageSize { A4, Letter }

        private readonly Template _template;
        private readonly DocumentData _documentData;
        private readonly bool _includeBackgroundObjects;
        private readonly DocumentProperties _documentProperties;
        private readonly bool _debug;
        
        private int _printPageCount;
        private bool _preRendered;
        private readonly IGraphicsFactory _graphicsFactory;

        internal Renderer(IGraphicsFactory graphicsFactory, Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
        {
            _template = template;
            _documentData = documentData;
            _includeBackgroundObjects = includeBackgroundObjects;
            _documentProperties = documentProperties;
            _debug = debug;
            _graphicsFactory = graphicsFactory;
        }

        public Renderer(Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
            : this(new MyGraphicsFactory(), template, documentData, includeBackgroundObjects, documentProperties, debug)
        {

        }

        #region Public access methods

        public void CreatePdfFile(string fileName, PageSize pageSize = PageSize.A4)
        {
            if (System.IO.File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            System.IO.File.WriteAllBytes(fileName, GetPdfBinary(pageSize));
        }

        public byte[] GetPdfBinary(PageSize pageSize = PageSize.A4)
        {
            PreRender(pageSize);

            var pdfDocument = CreatePdfDocument();
            RenderPdfDocument(pdfDocument, false, pageSize);

            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        public void Print(PrinterSettings printerSettings)
        {
            _printPageCount = 0;

            PageSize pageSize;
            if (!Enum.TryParse(printerSettings.DefaultPageSettings.PaperSize.Kind.ToString(), out pageSize))
                throw new InvalidOperationException(string.Format("Unable to parse {0} from as printerSettings to a page size.", printerSettings.DefaultPageSettings.PaperSize.Kind));

            PreRender(pageSize);

            var doc = GetDocument(false);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            var printDocument = new MigraDocPrintDocument();

            printDocument.PrintPage += printDocument_PrintPage;

            printDocument.Renderer = docRenderer;
            printDocument.PrinterSettings = printerSettings;
            printDocument.Print();
        }


        #endregion

        private void RenderPdfDocument(PdfDocument pdfDocument, bool preRender, PageSize pageSize)
        {
            if ( _preRendered && preRender)
                throw new InvalidOperationException("Prerender has already been performed.");

            var doc = GetDocument(preRender);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (var ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = pdfDocument.AddPage();

                page.Size = pageSize.ToPageSize();

                var gfx = _graphicsFactory.PrepareGraphics(page, docRenderer, ii);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height), preRender, ii, _template.SectionList.Sum(x => x.GetRenderPageCount()));
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

            //TODO: Add other properties as well
            //pdfDocument.Info.Keywords 
            //pdfDocument.Info.Owner 
            //pdfDocument.Info.Producer 
            //pdfDocument.PageLayout = PdfSharp.Pdf.PdfPageLayout.OneColumn 
            //pdfDocument.PageMode = PdfSharp.Pdf.PdfPageMode.FullScreen 
            //pdfDocument.Language 
            //pdfDocument.Outlines
            //pdfDocument.SecurityHandler 
            //pdfDocument.SecuritySettings 
            //pdfDocument.Settings.PrivateFontCollection 
            //pdfDocument.Version 
            //pdfDocument.ViewerPreferences.

            //TODO: Provide security settings
            pdfDocument.SecuritySettings.DocumentSecurityLevel = PdfSharp.Pdf.Security.PdfDocumentSecurityLevel.Encrypted128Bit;
            pdfDocument.SecuritySettings.OwnerPassword = "qwerty12";
            pdfDocument.SecuritySettings.PermitAccessibilityExtractContent = false;
            pdfDocument.SecuritySettings.PermitAnnotations = false;
            pdfDocument.SecuritySettings.PermitAssembleDocument = false;
            pdfDocument.SecuritySettings.PermitExtractContent = true; //Is this copy-paste block?
            pdfDocument.SecuritySettings.PermitFormsFill = false;
            pdfDocument.SecuritySettings.PermitFullQualityPrint = true;
            pdfDocument.SecuritySettings.PermitModifyDocument = false;
            pdfDocument.SecuritySettings.PermitPrint = true;
            //pdfDocument.SecuritySettings.UserPassword = "qwerty";
            return pdfDocument;
        }

        private void PreRender(PageSize pageSize)
        {
            //TODO: If prerender with one format (pageSize) and printing with another.
            //or, if template or document data changed between render and pre-render then things will be messed up.
            if (!_preRendered)
            {
                var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
                if (hasMultiPageElements)
                {
                    var pdfDocument = CreatePdfDocument();
                    RenderPdfDocument(pdfDocument, true, pageSize);
                }
            }
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var rawSize = GetSize(e);
            var unitSize = GetSize(rawSize);
            var scale = GetScale(unitSize);

            var gfx = XGraphics.FromGraphics(e.Graphics, rawSize, XGraphicsUnit.Point);
            gfx.ScaleTransform(scale);

            DoRenderStuff(new MyGraphics(gfx), new XRect(unitSize), false, _printPageCount++, _template.SectionList.Sum(x => x.GetRenderPageCount()));
        }

        private static XSize GetSize(XSize rawSize)
        {
            var wm = PrinterUnitConvert.Convert(rawSize.Width, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter)/100;
            var wx = new XUnit(wm, XGraphicsUnit.Millimeter);
            var hm = PrinterUnitConvert.Convert(rawSize.Height, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter)/100;
            var hx = new XUnit(hm, XGraphicsUnit.Millimeter);
            var unitSize = new XSize(wx, hx);
            return unitSize;
        }

        private static double GetScale(XSize xSize)
        {
            var wm = PrinterUnitConvert.Convert(xSize.Width, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter)/100;
            var wx = new XUnit(wm, XGraphicsUnit.Millimeter);
            var scale = xSize.Width/wx.Point;
            return scale;
        }

        private static XSize GetSize(PrintPageEventArgs e)
        {
            var w = e.PageBounds.Width;
            var h = e.PageBounds.Height;
            var xSize = new XSize(w, h);
            return xSize;
        }

        private void DoRenderStuff(IGraphics gfx, XRect size, bool preRender, int page, int? totalPages)
        {
            var debugPen = new XPen(XColor.FromArgb(System.Drawing.Color.Gray), 0.5);
            var debugFont = new XFont("Verdana", 10);
            var debugBrush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Gray));

            var postRendering = new List<Action>();

            var pageNumberInfo = new PageNumberInfo(page + 1, totalPages);

            var section = GetSection(preRender, page);
            section.Pane.ClearRenderPointers();

            var sectionBounds = new XRect(section.Margin.GetLeft(size.Width), section.Margin.GetTop(size.Height), section.Margin.GetWidht(size.Width), section.Margin.GetHeight(size.Height));

            if (_debug)
            {
                var sectionName = string.IsNullOrEmpty(section.Name) ? "Unnamed section" : section.Name;
                var textSize = gfx.MeasureString(sectionName, debugFont);
                gfx.DrawString(sectionName, debugFont, debugBrush, 0, textSize.Height);

                //Left margin
                gfx.DrawLine(debugPen, sectionBounds.Left, 0, sectionBounds.Left, size.Height);

                //Right margin
                gfx.DrawLine(debugPen, sectionBounds.Right, 0, sectionBounds.Right, size.Height);

                //Top margin
                gfx.DrawLine(debugPen, 0, sectionBounds.Top, size.Width, sectionBounds.Top);

                //Bottom margin
                gfx.DrawLine(debugPen, 0, sectionBounds.Bottom, size.Width, sectionBounds.Bottom);
            }

            var headerHeight = section.Header.Height.GetXUnitValue(sectionBounds.Height);
            var footerHeight = section.Footer.Height.GetXUnitValue(sectionBounds.Height);
            var paneBounds = new XRect(sectionBounds.Left, sectionBounds.Top + headerHeight, sectionBounds.Width, sectionBounds.Height - headerHeight - footerHeight);

            var renderData = new RenderData(gfx, paneBounds, section, _documentData, pageNumberInfo, _debug, _includeBackgroundObjects);

            if (preRender)
            {
                var pageCount = section.Pane.PreRender(renderData);
                section.SetRenderPageCount(pageCount);
            }
            else
            {
                section.Pane.Render(renderData, page);

                //Header
                if (section.Header != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                    var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debug, _includeBackgroundObjects);
                    postRendering.Add(() => section.Header.Render(renderDataHeader, page));

                    if (_debug)
                    {
                        gfx.DrawLine(debugPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                    }
                }

                //Footer
                if (section.Footer != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                    var renderDataFooter = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debug, _includeBackgroundObjects);
                    postRendering.Add(() => section.Footer.Render(renderDataFooter, page));

                    if (_debug)
                    {
                        gfx.DrawLine(debugPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
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
                        tot += s.GetRenderPageCount();
                        if (tot >= page+1)
                        {
                            section = s;
                            section.SetPageOffset(tot-s.GetRenderPageCount());
                            break;
                        }
                    }
                }
            }
            return section;
        }

        private Document GetDocument(bool preRender)
        {
            var doc = new Document();

            foreach (var section in _template.SectionList)
            {
                if (preRender)
                    doc.AddSection();
                else
                {
                    for (var i = 0; i < section.GetRenderPageCount(); i++)
                    {
                        doc.AddSection();
                    }
                }
            }

            return doc;
        }
    }

    internal class MyGraphicsFactory : IGraphicsFactory
    {
        public IGraphics PrepareGraphics(PdfPage page, DocumentRenderer docRenderer, int ii)
        {
            var gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;
            docRenderer.RenderPage(gfx, ii + 1);
            return new MyGraphics(gfx);
        }
    }

    internal class MyGraphics : IGraphics
    {
        private readonly XGraphics _gfx;

        public MyGraphics(XGraphics gfx)
        {
            _gfx = gfx;
        }

        public XSize MeasureString(string code, XFont legendFont)
        {
            return _gfx.MeasureString(code, legendFont);
        }

        public XSize MeasureString(string code, XFont legendFont, XStringFormat topLeft)
        {
            return _gfx.MeasureString(code, legendFont, topLeft);
        }

        public void DrawImage(XImage image, XRect xRect)
        {
            _gfx.DrawImage(image, xRect);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XPoint xPoint, XStringFormat topLeft)
        {
            _gfx.DrawString(code, legendFont, legendBrush, xPoint, topLeft);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, XRect rect)
        {
            _gfx.DrawString(code, legendFont, legendBrush, rect);
        }

        public void DrawString(string code, XFont legendFont, XSolidBrush legendBrush, double x, double y)
        {
            _gfx.DrawString(code, legendFont, legendBrush, x, y);
        }

        public void DrawString(string text, XFont font, XSolidBrush brush, XRect elementBounds, XStringFormat formats)
        {
            _gfx.DrawString(text, font, brush, elementBounds, formats);
        }

        public void DrawString(string line, XFont font, XBrush brush, double left, double top, XStringFormat formats)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string text, XFont font, XBrush brush, XPoint point)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(XPen debugPen, double left, double p2, double p3, double height)
        {
            _gfx.DrawLine(debugPen, left, p2, p3, height);
        }

        public void DrawRectangle(XPen debugPen, double left, double top, double width, double height)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(XPen pen, XRect rect)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(XPen borderPen, XBrush brush, double p1, double p2, double p3, double p4)
        {
            throw new NotImplementedException();
        }

        public void DrawEllipse(XPen pen, double d, double d1, int i, int i1)
        {
            throw new NotImplementedException();
        }
    }
}