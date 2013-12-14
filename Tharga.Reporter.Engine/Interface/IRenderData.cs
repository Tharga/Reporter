using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;

namespace Tharga.Reporter.Engine.Interface
{
    internal interface IDebugData
    {
        XPen Pen { get; }
        XBrush Brush { get; }
        XFont Font { get; }
    }

    internal interface IRenderData
    {
        XRect ParentBounds { get; }
        XRect ElementBounds { get; set; }
        bool IncludeBackground { get; }
        IGraphics Graphics { get; }
        Section Section { get; }
        DocumentData DocumentData { get; }
        PageNumberInfo PageNumberInfo { get; }
        //bool Debug { get; }
        IDebugData DebugData { get; }
    }
}