using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class DocumentData_Test
{
    [Fact]
    public void Default_Document()
    {
        //Arrange
        var documentData = new DocumentData();
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        other.Get("A").Should().BeNull();
        other.GetDataTable("TableA").Should().BeNull();
        other.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Document_with_data()
    {
        //Arrange
        var documentData = new DocumentData();
        documentData.Add("A", "DataA");
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        other.Get("A").Should().Be("DataA");
        other.GetDataTable("TableA").Should().BeNull();
        other.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Document_with_table()
    {
        //Arrange
        var documentData = new DocumentData();
        var table = new DocumentDataTable("TableA");
        table.AddRow(new Dictionary<string, string> { { "Row1A", "RowData1A" }, { "Row1B", "RowData1B" }, { "Row1C", "RowData1C" } });
        table.AddRow(new Dictionary<string, string> { { "Row2A", "RowData2A" }, { "Row2B", "RowData2B" }, { "Row2C", "RowData2C" } });
        documentData.Add(table);
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        var rows = other.GetDataTable("TableA").Rows.Cast<DocumentDataTableData>().ToArray();
        rows.Should().HaveCount(2);
        rows[0].Columns.Should().Equal(new Dictionary<string, string> { { "Row1A", "RowData1A" }, { "Row1B", "RowData1B" }, { "Row1C", "RowData1C" } });
        rows[1].Columns.Should().Equal(new Dictionary<string, string> { { "Row2A", "RowData2A" }, { "Row2B", "RowData2B" }, { "Row2C", "RowData2C" } });
        other.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }

    [Fact]
    public void Document_with_group_lines()
    {
        //Arrange
        var documentData = new DocumentData();
        var table = new DocumentDataTable("A");
        table.AddGroup("Group1");
        table.AddRow(new Dictionary<string, string> { { "Col1", "Data1A" }, { "Col2", "Data2A" } });
        table.AddRow(new Dictionary<string, string> { { "Col1", "Data1B" }, { "Col2", "Data2B" } });
        table.AddGroup("Group2");
        table.AddRow(new Dictionary<string, string> { { "Col1", "Data1C" }, { "Col2", "Data2C" } });
        documentData.Add(table);
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        var rows = other.GetDataTable("A").Rows;
        rows.Select(x => x.GetType()).Should().Equal(typeof(DocumentDataTableGroup), typeof(DocumentDataTableData), typeof(DocumentDataTableData), typeof(DocumentDataTableGroup), typeof(DocumentDataTableData));
        rows.OfType<DocumentDataTableGroup>().Select(x => x.Content).Should().Equal("Group1", "Group2");
        ((DocumentDataTableData)rows[1]).Columns.Should().Equal(new Dictionary<string, string> { { "Col1", "Data1A" }, { "Col2", "Data2A" } });
        other.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }
}
