using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity.Element.Base;
using Tharga.Reporter.Entity.Element.Extensions;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Extensions;
using Tharga.Reporter.Interface;

namespace Tharga.Reporter.Entity.Element;

public class Table : MultiPageAreaElement
{
    public enum Alignment
    {
        Left,
        Right
    }

    public enum WidthMode
    {
        Specific,
        Auto,
        Spring
    }

    private const double BorderPenWidth = 0.1;

    private readonly UnitValue _defaultColumnPadding = "2px";
    private readonly Font _defaultContentFont = new();
    private readonly Font _defaultHeaderFont = new() { Size = 18 };
    private readonly UnitValue _defaultRowPadding = "2px";
    private UnitValue? _columnPadding;
    private Color? _contentBackgroundColor;
    private Color? _contentBorderColor;

    private Font _contentFont;
    private string _contentFontClass;
    private Color? _groupBackgroundColor;
    private Color? _groupBorderColor;

    private Font _groupFont;
    private UnitValue? _groupSpacing;
    private Color? _headerBackgroundColor;
    private Color? _headerBorderColor;

    private Font _headerFont;
    private string _headerFontClass;

    private UnitValue? _rowPadding;

    internal Dictionary<string, TableColumn> Columns { get; } = new();

    public Font HeaderFont
    {
        get => _headerFont ?? _defaultHeaderFont;
        set
        {
            if (!string.IsNullOrEmpty(_headerFontClass))
            {
                throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFontClass has already been set.");
            }

            _headerFont = value;
        }
    }

    // Internal until font classes are applied when rendering. See #30.
    internal string HeaderFontClass
    {
        get => _headerFontClass ?? string.Empty;
        set
        {
            if (_headerFont != null)
            {
                throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFont has already been set.");
            }

            _headerFontClass = value;
        }
    }

    public Color? HeaderBorderColor
    {
        get => _headerBorderColor;
        set => _headerBorderColor = value;
    }

    public Color? HeaderBackgroundColor
    {
        get => _headerBackgroundColor;
        set => _headerBackgroundColor = value;
    }

    public Font ContentFont
    {
        get => _contentFont ?? _defaultContentFont;
        set
        {
            if (!string.IsNullOrEmpty(_contentFontClass))
            {
                throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFontClass has already been set.");
            }

            _contentFont = value;
        }
    }

    public UnitValue GroupSpacing
    {
        get => _groupSpacing ?? new UnitValue();
        set => _groupSpacing = value;
    }

    public Font GroupFont
    {
        get => _groupFont ?? _contentFont ?? _defaultContentFont;
        set => _groupFont = value;
    }

    // Internal until font classes are applied when rendering. See #30.
    internal string ContentFontClass
    {
        get => _contentFontClass ?? string.Empty;
        set
        {
            if (_contentFont != null)
            {
                throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFont has already been set.");
            }

            _contentFontClass = value;
        }
    }

    public Color? ContentBorderColor
    {
        get => _contentBorderColor;
        set => _contentBorderColor = value;
    }

    public Color? ContentBackgroundColor
    {
        get => _contentBackgroundColor;
        set => _contentBackgroundColor = value;
    }

    public Color? GroupBorderColor
    {
        get => _groupBorderColor;
        set => _groupBorderColor = value;
    }

    public Color? GroupBackgroundColor
    {
        get => _groupBackgroundColor;
        set => _groupBackgroundColor = value;
    }

    public SkipLine SkipLine { get; set; }

    public UnitValue RowPadding
    {
        get => _rowPadding ?? _defaultRowPadding;
        set => _rowPadding = value;
    }

    public UnitValue ColumnPadding
    {
        get => _columnPadding ?? _defaultColumnPadding;
        set => _columnPadding = value;
    }

    internal override int PreRender(IRenderData renderData)
    {
        var pageRowSets = new List<PageRowSet>();
        renderData.Context.SetPages(this, pageRowSets);

        renderData.ElementBounds = GetBounds(renderData);

        if (IsBackground && !renderData.IncludeBackground)
        {
            return 0;
        }

        var headerFont = new XFont(_headerFont.GetName(renderData.Section), _headerFont.GetSize(renderData.Section), _headerFont.GetStyle(renderData.Section));
        var lineFont = new XFont(_contentFont.GetName(renderData.Section), _contentFont.GetSize(renderData.Section), _contentFont.GetStyle(renderData.Section));
        var groupFont = new XFont(_groupFont.GetName(renderData.Section), _groupFont.GetSize(renderData.Section), _groupFont.GetStyle(renderData.Section));

        var firstColumn = Columns.FirstOrDefault();
        if (firstColumn.Value == null)
        {
            return 0;
        }

        var headerSize = renderData.Graphics.MeasureString(firstColumn.Value.DisplayName, headerFont, XStringFormats.TopLeft);
        var stdLineSize = renderData.Graphics.MeasureString(firstColumn.Value.DisplayName, lineFont, XStringFormats.TopLeft);

        if (renderData.DocumentData != null)
        {
            var dataTable = renderData.DocumentData.GetDataTable(Name);
            if (dataTable != null)
            {
                var top = headerSize.Height + RowPadding.GetXUnitValue(renderData.ElementBounds.Height);
                var pageIndex = 1;
                var firstLineOnPage = 0;
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    var lineSize = stdLineSize;
                    if (dataTable.Rows[i] is DocumentDataTableData)
                    {
                        if (SkipLine != null && pageIndex % SkipLine.Interval == 0)
                        {
                            top += SkipLine.Height.GetXUnitValue(renderData.ElementBounds.Height);
                        }
                    }
                    else if (dataTable.Rows[i] is DocumentDataTableGroup)
                    {
                        top += GroupSpacing.GetXUnitValue(renderData.ElementBounds.Height);
                        lineSize = renderData.Graphics.MeasureString("X", groupFont, XStringFormats.TopLeft);
                        pageIndex = 0;
                    }

                    top += lineSize.Height;
                    top += RowPadding.GetXUnitValue(renderData.ElementBounds.Height);

                    pageIndex++;

                    if (top > renderData.ElementBounds.Height - lineSize.Height)
                    {
                        pageRowSets.Add(new PageRowSet { FromRow = firstLineOnPage, ToRow = i });
                        firstLineOnPage = i + 1;
                        top = headerSize.Height + RowPadding.GetXUnitValue(renderData.ElementBounds.Height);
                    }
                }

                if (firstLineOnPage != dataTable.Rows.Count)
                {
                    pageRowSets.Add(new PageRowSet { FromRow = firstLineOnPage, ToRow = dataTable.Rows.Count - 1 });
                }
            }
        }

        return pageRowSets.Count;
    }

    internal override void Render(IRenderData renderData, int page)
    {
        if (IsNotVisible(renderData))
        {
            return;
        }

        var pageRowSets = renderData.Context.GetPages<PageRowSet>(this);
        if (pageRowSets == null) throw new InvalidOperationException("PreRendering has not yet been performed.");

        renderData.ElementBounds = GetBounds(renderData);

        if (!renderData.IncludeBackground && IsBackground)
        {
            return;
        }

        var headerFont = new XFont(_headerFont.GetName(renderData.Section), _headerFont.GetSize(renderData.Section), _headerFont.GetStyle(renderData.Section));
        var headerBrush = new XSolidBrush(_headerFont.GetColor(renderData.Section).ToXColor());
        var lineFont = new XFont(_contentFont.GetName(renderData.Section), _contentFont.GetSize(renderData.Section), _contentFont.GetStyle(renderData.Section));
        var lineBrush = new XSolidBrush(_contentFont.GetColor(renderData.Section).ToXColor());
        var groupFont = new XFont(_groupFont.GetName(renderData.Section), _groupFont.GetSize(renderData.Section), _groupFont.GetStyle(renderData.Section));
        var groupBrush = new XSolidBrush(_groupFont.GetColor(renderData.Section).ToXColor());

        var firstColumn = Columns.FirstOrDefault();
        if (firstColumn.Value == null)
        {
            return;
        }

        var headerSize = renderData.Graphics.MeasureString(firstColumn.Value.DisplayName, headerFont, XStringFormats.TopLeft);
        var stdLineSize = renderData.Graphics.MeasureString(firstColumn.Value.DisplayName, lineFont, XStringFormats.TopLeft);

        RenderBorder(renderData.ElementBounds, renderData.Graphics, headerSize);

        if (renderData.DebugData != null)
        {
            renderData.Graphics.DrawString(string.Format("Table: {0}", Name), renderData.DebugData.Font, renderData.DebugData.Brush, renderData.ElementBounds.Center);
        }

        if (renderData.DocumentData != null)
        {
            var dataTable = renderData.DocumentData.GetDataTable(Name);
            if (dataTable != null)
            {
                var columnPadding = ColumnPadding.GetXUnitValue(renderData.ElementBounds.Width);
                var visibleColumns = GetVisibleColumnWidths(renderData, dataTable, headerFont, lineFont, columnPadding);

                //Create header
                double left = 0;
                foreach (var (key, columnWidth) in visibleColumns)
                {
                    var column = Columns[key];
                    var alignmentJusttification = 0D;
                    if (column.Align == Alignment.Right)
                    {
                        var stringSize = renderData.Graphics.MeasureString(column.DisplayName, headerFont, XStringFormats.TopLeft);
                        alignmentJusttification = columnWidth - stringSize.Width - columnPadding;
                    }
                    else
                    {
                        alignmentJusttification += columnPadding;
                    }

                    renderData.Graphics.DrawString(column.DisplayName, headerFont, headerBrush, new XPoint(renderData.ElementBounds.Left + left + alignmentJusttification, renderData.ElementBounds.Top), XStringFormats.TopLeft);
                    left += columnWidth;

                    if (renderData.DebugData != null)
                    {
                        renderData.Graphics.DrawLine(renderData.DebugData.Pen, renderData.ElementBounds.Left + left, renderData.ElementBounds.Top, renderData.ElementBounds.Left + left, renderData.ElementBounds.Bottom);
                    }
                }

                var top = headerSize.Height + RowPadding.GetXUnitValue(renderData.ElementBounds.Height);
                var pageIndex = 1;

                var defaultRowset = true;
                var pageRowSet = new PageRowSet { FromRow = 1 };
                var index = page - renderData.Context.GetPageOffset(renderData.Section);
                if (pageRowSets.Count > index)
                {
                    try
                    {
                        defaultRowset = false;
                        pageRowSet = pageRowSets[index];
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException(string.Format("pageRowSets.Count={0}, index={1}", pageRowSets.Count, index), exception);
                    }
                }

                for (var i = pageRowSet.FromRow; i < pageRowSet.ToRow + 1; i++)
                {
                    DocumentDataTableLine row;
                    try
                    {
                        row = dataTable.Rows[i];
                    }
                    catch (Exception exception)
                    {
                        var msg = string.Format("Looping from {0} to {1}. Currently at {2}, collection has {3} lines.", pageRowSet.FromRow, pageRowSet.ToRow + 1, i, dataTable.Rows.Count);
                        throw new InvalidOperationException(msg + string.Format("dataTable.Rows.Count={0}, i={1}, pageIndex={2}, page={3}, GetPageOffset={4}, index={5}, FromRow={6}, ToRow={7}, pageRowSets.Count={8}, defaultRowset={9}", dataTable.Rows.Count, i, pageIndex, page, renderData.Context.GetPageOffset(renderData.Section), index, pageRowSet.FromRow, pageRowSet.ToRow, pageRowSets.Count, defaultRowset), exception);
                    }

                    left = 0;
                    var lineSize = stdLineSize;
                    if (row is DocumentDataTableData)
                    {
                        var rowData = row as DocumentDataTableData;

                        foreach (var (key, columnWidth) in visibleColumns)
                        {
                            var column = Columns[key];
                            var cellData = GetValue(key, rowData.Columns);

                            var alignmentJusttification = 0D;
                            if (column.Align == Alignment.Right)
                            {
                                var stringSize = renderData.Graphics.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                                alignmentJusttification = columnWidth - stringSize.Width - columnPadding;
                            }
                            else
                            {
                                alignmentJusttification += columnPadding;
                            }

                            var parsedHideValue = GetValue(column.HideValue, rowData.Columns);
                            if (parsedHideValue == cellData)
                            {
                                cellData = "";
                            }

                            renderData.Graphics.DrawString(cellData, lineFont, lineBrush, new XPoint(renderData.ElementBounds.Left + left + alignmentJusttification, renderData.ElementBounds.Top + top), XStringFormats.TopLeft);
                            left += columnWidth;
                        }

                        if (SkipLine != null && pageIndex % SkipLine.Interval == 0)
                        {
                            top += SkipLine.Height.GetXUnitValue(renderData.ElementBounds.Height);
                        }
                    }
                    else if (row is DocumentDataTableGroup)
                    {
                        var group = row as DocumentDataTableGroup;

                        if (pageIndex != 1)
                        {
                            top += GroupSpacing.GetXUnitValue(renderData.ElementBounds.Height);
                        }

                        var groupData = group.Content;
                        var stringSize = renderData.Graphics.MeasureString(groupData, groupFont, XStringFormats.TopLeft);
                        lineSize = stringSize;
                        var topLeft = new XPoint(renderData.ElementBounds.Left + left, renderData.ElementBounds.Top + top);

                        RenderArea(renderData.Graphics, new XRect(topLeft, new XSize(renderData.ElementBounds.Width, stringSize.Height)), GroupBorderColor, GroupBackgroundColor);

                        renderData.Graphics.DrawString(groupData, groupFont, groupBrush, topLeft, XStringFormats.TopLeft);
                        pageIndex = 0;
                    }

                    top += lineSize.Height;
                    top += RowPadding.GetXUnitValue(renderData.ElementBounds.Height);

                    pageIndex++;
                }
            }
        }

        if (renderData.DebugData != null)
        {
            renderData.Graphics.DrawRectangle(renderData.DebugData.Pen, renderData.ElementBounds);
        }
    }

    private IReadOnlyList<(string Key, double Width)> GetVisibleColumnWidths(IRenderData renderData, DocumentDataTable dataTable, XFont headerFont, XFont lineFont, double columnPadding)
    {
        var totalWidth = renderData.ElementBounds.Width;
        var widths = new Dictionary<string, double>();
        var hidden = new HashSet<string>();

        foreach (var (key, column) in Columns.Where(x => x.Value.WidthMode != WidthMode.Spring))
        {
            if (column.Width == null)
            {
                throw new NullReferenceException(string.Format("Column {0} does not have a width.", column.DisplayName));
            }

            var width = column.Width.Value.GetXUnitValue(totalWidth);
            var headerSize = renderData.Graphics.MeasureString(column.DisplayName, headerFont, XStringFormats.TopLeft);
            if (headerSize.Width > width)
            {
                width = headerSize.Width;
            }

            var hide = column.HideValue != null;
            foreach (var rowData in dataTable.Rows.OfType<DocumentDataTableData>())
            {
                var cellData = GetValue(key, rowData.Columns);
                var cellSize = renderData.Graphics.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                if (cellSize.Width > width)
                {
                    width = cellSize.Width + columnPadding * 2;
                }

                if (GetValue(column.HideValue, rowData.Columns) != cellData)
                {
                    hide = false;
                }
            }

            if (hide)
            {
                hidden.Add(key);
            }

            widths[key] = hide ? 0 : width;
        }

        var springCount = Columns.Count(x => x.Value.WidthMode == WidthMode.Spring);
        var springWidth = springCount == 0 ? 0 : (totalWidth - widths.Values.Sum()) / springCount;

        return Columns
            .Where(x => !hidden.Contains(x.Key))
            .Select(x => (x.Key, x.Value.WidthMode == WidthMode.Spring ? springWidth : widths[x.Key]))
            .ToArray();
    }

    private void RenderBorder(XRect elementBounds, IGraphics gfx, XSize headerSize)
    {
        RenderArea(gfx, new XRect(elementBounds.Left, elementBounds.Top + headerSize.Height, elementBounds.Width, elementBounds.Height - headerSize.Height), ContentBorderColor, ContentBackgroundColor);
        RenderArea(gfx, new XRect(elementBounds.Left, elementBounds.Top, elementBounds.Width, headerSize.Height), HeaderBorderColor, HeaderBackgroundColor);
    }

    private static void RenderArea(IGraphics gfx, XRect area, Color? borderColor, Color? backgroundColor)
    {
        var pen = borderColor == null ? null : new XPen(borderColor.Value.ToXColor(), BorderPenWidth);
        var brush = backgroundColor == null ? null : new XSolidBrush(backgroundColor.Value.ToXColor());

        if (pen != null && brush != null)
        {
            gfx.DrawRectangle(pen, brush, area);
        }
        else if (brush != null)
        {
            gfx.DrawRectangle(brush, area);
        }
        else if (pen != null)
        {
            gfx.DrawRectangle(pen, area);
        }
    }

    private string GetValue(string input, Dictionary<string, string> row)
    {
        var result = input.ParseValue(row);
        return result;
    }

    public void AddColumn(string displayFormat, string displayName, UnitValue? width = null,
        WidthMode widthMode = WidthMode.Auto, Alignment alignment = Alignment.Left,
        string hideValue = null)
    {
        Columns.Add(displayFormat, new TableColumn(displayName, width, widthMode, alignment, hideValue));
    }

    internal override XmlElement ToXme()
    {
        var xme = base.ToXme();

        if (_contentBackgroundColor != null)
        {
            xme.SetAttribute("ContentBackgroundColor", string.Format("{0}{1}{2}", _contentBackgroundColor.Value.R.ToString("X2"), _contentBackgroundColor.Value.G.ToString("X2"), _contentBackgroundColor.Value.B.ToString("X2")));
        }

        if (_contentBorderColor != null)
        {
            xme.SetAttribute("ContentBorderColor", string.Format("{0}{1}{2}", _contentBorderColor.Value.R.ToString("X2"), _contentBorderColor.Value.G.ToString("X2"), _contentBorderColor.Value.B.ToString("X2")));
        }

        if (_groupBackgroundColor != null)
        {
            xme.SetAttribute("GroupBackgroundColor", string.Format("{0}{1}{2}", _groupBackgroundColor.Value.R.ToString("X2"), _groupBackgroundColor.Value.G.ToString("X2"), _groupBackgroundColor.Value.B.ToString("X2")));
        }

        if (_groupBorderColor != null)
        {
            xme.SetAttribute("GroupBorderColor", string.Format("{0}{1}{2}", _groupBorderColor.Value.R.ToString("X2"), _groupBorderColor.Value.G.ToString("X2"), _groupBorderColor.Value.B.ToString("X2")));
        }

        if (_headerBackgroundColor != null)
        {
            xme.SetAttribute("HeaderBackgroundColor", string.Format("{0}{1}{2}", _headerBackgroundColor.Value.R.ToString("X2"), _headerBackgroundColor.Value.G.ToString("X2"), _headerBackgroundColor.Value.B.ToString("X2")));
        }

        if (_headerBorderColor != null)
        {
            xme.SetAttribute("HeaderBorderColor", string.Format("{0}{1}{2}", _headerBorderColor.Value.R.ToString("X2"), _headerBorderColor.Value.G.ToString("X2"), _headerBorderColor.Value.B.ToString("X2")));
        }

        if (_headerFont != null)
        {
            var fontXme = _headerFont.ToXme("HeaderFont");
            var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
            xme.AppendChild(importedFont);
        }

        if (_headerFontClass != null)
        {
            xme.SetAttribute("HeaderFontClass", _headerFontClass);
        }

        if (_contentFont != null)
        {
            var fontXme = _contentFont.ToXme("ContentFont");
            var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
            xme.AppendChild(importedFont);
        }

        if (_contentFontClass != null)
        {
            xme.SetAttribute("ContentFontClass", _contentFontClass);
        }

        if (_groupFont != null)
        {
            var fontXme = _groupFont.ToXme("GroupFont");
            var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
            xme.AppendChild(importedFont);
        }

        if (SkipLine != null)
        {
            var xmeSkipLine = SkipLine.ToXme();
            var importeSkipLine = xme.OwnerDocument.ImportNode(xmeSkipLine, true);
            xme.AppendChild(importeSkipLine);
        }

        if (_rowPadding != null)
        {
            xme.SetAttribute("RowPadding", _rowPadding.Value.ToString());
        }

        if (_columnPadding != null)
        {
            xme.SetAttribute("ColumnPadding", _columnPadding.Value.ToString());
        }

        if (_groupSpacing != null)
        {
            xme.SetAttribute("GroupSpacing", _groupSpacing.Value.ToString());
        }

        var columns = xme.OwnerDocument.CreateElement("Columns");
        xme.AppendChild(columns);
        foreach (var column in Columns)
        {
            var xmeColumn = column.Value.ToXme();

            xmeColumn.SetAttribute("Key", column.Key);

            var col = columns.OwnerDocument.ImportNode(xmeColumn, true);
            columns.AppendChild(col);
        }

        return xme;
    }

    internal static Table Load(XmlElement xme)
    {
        var table = new Table();

        table.AppendData(xme);

        var xmlBackgroundColor = xme.Attributes["ContentBackgroundColor"];
        if (xmlBackgroundColor != null)
        {
            table.ContentBackgroundColor = xmlBackgroundColor.Value.ToColor();
        }

        var xmlBorderColor = xme.Attributes["ContentBorderColor"];
        if (xmlBorderColor != null)
        {
            table.ContentBorderColor = xmlBorderColor.Value.ToColor();
        }

        var xmlGroupBackgroundColor = xme.Attributes["GroupBackgroundColor"];
        if (xmlGroupBackgroundColor != null)
        {
            table.GroupBackgroundColor = xmlGroupBackgroundColor.Value.ToColor();
        }

        var xmlGroupBorderColor = xme.Attributes["GroupBorderColor"];
        if (xmlGroupBorderColor != null)
        {
            table.GroupBorderColor = xmlGroupBorderColor.Value.ToColor();
        }

        var xmlHeaderBackgroundColor = xme.Attributes["HeaderBackgroundColor"];
        if (xmlHeaderBackgroundColor != null)
        {
            table.HeaderBackgroundColor = xmlHeaderBackgroundColor.Value.ToColor();
        }

        var xmlHeaderBorderColor = xme.Attributes["HeaderBorderColor"];
        if (xmlHeaderBorderColor != null)
        {
            table.HeaderBorderColor = xmlHeaderBorderColor.Value.ToColor();
        }

        var xmlHeaderFontClass = xme.Attributes["HeaderFontClass"];
        if (xmlHeaderFontClass != null)
        {
            table.HeaderFontClass = xmlHeaderFontClass.Value;
        }

        var xmlLineFontClass = xme.Attributes["ContentFontClass"];
        if (xmlLineFontClass != null)
        {
            table.ContentFontClass = xmlLineFontClass.Value;
        }

        var xmlRowPadding = xme.Attributes["RowPadding"];
        if (xmlRowPadding != null)
        {
            table.RowPadding = xmlRowPadding.Value;
        }

        var xmlColumnPadding = xme.Attributes["ColumnPadding"];
        if (xmlColumnPadding != null)
        {
            table.ColumnPadding = xmlColumnPadding.Value;
        }

        var xmlGroupSpacing = xme.Attributes["GroupSpacing"];
        if (xmlGroupSpacing != null)
        {
            table.GroupSpacing = xmlGroupSpacing.Value;
        }

        foreach (XmlElement child in xme)
        {
            switch (child.Name)
            {
                case "HeaderFont":
                    table.HeaderFont = Font.Load(child);
                    break;
                case "ContentFont":
                    table.ContentFont = Font.Load(child);
                    break;
                case "GroupFont":
                    table.GroupFont = Font.Load(child);
                    break;
                case "SkipLine":
                    table.SkipLine = SkipLine.Load(child);
                    break;
                case "Columns":
                    foreach (XmlElement xmlColumn in child.ChildNodes)
                    {
                        var name = xmlColumn.Attributes["Key"].Value;
                        var col = TableColumn.Load(xmlColumn);
                        table.AddColumn(name, col.DisplayName, col.Width, col.WidthMode, col.Align, col.HideValue);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
            }
        }

        return table;
    }
}