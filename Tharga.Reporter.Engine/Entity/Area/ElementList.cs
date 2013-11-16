using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Reporter.Engine.Entity.Area
{
    //TODO: Hide List inside this class
    public class ElementList : List<Element.Element> //List<AreaElement.AreaElement>
    {
        public T Get<T>(string elementName)
            where T : Element.Element // AreaElement.AreaElement
        {
            var item = this.SingleOrDefault(x => string.Compare(x.Name, elementName, StringComparison.InvariantCulture) == 0 && x.GetType() == typeof (T));
            return item as T;
        }
    }
}