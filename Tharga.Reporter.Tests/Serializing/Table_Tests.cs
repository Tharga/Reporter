using System.Drawing;
using System.Text;
using AwesomeAssertions;
using Moq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Tharga.Reporter.Tests.Rendering;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing;

public class Table_Tests
{
    [Fact]
    public void Default_Table()
    {
        //Arrange
        var table = new Table();
        var xme = table.ToXme();

        //Act
        var other = Table.Load(xme);

        //Assert
        other.Left.Should().Be(table.Left);
        other.Right.Should().Be(table.Right);
        other.Width.Should().Be(table.Width);
        other.Top.Should().Be(table.Top);
        other.Bottom.Should().Be(table.Bottom);
        other.Height.Should().Be(table.Height);
        other.IsBackground.Should().Be(table.IsBackground);
        other.Name.Should().Be(table.Name);
        other.ContentBackgroundColor.Should().Be(table.ContentBackgroundColor);
        other.ContentBorderColor.Should().Be(table.ContentBorderColor);
        other.HeaderBackgroundColor.Should().Be(table.HeaderBackgroundColor);
        other.HeaderBorderColor.Should().Be(table.HeaderBorderColor);
        other.HeaderFontClass.Should().Be(table.HeaderFontClass);
        other.HeaderFont.Size.Should().Be(table.HeaderFont.Size);
        other.ContentFontClass.Should().Be(table.ContentFontClass);
        other.ContentFont.FontName.Should().Be(table.ContentFont.FontName);
        other.SkipLine.Should().BeNull();
        other.ColumnPadding.Should().Be(table.ColumnPadding);
        other.RowPadding.Should().Be(table.RowPadding);
        other.Columns.Should().BeEmpty();
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void Table_with_all_properties_set()
    {
        //Arrange
        var table = new Table
        {
            Bottom = UnitValue.Parse("10px"),
            Height = UnitValue.Parse("20px"),
            IsBackground = true,
            Left = UnitValue.Parse("10cm"),
            Right = UnitValue.Parse("20cm"),
            Name = "Bob",

            ContentBackgroundColor = Color.Navy,
            ContentBorderColor = Color.Olive,
            ContentFont = new Font { FontName = "Times", Size = 7 },

            HeaderBackgroundColor = Color.MediumTurquoise,
            HeaderBorderColor = Color.MediumVioletRed,
            HeaderFontClass = "A",

            GroupBackgroundColor = Color.Plum,
            GroupBorderColor = Color.Red,
            GroupSpacing = "5mm",
            GroupFont = new Font { Bold = true, Color = Color.Red, FontName = "Times", Size = 12 },

            SkipLine = new SkipLine { Interval = 5, Height = "8mm" },
            ColumnPadding = UnitValue.Parse("7mm"),
            RowPadding = UnitValue.Parse("6mm")
        };
        table.AddColumn("A0", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
        table.AddColumn("A1", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
        table.AddColumn("A2", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
        var xme = table.ToXme();

        //Act
        var other = Table.Load(xme);

        //Assert
        other.Left.Should().Be(table.Left);
        other.Right.Should().Be(table.Right);
        other.Width.Should().Be(table.Width);
        other.Top.Should().Be(table.Top);
        other.Bottom.Should().Be(table.Bottom);
        other.Height.Should().Be(table.Height);
        other.IsBackground.Should().BeTrue();
        other.Name.Should().Be("Bob");

        ArgbOf(other.ContentBackgroundColor).Should().Be(Color.Navy.ToArgb());
        ArgbOf(other.ContentBorderColor).Should().Be(Color.Olive.ToArgb());
        other.ContentFontClass.Should().Be(table.ContentFontClass);
        other.ContentFont.FontName.Should().Be("Times");

        ArgbOf(other.HeaderBackgroundColor).Should().Be(Color.MediumTurquoise.ToArgb());
        ArgbOf(other.HeaderBorderColor).Should().Be(Color.MediumVioletRed.ToArgb());
        other.HeaderFontClass.Should().Be("A");
        other.HeaderFont.Size.Should().Be(table.HeaderFont.Size);

        ArgbOf(other.GroupBackgroundColor).Should().Be(Color.Plum.ToArgb());
        ArgbOf(other.GroupBorderColor).Should().Be(Color.Red.ToArgb());
        other.GroupSpacing.Should().Be(UnitValue.Parse("5mm"));
        other.GroupFont.FontName.Should().Be("Times");
        other.GroupFont.Size.Should().Be(12);

        other.SkipLine.Interval.Should().Be(5);
        other.SkipLine.Height.Should().Be(UnitValue.Parse("8mm"));
        other.ColumnPadding.Should().Be(UnitValue.Parse("7mm"));
        other.RowPadding.Should().Be(UnitValue.Parse("6mm"));
        other.Columns.Keys.Should().Equal("A0", "A1", "A2");
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void When_rendering_an_empty_document_with_debugging()
    {
        //Arrange
        const string sectionName = "ABC123";
        var template = new Template(new Section { Name = sectionName });
        var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
        graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize(10, 10));
        graphicsMock.Setup(x => x.DrawString(sectionName, It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()));
        graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
        var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
        graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>())).Returns(graphicsMock.Object);
        var renderer = new Renderer(graphicsFactoryMock.Object, template, null, true, null, true);

        //Act
        var data = renderer.GetPdfBinary(PageSize.A4);

        //Assert
        IsPdf(data).Should().BeTrue();
        graphicsMock.Verify(x => x.DrawString(sectionName, It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()), Times.Once);
        graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Exactly(6));
    }

    [Fact]
    public void When_rendering_a_document_with_an_empty_table()
    {
        //Arrange
        var section = new Section();
        section.Pane.ElementList.Add(new Table());
        var renderer = new Renderer(new Template(section), new DocumentData());

        //Act
        var data = renderer.GetPdfBinary(PageSize.A4);

        //Assert
        IsPdf(data).Should().BeTrue();
    }

    [Fact]
    public void When_rendering_several_times_with_the_same_template()
    {
        //Arrange
        var table = new Table { Name = "TableA", Top = "2cm", Height = "5cm" };
        table.Columns.Add("ColumnA1", new TableColumn("Column A", "2cm", Table.WidthMode.Auto, Table.Alignment.Left, string.Empty));
        var section = new Section();
        section.Pane.ElementList.Add(table);
        var template = new Template(section);

        //Act
        var thirtyRows = Render(template, CreateData(30));
        var tenRows = Render(template, CreateData(10));
        var twentyRows = Render(template, CreateData(20));
        var thirtyRowsAgain = Render(template, CreateData(30));

        //Assert
        tenRows.Should().BeLessThan(twentyRows);
        twentyRows.Should().BeLessThan(thirtyRows);
        thirtyRowsAgain.Should().Be(thirtyRows);
    }

    private static DocumentData CreateData(int rowCount)
    {
        var documentData = new DocumentData();
        var table = new DocumentDataTable("TableA");
        documentData.Add(table);
        for (var i = 0; i < rowCount; i++)
        {
            table.AddRow(new Dictionary<string, string> { { "ColumnA1", $"DataA{i}" } });
        }

        return documentData;
    }

    private static int Render(Template template, DocumentData documentData)
    {
        var graphicsFactory = new PageCountingGraphicsFactory();
        var data = new Renderer(graphicsFactory, template, documentData).GetPdfBinary(PageSize.A4);
        IsPdf(data).Should().BeTrue();
        return graphicsFactory.RenderedPageCount;
    }

    private static bool IsPdf(byte[] data)
    {
        return data.Length > 4 && Encoding.ASCII.GetString(data, 0, 4) == "%PDF";
    }

    private static int ArgbOf(Color? color)
    {
        color.Should().NotBeNull();
        return color.GetValueOrDefault().ToArgb();
    }
}
