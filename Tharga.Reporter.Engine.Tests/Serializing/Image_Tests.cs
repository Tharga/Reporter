using Tharga.Reporter.Engine.Entity;
using Xunit;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;

namespace Tharga.Reporter.Test
{
    public class Image_Tests
    {
        [Fact]
        public void Default_Image()
        {
            //Arrange
            var image = new Image();
            var xme = image.ToXme();

            //Act
            var otherLine = Image.Load(xme);

            //Assert
            //Assert.AreEqual(image.Left, otherLine.Left);
            //Assert.AreEqual(image.Right, otherLine.Right);
            //Assert.AreEqual(image.Width, otherLine.Width);
            //Assert.AreEqual(image.Top, otherLine.Top);
            //Assert.AreEqual(image.Bottom, otherLine.Bottom);
            //Assert.AreEqual(image.Height, otherLine.Height);
            //Assert.AreEqual(image.Source, otherLine.Source);
            //Assert.AreEqual(image.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(image.Name, otherLine.Name);
            //Assert.AreEqual(image.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }

        [Fact]
        public void Image_with_all_propreties_set()
        {
            //Arrange
            var image = new Image
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
            var otherLine = Image.Load(xme);

            //Assert
            //Assert.AreEqual(image.Left, otherLine.Left);
            //Assert.AreEqual(image.Right, otherLine.Right);
            //Assert.AreEqual(image.Width, otherLine.Width);
            //Assert.AreEqual(image.Top, otherLine.Top);
            //Assert.AreEqual(image.Bottom, otherLine.Bottom);
            //Assert.AreEqual(image.Height, otherLine.Height);
            //Assert.AreEqual(image.Source, otherLine.Source);
            //Assert.AreEqual(image.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(image.Name, otherLine.Name);
            //Assert.AreEqual(image.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }
    }
}