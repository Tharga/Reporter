using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IRenderData
    {
        XRect ParentBounds { get; }
        XRect ElementBounds { get; set; }
        bool IncludeBackground { get; }
        IGraphics Graphics { get; }
        Section Section { get; }
        DocumentData DocumentData { get; }
        PageNumberInfo PageNumberInfo { get; }
        bool Debug { get; }
    }
}