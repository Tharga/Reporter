using FluentAssertions;
using Tharga.Reporter.Extensions;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_parsing_a_string
{
    [Fact]
    public void Basic()
    {
        //Arrange
        var input = "ABC";

        //Act
        var result = input.ParseValue(null, null);

        //Assert
        result.Should().Be(input);
        result.Last().Should().NotBe(' ');
    }
}