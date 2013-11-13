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
    public class Table : MultiPageElement
    {
        private readonly Color _defaultBorderColor = Color.Black;

        public enum Alignment { Left, Right };
        public enum WidthMode { Specific, Auto, Spring }

        private Color? _borderColor;
        private Color? _backgroundColor;
        private Font _headerFont;
        private string _headerFontClass;
        private Font _lineFont;
        private string _lineFontClass;
        private readonly Dictionary<string, TableColumn> _columns = new Dictionary<string, TableColumn>();

        public Color BorderColor { get { return _borderColor ?? _defaultBorderColor; } set { _borderColor = value; } }
        public Color? BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        internal Font HeaderFont
        {
            get
            {
                if (!string.IsNullOrEmpty(_headerFontClass)) 
                    throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFontClass has already been set.");
                return _headerFont ?? (_headerFont = new Font());
            }
        }
        public string HeaderFontClass
        {
            get { return _headerFontClass; }
            set
            {
                if (_headerFont != null) throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFont has already been set.");
                _headerFontClass = value;
            }
        }
        internal Font LineFont
        {
            get
            {
                if (!string.IsNullOrEmpty(_lineFontClass))
                    throw new InvalidOperationException("Cannot set both LineFont and LineFontClass. LineFontClass has already been set.");
                return _lineFont ?? (_lineFont = new Font());
            }
        }
        public string LineFontClass
        {
            get { return _lineFontClass; }
            set
            {
                if (_lineFont != null)
                    throw new InvalidOperationException("Cannot set both LineFont and LineFontClass. LineFont has already been set.");
                _lineFontClass = value;
            }
        }
        internal Dictionary<string, TableColumn> Columns { get { return _columns; } }

        private int _rowPointer; //Rememgers the row possition between pages



        //public string Name { get { return _name ?? string.Empty; } set { _name = value; } }

        //public Table() //string name)
        //{
        //    //Name = name;
        //}

        protected internal override void ClearRenderPointer()
        {
            _rowPointer = 0;
        }

        protected internal override bool Render(PdfPage page, XRect parentBounds, DocumentData documentData,
            out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo)
        {
            elementBounds = GetBounds(parentBounds);

            if (!includeBackground && IsBackground) return false;

            using (var gfx = XGraphics.FromPdfPage(page))
            {
                var debugPen = new XPen(XColor.FromArgb(Color.Yellow), 0.1);
                var headerFont = new XFont(GetHeaderName(), GetHeaderSize(), XFontStyle.Regular);
                var headerBrush = new XSolidBrush(XColor.FromArgb(GetHeaderColor()));
                var lineFont = new XFont(GetLineName(), GetLineSize(), XFontStyle.Regular);
                var lineBrush = new XSolidBrush(XColor.FromArgb(GetLineColor()));

                var headerSize = gfx.MeasureString(_columns.First().Value.DisplayName, headerFont, XStringFormats.TopLeft);
                var lineSize = gfx.MeasureString(_columns.First().Value.DisplayName, lineFont, XStringFormats.TopLeft);

                RenderBorder(elementBounds, gfx, headerSize);

                var dataTable = documentData.GetDataTable(Name);
                const double padding = 2;

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
                            column.Value.Width = UnitValue.Parse((stringSize.Width + (padding*2)).ToString(CultureInfo.InvariantCulture) + "px");

                        var parsedHideValue = GetValue(column.Value.HideValue, row);
                        if (parsedHideValue != cellData)
                            column.Value.Hide = false;
                    }

                    if (column.Value.Hide)
                        //column.Value.Width.Value = 0;
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
                foreach (var column in _columns.Values.Where(x => !x.Hide).ToList())
                {
                    var alignmentJusttification = 0D;
                    if (column.Align == Alignment.Right)
                    {
                        var stringSize = gfx.MeasureString(column.DisplayName, headerFont, XStringFormats.TopLeft);
                        alignmentJusttification = column.Width.Value.GetXUnitValue(elementBounds.Width) - stringSize.Width - padding;
                    }
                    else
                        alignmentJusttification += padding;
                    gfx.DrawString(column.DisplayName, headerFont, headerBrush, elementBounds.Left + left + alignmentJusttification, elementBounds.Top, XStringFormats.TopLeft);
                    left += column.Width.Value.GetXUnitValue(elementBounds.Width);

                    if (debug)
                        gfx.DrawLine(debugPen, elementBounds.Left + left, elementBounds.Top, elementBounds.Left + left, elementBounds.Bottom);
                }


                var top = headerSize.Height;
                //foreach (var row in dataTable.Rows)
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
                            alignmentJusttification = column.Value.Width.Value.GetXUnitValue(elementBounds.Width) - stringSize.Width - padding;
                        }
                        else
                            alignmentJusttification += padding;

                        var parsedHideValue = GetValue(column.Value.HideValue, row);
                        if (parsedHideValue == cellData)
                            cellData = "";

                        gfx.DrawString(cellData, lineFont, lineBrush, elementBounds.Left + left + alignmentJusttification, elementBounds.Top + top, XStringFormats.TopLeft);
                        left += column.Value.Width.Value.GetXUnitValue(elementBounds.Width);
                    }
                    top += lineSize.Height;

                    if (top > elementBounds.Height - lineSize.Height)
                    {
                        _rowPointer = i+1;
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
            if (BorderColor != null)
            {
                var borderPen = new XPen(BorderColor);

                //Rectangle around the entire table
                gfx.DrawRectangle(borderPen, elementBounds);

                //Rectangle around the title
                if (BackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(BackgroundColor.Value));
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

        private string GetHeaderName()
        {
            return HeaderFont.GetRenderName(HeaderFontClass);
        }

        private double GetHeaderSize()
        {
            return HeaderFont.GetRenderSize(HeaderFontClass);
        }

        private Color GetHeaderColor()
        {
            return HeaderFont.GetRenderColor(HeaderFontClass);
        }

        private string GetLineName()
        {
            return LineFont.GetRenderName(LineFontClass);
        }

        private double GetLineSize()
        {
            return LineFont.GetRenderSize(LineFontClass);
        }

        private Color GetLineColor()
        {
            return LineFont.GetRenderColor(LineFontClass);
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

            if (_backgroundColor != null)
                xme.SetAttribute("BackgroundColor", string.Format("{0}{1}{2}", _backgroundColor.Value.R.ToString("X2"), _backgroundColor.Value.G.ToString("X2"), _backgroundColor.Value.B.ToString("X2")));

            if (_borderColor != null)
                xme.SetAttribute("BorderColor", string.Format("{0}{1}{2}", _borderColor.Value.R.ToString("X2"), _borderColor.Value.G.ToString("X2"), _borderColor.Value.B.ToString("X2")));

            if (_headerFontClass != null)
                xme.SetAttribute("HeaderFontClass", _headerFontClass);

            if (_lineFontClass != null)
                xme.SetAttribute("LineFontClass", _lineFontClass);

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

            var xmlBackgroundColor = xme.Attributes["BackgroundColor"];
            if (xmlBackgroundColor != null)
                table.BackgroundColor = xmlBackgroundColor.Value.ToColor();

            var xmlBorderColor = xme.Attributes["BorderColor"];
            if (xmlBorderColor != null)
                table.BorderColor = xmlBorderColor.Value.ToColor();

            var xmlHeaderFontClass = xme.Attributes["HeaderFontClass"];
            if (xmlHeaderFontClass != null)
                table.HeaderFontClass = xmlHeaderFontClass.Value;

            var xmlLineFontClass = xme.Attributes["LineFontClass"];
            if (xmlLineFontClass != null)
                table.LineFontClass = xmlLineFontClass.Value;

            var xmlTemplate = xme.FirstChild;

            if (xmlTemplate.Name != "Columns") throw new InvalidOperationException(string.Format("Columns level cannot be parsed as element of type {0}.", xmlTemplate.Name));
            foreach (XmlElement xmlColumn in xmlTemplate.ChildNodes)
            {
                var name = xmlColumn.Attributes["Key"].Value;

                var col = TableColumn.Load(xmlColumn);

                table.AddColumn(name, col.DisplayName, col.Width, col.WidthMode, col.Align, col.HideValue);
            }

            return table;
        }
    }
}