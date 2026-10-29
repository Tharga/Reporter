using System.Globalization;
using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests;

public class UnitValue_parse_culture
{
    [Theory]
    [InlineData("en-US", "0,1cm", 0.1, UnitValue.EUnit.Centimeter)]
    [InlineData("sv-SE", "0,1cm", 0.1, UnitValue.EUnit.Centimeter)]
    [InlineData("en-US", "2,5mm", 2.5, UnitValue.EUnit.Millimeter)]
    [InlineData("sv-SE", "2,5mm", 2.5, UnitValue.EUnit.Millimeter)]
    [InlineData("en-US", "2.5mm", 2.5, UnitValue.EUnit.Millimeter)]
    [InlineData("sv-SE", "2.5mm", 2.5, UnitValue.EUnit.Millimeter)]
    [InlineData("en-US", "-1,25in", -1.25, UnitValue.EUnit.Inch)]
    [InlineData("sv-SE", "-1,25in", -1.25, UnitValue.EUnit.Inch)]
    [InlineData("en-US", ",5%", 0.5, UnitValue.EUnit.Percentage)]
    [InlineData("sv-SE", ".5%", 0.5, UnitValue.EUnit.Percentage)]
    public void A_comma_or_a_point_is_a_decimal_separator_on_every_culture(string culture, string input, double expectedValue, UnitValue.EUnit expectedUnit)
    {
        //Act
        var result = InCulture(culture, () => UnitValue.Parse(input));

        //Assert
        result.Value.Should().Be(expectedValue);
        result.Unit.Should().Be(expectedUnit);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("sv-SE")]
    public void A_value_written_on_one_culture_reads_back_the_same_on_another(string readCulture)
    {
        //Arrange
        var written = InCulture("sv-SE", () => UnitValue.Parse("12,75mm").ToString());

        //Act
        var result = InCulture(readCulture, () => UnitValue.Parse(written));

        //Assert
        result.Should().Be(new UnitValue(12.75, UnitValue.EUnit.Millimeter));
    }

    [Theory]
    [InlineData("en-US", "1,000,5mm")]
    [InlineData("sv-SE", "1,000,5mm")]
    [InlineData("en-US", "1.,5mm")]
    public void More_than_one_decimal_separator_is_rejected(string culture, string input)
    {
        //Act
        var act = () => InCulture(culture, () => UnitValue.Parse(input));

        //Assert
        act.Should().Throw<InvalidOperationException>();
    }

    private static T InCulture<T>(string culture, Func<T> action)
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            return action();
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
