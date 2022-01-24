using System.Drawing;
using Tharga.Reporter.Entity;
using Xunit;
using Rectangle = Tharga.Reporter.Entity.Element.Rectangle;

namespace Tharga.Reporter.Tests.Serializing;

public class Rectangle_Tests
{
    [Fact(Skip = "Fix!")]
    public void Default_Rectangle()
    {
        //Arrange
        var rectangle = new Rectangle();
        var xme = rectangle.ToXme();

        //Act
        var other = Rectangle.Load(xme);

        //Assert
        //Assert.AreEqual(rectangle.Left, other.Left);
        //Assert.AreEqual(rectangle.Top, other.Top);
        //Assert.AreEqual(rectangle.Right, other.Right);
        //Assert.AreEqual(rectangle.Bottom, other.Bottom);
        //Assert.AreEqual(rectangle.Width, other.Width);
        //Assert.AreEqual(rectangle.Height, other.Height);
        //Assert.AreEqual(rectangle.BackgroundColor, other.BackgroundColor);
        //Assert.AreEqual(rectangle.BorderColor, other.BorderColor);
        //Assert.AreEqual(rectangle.BorderWidth, other.BorderWidth);
        //Assert.AreEqual(rectangle.IsBackground, other.IsBackground);
        //Assert.AreEqual(rectangle.Name, other.Name);
        //Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        throw new NotImplementedException();
    }

    [Fact(Skip = "Fix!")]
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
            Name = "Rea Padda"
        };
        var xme = rectangle.ToXme();

        //Act
        var other = Rectangle.Load(xme);

        //Assert
        //Assert.AreEqual(rectangle.Left, other.Left);
        //Assert.AreEqual(rectangle.Top, other.Top);
        //Assert.AreEqual(rectangle.Right, other.Right);
        //Assert.AreEqual(rectangle.Bottom, other.Bottom);
        //Assert.AreEqual(rectangle.Width, other.Width);
        //Assert.AreEqual(rectangle.Height, other.Height);
        //Assert.AreEqual(rectangle.BackgroundColor.Value.ToArgb(), other.BackgroundColor.Value.ToArgb());
        //Assert.AreEqual(rectangle.BorderColor.ToArgb(), other.BorderColor.ToArgb());
        //Assert.AreEqual(rectangle.BorderWidth, other.BorderWidth);
        //Assert.AreEqual(rectangle.IsBackground, other.IsBackground);
        //Assert.AreEqual(rectangle.Name, other.Name);
        //Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
        throw new NotImplementedException();
    }
}