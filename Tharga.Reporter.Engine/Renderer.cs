using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Section = Tharga.Reporter.Engine.Entity.Section;

namespace Tharga.Reporter.Engine
{
    public class Renderer
    {
        //TODO: Lock theese object when rendering starts
        private DocumentProperties _documentProperties;
        private DocumentData _documentData;
        private Template _template;
        private int _printPageCount;
        private bool _preRendered; //TODO: Have a record on the data used for pre-rendering. For instance pageSize (A4 or Letter), _template and _documentData.

        public Template Template { get { return _template; } set { _template = value; } }
        public DocumentProperties DocumentProperties { get { return _documentProperties; } set { _documentProperties = value; } }
        public DocumentData DocumentData { get { return _documentData; } set { _documentData = value; } }
        public bool Debug { get; set; }

        public async Task CreatePdf(string fileName, PageSize pageSize = PageSize.A4)
        {
            if (System.IO.File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            System.IO.File.WriteAllBytes(fileName, await GetPDFDocumentAsync(pageSize));
        }

        public async Task<byte[]> GetPDFDocumentAsync(PageSize pageSize = PageSize.A4)
        {
            PreRender(pageSize);

            var pdfDocument = CreatePDFDocument();
            RenderPDFDocument(pdfDocument, false, pageSize);

            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        private void RenderPDFDocument(PdfDocument pdfDocument, bool preRender, PageSize pageSize)
        {
            if ( _preRendered && preRender)
                throw new InvalidOperationException("Prerender has already been performed.");

            var doc = GetDocument(preRender);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (var ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = pdfDocument.AddPage();

                //TODO: Use printer settings information to get this value (Same as when the document is actually printed)
                page.Size = pageSize;
                //page.Size = PdfSharp.PageSize.Letter;
                //page.Size = PdfSharp.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                // HACK²
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;

                docRenderer.RenderPage(gfx, ii + 1);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height), preRender, ii);
            }

            if (preRender)
                _preRendered = true;
        }

        private PdfDocument CreatePDFDocument()
        {
            var pdfDocument = new PdfDocument();
            if (_documentProperties != null)
            {
                pdfDocument.Info.Author = _documentProperties.Author;
                pdfDocument.Info.Subject = _documentProperties.Subject;
                pdfDocument.Info.Title = _documentProperties.Title;
                pdfDocument.Info.Creator = "APPLICATION"; //TODO: Assign this better
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

        public async Task PrintAsync(PrinterSettings printerSettings)
        {
            _printPageCount = 0;

            PageSize pageSize;
            if (!Enum.TryParse(printerSettings.DefaultPageSettings.PaperSize.Kind.ToString(), out pageSize))
                throw new InvalidOperationException(string.Format("Unable to parse {0} as PageSize.", printerSettings.DefaultPageSettings.PaperSize.Kind));

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

        private void PreRender(PageSize pageSize)
        {
            //TODO: If prerender with one format (pageSize) and printing with another.
            //or, if template or document data changed between render and pre-render then things will be messed up.
            if (!_preRendered)
            {
                var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
                if (hasMultiPageElements)
                {
                    var pdfDocument = CreatePDFDocument();
                    RenderPDFDocument(pdfDocument, true, pageSize);
                }
            }
        }

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var rawSize = GetSize(e);
            var unitSize = GetSize(rawSize);
            var scale = GetScale(unitSize);

            var gfx = XGraphics.FromGraphics(e.Graphics, rawSize, XGraphicsUnit.Point);
            gfx.ScaleTransform(scale);

            //TODO: Provide correct rendering method
            DoRenderStuff(gfx, new XRect(unitSize), false, _printPageCount++);
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

        private void DoRenderStuff(XGraphics gfx, XRect size, bool preRender, int page)
        {
            var debugPen = new XPen(XColor.FromArgb(System.Drawing.Color.Gray), 0.5);
            var debugFont = new XFont("Verdana", 10);
            var debugBrush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Gray));

            var postRendering = new List<Action>();

            var pageNumberInfo = new PageNumberInfo(page + 1);

            var section = GetSection(preRender, page);
            section.Pane.ClearRenderPointers();

            var sectionBounds = new XRect(section.Margin.GetLeft(size.Width), section.Margin.GetTop(size.Height),
                                          section.Margin.GetWidht(size.Width), section.Margin.GetHeight(size.Height));

            if (Debug)
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

            var renderData = new RenderData(gfx, paneBounds, section, _documentData, pageNumberInfo, Debug);

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
                    var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, Debug);
                    postRendering.Add(() => section.Header.Render(renderDataHeader, page));

                    if (Debug)
                    {
                        gfx.DrawLine(debugPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                    }
                }

                //Footer
                if (section.Footer != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                    var renderDataFooter = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, Debug);
                    postRendering.Add(() => section.Footer.Render(renderDataFooter, page));

                    if (Debug)
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
}