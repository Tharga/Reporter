using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class TableColumn_Tests
{
    [Fact]
    public void TableColumn_with_all_properties_set()
    {
        //Arrange
        var column = new TableColumn("A", "2cm", Table.WidthMode.Specific, Table.Alignment.Left, "***");
        var xme = column.ToXme();

        //Act
        var other = TableColumn.Load(xme);

        //Assert
        other.Width.Should().Be(UnitValue.Parse("2cm"));
        other.Align.Should().Be(Table.Alignment.Left);
        other.DisplayName.Should().Be("A");
        other.Hide.Should().BeFalse();
        other.HideValue.Should().Be("***");
        other.WidthMode.Should().Be(Table.WidthMode.Specific);
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
