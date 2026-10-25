using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class Template_Tests
{
    [Fact]
    public void Default_template()
    {
        //Arrange
        var template = new Template(new Section());
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        otherTemplate.SectionList.Should().ContainSingle();
        otherTemplate.ToXml().OuterXml.Should().Be(xml.OuterXml);
    }
}
