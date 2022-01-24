//using System.Xml;
//using Tharga.Reporter.Engine.Entity;
//using Tharga.Reporter.Engine.Entity.Area;
//using Tharga.Reporter.Engine.Entity.Element;
//using Text = Tharga.Reporter.Engine.Entity.Element.Text;

//namespace Tharga.Reporter.Test
//{
//    abstract class AaaTest
//    {
//        protected AaaTest()
//        {
//            Arrange();
//            Act();
//        }

//        //[TestFixtureSetUp]
//        protected abstract void Arrange();
//        protected abstract void Act();
//    }

//    class When_serializing_a_defailt_referencePoint : AaaTest
//    {
//        private XmlElement _xme;
//        private ReferencePoint _other;
//        private ReferencePoint _item;

//        protected override void Arrange()
//        {
//            _item = new ReferencePoint();
//            _xme = _item.ToXme();
//        }

//        protected override void Act()
//        {
//            _other = ReferencePoint.Load(_xme);
//        }

//        [Test]
//        public void Then_Left_should_be_correct()
//        {
//            Assert.AreEqual(_item.Left, _other.Left);
//        }

//        [Test]
//        public void Then_Top_should_be_correct()
//        {
//            Assert.AreEqual(_item.Top, _other.Top);
//        }

//        [Test]
//        public void Then_Stack_should_be_correct()
//        {
//            Assert.AreEqual(_item.Stack, _other.Stack);
//        }

//        [Test]
//        public void Then_ElementList_should_be_correct()
//        {
//            Assert.AreEqual(_item.ElementList, _other.ElementList);
//        }

//        [Test]
//        public void Then_Name_should_be_correct()
//        {
//            Assert.AreEqual(_item.Name, _other.Name);
//        }

//        [Test]
//        public void Then_IsBackground_should_be_correct()
//        {
//            Assert.AreEqual(_item.IsBackground, _other.IsBackground);
//        }

//        [Test]
//        public void Then_string_conversion_should_be_correct()
//        {
//            Assert.AreEqual(_item.ToString(), _other.ToString());
//        }

//        [Test]
//        public void Then_xml_conversion_should_be_correct()
//        {
//            Assert.AreEqual(_xme.OuterXml, _other.ToXme().OuterXml);
//        }
//    }

//    [TestFixture]
//    class ReferencePoint_Test
//    {
//        [Test]
//        public void Text_with_all_propreties_set()
//        {
//            //Arrange
//            var referencePoint = new ReferencePoint
//                {
//                    IsBackground = true,
//                    Name = "Rea Padda",
//                    Left = UnitValue.Parse("10cm"),
//                    Top = UnitValue.Parse("3px"),
//                    Stack = ReferencePoint.StackMethod.Vertical,
//                    ElementList = new ElementList
//                        {
//                            new Image(),
//                            new Line(),
//                            new Rectangle(),
//                            new Table(),
//                            new Text(),
//                            new TextBox(),
//                        },
//                };
//            var xme = referencePoint.ToXme();

//            //Act
//            var other = ReferencePoint.Load(xme);

//            //Assert
//            Assert.AreEqual(referencePoint.Left, other.Left);
//            Assert.AreEqual(referencePoint.Top, other.Top);
//            Assert.AreEqual(referencePoint.Stack, other.Stack);
//            Assert.AreEqual(referencePoint.ElementList.Count, other.ElementList.Count);
//            Assert.AreEqual(referencePoint.Name, other.Name);
//            Assert.AreEqual(referencePoint.IsBackground, other.IsBackground);
//            Assert.AreEqual(referencePoint.Name, other.Name);
//            Assert.AreEqual(referencePoint.ToString(), other.ToString());
//            Assert.AreEqual(xme.OuterXml, other.ToXme().OuterXml);
//        }
//    }
//}

