using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element.Base;

public abstract class MultiPageAreaElement : AreaElement
{
    internal abstract int PreRender(IRenderData renderData);
    internal abstract void Render(IRenderData renderData, int page);
}