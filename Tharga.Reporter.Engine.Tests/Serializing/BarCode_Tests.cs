using FluentAssertions;
using Tharga.Reporter.Engine.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Test;

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
        text.Left.Should().Be(otherLine.Left);
        text.Left.Should().Be(otherLine.Left);
        text.Right.Should().Be(otherLine.Right);
        text.Width.Should().Be(otherLine.Width);
        text.Top.Should().Be(otherLine.Top);
        text.Bottom.Should().Be(otherLine.Bottom);
        text.Height.Should().Be(otherLine.Height);
        text.Name.Should().Be(otherLine.Name);
        text.IsBackground.Should().Be(otherLine.IsBackground);
        text.Name.Should().Be(otherLine.Name);
        text.Code.Should().Be(text.Code);
        text.ToString().Should().Be(otherLine.ToString());
        xme.OuterXml.Should().Be(otherLine.ToXme().OuterXml);
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
            Code = "123"
        };
        var xme = text.ToXme();

        //Act
        var otherLine = BarCode.Load(xme);

        //Assert
        text.Left.Should().Be(otherLine.Left);
        text.Right.Should().Be(otherLine.Right);
        text.Width.Should().Be(otherLine.Width);
        text.Top.Should().Be(otherLine.Top);
        text.Bottom.Should().Be(otherLine.Bottom);
        text.Height.Should().Be(otherLine.Height);
        text.Name.Should().Be(otherLine.Name);
        text.IsBackground.Should().Be(otherLine.IsBackground);
        text.Name.Should().Be(otherLine.Name);
        text.Code.Should().Be(text.Code);
        text.ToString().Should().Be(otherLine.ToString());
        xme.OuterXml.Should().Be(otherLine.ToXme().OuterXml);
    }
}