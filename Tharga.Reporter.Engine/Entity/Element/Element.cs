using System;
using System.Drawing;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public static class ElementExtensions
    {
        internal static Color ToColor(this string value)
        {
            var rs = value.Substring(0, 2);
            var gs = value.Substring(2, 2);
            var bs = value.Substring(4, 2);

            var r = Int32.Parse(rs, System.Globalization.NumberStyles.HexNumber);
            var g = Int32.Parse(gs, System.Globalization.NumberStyles.HexNumber);
            var b = Int32.Parse(bs, System.Globalization.NumberStyles.HexNumber);

            var color = Color.FromArgb(r, g, b);
            return color;
        }
    }

    public abstract class MultiPageElement : Element
    {
        protected internal abstract void ClearRenderPointer();
        protected internal abstract bool Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo);
    }

    public abstract class SinglePageElement : Element
    {
        protected internal abstract void Render(PdfPage page, XRect parentBounds, DocumentData documentData, out XRect elementBounds, bool includeBackground, bool debug, PageNumberInfo pageNumberInfo);
    }

    public abstract class Element
    {
        private readonly UnitRectangle _relativeAlignment;

        protected bool? _isBackground;
        protected string _name;

        public string Name { get { return _name ?? string.Empty; } set { _name = value; } }
        public bool IsBackground { get { return _isBackground ?? false; } set { _isBackground = value; } }

        protected Element()
        {
            _relativeAlignment = new UnitRectangle();
        }

        protected Element(UnitRectangle relativeAlignment)
        {
            _relativeAlignment = relativeAlignment;
        }

        //protected Element(XmlElement xmlElement)
        //{
        //    _relativeAlignment = new UnitRectangle();

        //    if (xmlElement.Attributes.GetNamedItem("Left") != null)
        //        _relativeAlignment.Left = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Left").Value);

        //    if (xmlElement.Attributes.GetNamedItem("Top") != null)
        //        _relativeAlignment.Top = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Top").Value);

        //    if (xmlElement.Attributes.GetNamedItem("Right") != null)
        //        _relativeAlignment.Right = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Right").Value);

        //    if (xmlElement.Attributes.GetNamedItem("Bottom") != null)
        //        _relativeAlignment.Bottom = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Bottom").Value);

        //    if (xmlElement.Attributes.GetNamedItem("Width") != null)
        //        _relativeAlignment.Width = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Width").Value);

        //    if (xmlElement.Attributes.GetNamedItem("Height") != null)
        //        _relativeAlignment.Height = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Height").Value);
        //}
        
        //protected internal abstract XmlElement AppendXml(ref XmlElement xmePane);

        protected XRect GetBounds(XRect parentBounds)
        {
            if (_relativeAlignment == null) throw new InvalidOperationException("No relative alignment for the Area.");
            var relativeAlignment = _relativeAlignment;

            var left = parentBounds.X + relativeAlignment.GetLeft(parentBounds.Width);
            //var right = parentBounds.Right - relativeAlignment.GetRight(parentBounds.Width);
            var width = _relativeAlignment.GetWidht(parentBounds.Width);

            var top = parentBounds.Y + relativeAlignment.GetTop(parentBounds.Height);
            //var bottom = parentBounds.Bottom - relativeAlignment.GetBottom(parentBounds.Height);
            var height = relativeAlignment.GetHeight(parentBounds.Height);

            if (height < 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Height is adjusted from {0} to 0.", height));
                height = 0;
            }

            return new XRect(left, top, width, height);
        }

        //protected XmlElement AppendXmlBase(ref XmlElement xmePane)
        //{
        //    if (xmePane == null) throw new ArgumentNullException("xmePane");
        //    if (xmePane.OwnerDocument == null) throw new ArgumentNullException("xmePane", "xmePane has no owner document.");

        //    var xmeElement = xmePane.OwnerDocument.CreateElement(GetType().ToShortTypeName());
        //    xmePane.AppendChild(xmeElement);

        //    if (Left != null) xmeElement.SetAttribute("Left", Left.ToString());
        //    if (Top != null) xmeElement.SetAttribute("Top", Top.ToString());
        //    if (Right != null) xmeElement.SetAttribute("Right", Right.ToString());
        //    if (Bottom != null) xmeElement.SetAttribute("Bottom", Bottom.ToString());
        //    if (Width != null) xmeElement.SetAttribute("Width", Width.ToString());
        //    if (Height != null) xmeElement.SetAttribute("Height", Height.ToString());

        //    return xmeElement;
        //}

        public UnitValue? Top { get { return _relativeAlignment.Top; } set { _relativeAlignment.Top = value; } }
        public virtual UnitValue? Bottom { get { return _relativeAlignment.Bottom; } set { _relativeAlignment.Bottom = value; } }
        public virtual UnitValue? Height { get { return _relativeAlignment.Height; } set { _relativeAlignment.Height = value; } }
        public UnitValue? Left { get { return _relativeAlignment.Left; } set { _relativeAlignment.Left = value; } }
        public virtual UnitValue? Right { get { return _relativeAlignment.Right; } set { _relativeAlignment.Right = value; } }
        public virtual UnitValue? Width { get { return _relativeAlignment.Width; } set { _relativeAlignment.Width = value; } }

        internal virtual XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            if (_name != null)
                xme.SetAttribute("Name", _name);

            if (Left != null)
                xme.SetAttribute("Left", Left.Value.ToString());

            if (Top != null)
                xme.SetAttribute("Top", Top.Value.ToString());

            if (Right != null)
                xme.SetAttribute("Right", Right.Value.ToString());

            if (Bottom != null)
                xme.SetAttribute("Bottom", Bottom.Value.ToString());

            if (Width != null)
                xme.SetAttribute("Width", Width.Value.ToString());

            if (Height != null)
                xme.SetAttribute("Height", Height.Value.ToString());

            if (_isBackground != null)
                xme.SetAttribute("IsBackground", _isBackground.Value.ToString());

            return xme;
        }

        protected virtual void AppendData(XmlElement xme)
        {
            _name = GetString(xme, "Name");
            Left = GetValue(xme, "Left");
            Top = GetValue(xme, "Top");
            Right = GetValue(xme, "Right");
            Bottom = GetValue(xme, "Bottom");
            Width = GetValue(xme, "Width");
            Height = GetValue(xme, "Height");
            _isBackground = GetBool(xme, "IsBackground");
        }

        private string GetString(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return val.Value;
            return null;
        }

        private bool? GetBool(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return bool.Parse(val.Value);
            return null;
        }

        private UnitValue? GetValue(XmlElement xme, string name)
        {
            var val = xme.Attributes[name];
            if (val != null)
                return UnitValue.Parse(val.Value);
            return null;
        }
    }
}