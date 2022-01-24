using PdfSharp.Drawing;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Util;

internal class RenderData : IRenderData
{
    public RenderData(IGraphics graphics, XRect parentBounds, Section section, IDocumentData documentData, PageNumberInfo pageNumberInfo, IDebugData debugData, bool includeBackground)
    {
        ParentBounds = parentBounds;
        Section = section;
        Graphics = graphics;
        DocumentData = documentData;
        PageNumberInfo = pageNumberInfo;
        DebugData = debugData;
        IncludeBackground = includeBackground;
    }

    public XRect ParentBounds { get; }
    public XRect ElementBounds { get; set; }
    public bool IncludeBackground { get; }

    public IGraphics Graphics { get; }

    public Section Section { get; }
    public IDocumentData DocumentData { get; }

    public PageNumberInfo PageNumberInfo { get; }
    public IDebugData DebugData { get; }
}