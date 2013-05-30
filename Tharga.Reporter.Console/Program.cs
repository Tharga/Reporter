using System;
using System.Diagnostics;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ////Test 1. Create a basic PDF document
            //var template = Template.Create(Section.Create());
            //var byteArray = Renderer.CreatePDFDocument(template);
            //ExecuteFile(byteArray);

            ////Test 2. Create a basic PDF document with some text on it
            //var section = Section.Create();
            //section.Pane.ElementList.Add(Text.Create("Hello world!"));
            //var template = Template.Create(section);
            //var byteArray = Renderer.CreatePDFDocument(template);
            //ExecuteFile(byteArray);

            //Test 3. Create a document from a separate template
            //var section = Section.Create();
            //section.Header.Height = UnitValue.Parse("10%");
            //section.Footer.Height = UnitValue.Parse("10%");
            //section.Margin.Left = UnitValue.Parse("3cm");
            //section.Margin.Top = UnitValue.Parse("1cm");
            //section.Margin.Right = UnitValue.Parse("1cm");
            //section.Margin.Bottom = UnitValue.Parse("0.5cm");
            
            //var text = Text.Create("More Text");
            //text.Font.FontName = "Times New Roman";
            //text.Font.Size = 22;
            //text.Left = UnitValue.Parse("5cm");
            //text.Top = UnitValue.Parse("5cm");
            //section.Pane.ElementList.Add(text);

            //var rect = Rectangle.Create();
            //rect.Width = UnitValue.Parse("3cm");
            //rect.Height = UnitValue.Parse("1cm");

            //section.Pane.ElementList.Add(rect);

            //section.Header.Height = UnitValue.Parse("15%");
            //section.Header.ElementList.Add(Text.Create("Header text"));
            //section.Footer.ElementList.Add(Text.Create("Footer text"));

            //var template = Template.Create(section);

            //var indexSection = Section.Create();
            //indexSection.Pane.ElementList.Add(Text.Create("This is the index page"));
            //template.SectionList.Add(indexSection);

            ////1. Convert to xml
            //var xmlString1 = template.ToXml();

            ////2. Convert back to objects (compare original)
            //var template2 = Template.Create(xmlString1);

            ////3. Convert to xml again and compare two two strings
            //var xmlString2 = template2.ToXml();
            ////System.Console.WriteLine(xmlString1);

            //if ( xmlString1.OuterXml != xmlString2.OuterXml)
            //    throw new InvalidOperationException("Diff");

            //var byteArray = Renderer.CreatePDFDocument(template);
            //ExecuteFile(byteArray);

            //Test 4. Create a table with data
            var section = Section.Create();
            section.Pane.ElementList.Add(Text.Create("Hello world!"));
            var template = Template.Create(section);
            var byteArray = Renderer.CreatePDFDocument(template);
            ExecuteFile(byteArray);


            //Test 5. Create a table with data that spans over more than one page

            //System.Console.WriteLine("Press any key to exit...");
            //System.Console.ReadKey();
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
