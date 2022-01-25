using FluentAssertions;
using Tharga.Reporter.Extensions;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_parsing_a_data_string_with_data_that_is_missing
{
    [Fact]
    protected void Basic()
    {
        //Arrange
        var _dataPart = "DataX";
        var input = $"ABC {{{_dataPart}}}";

        //Act
        var result = input.ParseValue(null, null);

        //Assert
        result.Should().NotBe(input);
        result.Last().Should().NotBe(' ');
        result.Should().Be($"ABC [Data '{_dataPart}' is missing]");
    }
}