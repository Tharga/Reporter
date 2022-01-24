using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element.Base;

public abstract class SinglePageAreaElement : AreaElement
{
    internal abstract void Render(IRenderData renderData);
}