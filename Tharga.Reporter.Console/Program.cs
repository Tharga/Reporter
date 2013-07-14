using System;
using System.Diagnostics;
using System.Text;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Blank_default_PDF_document();
            //Basic_PDF_document_with_some_text_on_it();
            //Multipage_PDF_document_by_section();
            //Multipage_PDF_by_spanning_text();
            //Create_PDF_document_from_template();
            //Create_PDF_document_with_basic_template();
            Create_PDF_document_with_template_that_spans_over_multiple_pages();
        }

        private static void Create_PDF_document_with_template_that_spans_over_multiple_pages()
        {
            var coverPage = new Section {Name = "Cover"};
            coverPage.Pane.ElementList.Add(new Text{Value = "This is the cover page.",Top = UnitValue.Parse("50%"), Left = UnitValue.Parse("50%")});

            var index = new Section {Name = "Index"};
            index.Pane.ElementList.Add(new Text { Value = "This is the index page.", Top = UnitValue.Parse("50mm") ,Left = UnitValue.Parse("2inch")});

            var content = new Section{Name = "Content"};

            var tableTemplate = new Table("MyTable") { BorderColor = System.Drawing.Color.Green,  Top = UnitValue.Parse("5cm"), Height = UnitValue.Parse("200"), Left = UnitValue.Parse("1cm"), Width = UnitValue.Parse("10cm") };
            content.Pane.ElementList.Add(tableTemplate);
            tableTemplate.AddColumn("A {Col1}", "Column 1", UnitValue.Parse("5cm"));
            tableTemplate.AddColumn("B {Col2}", "Column 2", UnitValue.Parse("5cm"));

            content.Header.Height = UnitValue.Parse("2cm");
            content.Footer.Height = UnitValue.Parse("2cm");
            content.Margin.Top = UnitValue.Parse("1cm");
            content.Margin.Bottom = UnitValue.Parse("1cm");
            content.Margin.Left = UnitValue.Parse("1cm");
            content.Margin.Right = UnitValue.Parse("1cm");

            //Handling page numbers
            content.Header.ElementList.Add(new Text { Value = "Some text in the header. Page {PageNumber} of {TotalPages}" });
            content.Footer.ElementList.Add(new Text { Value = "Some text in the footer. Page {PageNumber} of {TotalPages}" });
            content.Pane.ElementList.Add(new Text { Value = "Some text in the pane. Page {PageNumber} of {TotalPages}" });

            var documentData = new DocumentData();
            var tableData = documentData.GetDataTable("MyTable");
            for (var i = 0; i < 100; i++)
            {
                var row = tableData.AddRow();
                row.Add("Col1", string.Format("some data for row {0}", i));
                row.Add("Col2", "some oter data");
            }
            var template = Template.Create(coverPage);
            template.SectionList.Add(content);
            template.SectionList.Add(index);
            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData, debug: true);

            ExecuteFile(byteArray);
        }

        private static void Create_PDF_document_with_basic_template()
        {
            var section = Section.Create();

            var tableTemplate = new Table("MyTable") {BorderColor = System.Drawing.Color.Blue};
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

            var template = Template.Create(section);
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
            var template = Template.Create(Section.Create());
            var byteArray = Rendering.CreatePDFDocument(template);
            ExecuteFile(byteArray);
        }

        private static void Multipage_PDF_by_spanning_text()
        {
            var section = new Section { Name = "First" };
            section.Margin = new UnitRectangle {Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm"), Right = UnitValue.Parse("1cm"), Bottom = UnitValue.Parse("1cm")};

            var rng = new Random();
            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                var c = (char)(65 + rng.Next(0, 20));
                var value = new string(c, rng.Next(1,10));
                sb.AppendFormat("{0} ",value);
            }
            var text = new TextBox {Value = sb.ToString(), Left = UnitValue.Parse("5cm"), Top = UnitValue.Parse("10cm")};
            section.Pane.ElementList.Add(text);

            //var rectangle = new Rectangle { Left = UnitValue.Parse("5cm"), Top = UnitValue.Parse("10cm"), Width = UnitValue.Parse("10cm") };
            //section.Pane.ElementList.Add(rectangle);

            var template = Template.Create(section);

            var byteArray = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray);

            //Render again should cause the same result
            var byteArray2 = Rendering.CreatePDFDocument(template, debug: true);
            ExecuteFile(byteArray2);
        }

        private static void Multipage_PDF_document_by_section()
        {
            var section = new Section { Name = "First"};
            var refPoint = new ReferencePoint { Stack = ReferencePoint.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm") };
            refPoint.ElementList.Add(new Text { Value = "Hello world! {Data1}", HideValue = "Data1" });
            section.Pane.ElementList.Add(refPoint);            

            var template = Template.Create(section);
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
            var section = Section.Create();
            var refPoint = new ReferencePoint {Stack = ReferencePoint.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm")};
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data1}", HideValue = "Data1"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data2}", HideValue = "Data2"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data3}", HideValue = "Data3"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data4}", HideValue = "Data4"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data5}", HideValue = "Data5"});
            refPoint.ElementList.Add(new Text {Value = "Data 2 have a value!", HideValue = "Data2"});
            section.Pane.ElementList.Add(refPoint);

            var elm = section.Pane.ElementList.Get<ReferencePoint>("SomeName");

            var template = Template.Create(section);
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
