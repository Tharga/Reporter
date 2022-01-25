using FluentAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Extensions;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class When_parsing_a_data_string
{
    [Fact]
    public void Basic()
    {
        //Arrange
        var dataPart = "DataX";
        var dataValue = "DataValue";
        var input = $"ABC {{{dataPart}}}";

        var documentData = new DocumentData();
        documentData.Add(dataPart, dataValue);

        //Act
        var result = input.ParseValue(documentData, null);

        //Assert
        result.Should().NotBe(input);
        result.Last().Should().NotBe(' ');
        result.Should().Be($"ABC {dataValue}");
    }
}