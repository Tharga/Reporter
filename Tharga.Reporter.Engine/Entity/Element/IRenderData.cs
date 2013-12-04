using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public interface IRenderData
    {
        XRect ParentBounds { get; }
        XRect ElementBounds { get; set; }
        bool IncludeBackground { get; }
        XGraphics Gfx { get; }
        Section Section { get; }
        DocumentData DocumentData { get; }
        PageNumberInfo PageNumberInfo { get; }
        bool Debug { get; }
    }
}