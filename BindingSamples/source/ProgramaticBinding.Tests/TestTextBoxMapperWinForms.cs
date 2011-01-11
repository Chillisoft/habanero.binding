using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.Smooth;
using Habanero.Testability;
using HabaneroProgramaticBinding.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace ProgramaticBinding.Tests
{
    [TestFixture]
    public class TestTextBoxMapperWinForms
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            GlobalUIRegistry.ControlFactory = new ControlFactoryManualBindingWin();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [Test]
        public void Test_SetBusinessObject_ShouldSetBusinessObject()
        {
            //---------------Set up test pack-------------------
            var mapper = new TextBoxMapperWinForms(GenerateStub<TextBox>(), "FakeStringProp", false, GlobalUIRegistry.ControlFactory);
            var expectedBO = new FakeBo();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(expectedBO);
            Assert.AreNotSame(expectedBO, mapper.BusinessObject);
            //---------------Execute Test ----------------------
            mapper.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedBO, mapper.BusinessObject);
        }
        private static T GenerateStub<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }
}