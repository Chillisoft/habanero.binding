using System;
using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgramaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.ProgramaticBinding.Tests
{

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestWinFormsNumericUpDownAdapter
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
            var mapper = new NumericUpDownIntegerMapper(new WinFormsNumericUpDownAdapter(GenerateStub<NumericUpDown>()), "FakeStringProp", false, new ControlFactoryWin());
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
            var numericUpDown = GenerateStub<NumericUpDown>();
            var adapter = new WinFormsNumericUpDownAdapter(numericUpDown);
            //---------------Assert Precondition----------------
            Assert.AreSame(numericUpDown, adapter.WrappedControl);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(numericUpDown);
            //---------------Test Result -----------------------
            Assert.IsTrue(@equals);
        }
        [Test]
        public void Test_Equals_Control_WhenNotSame_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var adapter = new WinFormsNumericUpDownAdapter(GenerateStub<NumericUpDown>());
            var otherNumericUpDown = GenerateStub<NumericUpDown>();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(otherNumericUpDown, adapter.WrappedControl);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(otherNumericUpDown);
            //---------------Test Result -----------------------
            Assert.IsFalse(@equals);
        }

        [Test]
        public void Test_Equals_Control_WhenOtherNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var adapter = new WinFormsNumericUpDownAdapter(GenerateStub<NumericUpDown>());
            NumericUpDown otherNumericUpDown = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(otherNumericUpDown);
            //---------------Execute Test ----------------------
            var @equals = adapter.Equals(otherNumericUpDown);
            //---------------Test Result -----------------------
            Assert.IsFalse(@equals);
        }

        private static T GenerateStub<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }
    }

}
// ReSharper restore InconsistentNaming