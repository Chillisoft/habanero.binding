using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.CF;
using Habanero.Faces.CF.Adapters;
using Habanero.Faces.CF.Controls;
using Habanero.ProgrammaticBinding;
using Habanero.Smooth;
using Habanero.Testability.CF;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestWinFormsTextBoxAdapter
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
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
            var mapper = new TextBoxMapper(new WinFormsTextBoxAdapter(GenerateStub<TextBox>()), "FakeStringProp", false, new ControlFactoryCF());
            var expectedBO = new FakeBo();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(expectedBO);
            Assert.AreNotSame(expectedBO, mapper.BusinessObject);
            //---------------Execute Test ----------------------
            mapper.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedBO, mapper.BusinessObject);
        }

        [Test]
        public void Test_Equals_Control_WhenIsSame_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var textBox = GenerateStub<TextBox>();
            var adapter = new WinFormsTextBoxAdapter(textBox);
            //---------------Assert Precondition----------------
            Assert.AreSame(textBox, adapter.WrappedControl);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(textBox);
            //---------------Test Result -----------------------
            Assert.IsTrue(@equals);
        }
        [Test]
        public void Test_Equals_Control_WhenNotSame_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var adapter = new WinFormsTextBoxAdapter(GenerateStub<TextBox>());
            var otherTextBox = GenerateStub<TextBox>();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(otherTextBox, adapter.WrappedControl);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(otherTextBox);
            //---------------Test Result -----------------------
            Assert.IsFalse(@equals);
        }
        [Test]
        public void Test_Equals_Control_WhenOtherNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var adapter = new WinFormsTextBoxAdapter(GenerateStub<TextBox>());
            TextBox otherTextBox = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(otherTextBox);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(otherTextBox);
            //---------------Test Result -----------------------
            Assert.IsFalse(@equals);
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