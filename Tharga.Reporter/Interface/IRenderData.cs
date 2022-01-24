using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Util;

namespace Tharga.Reporter.Interface;

internal interface IRenderData
{
    XRect ParentBounds { get; }
    XRect ElementBounds { get; set; }
    bool IncludeBackground { get; }
    IGraphics Graphics { get; }
    Section Section { get; }
    IDocumentData DocumentData { get; }
    PageNumberInfo PageNumberInfo { get; }
    IDebugData DebugData { get; }
}