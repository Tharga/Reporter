using System.Drawing;
using Tharga.Reporter.Engine.Entity.Element;
using Xunit;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Test
{
    public class BarCode_Tests
    {
        [Fact]
        public void Default_BarCode()
        {
            //Arrange
            var text = new BarCode();
            var xme = text.ToXme();

            //Act
            var otherLine = BarCode.Load(xme);

            //Assert
            //Assert.AreEqual(text.Left, otherLine.Left);
            //Assert.AreEqual(text.Right, otherLine.Right);
            //Assert.AreEqual(text.Width, otherLine.Width);
            //Assert.AreEqual(text.Top, otherLine.Top);
            //Assert.AreEqual(text.Bottom, otherLine.Bottom);
            //Assert.AreEqual(text.Height, otherLine.Height);
            //Assert.AreEqual(text.Name, otherLine.Name);
            //Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(text.Name, otherLine.Name);
            //Assert.AreEqual(text.Code, text.Code);
            //Assert.AreEqual(text.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }

        [Fact]
        public void BarCode_with_all_propreties_set()
        {
            //Arrange
            var text = new BarCode
                {
                    Left = "1cm",
                    Top = "2cm",
                    Width = "10cm",
                    Height = "3cm",
                    IsBackground = true,
                    Name = "MyBarCode",
                    Code = "123",
                };
            var xme = text.ToXme();

            //Act
            var otherLine = BarCode.Load(xme);

            //Assert
            //Assert.AreEqual(text.Left, otherLine.Left);
            //Assert.AreEqual(text.Right, otherLine.Right);
            //Assert.AreEqual(text.Width, otherLine.Width);
            //Assert.AreEqual(text.Top, otherLine.Top);
            //Assert.AreEqual(text.Bottom, otherLine.Bottom);
            //Assert.AreEqual(text.Height, otherLine.Height);
            //Assert.AreEqual(text.Name, otherLine.Name);
            //Assert.AreEqual(text.IsBackground, otherLine.IsBackground);
            //Assert.AreEqual(text.Name, otherLine.Name);
            //Assert.AreEqual(text.Code, text.Code);
            //Assert.AreEqual(text.ToString(), otherLine.ToString());
            //Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
            throw new NotImplementedException();
        }
    }
}