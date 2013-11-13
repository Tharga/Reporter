using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Test
{
    [TestClass]
    public class Font_Tests
    {
        [TestMethod]
        public void Default_Font()
        {
            //Arrange
            var font = new Font();
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color, otherLine.Color);
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [TestMethod]
        public void Font_with_all_properties_set()
        {
            //Arrange
            var font = new Font
                {
                    Size = 12,
                    Color = Color.Orange,
                    FontName = "Some Name",
                };
            var xme = font.ToXme();

            //Act
            var otherLine = Font.Load(xme);

            //Assert
            Assert.AreEqual(font.Color.Value.ToArgb(), otherLine.Color.Value.ToArgb());
            Assert.AreEqual(font.FontName, otherLine.FontName);
            Assert.AreEqual(font.Size, otherLine.Size);
            Assert.AreEqual(font.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}