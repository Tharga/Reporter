using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class MultiPageElement : Element
    {
        protected internal abstract void ClearRenderPointer();
        protected internal abstract bool Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section);
        protected internal abstract bool Render();
    }
}