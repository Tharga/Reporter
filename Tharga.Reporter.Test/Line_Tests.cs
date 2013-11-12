using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Test
{
    [TestClass]
    public class Image_Tests
    {
        [TestMethod]
        public void Default_Image()
        {
            //Arrange
            var image = new Tharga.Reporter.Engine.Entity.Element.Image();
            var xme = image.ToXme();

            //Act
            var otherLine = Tharga.Reporter.Engine.Entity.Element.Image.Load(xme);

            //Assert
            Assert.AreEqual(image.Left, otherLine.Left);
            Assert.AreEqual(image.Right, otherLine.Right);
            Assert.AreEqual(image.Width, otherLine.Width);
            Assert.AreEqual(image.Top, otherLine.Top);
            Assert.AreEqual(image.Bottom, otherLine.Bottom);
            Assert.AreEqual(image.Height, otherLine.Height);
            Assert.AreEqual(image.Source, otherLine.Source);
            Assert.AreEqual(image.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(image.Name, otherLine.Name);
            Assert.AreEqual(image.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [TestMethod]
        public void Image_with_all_propreties_set()
        {
            //Arrange
            var image = new Tharga.Reporter.Engine.Entity.Element.Image
                {
                    Right = UnitValue.Parse("1cm"),
                    Width = UnitValue.Parse("2cm"),
                    Bottom = UnitValue.Parse("2cm"),
                    Height = UnitValue.Parse("3cm"),
                    IsBackground = true,
                    Name = "Blah",
                    Source = "Blab_blah"
                };
            var xme = image.ToXme();

            //Act
            var otherLine = Tharga.Reporter.Engine.Entity.Element.Image.Load(xme);

            //Assert
            Assert.AreEqual(image.Left, otherLine.Left);
            Assert.AreEqual(image.Right, otherLine.Right);
            Assert.AreEqual(image.Width, otherLine.Width);
            Assert.AreEqual(image.Top, otherLine.Top);
            Assert.AreEqual(image.Bottom, otherLine.Bottom);
            Assert.AreEqual(image.Height, otherLine.Height);
            Assert.AreEqual(image.Source, otherLine.Source);
            Assert.AreEqual(image.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(image.Name, otherLine.Name);
            Assert.AreEqual(image.ToString(), otherLine.ToString());
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);

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

        [TestMethod]
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
