using System.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing
{
    public class Line_Tests
    {
        [Fact(Skip = "Fix!")]
        public void Default_line()
        {
            //Arrange
            var line = new Line();
            var xme = line.ToXme();

            //Act
            var otherLine = Line.Load(xme);

            //Assert
            //Assert.AreEqual(line.Left, otherLine.Left);
            //Assert.AreEqual(line.Right, otherLine.Right);
            //Assert.AreEqual(line.Width, otherLine.Width);
            //Assert.AreEqual(line.Top, otherLine.Top);
            //Assert.AreEqual(line.Bottom, otherLine.Bottom);
            //Assert.AreEqual(line.Height, otherLine.Height);
            //Assert.AreEqual(line.Color.ToArgb(), otherLine.Color.ToArgb());
            //Assert.AreEqual(line.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(line.Name, otherLine.Name);
            //Assert.AreEqual(line.Thickness, otherLine.Thickness);
            //Assert.AreEqual(line.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }

        [Fact(Skip = "Fix!")]
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
                Color = Color.Aquamarine,
                Name = "Bob Loblaw",
                Thickness = "1cm",                
            };
            var xme = line.ToXme();

            //Act
            var otherLine = Line.Load(xme);

            //Assert
            //TODO: Have reflection go over all properties and compare them
            //Assert.AreEqual(line.Left, otherLine.Left);
            //Assert.AreEqual(line.Right, otherLine.Right);
            //Assert.AreEqual(line.Width, otherLine.Width);
            //Assert.AreEqual(line.Top, otherLine.Top);
            //Assert.AreEqual(line.Bottom, otherLine.Bottom);
            //Assert.AreEqual(line.Height, otherLine.Height);
            //Assert.AreEqual(line.Color.ToArgb(), otherLine.Color.ToArgb());
            //Assert.AreEqual(line.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(line.Name, otherLine.Name);
            //Assert.AreEqual(line.Thickness, otherLine.Thickness);
            //Assert.AreEqual(line.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }
    }
}
