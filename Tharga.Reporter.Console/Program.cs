using System.Drawing;
using PdfSharp;
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
        switch (args.FirstOrDefault())
        {
            case "text":
                TextButNoData();
                break;
            case "barcode":
                Barcode_Sample();
                break;
            case "elements":
                SinglePageAreaElement_Sample();
                break;
            default:
                Card_Sample();
                break;
        }
    }

    private static void Card_Sample()
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

    private static void TextButNoData()
    {
        var section = new Section();
        section.Pane.ElementList.Add(new Text { Value = "Data: {SomeData}" });
        var template = new Template(section);
        SampleOutput(template, null);
    }

    private static void Barcode_Sample()
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

    private static void SinglePageAreaElement_Sample()
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
        WriteSample(template, documentData, PageSize.A4, useBackground, debug);
    }

    private static void SampleCardOutput(Template template, DocumentData documentData, bool useBackground = true, bool debug = true)
    {
        WriteSample(template, documentData, PageFormat.PlasticCard, useBackground, debug);
    }

    private static void WriteSample(Template template, DocumentData documentData, PageFormat pageFormat, bool useBackground, bool debug)
    {
        var renderer = new Renderer(template, documentData, useBackground, null, debug);
        FileManager.ExecuteFile(renderer.GetPdfBinary(pageFormat));
    }
}