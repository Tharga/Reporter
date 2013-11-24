using System;
using System.Collections.Generic;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine
{
    public class Rendering
    {
        private readonly Template _template;
        private readonly DocumentProperties _documentProperties;
        private readonly DocumentData _documentData;
        private readonly bool _background;
        private readonly bool _debug;

        //private List<FontClass> _fontClassList;
        //internal DocumentData _documentData;

        private Rendering(Template template,
            DocumentProperties documentProperties = null,
            DocumentData documentData = null,
            bool background = true,
            bool debug = false)
        {
            _template = template;
            _documentProperties = documentProperties;
            _documentData = documentData ?? new DocumentData();
            _background = background;
            _debug = debug;
            //_fontClassList = template.FontClassList ?? new List<FontClass>();
        }

        public static byte[] CreatePDFDocument(Template template,
            DocumentProperties documentProperties = null,
            DocumentData documentData = null,
            bool background = true,
            bool debug = false)
        {
            //Create an instance of the engine class (So that global data can be stored)
            var engine = new Rendering(template, documentProperties, documentData, background, debug);
            return engine.Render();
        }        

        private byte[] Render()
        {

            var pdfDocument = new PdfSharp.Pdf.PdfDocument();
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

            var postRendering = new List<Action>();

            var pageNumber = 0;
            foreach (var section in _template.SectionList)
            {
                section.Pane.ClearRenderPointers();

                var needAnotherPage = true;
                while (needAnotherPage)
                {
                    //Add a new page for the section
                    var page = pdfDocument.AddPage();
                    var pageNumberInfo = new PageNumberInfo(++pageNumber);

                    //TODO: read page size from section
                    //page.Size = PdfSharp.PageSize.Letter;
                    page.Size = PdfSharp.PageSize.A4;
                    //page.Rotate = 90;

                    var sectionBounds = new XRect(section.Margin.GetLeft(page.Width.Value), section.Margin.GetTop(page.Height.Value),
                                                  section.Margin.GetWidht(page.Width.Value), section.Margin.GetHeight(page.Height.Value));

                    var debugPen = new XPen(XColor.FromArgb(System.Drawing.Color.Gray), 0.1);
                    //debugPen.DashPattern = ;
                    debugPen.DashStyle = XDashStyle.Dash;
                    //debugPen.LineCap = XLineCap.Round;
                    //debugPen.MiterLimit = 1;
                    //debugPen.LineJoin = XLineJoin.Round;
                    //debugPen.DashOffset 
                    //debugPen.DashOffset = 1000;

                    var debugFont = new XFont("Verdana", 10);
                    var debugBrush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Gray));

                    if (_debug)
                    {
                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            var sectionName = section.Name ?? "Unnamed section";
                            var textSize = gfx.MeasureString(sectionName, debugFont);
                            gfx.DrawString(sectionName, debugFont, debugBrush, 0, textSize.Height);

                            //Left margin
                            gfx.DrawLine(debugPen, sectionBounds.Left, 0, sectionBounds.Left, page.Height);

                            //Right margin
                            gfx.DrawLine(debugPen, sectionBounds.Right, 0, sectionBounds.Right, page.Height);

                            //Top margin
                            gfx.DrawLine(debugPen, 0, sectionBounds.Top, page.Width, sectionBounds.Top);

                            //Bottom margin
                            gfx.DrawLine(debugPen, 0, sectionBounds.Bottom, page.Width, sectionBounds.Bottom);
                        }
                    }

                    //Pane
                    //var paneBounds = section.Pane.GetBounds(sectionBounds);
                    //TODO: Calculate using the header and footer sizes.
                    //var headerHeight = UnitValue.Parse(section.Header.Height).GetXUnitValue(sectionBounds.Height);
                    var headerHeight = section.Header.Height.GetXUnitValue(sectionBounds.Height);
                    //var footerHeight = UnitValue.Parse(section.Footer.Height).GetXUnitValue(sectionBounds.Height);
                    var footerHeight = section.Footer.Height.GetXUnitValue(sectionBounds.Height);
                    var paneBounds = new XRect(sectionBounds.Left, sectionBounds.Top + headerHeight, sectionBounds.Width, sectionBounds.Height - headerHeight - footerHeight);
                    
                    needAnotherPage = section.Pane.Render(page, paneBounds, _documentData, _background, _debug, pageNumberInfo, section);


                    ////Draw fingerprint
                    //using (var gfx = XGraphics.FromPdfPage(page))
                    //{
                    //    gfx.RotateAtTransform(-90, paneBounds.BottomLeft);
                    //    var font = new XFont("Verdana", 5);
                    //    var brush = new XSolidBrush(XColor.FromArgb(System.Drawing.Color.Black));
                    //    gfx.DrawString("www.thargelion.se", font, brush, paneBounds.Left, paneBounds.Bottom - 3);
                    //}

                    //TODO: Apply the header and footer for all pages in the section

                    //Header
                    if (section.Header != null)
                    {
                        var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                        postRendering.Add(() => section.Header.Render(page, bounds, _documentData, _background, _debug, pageNumberInfo,section));

                        if (_debug)
                        {
                            using (var gfx = XGraphics.FromPdfPage(page))
                            {
                                gfx.DrawLine(debugPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                            }
                        }
                    }

                    //Footer
                    if (section.Footer != null)
                    {
                        var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                        postRendering.Add(() => section.Footer.Render(page, bounds, _documentData, _background, _debug, pageNumberInfo,section));

                        if (_debug)
                        {
                            using (var gfx = XGraphics.FromPdfPage(page))
                            {
                                gfx.DrawLine(debugPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                            }
                        }
                    }
                }
            }

            PageNumberInfo.TotalPages = pageNumber;

            foreach (var action in postRendering)
            {
                action();
            }

            //Create stream to return
            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }
    }
}
