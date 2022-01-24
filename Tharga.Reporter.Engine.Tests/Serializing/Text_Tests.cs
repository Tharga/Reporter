using System.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element.Base;
using Xunit;
using Font = Tharga.Reporter.Entity.Font;
using Text = Tharga.Reporter.Entity.Element.Text;

namespace Tharga.Reporter.Tests.Serializing
{
    public class Text_Tests
    {
        [Fact(Skip = "Fix!")]
        public void Default_Text()
        {
            //Arrange
            var text = new Text();
            var xme = text.ToXme();

            //Act
            var other = Text.Load(xme);

            //Assert
            //Assert.AreEqual(text.Left, other.Left);
            //Assert.AreEqual(text.Right, other.Right);
            //Assert.AreEqual(text.Width, other.Width);
            //Assert.AreEqual(text.Top, other.Top);
            //Assert.AreEqual(text.Bottom, other.Bottom);
            //Assert.AreEqual(text.Height, other.Height);
            //Assert.AreEqual(text.Font.FontName, other.Font.FontName);
            //Assert.AreEqual(text.Font.Size, other.Font.Size);
            //Assert.AreEqual(text.Font.Color, other.Font.Color);
            //Assert.AreEqual(text.FontClass, other.FontClass);
            //Assert.AreEqual(text.HideValue, other.HideValue);
            //Assert.AreEqual(text.TextAlignment, other.TextAlignment);
            //Assert.AreEqual(text.Value, other.Value);
            //Assert.AreEqual(text.IsBackground, other.IsBackground);
            //Assert.AreEqual(text.Name, other.Name);
            //Assert.AreEqual(text.Visibility, other.Visibility);
            //Assert.AreEqual(text.ToString(), other.ToString());
            //Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
            throw new NotImplementedException();
        }

        [Fact(Skip = "Fix!")]
        public void Text_with_fontclass()
        {
            //Arrange
            var text = new Text
                {
                    FontClass = "Yahoo",
                };
            var xme = text.ToXme();

            //Act
            var other = Text.Load(xme);

            //Assert
            //Assert.AreEqual(text.Left, other.Left);
            //Assert.AreEqual(text.Right, other.Right);
            //Assert.AreEqual(text.Width, other.Width);
            //Assert.AreEqual(text.Top, other.Top);
            //Assert.AreEqual(text.Bottom, other.Bottom);
            //Assert.AreEqual(text.Height, other.Height);
            //Assert.AreEqual(text.Font.FontName, other.Font.FontName);
            //Assert.AreEqual(text.Font.Size, other.Font.Size);
            //Assert.AreEqual(text.Font.Color.ToArgb(), other.Font.Color.ToArgb());
            //Assert.AreEqual(text.FontClass, other.FontClass);
            //Assert.AreEqual(text.HideValue, other.HideValue);
            //Assert.AreEqual(text.Visibility, other.Visibility);
            //Assert.AreEqual(text.TextAlignment, other.TextAlignment);
            //Assert.AreEqual(text.Value, other.Value);
            //Assert.AreEqual(text.IsBackground, other.IsBackground);
            //Assert.AreEqual(text.Name, other.Name);
            //Assert.AreEqual(text.ToString(), other.ToString());
            //Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
            throw new NotImplementedException();
        }

        [Fact(Skip = "Fix!")]
        public void Text_with_all_propreties_set()
        {
            //Arrange
            var text = new Text
                {
                    IsBackground = true,
                    Name = "Rea Padda",
                    Right = UnitValue.Parse("1px"),
                    Width = UnitValue.Parse("10cm"),
                    Bottom = UnitValue.Parse("2px"),
                    Top = UnitValue.Parse("3px"),
                    HideValue = "ABC",
                    TextAlignment = TextBase.Alignment.Right,
                    Value = "Bob Loblaw",
                    Font = new Font
                        {
                            FontName = "Verdana",
                            Color = Color.MistyRose,
                            Size = 13,
                        },
                    Visibility = PageVisibility.AllButLast,
                };
            var xme = text.ToXme();

            //Act
            var other = Text.Load(xme);

            //Assert
            //Assert.AreEqual(text.Left, other.Left);
            //Assert.AreEqual(text.Right, other.Right);
            //Assert.AreEqual(text.Width, other.Width);
            //Assert.AreEqual(text.Top, other.Top);
            //Assert.AreEqual(text.Bottom, other.Bottom);
            //Assert.AreEqual(text.Height, other.Height);
            //Assert.AreEqual(text.Font.FontName, other.Font.FontName);
            //Assert.AreEqual(text.Font.Size, other.Font.Size);
            //Assert.AreEqual(text.Font.Color.ToArgb(), other.Font.Color.ToArgb());
            //Assert.AreEqual(text.FontClass, other.FontClass);
            //Assert.AreEqual(text.HideValue, other.HideValue);
            //Assert.AreEqual(text.Visibility, other.Visibility); 
            //Assert.AreEqual(text.TextAlignment, other.TextAlignment);
            //Assert.AreEqual(text.Value, other.Value);
            //Assert.AreEqual(text.IsBackground, other.IsBackground);
            //Assert.AreEqual(text.Name, other.Name);
            //Assert.AreEqual(text.ToString(), other.ToString());
            //Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
            throw new NotImplementedException();
        }
    }
}