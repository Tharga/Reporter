using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Font = Tharga.Reporter.Engine.Entity.Font;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Blank_default_PDF_document();
            Basic_elements();
            //Basic_PDF_document_with_some_text_on_it();
            //Multipage_PDF_document_by_section();
            //Multipage_PDF_by_spanning_text();
            //Multipage_PDF_by_spanning_text_using_a_reference_point();
            //Multipage_PDF_by_spanning_text_using_a_reference_point_with_vertical_stacking();
            //Multipage_PDF_by_spanning_text_border_case_where_text_ends_up_exactly();

            //Create_PDF_document_from_template();
            //Create_PDF_document_with_basic_template();
            //Create_PDF_document_with_template_that_spans_over_multiple_pages();

            //SkallebergSample1();

            //Bugs: 
            //When spanning with textBoxes an empty page can sometimes be added, if the last word fitx exactly

            //What else do we want??
            //- Serialize/unzerialize template to xml
        }

        private static void Basic_elements()
        {
            var section = new Section();

            section.Margin = new UnitRectangle {Left = "1cm", Top = "2cm", Right = "3cm", Bottom = "4cm"};

            section.Header.Height = "5cm";
            section.Footer.Height = "1cm";

            section.Pane.ElementList.Add(new Line { Thickness = "1mm", Color = Color.MidnightBlue });
            section.Pane.ElementList.Add(new Line { Left = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
            section.Pane.ElementList.Add(new Line { Top = "10cm", Thickness = "1mm", Color = Color.MidnightBlue });
            section.Pane.ElementList.Add(new Text { Value = "Bob Lablow" });

            var template = new Template(section);

            //Old way
            var oldBytes = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(oldBytes);

            //New way
            var renderer = new Renderer{ Template = template, Debug = true};
            var bytes = renderer.GetPDFDocument();
            ExecuteFile(bytes);

            //Directly to printer
            var printerSettings = new PrinterSettings
                {
                    PrinterName = "Microsoft XPS Document Writer",
                    PrintToFile = true,
                    PrintFileName = @"C:\Users\Daniel\Desktop\b1.xps",
                };
            renderer.Print(printerSettings);
        }

        private static void SkallebergSample1()
        {
            var documentData = GetDocumentData();

            var section = SkallSim1.GetDeliveryNoteSection(Color.FromArgb(255, 0, 0, 0));
            var template = new Template(section);

            var xme = template.ToXml();

            var t2 = Template.Load(xme);
            var xme2 = t2.ToXml();

            if (xme.OuterXml != xme2.OuterXml)
                Debug.WriteLine("Oups!");

            var byteArrayA = Rendering.CreatePDFDocument(t2, debug: false, documentData: documentData, background: false);
            ExecuteFile(byteArrayA);

            var byteArray = Rendering.CreatePDFDocument(template, debug: false, documentData: documentData, background: false);
            ExecuteFile(byteArray);

            if (byteArrayA.Length != byteArray.Length)
                Debug.WriteLine("Oups!");
        }

        private static DocumentData GetDocumentData()
        {
            var documentData = new DocumentData();

            documentData.Add("Number", "12345");
            documentData.Add("Time", "10.00.00");
            documentData.Add("Date", "2001-01-01");

            documentData.Add("User.Name", "Rea Padda");

            documentData.Add("Customer.Name", "Bob Loblaw");

            return documentData;
        }

        private static void Create_PDF_document_with_template_that_spans_over_multiple_pages()
        {
            var coverPage = new Section {Name = "Cover"};
            coverPage.Pane.ElementList.Add(new Text{Value = "This is the cover page.",Top = UnitValue.Parse("50%"), Left = UnitValue.Parse("50%")});
            coverPage.Pane.ElementList.Add(new Image { Right = "0", Top = "0", Height = "2cm", Width = "4cm", Source = "http://thargelion.se/images/thargelion-logo.png" });

            var index = new Section {Name = "Index"};
            index.Pane.ElementList.Add(new Text { Value = "This is the index page.", Top = UnitValue.Parse("50mm") ,Left = UnitValue.Parse("2inch")});

            var content = new Section
                {
                    Name = "Content",
                    DefaultFont = new Font
                        {
                            Size = 14,
                            Color = Color.Orange,
                            FontName = "Times"
                        }
                };

            var rect = new Rectangle { BackgroundColor = Color.NavajoWhite, Width="33%", Height = "100%", BorderColor = Color.MediumTurquoise };
            content.Pane.ElementList.Add(rect);

            //TODO: Possible to assign and use a font class, instead of a specific font
            string someFontClass = null; // "MyFontClass";

            //var tableTemplate = new Table { Name ="MyTable", Color = Color.Green,  Top = UnitValue.Parse("5cm"), Height = UnitValue.Parse("200"), Left = UnitValue.Parse("1cm"), Width = UnitValue.Parse("10cm") };
            var tableTemplate = new Table
                {
                    Name = "MyTable",
                    Top = UnitValue.Parse("5cm"),
                    Height = UnitValue.Parse("200"),
                    Left = UnitValue.Parse("1cm"),
                    Width = UnitValue.Parse("10cm"),
                    ContentFont = new Font {FontName = "Times", Size = 12, Color = Color.Blue},
                    //HeaderFont = new Font {FontName = "Arial", Size = 21},
                    //HeaderFontClass = someFontClass

                    ContentBorderColor = Color.MediumSeaGreen,
                    ContentBackgroundColor = Color.LightGreen,

                    HeaderBorderColor = Color.MediumSlateBlue,
                    HeaderBackgroundColor = Color.LightSkyBlue,

                    //TODO: Possible to specify line, skip
                    //TODO: Possible to set content line height (More spacing on each line)
                    SkipLine = new SkipLine
                        {
                            Interval = 3,
                            Height = "3mm",
                        },

                        ColumnPadding = "5mm",
                        //RowPadding = "1mm",
                };

            content.Pane.ElementList.Add(tableTemplate);
            tableTemplate.AddColumn("A {Col1}", "Column 1", UnitValue.Parse("5cm"));
            tableTemplate.AddColumn("B {Col2}", "Column 2", UnitValue.Parse("5cm"));

            var bc = new BarCode { Left = "3cm", Top = "1cm", Width = "5cm", Height = "2cm", Code = "12345" };
            content.Pane.ElementList.Add(bc);
            
            //var rc = new Rectangle { Left = "3cm", Top = "1cm", Width = "5cm", Height = "2cm" };
            //content.Pane.ElementList.Add(rc);

            //var img = new Image { Source = @"C:\Users\Daniel\AppData\Local\Temp\tmpBF4F.tmp.png", Left = "3cm", Top = "5cm", Width = "50%", Height = "3cm" };
            //content.Pane.ElementList.Add(img);

            content.Header.Height = UnitValue.Parse("2cm");
            content.Footer.Height = UnitValue.Parse("2cm");
            content.Margin.Top = UnitValue.Parse("1cm");
            content.Margin.Bottom = UnitValue.Parse("1cm");
            content.Margin.Left = UnitValue.Parse("1cm");
            content.Margin.Right = UnitValue.Parse("1cm");

            //Handling page numbers
            content.Header.ElementList.Add(new Text {Value = "Some text in the header. Page {PageNumber} of {TotalPages}",}); //FontClass = someFontClass });
            content.Header.ElementList.Add(new Line { Top = "1cm", Left = "10%", Width = "50%", Height = "0", Thickness = "0.1" });
            content.Header.ElementList.Add(new Line { Top = "1,2cm", Left = "10%", Width = "50%", Height = "0", Thickness = "1" });
            content.Header.ElementList.Add(new Line { Top = "1,4cm", Left = "10%", Width = "50%", Height = "0" , Thickness = "2"});
            content.Footer.ElementList.Add(new Text { Value = "Some text in the footer. Page {PageNumber} of {TotalPages}" });

            content.Footer.ElementList.Add(new Text { Value = "Some normal text in the footer.", Top = "0,5cm", Font = new Font { Bold = false, Italic = false } });
            content.Footer.ElementList.Add(new Text {Value = "Some bold text in the footer.", Top = "1cm", Font = new Font {Bold = true}});
            content.Footer.ElementList.Add(new Text { Value = "Some italic text in the footer.", Top = "1,5cm", Font = new Font { Italic = true } });
            content.Footer.ElementList.Add(new Text { Value = "Some bold italic text in the footer.", Top = "0,5cm", Left = "5cm", Font = new Font { Italic = true, Bold = true } });
            content.Footer.ElementList.Add(new Text { Value = "Some undelined text in the footer.", Top = "1cm", Left = "5cm", Font = new Font { Underline = true } });
            content.Footer.ElementList.Add(new Text { Value = "Some strikeout text in the footer.", Top = "1,5cm", Left = "5cm", Font = new Font { Strikeout = true } });
            
            content.Pane.ElementList.Add(new Text { Value = "Some text in the pane. Page {PageNumber} of {TotalPages}" });

            content.Pane.ElementList.Add(new TextBox { Left = "10cm", Height="2cm", Value = "Vill du automatiskt få denna följesedel skickad via mejl varje gång du handlar hos Skalleberg. Skicka då ett meddelande till support@thargelion.se med ditt kundnummer så fixar vi det." });

            var documentData = new DocumentData();
            var tableData = documentData.GetDataTable("MyTable");
            for (var i = 0; i < 100; i++)
            {
                var row = tableData.AddRow();
                row.Add("Col1", string.Format("some data for row {0}", i));
                row.Add("Col2", "some oter data");
            }
            var template = new Template(coverPage);
            template.SectionList.Add(content);
            template.SectionList.Add(index);
            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData, debug: false);

            ExecuteFile(byteArray);
        }

        private static void Create_PDF_document_with_basic_template()
        {
            var section = new Section();

            var tableTemplate = new Table { Name = "MyTable", ContentBorderColor = System.Drawing.Color.Blue };
            section.Pane.ElementList.Add(tableTemplate);
            tableTemplate.AddColumn("{Col1}", "Column 1", UnitValue.Parse("5cm"));
            tableTemplate.AddColumn("{Col2}", "Column 2", UnitValue.Parse("5cm"));

            var documentData = new DocumentData();
            var tableData = documentData.GetDataTable("MyTable");
            var row = tableData.AddRow();
            row.Add("Col1", "some data for row 1");
            row.Add("Col2", "some oter data");

            var row2 = tableData.AddRow();
            row2.Add("Col1", "some data for row 2");
            row2.Add("Col2", "some oter data");

            var template = new Template(section);
            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData);
            ExecuteFile(byteArray);


            var x = UnitValue.Parse("2cm") - UnitValue.Parse("1cm");
            x += UnitValue.Parse("4mm");
            var y = UnitValue.Parse("2cm") + UnitValue.Parse("1cm");

            if (UnitValue.Parse("1cm") == UnitValue.Parse("10mm"))
                System.Diagnostics.Debug.WriteLine("Same");

            var z = UnitValue.Parse("100%").ToString() + UnitValue.Parse("1cm");
        }

        //private static void Create_PDF_document_from_template()
        //{
        //    var section = Section.Create();
        //    section.Header.Height = UnitValue.Parse("10%");
        //    section.Footer.Height = UnitValue.Parse("10%");
        //    section.Margin.Left = UnitValue.Parse("3cm");
        //    section.Margin.Top = UnitValue.Parse("1cm");
        //    section.Margin.Right = UnitValue.Parse("1cm");
        //    section.Margin.Bottom = UnitValue.Parse("0.5cm");

        //    var text = Text.Create("More Text");
        //    text.Font.FontName = "Times New Roman";
        //    text.Font.Size = 22;
        //    text.Left = UnitValue.Parse("5cm");
        //    text.Top = UnitValue.Parse("5cm");
        //    section.Pane.ElementList.Add(text);

        //    var rect = new Rectangle {Width = UnitValue.Parse("3cm"), Height = UnitValue.Parse("1cm")};

        //    section.Pane.ElementList.Add(rect);

        //    section.Header.Height = UnitValue.Parse("15%");
        //    section.Header.ElementList.Add(Text.Create("Header text"));
        //    section.Footer.ElementList.Add(Text.Create("Footer text"));

        //    var template = Template.Create(section);

        //    var indexSection = Section.Create();
        //    indexSection.Pane.ElementList.Add(Text.Create("This is the index page"));
        //    template.SectionList.Add(indexSection);

        //    //1. Convert to xml
        //    var xmlString1 = template.ToXml();

        //    //2. Convert back to objects (compare original)
        //    var template2 = Template.Create(xmlString1);

        //    //3. Convert to xml again and compare two two strings
        //    var xmlString2 = template2.ToXml();
        //    //System.Console.WriteLine(xmlString1);

        //    if (xmlString1.OuterXml != xmlString2.OuterXml)
        //        throw new InvalidOperationException("Diff");

        //    var byteArray = Rendering.CreatePDFDocument(template);
        //    ExecuteFile(byteArray);
        //}

        private static void Blank_default_PDF_document()
        {
            var template = new Template(new Section());
            var byteArray = Rendering.CreatePDFDocument(template);
            ExecuteFile(byteArray);
        }

        private static void Multipage_PDF_by_spanning_text_border_case_where_text_ends_up_exactly()
        {
            var section = new Section { Name = "Content" };

            var data = "aaaa bbbb cccc dddd eeee ffff gggg hhhh iiii jjjj kkkk llll mmmm nnnn oooo ppp qqqq rrr sss tt uuu vvv xyz ";
            data = "START " + data + "MIDDLE1 " + data + "MIDDLE2 " + data + "MIDDLE3 " + data + "12345 67 END";

            section.Pane.ElementList.Add(new TextBox { Value = data, Top = UnitValue.Parse("1cm"), Left=UnitValue.Parse("1cm"), Width = UnitValue.Parse("5cm"), Height = UnitValue.Parse("4cm"), Name = "SpanText" });

            var template = new Template(section);

            var byteArray = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray);
        }

        public static void Multipage_PDF_by_spanning_text_using_a_reference_point_with_vertical_stacking()
        {
            var section = new Section { Name = "Content" };

            var referencePoint = new ReferencePoint { Top = UnitValue.Parse("5cm"), Left = UnitValue.Parse("4cm"), Stack = ReferencePoint.StackMethod.Vertical };

            referencePoint.ElementList.Add(new TextBox { Value = GetRandomText(), Width = UnitValue.Parse("5cm"), Height = UnitValue.Parse("8cm"), Name = "SpanText" });
            section.Pane.ElementList.Add(referencePoint);

            referencePoint.ElementList.Add(new TextBox { Value = "Tiny Text", Height = UnitValue.Parse("2cm"), Left= UnitValue.Parse("1cm"), Name ="SmallTextBox" });
            referencePoint.ElementList.Add(new Text { Value = "Regular text", Name = "RegularText" });

            var template = new Template(section);

            var byteArray = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray);

            var byteArray2 = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray2);            
        }

        public static void Multipage_PDF_by_spanning_text_using_a_reference_point()
        {
            var section = new Section { Name = "Content" };

            var referencePoint = new ReferencePoint{ Top = UnitValue.Parse("5cm"), Left = UnitValue.Parse("4cm")};
            referencePoint.ElementList.Add(new TextBox{ Value = GetRandomText(), Width = UnitValue.Parse("5cm"), Height = UnitValue.Parse("8cm")});
            section.Pane.ElementList.Add(referencePoint);

            referencePoint.ElementList.Add(new TextBox { Value = "Tiny Text", Left = UnitValue.Parse("8cm") });
            referencePoint.ElementList.Add(new Text { Value = "Regular text", Top = UnitValue.Parse("10cm") });

            var template = new Template(section);
            
            var byteArray = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray);

            var byteArray2 = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray2);
        }

        private static void Multipage_PDF_by_spanning_text()
        {
            var section = new Section {Name = "First"};
            section.Header.ElementList.Add(new Text {Value = "{PageNumber} ({TotalPages})"});
            section.Margin = new UnitRectangle {Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm"), Right = UnitValue.Parse("1cm"), Bottom = UnitValue.Parse("1cm")};

            var s = GetRandomText();
            var text = new TextBox {Value = s, Left = UnitValue.Parse("5cm"), Top = UnitValue.Parse("10cm")};
            section.Pane.ElementList.Add(text);

            //var rectangle = new Rectangle { Left = UnitValue.Parse("5cm"), Top = UnitValue.Parse("10cm"), Width = UnitValue.Parse("10cm") };
            //section.Pane.ElementList.Add(rectangle);

            var template = new Template(section);

            var byteArray = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray);

            //Render again should cause the same result
            var byteArray2 = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray2);
        }

        private static string GetRandomText()
        {
            var rng = new Random();
            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                var c = (char) (65 + rng.Next(0, 20));
                var value = new string(c, rng.Next(1, 10));
                sb.AppendFormat("{0} {{PageNumber}} ", value);
            }
            string s = sb.ToString();
            return s;
        }

        private static void Multipage_PDF_document_by_section()
        {
            var section = new Section { Name = "First"};
            var refPoint = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm") };
            refPoint.ElementList.Add(new Text { Value = "Hello world! {Data1}", HideValue = "Data1" });
            section.Pane.ElementList.Add(refPoint);            

            var template = new Template(section);
            var documentData = new DocumentData();
            documentData.Add("Data1", "Bob");

            //Adding a new page, by adding a new section
            var nextPage = new Section { Name = "Second" };
            nextPage.Pane.ElementList.Add(new Text{ Value = "More one next page...", Top = UnitValue.Parse("1cm"),Right = UnitValue.Parse("1cm")});

            template.SectionList.Add(nextPage);

            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData,debug: true);
            ExecuteFile(byteArray);
        }

        private static void Basic_PDF_document_with_some_text_on_it()
        {
            var section = new Section();
            var refPoint = new ReferencePoint {Stack = ReferencePoint.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm")};
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data1}", HideValue = "Data1"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data2}", HideValue = "Data2"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data3}", HideValue = "Data3"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data4}", HideValue = "Data4"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data5}", HideValue = "Data5"});
            refPoint.ElementList.Add(new Text {Value = "Data 2 have a value!", HideValue = "Data2"});
            section.Pane.ElementList.Add(refPoint);

            var elm = section.Pane.ElementList.Get<ReferencePoint>("SomeName");

            var template = new Template(section);
            var documentData = new DocumentData();
            documentData.Add("Data1", "Bob");
            documentData.Add("Data2", "");
            documentData.Add("Data3", null);
            documentData.Add("Data5", "Loblaw");
            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData);
            ExecuteFile(byteArray);
        }

        private static void ExecuteFile(byte[] byteArray)
        {
            var fileName = string.Format("{0}.pdf", System.IO.Path.GetTempFileName());
            System.IO.File.WriteAllBytes(fileName, byteArray);
            Process.Start(fileName);

            System.Threading.Thread.Sleep(5000);
            
            while(System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (System.IO.IOException)
                {
                    System.Console.WriteLine("Waiting for the document to close before it can be deleted...");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
