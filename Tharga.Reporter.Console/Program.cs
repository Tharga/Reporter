using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Element.Base;
using Image = Tharga.Reporter.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Entity.Element.Rectangle;

namespace Tharga.Reporter.Console;

internal static class Program
{
    private static void Main(string[] args)
    {
        //Blank_default_PDF_document();
        //TextButNoData();
        //BackgroundObjectsOrNot();
        //SinglePageAreaElement_Sample();
        //Barcode_Sample();
        //MultiPageAreaElement_Sample();
        //Basic_PDF_document_with_some_text_on_it();
        //Multiple_sections();
        //Multipage_PDF_document_by_section();
        //Multipage_PDF_by_spanning_text();
        //Multipage_PDF_by_spanning_text_using_a_reference_point();
        //Multipage_PDF_by_spanning_text_using_a_reference_point_with_vertical_stacking();
        //Multipage_PDF_by_spanning_text_border_case_where_text_ends_up_exactly();
        //Create_PDF_document_with_basic_table();
        //Create_PDF_document_with_template_that_spans_over_multiple_pages();
        //Different_text_on_different_pages();
        //RefWithOnePageTextBox();
        //SkallebergSample1();
        //SkallebergSample2();
        //RenderFromFile(@"C:\Users\Daniel\Documents\template.xml", @"C:\Users\Daniel\Documents\data.xml");
        //RenderFromFile(@"C:\Users\Daniel\Desktop\InvoiceTemplate.txt", @"C:\Users\Daniel\Desktop\InvoiceData.txt");
        //DataWithLinefeed();
        //Table_With_grouping();

        HallenMedlemCard();
    }

    private static void HallenMedlemCard()
    {
        Template template = null;
        foreach (var page in new[] { ("A", "67890"), ("B", "1234567890") })
        {
            var section = new Section
            {
                Margin = new UnitRectangle { Left = "0", Top = "0", Right = "0", Bottom = "0" },
                Header = { Height = "0" },
                Footer = { Height = "0" }
            };
            section.Pane.ElementList.Add(new BarCode { Code = page.Item2, Top = "18mm", Left = "8mm", Bottom = "18mm", Right = "8mm" });
            section.Pane.ElementList.Add(new Text { Value = page.Item1, Top = "38mm", Left = "9mm", Right = "9mm", TextAlignment = TextBase.Alignment.Left });
            section.Pane.ElementList.Add(new Text { Value = page.Item2, Top = "38mm", Left = "9mm", Right = "9mm", TextAlignment = TextBase.Alignment.Right });

            if (template == null)
            {
                template = new Template(section);
            }
            else
            {
                template.SectionList.Add(section);
            }
        }

        SampleCardOutput(template, null, false, false);
    }

    private async static void TextButNoData()
    {
        var section = new Section();
        section.Pane.ElementList.Add(new Text { Value = "Data: {SomeData}" });
        var template = new Template(section);
        SampleOutput(template, null);
    }

    private async static void Barcode_Sample()
    {
        var section = new Section();

        section.Margin = new UnitRectangle { Left = "1cm", Top = "2cm", Right = "3cm", Bottom = "4cm" };

        section.Header.Height = "5cm";
        section.Footer.Height = "1cm";

        section.Pane.ElementList.Add(new Line { Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Line { Left = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Line { Top = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Text { Value = "Bob Lablow" });

        section.Pane.ElementList.Add(new Rectangle { BorderColor = Color.Green, Width = "50%", Height = "50%", Left = "25%", Top = "25%" });
        section.Pane.ElementList.Add(new BarCode { Code = "S1309799801", Height = "1cm" });


        section.Footer.ElementList.Add(new Text { Value = "Data: {SomeData}" });

        var template = new Template(section);

        var documentData = new DocumentData();
        documentData.Add("SomeData", "Reapadda");

        SampleOutput(template, documentData);
    }

    private async static void SinglePageAreaElement_Sample()
    {
        var section = new Section();

        section.Margin = new UnitRectangle { Left = "1cm", Top = "2cm", Right = "3cm", Bottom = "4cm" };

        section.Header.Height = "5cm";
        section.Footer.Height = "1cm";

        section.Pane.ElementList.Add(new Line { Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Line { Left = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Line { Top = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
        section.Pane.ElementList.Add(new Text { Value = "Bob Lablow" });
        section.Pane.ElementList.Add(new Image { Source = "http://skalleberg.se/wp-content/uploads/2013/10/bildspel3-840x270.png" });

        section.Header.ElementList.Add(new Image { Source = "http://skalleberg.se/wp-content/uploads/2013/10/bildspel2-840x270.png" });

        section.Footer.ElementList.Add(new Image { Source = "http://skalleberg.se/wp-content/uploads/2013/10/bildspel-1-840x270.png", Left = "50%" });

        section.Pane.ElementList.Add(new Rectangle { BorderColor = Color.Green, Width = "50%", Height = "50%", Left = "25%", Top = "25%" });
        section.Pane.ElementList.Add(new BarCode { Code = "S1309799801", Height = "1cm" });

        section.Footer.ElementList.Add(new Text { Value = "Data: {SomeData}" });

        var template = new Template(section);

        var documentData = new DocumentData();
        documentData.Add("SomeData", "Reapadda");

        SampleOutput(template, documentData);
    }

    private static void SampleOutput(Template template, DocumentData documentData, bool useBackground = true, bool debug = true)
    {
        try
        {
            //Prep
            var renderer = new Renderer(template, documentData, useBackground, null, debug);
            var stopWatch = new Stopwatch();

            //New way
            stopWatch.Reset();
            stopWatch.Start();
            var bytes = renderer.GetPdfBinary();
            System.Console.WriteLine("New: " + stopWatch.Elapsed.TotalSeconds.ToString("0.0000"));
            FileManager.ExecuteFile(bytes);

            //Directly to printer
            //var printerSettings = new PrinterSettings
            //{
            //    PrinterName = "Microsoft XPS Document Writer",
            //    PrintToFile = true,
            //    PrintFileName = @"C:\Users\Daniel\Desktop\b1.xps",
            //};
            //stopWatch.Reset();
            //stopWatch.Start();
            //renderer.Print(printerSettings);
            //System.Console.WriteLine("Prn: " + stopWatch.Elapsed.TotalSeconds.ToString("0.0000"));
        }
        catch (Exception exception)
        {
            System.Console.WriteLine(exception.Message);
        }
    }

    private static void SampleCardOutput(Template template, DocumentData documentData, bool useBackground = true, bool debug = true)
    {
        try
        {
            var renderer = new Renderer(template, documentData, useBackground, null, debug);
            var bytes = renderer.GetPdfBinary(Renderer.PageSize.PlasticCard);
            //FileManager.ExecuteFile(bytes);

            //Directly to printer
            //var printerSettings = new PrinterSettings
            //{
            //    PrinterName = "Microsoft XPS Document Writer",
            //    PrintToFile = true,
            //    PrintFileName = @"C:\Users\Daniel\Desktop\b1.xps",
            //};
            //stopWatch.Reset();
            //stopWatch.Start();
            //renderer.Print(printerSettings);
            //System.Console.WriteLine("Prn: " + stopWatch.Elapsed.TotalSeconds.ToString("0.0000"));
        }
        catch (Exception exception)
        {
            System.Console.WriteLine(exception.Message);
        }
    }
}