using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests;

public class UnitValue_implicit_string_parse
{
    [Fact]
    public void From_string()
    {
        //Act
        UnitValue value = "10cm";

        //Assert
        value.Should().Be(UnitValue.Parse("10cm"));
    }

    [Fact]
    public void To_string()
    {
        //Arrange
        var value = UnitValue.Parse("10cm");

        //Act
        string text = value;

        //Assert
        text.Should().Be("10cm");
    }
}
