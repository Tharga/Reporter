using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Table : MultiPageAreaElement
    {
        private readonly Font _defaultContentFont = new Font();
        private readonly Font _defaultHeaderFont = new Font {Size = 18};
        private readonly UnitValue _defaultRowPadding = "2px";
        private readonly UnitValue _defaultColumnPadding = "2px";

        public enum Alignment { Left, Right };
        public enum WidthMode { Specific, Auto, Spring }

        private readonly Dictionary<string, TableColumn> _columns = new Dictionary<string, TableColumn>();

        private Font _headerFont;
        private string _headerFontClass;
        private Color? _headerBorderColor;
        private Color? _headerBackgroundColor;

        private Font _contentFont;
        private string _contentFontClass;
        private Color? _contentBorderColor;
        private Color? _contentBackgroundColor;

        private SkipLine _skipLine;

        private int _rowPointer;
        private UnitValue? _rowPadding;
        private UnitValue? _columnPadding;

        internal Dictionary<string, TableColumn> Columns { get { return _columns; } }

        public Font HeaderFont
        {
            get
            {
                return _headerFont ?? _defaultHeaderFont;
            }
            set
            {
                if (!string.IsNullOrEmpty(_headerFontClass))
                    throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFontClass has already been set.");
                _headerFont = value;
            }
        }
        internal string HeaderFontClass //TODO: Hidden because it is not yet fully implemented
        {
            get { return _headerFontClass ?? string.Empty; }
            set
            {
                if (_headerFont != null) 
                    throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFont has already been set.");
                _headerFontClass = value;
            }
        }
        public Color? HeaderBorderColor { get { return _headerBorderColor; } set { _headerBorderColor = value; } }
        public Color? HeaderBackgroundColor { get { return _headerBackgroundColor; } set { _headerBackgroundColor = value; } }
        public Font ContentFont
        {
            get
            {
                return _contentFont ?? _defaultContentFont;
            }
            set
            {
                if (!string.IsNullOrEmpty(_contentFontClass))
                    throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFontClass has already been set.");
                _contentFont = value;
            }
        }
        internal string ContentFontClass  //TODO: Hidden because it is not yet fully implemented
        {
            get { return _contentFontClass ?? string.Empty; }
            set
            {
                if (_contentFont != null)
                    throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFont has already been set.");
                _contentFontClass = value;
            }
        }
        public Color? ContentBorderColor { get { return _contentBorderColor; } set { _contentBorderColor = value; } }
        public Color? ContentBackgroundColor { get { return _contentBackgroundColor; } set { _contentBackgroundColor = value; } }
        public SkipLine SkipLine { get { return _skipLine; } set { _skipLine = value; } }
        public UnitValue RowPadding { get { return _rowPadding ?? _defaultRowPadding; } set { _rowPadding = value; } }
        public UnitValue ColumnPadding { get { return _columnPadding ?? _defaultColumnPadding; } set { _columnPadding = value; } }

        protected internal override void ClearRenderPointer()
        {
            _rowPointer = 0;
        }

        protected internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo, Section section)
        {
            elementBounds = GetBounds(parentBounds);

            if (!includeBackground && IsBackground) return false;

            using (var gfx = XGraphics.FromPdfPage(page))
            {
                var debugPen = new XPen(XColor.FromArgb(Color.Yellow), 0.1);
                var headerFont = new XFont(_headerFont.GetName(section), _headerFont.GetSize(section), _headerFont.GetStyle(section));
                var headerBrush = new XSolidBrush(XColor.FromArgb(_headerFont.GetColor(section)));
                var lineFont = new XFont(_contentFont.GetName(section), _contentFont.GetSize(section), _contentFont.GetStyle(section));
                var lineBrush = new XSolidBrush(XColor.FromArgb(_contentFont.GetColor(section)));

                var headerSize = gfx.MeasureString(_columns.First().Value.DisplayName, headerFont, XStringFormats.TopLeft);
                var lineSize = gfx.MeasureString(_columns.First().Value.DisplayName, lineFont, XStringFormats.TopLeft);

                RenderBorder(elementBounds, gfx, headerSize);

                var dataTable = documentData.GetDataTable(Name);
                var columnPadding = ColumnPadding.GetXUnitValue(elementBounds.Width);

                var springCount = _columns.Count(x => x.Value.WidthMode == WidthMode.Spring);

                //Calculate column width
                foreach (var column in _columns.Where(x => x.Value.WidthMode != WidthMode.Spring).ToList())
                {
                    var stringSize = gfx.MeasureString(column.Value.DisplayName, headerFont, XStringFormats.TopLeft);

                    if (stringSize.Width > column.Value.Width.Value.GetXUnitValue(elementBounds.Width))
                        column.Value.Width = UnitValue.Parse(stringSize.Width.ToString(CultureInfo.InvariantCulture));

                    if (column.Value.HideValue != null)
                        column.Value.Hide = true;

                    foreach (var row in dataTable.Rows)
                    {
                        var cellData = GetValue(column.Key, row);
                        stringSize = gfx.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                        if (stringSize.Width > column.Value.Width.Value.GetXUnitValue(elementBounds.Width))
                            column.Value.Width = UnitValue.Parse((stringSize.Width + (columnPadding*2)).ToString(CultureInfo.InvariantCulture) + "px");

                        var parsedHideValue = GetValue(column.Value.HideValue, row);
                        if (parsedHideValue != cellData)
                            column.Value.Hide = false;
                    }

                    if (column.Value.Hide)
                        column.Value.Width = new UnitValue();
                }

                var totalWidth = elementBounds.Width;
                var nonSpringWidth = _columns.Where(x => x.Value.WidthMode != WidthMode.Spring).Sum(x => x.Value.Width.Value.GetXUnitValue(totalWidth));

                if (springCount > 0)
                {
                    foreach (var column in _columns.Where(x => x.Value.WidthMode == WidthMode.Spring && !x.Value.Hide).ToList())
                    {
                        column.Value.Width = UnitValue.Parse(((elementBounds.Width - nonSpringWidth)/springCount).ToString(CultureInfo.InvariantCulture));
                    }
                }

                //Create header
                double left = 0;
                var tableColumns = _columns.Values.Where(x => !x.Hide).ToList();
                foreach (var column in tableColumns)
                {
                    var alignmentJusttification = 0D;
                    if (column.Align == Alignment.Right)
                    {
                        var stringSize = gfx.MeasureString(column.DisplayName, headerFont, XStringFormats.TopLeft);
                        alignmentJusttification = column.Width.Value.GetXUnitValue(elementBounds.Width) - stringSize.Width - columnPadding;
                    }
                    else
                        alignmentJusttification += columnPadding;
                    gfx.DrawString(column.DisplayName, headerFont, headerBrush, elementBounds.Left + left + alignmentJusttification, elementBounds.Top, XStringFormats.TopLeft);
                    left += column.Width.Value.GetXUnitValue(elementBounds.Width);

                    if (debug)
                        gfx.DrawLine(debugPen, elementBounds.Left + left, elementBounds.Top, elementBounds.Left + left, elementBounds.Bottom);
                }

                var top = headerSize.Height + RowPadding.GetXUnitValue(elementBounds.Height);
                var pageIndex = 1;
                for (var i = _rowPointer; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];

                    left = 0;
                    foreach (var column in _columns.Where(x => !x.Value.Hide).ToList())
                    {
                        var cellData = GetValue(column.Key, row);

                        var alignmentJusttification = 0D;
                        if (column.Value.Align == Alignment.Right)
                        {
                            var stringSize = gfx.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                            alignmentJusttification = column.Value.Width.Value.GetXUnitValue(elementBounds.Width) - stringSize.Width - columnPadding;
                        }
                        else
                            alignmentJusttification += columnPadding;

                        var parsedHideValue = GetValue(column.Value.HideValue, row);
                        if (parsedHideValue == cellData)
                            cellData = "";

                        gfx.DrawString(cellData, lineFont, lineBrush, elementBounds.Left + left + alignmentJusttification, elementBounds.Top + top, XStringFormats.TopLeft);
                        left += column.Value.Width.Value.GetXUnitValue(elementBounds.Width);
                    }
                    top += lineSize.Height;
                    top += RowPadding.GetXUnitValue(elementBounds.Height);

                    if (_skipLine != null && pageIndex%SkipLine.Interval == 0)
                        top += SkipLine.Height.GetXUnitValue(elementBounds.Height);

                    pageIndex++;

                    if (top > elementBounds.Height - lineSize.Height)
                    {
                        _rowPointer = i + 1;
                        return true;
                    }
                }

                if (debug)
                    gfx.DrawRectangle(debugPen, elementBounds);
            }
            return false;
        }

        private void RenderBorder(XRect elementBounds, XGraphics gfx, XSize headerSize)
        {
            if (ContentBorderColor != null || ContentBackgroundColor != null)
            {
                var borderPen = new XPen(ContentBorderColor ?? ContentBackgroundColor.Value, 0.1); //TODO: Se the thickness of the boarder

                if (ContentBackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(ContentBackgroundColor.Value));
                    gfx.DrawRectangle(borderPen, brush, elementBounds.Left, elementBounds.Top + headerSize.Height, elementBounds.Width, elementBounds.Height - headerSize.Height);
                }
                else
                    gfx.DrawRectangle(borderPen, elementBounds.Left, elementBounds.Top + headerSize.Height, elementBounds.Width, elementBounds.Height - headerSize.Height);
            }

            if (HeaderBorderColor != null || HeaderBackgroundColor != null)
            {
                var borderPen = new XPen(HeaderBorderColor ?? HeaderBackgroundColor.Value, 0.1); //TODO: Se the thickness of the boarder

                if (HeaderBackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(HeaderBackgroundColor.Value));
                    gfx.DrawRectangle(borderPen, brush, elementBounds.Left, elementBounds.Top, elementBounds.Width, headerSize.Height);
                }
                else
                    gfx.DrawRectangle(borderPen, elementBounds.Left, elementBounds.Top, elementBounds.Width, headerSize.Height);                
            }
        }

        private string GetValue(string input, Dictionary<string, string> row)
        {
            var result = input.ParseValue(row);
            return result;
        }

        public void AddColumn(string displayFormat, string displayName,  UnitValue? width = null,
            WidthMode widthMode = WidthMode.Auto, Alignment alignment = Alignment.Left,
            string hideValue = null)
        {
            _columns.Add(displayFormat, new TableColumn(displayName, width, widthMode, alignment, hideValue));
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_contentBackgroundColor != null)
                xme.SetAttribute("ContentBackgroundColor", string.Format("{0}{1}{2}", _contentBackgroundColor.Value.R.ToString("X2"), _contentBackgroundColor.Value.G.ToString("X2"), _contentBackgroundColor.Value.B.ToString("X2")));

            if (_contentBorderColor != null)
                xme.SetAttribute("ContentBorderColor", string.Format("{0}{1}{2}", _contentBorderColor.Value.R.ToString("X2"), _contentBorderColor.Value.G.ToString("X2"), _contentBorderColor.Value.B.ToString("X2")));

            if (_headerBackgroundColor != null)
                xme.SetAttribute("HeaderBackgroundColor", string.Format("{0}{1}{2}", _headerBackgroundColor.Value.R.ToString("X2"), _headerBackgroundColor.Value.G.ToString("X2"), _headerBackgroundColor.Value.B.ToString("X2")));

            if (_headerBorderColor != null)
                xme.SetAttribute("HeaderBorderColor", string.Format("{0}{1}{2}", _headerBorderColor.Value.R.ToString("X2"), _headerBorderColor.Value.G.ToString("X2"), _headerBorderColor.Value.B.ToString("X2")));

            if (_headerFont != null)
            {
                var fontXme = _headerFont.ToXme("HeaderFont");
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_headerFontClass != null)
                xme.SetAttribute("HeaderFontClass", _headerFontClass);

            if (_contentFont != null)
            {
                var fontXme = _contentFont.ToXme("ContentFont");
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_contentFontClass != null)
                xme.SetAttribute("ContentFontClass", _contentFontClass);

            if (_skipLine != null)
            {
                var xmeSkipLine = _skipLine.ToXme();
                var importeSkipLine = xme.OwnerDocument.ImportNode(xmeSkipLine, true);
                xme.AppendChild(importeSkipLine);
            }

            if (_rowPadding != null)
                xme.SetAttribute("RowPadding", _rowPadding.Value.ToString());

            if (_columnPadding != null)
                xme.SetAttribute("ColumnPadding", _columnPadding.Value.ToString());

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
                table.ContentBackgroundColor = xmlBackgroundColor.Value.ToColor();

            var xmlBorderColor = xme.Attributes["ContentBorderColor"];
            if (xmlBorderColor != null)
                table.ContentBorderColor = xmlBorderColor.Value.ToColor();

            var xmlHeaderBackgroundColor = xme.Attributes["HeaderBackgroundColor"];
            if (xmlHeaderBackgroundColor != null)
                table.HeaderBackgroundColor = xmlHeaderBackgroundColor.Value.ToColor();

            var xmlHeaderBorderColor = xme.Attributes["HeaderBorderColor"];
            if (xmlHeaderBorderColor != null)
                table.HeaderBorderColor = xmlHeaderBorderColor.Value.ToColor();

            var xmlHeaderFontClass = xme.Attributes["HeaderFontClass"];
            if (xmlHeaderFontClass != null)
                table.HeaderFontClass = xmlHeaderFontClass.Value;

            var xmlLineFontClass = xme.Attributes["ContentFontClass"];
            if (xmlLineFontClass != null)
                table.ContentFontClass = xmlLineFontClass.Value;

            var xmlRowPadding = xme.Attributes["RowPadding"];
            if (xmlRowPadding != null)
                table.RowPadding = xmlRowPadding.Value;

            var xmlColumnPadding = xme.Attributes["ColumnPadding"];
            if (xmlColumnPadding != null)
                table.ColumnPadding = xmlColumnPadding.Value;

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
}