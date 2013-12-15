using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTable
    {
        private readonly string _tableName;
        private readonly List<KeyValuePair<string, string>> _data = new List<KeyValuePair<string, string>>();

        public string TableName { get { return _tableName; } }
        public List<KeyValuePair<string, string>> Rows { get { return _data; } }

        internal DocumentDataTable(string tableName)
        {
            _tableName = tableName;
        }

        public void AddRow(string key, string value)
        {
            _data.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}