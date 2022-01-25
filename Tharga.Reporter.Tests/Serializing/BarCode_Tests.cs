using FluentAssertions;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

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
        otherLine.Left.Should().Be(text.Left);
        otherLine.Left.Should().Be(text.Left);
        otherLine.Right.Should().Be(text.Right);
        otherLine.Width.Should().Be(text.Width);
        otherLine.Top.Should().Be(text.Top);
        otherLine.Bottom.Should().Be(text.Bottom);
        otherLine.Height.Should().Be(text.Height);
        otherLine.Name.Should().Be(text.Name);
        otherLine.IsBackground.Should().Be(text.IsBackground);
        otherLine.Name.Should().Be(text.Name);
        otherLine.Code.Should().Be(text.Code);
        otherLine.ToString().Should().Be(text.ToString());
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
        otherLine.Left.Should().Be(text.Left);
        otherLine.Right.Should().Be(text.Right);
        otherLine.Width.Should().Be(text.Width);
        otherLine.Top.Should().Be(text.Top);
        otherLine.Bottom.Should().Be(text.Bottom);
        otherLine.Height.Should().Be(text.Height);
        otherLine.Name.Should().Be(text.Name);
        otherLine.IsBackground.Should().Be(text.IsBackground);
        otherLine.Name.Should().Be(text.Name);
        otherLine.Code.Should().Be(text.Code);
        otherLine.ToString().Should().Be(otherLine.ToString());
        xme.OuterXml.Should().Be(otherLine.ToXme().OuterXml);
    }
}