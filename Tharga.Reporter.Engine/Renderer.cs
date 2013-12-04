using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine
{
    public class Renderer
    {
        //TODO: Lock theese object when rendering starts
        private DocumentProperties _documentProperties;
        private DocumentData _documentData;
        private Template _template;

        public Template Template { get { return _template; } set { _template = value; } }
        public DocumentProperties DocumentProperties { get { return _documentProperties; } set { _documentProperties = value; } }
        public DocumentData DocumentData { get { return _documentData; } set { _documentData = value; } }
        public bool Debug { get; set; }

        //TODO: Async
        public void CreatePdf(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            System.IO.File.WriteAllBytes(fileName, GetPDFDocument());
        }

        //TODO: Async
        public byte[] GetPDFDocument()
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


            var doc = GetDocument();

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (int ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = pdfDocument.AddPage();

                //TODO: Use printer settings information to get this value (Same as when the document is actually printed)
                //page.Size = PdfSharp.PageSize.Letter;
                page.Size = PdfSharp.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                // HACK²
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;

                docRenderer.RenderPage(gfx, ii + 1);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height));
            }

            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        //TODO: Async
        public void Print(PrinterSettings printerSettings)
        {
            var doc = GetDocument();

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            var printDocument = new MigraDocPrintDocument();

            printDocument.PrintPage += printDocument_PrintPage;

            printDocument.Renderer = docRenderer;
            printDocument.PrinterSettings = printerSettings;
            printDocument.Print();
        }

        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var rawSize = GetSize(e);
            var unitSize = GetSize(rawSize);
            var scale = GetScale(unitSize);

            var gfx = XGraphics.FromGraphics(e.Graphics, rawSize, XGraphicsUnit.Point);
            gfx.ScaleTransform(scale);

            DoRenderStuff(gfx, new XRect(unitSize));
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

        private void DoRenderStuff(XGraphics gfx, XRect size)
        {
            var debugPen = new XPen(XColor.FromArgb(System.Drawing.Color.Gray), 0.5);
            var debugFont = new XFont("Verdana", 10);
            var debugBrush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Gray));

            var postRendering = new List<Action>();

            var pageNumber = 0;
            var pageNumberInfo = new PageNumberInfo(++pageNumber);
            foreach (var section in _template.SectionList)
            {
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

                //needAnotherPage = section.Pane.Render(page, paneBounds, _documentData, _background, _debug, pageNumberInfo, section);
                section.Pane.Render(renderData);

                //Header
                if (section.Header != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                    var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, Debug);
                    //postRendering.Add(() => section.Header.Render(page, bounds, _documentData, _background, _debug, pageNumberInfo, section));
                    postRendering.Add(() => section.Header.Render(renderDataHeader));

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
                    //postRendering.Add(() => section.Footer.Render(page, bounds, _documentData, _background, _debug, pageNumberInfo, section));
                    postRendering.Add(() => section.Footer.Render(renderDataFooter));

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

        private Document GetDocument()
        {
            var doc = new Document();

            foreach (var section in _template.SectionList)
            {
                //TODO: Count the number of pages needed and add them here
                var sec = doc.AddSection();
            }

            return doc;
        }
    }
}