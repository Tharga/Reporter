using Tharga.Reporter.Engine.Entity;
using Xunit;

namespace Tharga.Reporter.Test
{
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
            //Assert.AreEqual(xml.OuterXml, otherTemplate.ToXml().OuterXml);
            throw new NotImplementedException();
        }   
    }
}