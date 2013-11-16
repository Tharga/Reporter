using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.Console
{
    public static class ReportBusinessHelper
    {
        public static UnitValue SumTop = UnitValue.Parse("15cm");

        internal static Section GetNoteBase(Color backLineColor, Color backFieldColor)
        {
            var section = new Section
            {
                Margin =
                {
                    Left = UnitValue.Parse("2cm"),
                    Right = UnitValue.Parse("1cm"),
                    Top = UnitValue.Parse("1cm"),
                    Bottom = UnitValue.Parse("0,5cm")
                }
            };

            #region Header


            section.Header.Height = UnitValue.Parse("6cm");
            section.Header.ElementList.Add(new Rectangle { Left = UnitValue.Parse("9,5cm"), Top = UnitValue.Parse("0cm"), Right = UnitValue.Parse("8,5cm"), Bottom = UnitValue.Parse("2cm"), BorderColor = backLineColor, BorderWidth = UnitValue.Parse("1px"), BackgroundColor = backFieldColor });
            section.Header.ElementList.Add(new Image { Source = "{Company.Logotype}", Height = UnitValue.Parse("3,5cm"), Top = UnitValue.Parse("0cm"), IsBackground = true });

            //Customer
            var customerArea = new ReferenceElement()
            {
                Left = UnitValue.Parse("10cm"),
                Top = UnitValue.Parse("2,5cm"),
                Stack = ReferenceElement.StackMethod.Vertical,
                Name = "CustomerArea"
            };
            section.Header.ElementList.Add(customerArea);
            customerArea.ElementList.Add(new Text { Value = "Kund nr: {Customer.Number}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.Name}" });
            //customerArea.ElementList.Add(Text.Create("Att: {Customer.DeliveryAddress.Representative.Name} ({Customer.DeliveryAddress.Representative.BuyerNumber})", hideValue: "Customer.DeliveryAddress.Representative.Name"));
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Note}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.StreetName}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.PostalCode} {Customer.DeliveryAddress.City}" });
            customerArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Country}" });

            var refArea = new ReferenceElement { Left = UnitValue.Parse("3cm"), Top = UnitValue.Parse("2,5cm"), Stack = ReferenceElement.StackMethod.Vertical };
            section.Header.ElementList.Add(refArea);
            refArea.ElementList.Add(new Text { Value = "Vår Referens" });
            refArea.ElementList.Add(new Text { Value = "{User.Name}" });
            refArea.ElementList.Add(new Text { Value = " " });
            refArea.ElementList.Add(new Text { Value = "Er Referens" }); //TODO: This should be hidden if there is no value for {Customer.DeliveryAddress.Representative.Name}.
            refArea.ElementList.Add(new Text { Value = "{Customer.DeliveryAddress.Representative.Name}" });


            #endregion
            #region Footer


            const string top = "3px";
            section.Footer.Height = UnitValue.Parse("2cm");
            var addressRefPoint = new ReferenceElement { Top = UnitValue.Parse(top), Stack = ReferenceElement.StackMethod.Vertical };
            section.Footer.ElementList.Add(addressRefPoint);
            addressRefPoint.ElementList.Add(new Text { Value = "Postadress", Font = { Color = backFieldColor, Size = 6 } });
            addressRefPoint.ElementList.Add(new Text { Value = "{Company.StreetName}", Font = { Color = backLineColor, Size = 8 } });
            addressRefPoint.ElementList.Add(new Text { Value = "{Company.PostalCode} {Company.City}", Font = { Color = backLineColor, Size = 8 } });

            var hallRefPoint = new ReferenceElement { Left = UnitValue.Parse("4cm"), Top = UnitValue.Parse(top), Stack = ReferenceElement.StackMethod.Vertical };
            section.Footer.ElementList.Add(hallRefPoint);
            hallRefPoint.ElementList.Add(new Text { Value = "Telefon", Font = { Color = backLineColor, Size = 6 } });
            hallRefPoint.ElementList.Add(new Text { Value = "{Company.Phone}", Font = { Color = backLineColor, Size = 8 } });
            hallRefPoint.ElementList.Add(new Text { Value = "Fax", Font = { Color = backLineColor, Size = 6 } });
            hallRefPoint.ElementList.Add(new Text { Value = "{Company.Fax}", Font = { Color = backLineColor, Size = 8 } });

            var tradRefPoint = new ReferenceElement { Left = UnitValue.Parse("8,5cm"), Top = UnitValue.Parse(top), Stack = ReferenceElement.StackMethod.Vertical };
            section.Footer.ElementList.Add(tradRefPoint);
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail1}", Font = { Color = backLineColor, Size = 8 } });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail2}", Font = { Color = backLineColor, Size = 8 } });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail3}", Font = { Color = backLineColor, Size = 8 } });

            var bg = new ReferenceElement { Left = UnitValue.Parse("16cm"), Top = UnitValue.Parse(top), Stack = ReferenceElement.StackMethod.Vertical };
            section.Footer.ElementList.Add(bg);
            bg.ElementList.Add(new Text { Value = "Bankgiro", Font = { Color = backLineColor, Size = 6 } });
            bg.ElementList.Add(new Text { Value = "{Bankgiro}", Font = { Color = backLineColor, Size = 8 } });


            #endregion
            #region Pane


            //Order Table
            //var orderItemTable = new Table("OrderItems", UnitValue.Parse("0cm"), UnitValue.Parse("0cm"), null)
            //{
            //    Height = UnitValue.Parse(SumTop),
            //    Width = UnitValue.Parse("18cm"),
            //    BorderColor = backLineColor,
            //    BackgroundColor = backFieldColor,
            //};
            //orderItemTable.AddColumn("{ArticleName}", "Specifikation", widthMode: Table.WidthMode.Spring);
            //orderItemTable.AddColumn("{BoxCount.NumberOfBoxes}*{BoxCount.ItemsPerBox}", "Lådor", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "*");
            //orderItemTable.AddColumn("{Count}", "Antal", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            //orderItemTable.AddColumn("{NetNormalItemPrice}", "Normal", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "{NetSaleItemPrice}");
            //orderItemTable.AddColumn("{DiscountPercentage}%", "Rabatt", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "0%");
            //orderItemTable.AddColumn("{NetSaleItemPrice}", "á-pris", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            //orderItemTable.AddColumn("{NetSaleTotalPrice}", "Belopp", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);

            //Order summary
            var orderSummaryBv = new ReferenceElement { Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryBv);
            orderSummaryBv.ElementList.Add(new Line { Width = UnitValue.Parse("100%"), BorderColor = backLineColor });
            //orderSummaryBv.ElementList.Add(new Rectangle("0cm", "0cm", "100%", "999cm", backLineColor));
            //orderSummaryBv.ElementList.Add(new Line("0cm", "1cm", "100%", "0cm", backLineColor));
            orderSummaryBv.ElementList.Add(new Line { Top = UnitValue.Parse("1cm"), Width = UnitValue.Parse("100%"), BorderColor = backLineColor });
            //orderSummaryBv.ElementList.Add(Text.Create("Betalningsvillkor", backLineColor, 6));

            //var orderSummaryDt = new ReferenceElement("7cm", sumTop);
            //section.Pane.ElementList.Add(orderSummaryDt);
            ////orderSummaryDt.ElementList.Add(new Line("0cm", "0cm", "0cm", "1cm", backLineColor));
            ////orderSummaryDt.ElementList.Add(Text.Create("Förfallodag", backLineColor, 6));

            //var orderSummaryOcr = new ReferenceElement("0cm", SumTop);
            //section.Pane.ElementList.Add(orderSummaryOcr);
            //var ocrTitle = Text.Create("OCR", backLineColor, 6);
            //ocrTitle.Left = UnitValue.Parse("2px");
            //orderSummaryOcr.ElementList.Add(ocrTitle);
            //var ocrText = Text.Create("{OCR}", top: "0,5cm");
            //ocrText.Width = UnitValue.Parse("2,45cm");
            //ocrText.TextAlignment = TextBase.Alignment.Left;
            //ocrText.Left = UnitValue.Parse("2px"); 
            //orderSummaryOcr.ElementList.Add(ocrText);

            var orderSummaryPe = new ReferenceElement { Left = UnitValue.Parse("10cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryPe);
            orderSummaryPe.ElementList.Add(new Line { Height = UnitValue.Parse("1cm"), BorderColor = backLineColor });
            var peTitle = new Text { Value = "Summa exkl. moms", Font = { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryPe.ElementList.Add(peTitle);
            var netSaleTotalPriceText = new Text { Value = "{NetSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,45cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryPe.ElementList.Add(netSaleTotalPriceText);

            var orderSummaryVat = new ReferenceElement { Left = UnitValue.Parse("12,5cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryVat);
            orderSummaryVat.ElementList.Add(new Line { Height = UnitValue.Parse("1cm"), BorderColor = backLineColor });
            var vatTiel = new Text { Value = "Moms", Font = { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryVat.ElementList.Add(vatTiel);
            var vatSaleTotalPriceText = new Text { Value = "{VatSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,45cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryVat.ElementList.Add(vatSaleTotalPriceText);

            var orderSummaryGross = new ReferenceElement { Left = UnitValue.Parse("15cm"), Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryGross);
            orderSummaryGross.ElementList.Add(new Line { Top = UnitValue.Parse("1cm"), BorderColor = backLineColor });
            var grossTitle = new Text { Value = "Att betala", Font = { Color = backLineColor, Size = 6 }, Left = UnitValue.Parse("2px") };
            orderSummaryGross.ElementList.Add(grossTitle);
            var grossSaleTotalPriceText = new Text { Value = "{GrossSaleTotalPrice}", Top = UnitValue.Parse("0,5cm"), Width = UnitValue.Parse("2,95cm"), TextAlignment = TextBase.Alignment.Right };
            orderSummaryGross.ElementList.Add(grossSaleTotalPriceText);

            //Extra info
            var vatInfoReference = new ReferenceElement { Left = UnitValue.Parse("0"), Top = SumTop };
            var vatInfoText = new Text { Value = "Org.nr {Company.OrgNo}. Innehar F-skattsedel. Vat nr {Company.VatNo}.", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1cm"), Font = { Size = 6, Color = backLineColor } };
            vatInfoReference.ElementList.Add(vatInfoText);
            //var lateInfoText = new Text { Value = "Vid betalning efter förfallodagen debiteras 18%", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1,5cm"), FontSize = 10, FontColor = backLineColor };
            //vatInfoReference.ElementList.Add(lateInfoText);

            //TODO: Add to template
            var signText = new Text { Value = "Er kvittens", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1,5cm"), Font = { Size = 10, Color = backLineColor } };
            vatInfoReference.ElementList.Add(signText);
            var signLine = new Line { Left = UnitValue.Parse("2cm"), Top = UnitValue.Parse("2cm"), Width = UnitValue.Parse("5cm"), BorderColor = backLineColor };
            vatInfoReference.ElementList.Add(signLine);

            section.Pane.ElementList.Add(vatInfoReference);

            //Pane
            //section.Pane.ElementList.Add(orderItemTable);
            //section.Pane.ElementList.Add(new Rectangle {BorderColor = backLineColor, BorderWidth = UnitValue.Parse("1px")});
            section.Pane.ElementList.Add(new Rectangle { BorderColor = backLineColor });


            #endregion

            return section;
        }

        internal static void AssignFooterData(ref DocumentData documentData)
        {
            //TODO: Load this information from the service
            documentData.Add("Company.Name", "Skalleberg HTRG");
            documentData.Add("Company.StreetName", "Ormbackavägen 67");
            documentData.Add("Company.PostalCode", "175 61");
            documentData.Add("Company.City", "JÄRFÄLLA");
            documentData.Add("Company.Phone", "08-91 99 66");
            documentData.Add("Company.Fax", "08-91 50 67");
            documentData.Add("Company.OrgNo", "916619-5488");
            documentData.Add("Company.VatNo", "SE916619548801");
            documentData.Add("Bankgiro", "364-1230");
            documentData.Add("EMail1", "lennart@skalleberg.se");
            documentData.Add("EMail2", "roger@skalleberg.se");
            documentData.Add("EMail3", "oddbjorn@skalleberg.se");
        }
    }

    class SkallSim1
    {
        public static Section GetDeliveryNoteSection(Color backLineColor)
        {
            //var backFieldColor = Color.FromArgb(backLineColor.A, backLineColor.R + 127, backLineColor.G + 127, backLineColor.B + 127);
            var backFieldColor = Color.White;
            //var backFieldColor = Color.FromArgb(255, 128, 128, 255);

            var section = ReportBusinessHelper.GetNoteBase(backLineColor, backFieldColor);

            #region Header

            section.Header.ElementList.Add(new Text { Value = "Följesedel", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("0"), Font = { Size = 22 } });
            section.Header.ElementList.Add(new Text { Value = "Nr:", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("24px") });
            section.Header.ElementList.Add(new Text { Value = "{Number}", Left = UnitValue.Parse("12.5cm"), Top = UnitValue.Parse("24px") });
            section.Header.ElementList.Add(new Text { Value = "Datum:", Left = UnitValue.Parse("10cm"), Top = UnitValue.Parse("36px") });
            section.Header.ElementList.Add(new Text { Value = "{Date}", Left = UnitValue.Parse("12,5cm"), Top = UnitValue.Parse("36px") });


            #endregion
            #region Footer



            #endregion
            #region Pane


            ////Signature for the representative
            //var signRefPoint = new ReferenceElement("1cm", "18cm");
            //section.Pane.ElementList.Add(signRefPoint);
            //signRefPoint.ElementList.Add(new Line("0cm", "0cm", "8cm", "0cm", backLineColor, "1px"));
            //signRefPoint.ElementList.Add(Text.Create("{Representative.Name}", "0", "0cm"));

            section.Pane.ElementList.Add(GetDeliveryNoteTable(backLineColor, backFieldColor));

            #endregion

            return section;
        }

        private static Table GetDeliveryNoteTable(Color? backLineColor, Color? backFieldColor)
        {
            var orderItemTable = new Table
            {
                Name = "OrderItems",
                Height = ReportBusinessHelper.SumTop,
                Width = UnitValue.Parse("100%"),
                BorderColor = backLineColor ?? Color.Black,
                BackgroundColor = backFieldColor,
            };
            //orderItemTable.AddColumn("{BoxCount.NumberOfBoxes}*{BoxCount.ItemsPerBox}", "Lådor", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "*");
            orderItemTable.AddColumn("{Description}", "Specifikation", widthMode: Table.WidthMode.Spring);
            orderItemTable.AddColumn("{AmountDescription}", "Lådor", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "");
            orderItemTable.AddColumn("{Count}", "Antal", widthMode: Table.WidthMode.Specific, width: UnitValue.Parse("3cm"), alignment: Table.Alignment.Right);
            orderItemTable.AddColumn("{NetNormalItemPrice}", "Pris", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "{NetSaleItemPrice}");
            orderItemTable.AddColumn("{DiscountPercentage}%", "Rabatt", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right, hideValue: "0%");
            orderItemTable.AddColumn("{NetSaleItemPrice}", "Säljpris", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            orderItemTable.AddColumn("{NetSaleTotalPrice}", "Belopp", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);
            //orderItemTable.AddColumn("{VAT}", "Moms", UnitValue.Parse("2cm"), alignment: Table.Alignment.Right);

            return orderItemTable;
        }

    }

    static class Program
    {
        static void Main(string[] args)
        {
            //Blank_default_PDF_document();
            //Basic_PDF_document_with_some_text_on_it();
            //Multipage_PDF_document_by_section();
            //Multipage_PDF_by_spanning_text();
            //Multipage_PDF_by_spanning_text_using_a_reference_point();
            //Multipage_PDF_by_spanning_text_using_a_reference_point_with_vertical_stacking();
            Multipage_PDF_by_spanning_text_border_case_where_text_ends_up_exactly();
            //Create_PDF_document_from_template();
            //Create_PDF_document_with_basic_template();
            //Create_PDF_document_with_template_that_spans_over_multiple_pages();
            //SkallebergSample1();

            //Bugs: 
            //When spanning with textBoxes an empty page can sometimes be added, if the last word fitx exactly

            //What else do we want??
            //- Serialize/unzerialize template to xml
        }

        private static void SkallebergSample1()
        {
            var section = SkallSim1.GetDeliveryNoteSection(Color.FromArgb(255, 0, 0, 0));
            var t = new Template(section);

            var xme = t.ToXml();

            var t2 = Template.Load(xme);
            var xme2 = t2.ToXml();

            if (xme.OuterXml != xme2.OuterXml)
                Debug.WriteLine("Oups!");
        }

        private static void Create_PDF_document_with_template_that_spans_over_multiple_pages()
        {
            var coverPage = new Section {Name = "Cover"};
            coverPage.Pane.ElementList.Add(new Text{Value = "This is the cover page.",Top = UnitValue.Parse("50%"), Left = UnitValue.Parse("50%")});

            var index = new Section {Name = "Index"};
            index.Pane.ElementList.Add(new Text { Value = "This is the index page.", Top = UnitValue.Parse("50mm") ,Left = UnitValue.Parse("2inch")});

            var content = new Section{Name = "Content"};

            var tableTemplate = new Table { Name ="MyTable", BorderColor = System.Drawing.Color.Green,  Top = UnitValue.Parse("5cm"), Height = UnitValue.Parse("200"), Left = UnitValue.Parse("1cm"), Width = UnitValue.Parse("10cm") };
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
            var template = new Template(coverPage);
            template.SectionList.Add(content);
            template.SectionList.Add(index);
            var byteArray = Rendering.CreatePDFDocument(template, documentData: documentData, debug: true);

            ExecuteFile(byteArray);
        }

        private static void Create_PDF_document_with_basic_template()
        {
            var section = new Section();

            var tableTemplate = new Table {Name = "MyTable", BorderColor = System.Drawing.Color.Blue};
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

            var referencePoint = new ReferenceElement { Top = UnitValue.Parse("5cm"), Left = UnitValue.Parse("4cm"), Stack = ReferenceElement.StackMethod.Vertical };

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

            var referencePoint = new ReferenceElement{ Top = UnitValue.Parse("5cm"), Left = UnitValue.Parse("4cm")};
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
            var section = new Section { Name = "First" };
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
            var refPoint = new ReferenceElement { Stack = ReferenceElement.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm") };
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
            var refPoint = new ReferenceElement {Stack = ReferenceElement.StackMethod.Vertical, Name = "SomeName", Left = UnitValue.Parse("1cm"), Top = UnitValue.Parse("1cm")};
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data1}", HideValue = "Data1"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data2}", HideValue = "Data2"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data3}", HideValue = "Data3"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data4}", HideValue = "Data4"});
            refPoint.ElementList.Add(new Text {Value = "Hello world! {Data5}", HideValue = "Data5"});
            refPoint.ElementList.Add(new Text {Value = "Data 2 have a value!", HideValue = "Data2"});
            section.Pane.ElementList.Add(refPoint);

            var elm = section.Pane.ElementList.Get<ReferenceElement>("SomeName");

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
