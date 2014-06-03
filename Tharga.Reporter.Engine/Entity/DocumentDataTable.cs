using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public abstract class DocumentDataTableLine
    {
    }

    public class DocumentDataTableGroup : DocumentDataTableLine
    {
        private readonly string _content;

        public DocumentDataTableGroup(string content)
        {
            _content = content;
        }

        public string Content { get { return _content; } }
    }

    public class DocumentDataTableData : DocumentDataTableLine
    {
        private readonly Dictionary<string, string> _columns;

        public DocumentDataTableData(Dictionary<string, string> columns)
        {
            _columns = columns;
        }

        public Dictionary<string, string> Columns { get { return _columns; } }
    }

    public class DocumentDataTable
    {
        private readonly string _tableName;
        private readonly List<DocumentDataTableLine> _data = new List<DocumentDataTableLine>();

        public string TableName { get { return _tableName; } }
        public List<DocumentDataTableLine> Rows { get { return _data; } }

        public DocumentDataTable(string tableName)
        {
            _tableName = tableName;
        }

        public void AddRow(Dictionary<string, string> row)
        {
            _data.Add(new DocumentDataTableData(row));
        }

        public void AddGroup(string groupContent)
        {
            _data.Add(new DocumentDataTableGroup(groupContent));
        }
    }
}