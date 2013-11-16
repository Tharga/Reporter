using System.Drawing;
using System.Linq;
using NUnit.Framework;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Test
{
    [TestFixture]
    public class Table_Tests
    {
        [Test]
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

        [Test]
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
            table.AddColumn("A", "B", UnitValue.Parse("1cm"), Table.WidthMode.Spring, Table.Alignment.Right, "123");
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
            
            Assert.AreEqual(table.Columns.Count, otherLine.Columns.Count);
            var colA = table.Columns.First();
            var colB = otherLine.Columns.First();
            Assert.AreEqual(colA.Key, colB.Key);
            Assert.AreEqual(colA.Value.Align, colB.Value.Align);
            Assert.AreEqual(colA.Value.DisplayName, colB.Value.DisplayName);
            Assert.AreEqual(colA.Value.Hide, colB.Value.Hide);
            Assert.AreEqual(colA.Value.HideValue, colB.Value.HideValue);
            Assert.AreEqual(colA.Value.Width, colB.Value.Width);
            Assert.AreEqual(colA.Value.WidthMode, colB.Value.WidthMode);

            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}