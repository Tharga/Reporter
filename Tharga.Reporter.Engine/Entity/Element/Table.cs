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
        public enum Alignment { Left, Right };
        public enum WidthMode { Specific, Auto, Spring }

        private readonly Dictionary<string, TableColumn> _columns = new Dictionary<string, TableColumn>();

        public Color? BorderColor { get; set; }
        public Color? BackgroundColor { get; set; }

        private Font _headerFont;
        private string _headerFontClass;
        private Font _lineFont;
        private string _lineFontClass;
        
        private int _rowPointer; //Rememgers the row possition between pages

        public string HeaderFontClass
        {
            get { return _headerFontClass; }
            set
            {
                if (_headerFont != null) throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFont has already been set.");
                _headerFontClass = value;
            }
        }

        public Font HeaderFont
        {
            get
            {
                if (!string.IsNullOrEmpty(_headerFontClass)) throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFontClass has already been set.");
                return _headerFont ?? (_headerFont = new Font());
            }
        }

        public string LineFontClass
        {
            get { return _lineFontClass; }
            set
            {
                if (_lineFont != null) throw new InvalidOperationException("Cannot set both LineFont and LineFontClass. LineFont has already been set.");
                _lineFontClass = value;
            }
        }

        public Font LineFont
        {
            get
            {
                if (!string.IsNullOrEmpty(_lineFontClass)) throw new InvalidOperationException("Cannot set both LineFont and LineFontClass. LineFontClass has already been set.");
                return _lineFont ?? (_lineFont = new Font());
            }
        }

        public string Name { get; private set; }

        public Table(string name)
        {
            Name = name;
        }

        //[Obsolete("Use simple constructor and property setters instead.")]
        //public Table(string name, UnitValue left = null, UnitValue top = null, UnitValue right = null, UnitValue bottom = null)
        //    : base(new UnitRectangle(left, top, right, bottom))
        //{
        //    Name = name;
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
                var borderPen = new XPen(BorderColor.Value);

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

        internal string GetHeaderName()
        {
            return HeaderFont.GetRenderName(HeaderFontClass);
        }

        internal double GetHeaderSize()
        {
            return HeaderFont.GetRenderSize(HeaderFontClass);
        }

        internal Color GetHeaderColor()
        {
            return HeaderFont.GetRenderColor(HeaderFontClass);
        }

        internal string GetLineName()
        {
            return LineFont.GetRenderName(LineFontClass);
        }

        internal double GetLineSize()
        {
            return LineFont.GetRenderSize(LineFontClass);
        }

        internal Color GetLineColor()
        {
            return LineFont.GetRenderColor(LineFontClass);
        }

        //protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayFormat">A static text, or a text with variables; "text {Data} text"</param>
        /// <param name="displayName"></param>
        /// <param name="width"></param>
        /// <param name="widthMode"></param>
        /// <param name="alignment"></param>
        /// <param name="hideValue"></param>
        public void AddColumn(string displayFormat, string displayName,  UnitValue? width = null,
            WidthMode widthMode = WidthMode.Auto, Alignment alignment = Alignment.Left,
            string hideValue = null)
        {
            _columns.Add(displayFormat, new TableColumn(displayName, width, widthMode, alignment, hideValue));
        }
    }
}