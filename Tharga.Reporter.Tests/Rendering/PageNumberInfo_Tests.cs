using System.Globalization;
using AwesomeAssertions;
using Tharga.Reporter.Entity.Util;
using Xunit;

namespace Tharga.Reporter.Tests.Rendering;

public class PageNumberInfo_Tests
{
    [Fact]
    public void PageNumber_returns_the_current_page()
    {
        //Arrange
        var info = new PageNumberInfo(3, 7);

        //Act
        var result = info.GetPageNumberInfo("PageNumber");

        //Assert
        result.Should().Be(3.ToString(CultureInfo.CurrentCulture));
    }

    [Fact]
    public void TotalPages_returns_the_page_count()
    {
        //Arrange
        var info = new PageNumberInfo(3, 7);

        //Act
        var result = info.GetPageNumberInfo("TotalPages");

        //Assert
        result.Should().Be(7.ToString(CultureInfo.CurrentCulture));
    }

    [Fact]
    public void TotalPages_is_not_available_before_the_page_count_is_known()
    {
        //Arrange
        var info = new PageNumberInfo(1, null);

        //Act
        var result = info.GetPageNumberInfo("TotalPages");

        //Assert
        result.Should().Be("N/A");
    }

    [Fact]
    public void An_unknown_name_returns_null()
    {
        //Arrange
        var info = new PageNumberInfo(1, 1);

        //Act
        var result = info.GetPageNumberInfo("SomethingElse");

        //Assert
        result.Should().BeNull();
    }
}
