using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Tests.Rendering
{
    [TestFixture]
    class When_pre_rendering_a_table : AaaTest
    {
        private Table _table;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private DocumentData _documentData;

        protected override void Arrange()
        {
            _table = new Table();
            _table.AddColumn("A1", "A2");
            _table.AddColumn("B1", "B2");

            _documentData = new DocumentData();
            _documentData.Add("A1", "Data_A1");
            _documentData.Add("A2", "Data_A2");
            _documentData.Add("B1", "Data_B1");
            _documentData.Add("B2", "Data_B2");

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
            _graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>());
            _renderDataMock.Setup(x => x.ElementBounds).Returns(It.IsAny<XRect>());
            _renderDataMock.Setup(x => x.Section).Returns(new Section());
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.DocumentData).Returns(_documentData);
            //_renderDataMock.Setup(x => x.DebugData).Returns(new DebugData());
        }

        protected override void Act()
        {
            _table.PreRender(_renderDataMock.Object);
        }

        [Test]
        public void Nothing_is_drawn()
        {
            _graphicsMock.Verify(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawEllipse(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawImage(It.IsAny<XImage>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Never);
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XBrush>(), It.IsAny<XRect>()), Times.Never);
            //TODO: Add all possible ways of output
        }
    }

    [TestFixture]
    class When_rendering_a_rectangle_with_visibility_first_page_on_the_first_page : AaaTest
    {
        private Rectangle _rectangle;
        private Mock<IRenderData> _renderDataMock;
        private Mock<IGraphics> _graphicsMock;
        private XRect _elementBounds;

        protected override void Arrange()
        {
            _rectangle = new Rectangle {Visibility = PageVisibility.FirstPage};

            _graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            _graphicsMock.Setup(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()));

            _renderDataMock = new Mock<IRenderData>(MockBehavior.Strict);
            _renderDataMock.Setup(x => x.ParentBounds).Returns(new XRect{ Width = 20, Height = 20});
            _renderDataMock.SetupSet(x => x.ElementBounds = It.IsAny<XRect>()).Callback<XRect>(x => _elementBounds = x);
            _renderDataMock.Setup(x => x.ElementBounds).Returns(new XRect{Width = 10, Height = 10});
            _renderDataMock.Setup(x => x.Graphics).Returns(_graphicsMock.Object);
            _renderDataMock.Setup(x => x.PageNumberInfo).Returns(new PageNumberInfo(1, 2));
        }

        protected override void Act()
        {
            _rectangle.Render(_renderDataMock.Object);
        }

        [Test]
        public void Then_the_rectangle_is_drawn()
        {
            _graphicsMock.Verify(x => x.DrawRectangle(It.IsAny<XPen>(), It.IsAny<XRect>()), Times.Once);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_some_width()
        {
            Assert.AreNotEqual(0, _elementBounds.Width);
        }

        [Test]
        public void Then_the_element_bounds_is_set_to_some_height()
        {
            Assert.AreNotEqual(0, _elementBounds.Height);
        }
    }
}