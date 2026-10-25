using System.Xml;
using AwesomeAssertions;
using Moq;
using PdfSharp.Drawing;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Element;
using Tharga.Reporter.Entity.Util;
using Tharga.Reporter.Interface;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class Null_safety_Tests
{
    [Fact]
    public void A_unit_rectangle_is_not_equal_to_null()
    {
        //Arrange
        var rectangle = new UnitRectangle { Left = "1cm" };

        //Act
        var equal = rectangle.Equals(null);

        //Assert
        equal.Should().BeFalse();
    }

    [Fact]
    public void Document_data_without_a_data_element_loads_its_tables()
    {
        //Arrange
        var xml = new XmlDocument();
        xml.LoadXml("<DocumentData><Table Name=\"A\"><Row><Col1>Value</Col1></Row></Table></DocumentData>");

        //Act
        var documentData = DocumentData.Load(xml);

        //Assert
        documentData.Get("Col1").Should().BeNull();
        documentData.GetDataTable("A").Rows.Should().ContainSingle();
    }

    [Theory]
    [InlineData("DisplayName")]
    [InlineData("Align")]
    [InlineData("WidthMode")]
    public void A_table_column_without_a_required_attribute_names_the_attribute(string attribute)
    {
        //Arrange
        var xme = new TableColumn("A", "2cm", Table.WidthMode.Specific, Table.Alignment.Left, null).ToXme();
        xme.RemoveAttribute(attribute);

        //Act
        var act = () => TableColumn.Load(xme);

        //Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{attribute}*");
    }

    [Fact]
    public void A_reference_point_without_a_position_renders_at_the_parent_origin()
    {
        //Arrange
        var referencePoint = new ReferencePoint();
        var renderData = new RenderData(Mock.Of<IGraphics>(), new XRect(10, 20, 300, 400), new Section(), null, new PageNumberInfo(1, 1), null, true);

        //Act
        referencePoint.Render(renderData, 0);

        //Assert
        renderData.ElementBounds.X.Should().Be(10);
    }
}
