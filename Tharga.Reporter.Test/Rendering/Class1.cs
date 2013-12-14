using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Interface;
using Text = Tharga.Reporter.Engine.Entity.Element.Text;

namespace Tharga.Reporter.Test.Rendering
{
    [TestFixture]
    class Class1
    {
        [Test]
        public void When_rendering_text()
        {
            //Arrange
            var text = new Text();
            var renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect());
            renderDataMock.Setup(x => x.Section).Returns(new Section());
            renderDataMock.Setup(x => x.DocumentData).Returns((DocumentData)null);
            renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));

            //Act
            text.Render(renderDataMock.Object);

            //Assert
        }
    }
}
