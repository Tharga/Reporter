using System;
using System.Xml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
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

        public string Name { get; set; }
        public bool IsBackground { get; set; }

        protected Element()
        {
            _relativeAlignment = new UnitRectangle();
        }

        protected Element(UnitRectangle relativeAlignment)
        {
            _relativeAlignment = relativeAlignment;
        }

        protected Element(XmlElement xmlElement)
        {
            _relativeAlignment = new UnitRectangle();

            if (xmlElement.Attributes.GetNamedItem("Left") != null)
                _relativeAlignment.Left = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Left").Value);

            if (xmlElement.Attributes.GetNamedItem("Top") != null)
                _relativeAlignment.Top = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Top").Value);

            if (xmlElement.Attributes.GetNamedItem("Right") != null)
                _relativeAlignment.Right = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Right").Value);

            if (xmlElement.Attributes.GetNamedItem("Bottom") != null)
                _relativeAlignment.Bottom = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Bottom").Value);

            if (xmlElement.Attributes.GetNamedItem("Width") != null)
                _relativeAlignment.Width = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Width").Value);

            if (xmlElement.Attributes.GetNamedItem("Height") != null)
                _relativeAlignment.Height = UnitValue.Parse(xmlElement.Attributes.GetNamedItem("Height").Value);
        }
        
        protected internal abstract XmlElement AppendXml(ref XmlElement xmePane);

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

        protected XmlElement AppendXmlBase(ref XmlElement xmePane)
        {
            if (xmePane == null) throw new ArgumentNullException("xmePane");
            if (xmePane.OwnerDocument == null) throw new ArgumentNullException("xmePane", "xmePane has no owner document.");

            var xmeElement = xmePane.OwnerDocument.CreateElement(GetType().ToShortTypeName());
            xmePane.AppendChild(xmeElement);

            if (Left != null) xmeElement.SetAttribute("Left", Left.ToString());
            if (Top != null) xmeElement.SetAttribute("Top", Top.ToString());
            if (Right != null) xmeElement.SetAttribute("Right", Right.ToString());
            if (Bottom != null) xmeElement.SetAttribute("Bottom", Bottom.ToString());
            if (Width != null) xmeElement.SetAttribute("Width", Width.ToString());
            if (Height != null) xmeElement.SetAttribute("Height", Height.ToString());

            return xmeElement;
        }

        public UnitValue Top { get { return _relativeAlignment.Top; } set { _relativeAlignment.Top = value; } }
        public virtual UnitValue Bottom { get { return _relativeAlignment.Bottom; } set { _relativeAlignment.Bottom = value; } }
        public virtual UnitValue Height { get { return _relativeAlignment.Height; } set { _relativeAlignment.Height = value; } }
        public UnitValue Left { get { return _relativeAlignment.Left; } set { _relativeAlignment.Left = value; } }
        public virtual UnitValue Right { get { return _relativeAlignment.Right; } set { _relativeAlignment.Right = value; } }
        public virtual UnitValue Width { get { return _relativeAlignment.Width; } set { _relativeAlignment.Width = value; } }
    }
}