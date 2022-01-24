using Tharga.Reporter.Entity;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class Template_Tests
{
    [Fact(Skip = "Fix!")]
    public void Default_template()
    {
        //Arrange
        var template = new Template(new Section());
        var xml = template.ToXml();

        //Act
        var otherTemplate = Template.Load(xml);

        //Assert
        //Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
        throw new NotImplementedException();
    }
}