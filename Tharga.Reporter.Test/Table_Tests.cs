using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Test
{
    [TestClass]
    public class Table_Tests
    {
        [TestMethod]
        public void Default_Table()
        {
            //Arrange
            var table = new Table();
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);            
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);
            Assert.AreEqual(table.BackgroundColor, otherLine.BackgroundColor);
            Assert.AreEqual(table.BorderColor, otherLine.BorderColor);
            Assert.AreEqual(table.HeaderFont.FontName, otherLine.HeaderFont.FontName);
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            Assert.AreEqual(table.LineFont.FontName, otherLine.LineFont.FontName);
            Assert.AreEqual(table.LineFontClass, otherLine.LineFontClass);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [TestMethod]
        public void Table_with_all_properties_set()
        {
            //Arrange
            var table = new Table
                {
                    BackgroundColor = Color.Navy,
                    BorderColor = Color.Olive,
                    Bottom = UnitValue.Parse("10px"),
                    HeaderFontClass = "A",
                    Height = UnitValue.Parse("20px"),
                    IsBackground = true,
                    Left = UnitValue.Parse("10cm"),
                    Right = UnitValue.Parse("20cm"),
                    Name = "Bob",
                    LineFontClass = "C"                    
                };
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);
            Assert.AreEqual(table.BackgroundColor.Value.ToArgb(), otherLine.BackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.BorderColor.ToArgb(), otherLine.BorderColor.ToArgb());
            //Assert.AreEqual(table.HeaderFont.FontName, otherLine.HeaderFont.FontName);
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            //Assert.AreEqual(table.LineFont.FontName, otherLine.LineFont.FontName);
            Assert.AreEqual(table.LineFontClass, otherLine.LineFontClass);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}