namespace Tharga.Reporter.Entity;

public abstract class DocumentDataTableLine
{
}

public class DocumentDataTableGroup : DocumentDataTableLine
{
    public DocumentDataTableGroup(string content)
    {
        Content = content;
    }

    public string Content { get; }
}

public class DocumentDataTableData : DocumentDataTableLine
{
    public DocumentDataTableData(Dictionary<string, string> columns)
    {
        Columns = columns;
    }

    public Dictionary<string, string> Columns { get; }
}

public class DocumentDataTable
{
    public DocumentDataTable(string tableName)
    {
        TableName = tableName;
    }

    public string TableName { get; }

    public List<DocumentDataTableLine> Rows { get; } = new();

    public void AddRow(Dictionary<string, string> row)
    {
        Rows.Add(new DocumentDataTableData(row));
    }

    public void AddGroup(string groupContent)
    {
        Rows.Add(new DocumentDataTableGroup(groupContent));
    }
}