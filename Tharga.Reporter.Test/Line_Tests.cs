using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.Test
{
    [TestClass]
    public class Rectangle_Tests
    {
        [TestMethod]
        public void Default_Rectangle()
        {
            //Arrange
            var rectangle = new Rectangle();
            var xme = rectangle.ToXme();

            //Act
            var other = Rectangle.Load(xme);

            //Assert
            Assert.AreEqual(rectangle.Left, other.Left);
            Assert.AreEqual(rectangle.Top, other.Top);
            Assert.AreEqual(rectangle.Right, other.Right);
            Assert.AreEqual(rectangle.Bottom, other.Bottom);
            Assert.AreEqual(rectangle.Width, other.Width);
            Assert.AreEqual(rectangle.Height, other.Height);
            Assert.AreEqual(rectangle.BackgroundColor, other.BackgroundColor);
            Assert.AreEqual(rectangle.BorderColor, other.BorderColor);
            Assert.AreEqual(rectangle.BorderWidth, other.BorderWidth);
            Assert.AreEqual(rectangle.IsBackground, other.IsBackground);
            Assert.AreEqual(rectangle.Name, other.Name);
            Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        }

        //TODO: Fix test!
        [TestMethod]
        [Ignore]
        public void Rectangle_with_all_propreties_set()
        {
            //Arrange
            var rectangle = new Rectangle
                {
                    Top = UnitValue.Parse("1cm"),
                    Left = UnitValue.Parse("2cm"),
                    Width = UnitValue.Parse("40%"),
                    Height = UnitValue.Parse("50%"),
                    BackgroundColor = Color.Fuchsia,
                    BorderColor = Color.ForestGreen,
                    BorderWidth = UnitValue.Parse("0,1cm"),
                    IsBackground = true,
                    Name = "Rea Padda",
                };
            var xme = rectangle.ToXme();

            //Act
            var other = Rectangle.Load(xme);

            //Assert
            Assert.AreEqual(rectangle.Left, other.Left);
            Assert.AreEqual(rectangle.Top, other.Top);
            Assert.AreEqual(rectangle.Right, other.Right);
            Assert.AreEqual(rectangle.Bottom, other.Bottom);
            Assert.AreEqual(rectangle.Width, other.Width);
            Assert.AreEqual(rectangle.Height, other.Height);
            Assert.AreEqual(rectangle.BackgroundColor.Value.ToArgb(), other.BackgroundColor.Value.ToArgb());
            Assert.AreEqual(rectangle.BorderColor, other.BorderColor);
            Assert.AreEqual(rectangle.BorderWidth, other.BorderWidth);
            Assert.AreEqual(rectangle.IsBackground, other.IsBackground);
            Assert.AreEqual(rectangle.Name, other.Name);
            Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        }
    }

    [TestClass]
    public class Line_Tests
    {
        [TestMethod]
        public void Default_line()
        {
            //Arrange
            var line = new Line();
            var xme = line.ToXme();

            //Act
            var otherLine = Line.Load(xme);

            //Assert
            Assert.AreEqual(line.Left, otherLine.Left);
            Assert.AreEqual(line.Right, otherLine.Right);
            Assert.AreEqual(line.Width, otherLine.Width);
            Assert.AreEqual(line.Top, otherLine.Top);
            Assert.AreEqual(line.Bottom, otherLine.Bottom);
            Assert.AreEqual(line.Height, otherLine.Height);
            Assert.AreEqual(line.BorderColor.ToArgb(), otherLine.BorderColor.ToArgb());
            Assert.AreEqual(line.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(line.Name, otherLine.Name);
            Assert.AreEqual(line.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        //TODO: Fix test!
        [TestMethod]
        [Ignore]
        public void Line_with_all_propreties_set()
        {
            //Arrange
            var line = new Line
            {
                Left = UnitValue.Parse("1mm"),
                Right = UnitValue.Parse("2mm"),
                Top = UnitValue.Parse("3mm"),
                Bottom = UnitValue.Parse("4mm"),
                IsBackground = true,
                BorderColor = Color.Aquamarine,
                Name = "Bob Loblaw",
                
            };
            var xme = line.ToXme();

            //Act
            var otherLine = Line.Load(xme);

            //Assert
            //TODO: Have reflection go over all properties and compare them
            Assert.AreEqual(line.Left, otherLine.Left);
            Assert.AreEqual(line.Right, otherLine.Right);
            Assert.AreEqual(line.Width, otherLine.Width);
            Assert.AreEqual(line.Top, otherLine.Top);
            Assert.AreEqual(line.Bottom, otherLine.Bottom);
            Assert.AreEqual(line.Height, otherLine.Height);
            Assert.AreEqual(line.BorderColor.ToArgb(), otherLine.BorderColor.ToArgb());
            Assert.AreEqual(line.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(line.Name, otherLine.Name);
            Assert.AreEqual(line.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }
    }
}
