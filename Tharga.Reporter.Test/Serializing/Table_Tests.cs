using System.Drawing;
using System.Drawing.Printing;
using MigraDoc.Rendering;
using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Table_Tests
    {
        [Test]
        public void Default_Table()
        {
            //Arrange
            var table = new Table();
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);
            Assert.AreEqual(table.ContentBackgroundColor, otherLine.ContentBackgroundColor);
            Assert.AreEqual(table.ContentBorderColor, otherLine.ContentBorderColor);
            Assert.AreEqual(table.HeaderBackgroundColor, otherLine.HeaderBackgroundColor);
            Assert.AreEqual(table.HeaderBorderColor, otherLine.HeaderBorderColor);
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            Assert.AreEqual(table.HeaderFont.Size, otherLine.HeaderFont.Size);
            Assert.AreEqual(table.ContentFontClass, otherLine.ContentFontClass);
            Assert.AreEqual(table.ContentFont.FontName, otherLine.ContentFont.FontName);
            Assert.AreEqual(table.SkipLine, otherLine.SkipLine);
            Assert.AreEqual(table.ColumnPadding, otherLine.ColumnPadding);
            Assert.AreEqual(table.RowPadding, otherLine.RowPadding);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(table.Columns.Count, otherLine.Columns.Count);
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Table_with_all_properties_set()
        {
            //Arrange
            var table = new Table
                {
                    ContentBackgroundColor = Color.Navy,
                    ContentBorderColor = Color.Olive,
                    HeaderBackgroundColor = Color.MediumTurquoise,
                    HeaderBorderColor = Color.MediumVioletRed,
                    Bottom = UnitValue.Parse("10px"),
                    HeaderFontClass = "A",
                    Height = UnitValue.Parse("20px"),
                    IsBackground = true,
                    Left = UnitValue.Parse("10cm"),
                    Right = UnitValue.Parse("20cm"),
                    Name = "Bob",
                    ContentFont = new Font {FontName = "Times", Size = 7},
                    SkipLine = new SkipLine {Interval = 5, Height = "8mm"},
                    ColumnPadding = UnitValue.Parse("7mm"),
                    RowPadding = UnitValue.Parse("6mm"),
                };
            table.AddColumn("A0", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
            table.AddColumn("A1", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
            table.AddColumn("A2", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);
            Assert.AreEqual(table.ContentBackgroundColor.Value.ToArgb(), otherLine.ContentBackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.ContentBorderColor.Value.ToArgb(), otherLine.ContentBorderColor.Value.ToArgb());
            Assert.AreEqual(table.HeaderBackgroundColor.Value.ToArgb(), otherLine.HeaderBackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.HeaderBorderColor.Value.ToArgb(), otherLine.HeaderBorderColor.Value.ToArgb());
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            Assert.AreEqual(table.HeaderFont.Size, otherLine.HeaderFont.Size);
            Assert.AreEqual(table.ContentFontClass, otherLine.ContentFontClass);
            Assert.AreEqual(table.ContentFont.FontName, otherLine.ContentFont.FontName);
            Assert.AreEqual(table.SkipLine.Interval, otherLine.SkipLine.Interval);
            Assert.AreEqual(table.SkipLine.Height, otherLine.SkipLine.Height);
            Assert.AreEqual(table.ColumnPadding, otherLine.ColumnPadding);
            Assert.AreEqual(table.RowPadding, otherLine.RowPadding);
            Assert.AreEqual(table.ToString(), otherLine.ToString());           
            Assert.AreEqual(table.Columns.Count, otherLine.Columns.Count);
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void When_rendering_an_empty_document_with_debugging()
        {
            //Arrange
            var sectionName = "ABC123";
            var section = new Section { Name = sectionName };
            var template = new Template(section);
            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize());
            graphicsMock.Setup(x => x.DrawString(sectionName, It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()));
            graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
            var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
            graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>(), It.IsAny<DocumentRenderer>(), It.IsAny<int>())).Returns(graphicsMock.Object);
            var renderer = new Renderer(graphicsFactoryMock.Object, template, null, true, null, true);

            //Act
            var data = renderer.GetPdfBinary();

            //Assert
            Assert.Greater(data.Length, 0);
        }

        [Test]
        public void When_rendering_a_document_with_an_empty_table()
        {
            //Arrange
            var table = new Table();
            var section = new Section();
            section.Pane.ElementList.Add(table);
            var template = new Template(section);
            var documentData = new DocumentData();
            var renderer = new Renderer(template, documentData);

            //Act
            var data = renderer.GetPdfBinary();

            //Assert
        }

        [Test]
        public void When_rendering_several_times_with_the_same_template()
        {
            //Arrange
            var table1 = new Table{ Name = "TableA", Top = "2cm", Height="5cm"};
            table1.Columns.Add("ColumnA1", new TableColumn("Column A", "2cm", Table.WidthMode.Auto, Table.Alignment.Left, string.Empty));
            var section = new Section();
            section.Pane.ElementList.Add(table1);
            var template = new Template(section);
            var documentData1 = new DocumentData();
            for (var i = 0; i < 30; i++)
            {
                var row1 = documentData1.GetDataTable("TableA").AddRow();
                row1.Add("ColumnA1", "DataA" + i);
            }

            var documentData2 = new DocumentData();
            for (var i = 0; i < 10; i++)
            {
                var row1 = documentData2.GetDataTable("TableA").AddRow();
                row1.Add("ColumnA1", "DataA" + i);
            }

            var documentData3 = new DocumentData();
            for (var i = 0; i < 20; i++)
            {
                var row1 = documentData2.GetDataTable("TableA").AddRow();
                row1.Add("ColumnA1", "DataA" + i);
            }

            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize());
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
            graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));

            var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
            graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>(), It.IsAny<DocumentRenderer>(), It.IsAny<int>())).Returns(graphicsMock.Object);

            var renderer1 = new Renderer(graphicsFactoryMock.Object, template, documentData1);
            var renderer2 = new Renderer(graphicsFactoryMock.Object, template, documentData2);
            var renderer3 = new Renderer(graphicsFactoryMock.Object, template, documentData3);

            //Act
            var data1 = renderer1.GetPdfBinary();
            var data2 = renderer2.GetPdfBinary();
            var data3 = renderer3.GetPdfBinary();

            //Assert           
        }
    }
}