using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Test
{
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
