namespace Tharga.Reporter.Entity.Area;

public class ElementList : List<Element.Base.Element>
{
    public T Get<T>(string elementName)
        where T : Element.Base.Element
    {
        var item = this.SingleOrDefault(x => string.Compare(x.Name, elementName, StringComparison.InvariantCulture) == 0 && x.GetType() == typeof(T));
        return item as T;
    }
}