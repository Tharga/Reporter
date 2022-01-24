using Tharga.Reporter.Engine.Entity;
using Xunit;

namespace Tharga.Reporter.Test
{
    public class UnitValue_implicit_string_parse
    {
        [Fact]
        public void From_string()
        {
            //Arrange
            var val1 = UnitValue.Parse("10cm");
            UnitValue val2;
            
            //Act
            val2 = "10cm";

            //Assert
            //Assert.AreEqual(val1, val2);
            throw new NotImplementedException();
        }

        [Fact]
        public void To_string()
        {
            //Arrange
            var val1 = "10cm";

            //Act
            var val2 = UnitValue.Parse( "10cm");

            //Assert
            //Assert.AreEqual(val1, (string)val2);
            throw new NotImplementedException();
        }
    }
}