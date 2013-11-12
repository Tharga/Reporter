using System;
using System.Drawing;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Helper;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Text : TextBase
    {
        public string Value { get; set; }
        public string HideValue { get; set; }
        public double FontSize { get { return Font.Size; } set { Font.Size = value; } }
        public Color? FontColor { get { return Font.Color; } set { Font.Color = value; } }

        #region Constructors


        public Text()
        {
            
        }

        internal Text(string value)
        {
            Value = value;
        }

        //internal Text(XmlElement xmlElement)
        //    :base(xmlElement)
        //{
        //    Value = xmlElement.Attributes.GetNamedItem("Value").Value;
        //}

        //internal Text(string value, string fontClass)
        //    : base(fontClass)
        //{
        //    Value = value;
        //}

        //internal Text(string value, string fontClass, UnitRectangle relativeAlignment)
        //    : base(fontClass, relativeAlignment)
        //{
        //    Value = value;
        //}


        #endregion
        #region Factory


        [Obsolete("Use default constructor and property setters instead.")]
        public static Text Create(string value, string left = null, string top = null, string width = null,
            Alignment textAlignment = Alignment.Left, double fontSize = 10, string hideValue = null)
        {
            var text = new Text
                {
                    Value = value,
                    Left = left != null ? UnitValue.Parse(left) : (UnitValue?)null,
                    Top = top != null ? UnitValue.Parse(top) : (UnitValue?)null,
                    Width = width != null ? UnitValue.Parse(width) : (UnitValue?)null,
                    TextAlignment = textAlignment,
                    FontSize = fontSize,
                    HideValue = hideValue,
                };
            return text;
        }

        [Obsolete("Use default constructor and property setters instead.")]
        public static Text Create(string value, Color fontColor, double fontSize, string left = null)
        {
            var text = new Text(value)
                           {
                               Left = left != null ? UnitValue.Parse(left) : (UnitValue?)null,
                               FontColor = fontColor,
                               FontSize = fontSize
                           };
            return text;
        }


        #endregion

        protected override string GetValue(DocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = documentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return string.Empty;
            }

            return Value.ParseValue(documentData, pageNumberInfo);
        }

        //protected internal override XmlElement AppendXml(ref XmlElement xmePane)
        //{
        //    var xmeElement = base.AppendXml(ref xmePane);

        //    xmeElement.SetAttribute("Value", Value);

        //    return xmeElement;
        //}
    }
}