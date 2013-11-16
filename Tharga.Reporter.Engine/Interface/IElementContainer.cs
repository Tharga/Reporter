using System.Collections.Generic;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IElementContainer
    {
        //List<AreaElement> ElementList { get; }
        ElementList ElementList { get; }
    }
}
