using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class DocumentData_Test
{
    [Fact(Skip = "Fix!")]
    public void Default_Document()
    {
        //Arrange
        var documentData = new DocumentData();
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        //Assert.AreEqual(documentData.Get("A"), other.Get("A"));
        //Assert.AreEqual(documentData.GetDataTable("TableA"), other.GetDataTable("TableA"));
        //Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        throw new NotImplementedException();
    }

    [Fact(Skip = "Fix!")]
    public void Document_with_data()
    {
        //Arrange
        var documentData = new DocumentData();
        documentData.Add("A", "DataA");
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        //Assert.AreEqual(documentData.Get("A"), other.Get("A"));            
        //Assert.AreEqual(documentData.GetDataTable("TableA"), other.GetDataTable("TableA"));
        //Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        throw new NotImplementedException();
    }

    [Fact(Skip = "Fix!")]
    public void Document_with_table()
    {
        //Arrange
        var documentData = new DocumentData();
        var t = new DocumentDataTable("TableA");
        var row = new Dictionary<string, string>();
        row.Add("Row1A", "RowData1A");
        row.Add("Row1B", "RowData1B");
        row.Add("Row1C", "RowData1C");
        t.AddRow(row);

        var row2 = new Dictionary<string, string>();
        row2.Add("Row2A", "RowData2A");
        row2.Add("Row2B", "RowData2A");
        row2.Add("Row2C", "RowData2A");
        t.AddRow(row2);

        documentData.Add(t);
        var xml = documentData.ToXml();

        //Act
        var other = DocumentData.Load(xml);

        //Assert
        //Assert.AreEqual(documentData.Get("A"), other.Get("A"));
        //Assert.AreEqual(documentData.GetDataTable("TableA").Rows.Count, other.GetDataTable("TableA").Rows.Count);
        //Assert.AreEqual(((DocumentDataTableData)documentData.GetDataTable("TableA").Rows[0]).Columns.First().Key, ((DocumentDataTableData)other.GetDataTable("TableA").Rows[0]).Columns.First().Key);
        //Assert.AreEqual(((DocumentDataTableData)documentData.GetDataTable("TableA").Rows[0]).Columns.First().Value, ((DocumentDataTableData)other.GetDataTable("TableA").Rows[0]).Columns.First().Value);
        //Assert.AreEqual(((DocumentDataTableData)documentData.GetDataTable("TableA").Rows[1]).Columns.First().Key, ((DocumentDataTableData)other.GetDataTable("TableA").Rows[1]).Columns.First().Key);
        //Assert.AreEqual(((DocumentDataTableData)documentData.GetDataTable("TableA").Rows[1]).Columns.First().Value, ((DocumentDataTableData)other.GetDataTable("TableA").Rows[1]).Columns.First().Value);
        //Assert.AreEqual(xml.OuterXml, other.ToXml().OuterXml);
        throw new NotImplementedException();
    }
}