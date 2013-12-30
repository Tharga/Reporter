namespace Tharga.Reporter.Engine.Entity
{
    public interface IDocumentData
    {
        string Get(string dataName);
        DocumentDataTable GetDataTable(string name);
    }
}