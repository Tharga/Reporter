using System.Drawing;
using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Font = Tharga.Reporter.Engine.Entity.Font;
using Text = Tharga.Reporter.Engine.Entity.Element.Text;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Text_Tests
    {
        [Test]
        public void Default_Text()
        {
            //Arrange
            var text = new Text();
            var xme = text.ToXme();

            //Act
            var otherLine = Text.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color, otherLine.Font.Color);
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.TextAlignment, otherLine.TextAlignment);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Text_with_fontclass()
        {
            //Arrange
            var text = new Text
                {
                    FontClass = "Yahoo",
                };
            var xme = text.ToXme();

            //Act
            var otherLine = Text.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color.ToArgb(), otherLine.Font.Color.ToArgb());
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.TextAlignment, otherLine.TextAlignment);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
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
                        }
                };
            var xme = text.ToXme();

            //Act
            var otherLine = Text.Load(xme);

            //Assert
            Assert.AreEqual(text.Left, otherLine.Left);
            Assert.AreEqual(text.Right, otherLine.Right);
            Assert.AreEqual(text.Width, otherLine.Width);
            Assert.AreEqual(text.Top, otherLine.Top);
            Assert.AreEqual(text.Bottom, otherLine.Bottom);
            Assert.AreEqual(text.Height, otherLine.Height);
            Assert.AreEqual(text.Font.FontName, otherLine.Font.FontName);
            Assert.AreEqual(text.Font.Size, otherLine.Font.Size);
            Assert.AreEqual(text.Font.Color.ToArgb(), otherLine.Font.Color.ToArgb());
            Assert.AreEqual(text.FontClass, otherLine.FontClass);
            Assert.AreEqual(text.HideValue, otherLine.HideValue);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.TextAlignment, otherLine.TextAlignment);
            Assert.AreEqual(text.Value, otherLine.Value);
            Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(text.Name, otherLine.Name);
            Assert.AreEqual(text.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}