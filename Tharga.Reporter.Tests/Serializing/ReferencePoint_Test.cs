using AwesomeAssertions;
using Tharga.Reporter.Entity;
using Tharga.Reporter.Entity.Area;
using Tharga.Reporter.Entity.Element;
using Xunit;

namespace Tharga.Reporter.Tests.Serializing;

public class ReferencePoint_Test
{
    [Fact]
    public void Default_ReferencePoint()
    {
        //Arrange
        var referencePoint = new ReferencePoint();
        var xme = referencePoint.ToXme();

        //Act
        var other = ReferencePoint.Load(xme);

        //Assert
        other.Left.Should().Be(referencePoint.Left);
        other.Top.Should().Be(referencePoint.Top);
        other.Stack.Should().Be(referencePoint.Stack);
        other.ElementList.Should().Equal(referencePoint.ElementList);
        other.Name.Should().Be(referencePoint.Name);
        other.IsBackground.Should().Be(referencePoint.IsBackground);
        other.ToString().Should().Be(referencePoint.ToString());
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }

    [Fact]
    public void ReferencePoint_with_all_propreties_set()
    {
        //Arrange
        var referencePoint = new ReferencePoint
        {
            IsBackground = true,
            Name = "Rea Padda",
            Left = UnitValue.Parse("10cm"),
            Top = UnitValue.Parse("3px"),
            Stack = ReferencePoint.StackMethod.Vertical,
            ElementList = new ElementList
            {
                new Image(),
                new Line(),
                new Rectangle(),
                new Table(),
                new Text(),
                new TextBox()
            }
        };
        var xme = referencePoint.ToXme();

        //Act
        var other = ReferencePoint.Load(xme);

        //Assert
        other.Left.Should().Be(referencePoint.Left);
        other.Top.Should().Be(referencePoint.Top);
        other.Stack.Should().Be(referencePoint.Stack);
        other.ElementList.Count.Should().Be(referencePoint.ElementList.Count);
        other.Name.Should().Be(referencePoint.Name);
        other.IsBackground.Should().Be(referencePoint.IsBackground);
        other.ToString().Should().Be(referencePoint.ToString());
        other.ToXme().OuterXml.Should().Be(xme.OuterXml);
    }
}
