using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class MultiPageElement : Element
    {
        internal abstract bool Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section);
        internal abstract int PreRender(IRenderData renderData);
        internal abstract void Render(IRenderData renderData, int page);
    }
}