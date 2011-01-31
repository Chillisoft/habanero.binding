using System;
using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgramaticBinding;
using Habanero.ProgramaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.ProgramaticBinding.Tests
{
    [TestFixture]
    public class TestWinFormsDateTimePickerAdapter
    {
        private static IControlFactory GetControlFactory()
        {
            return new ControlFactoryWin();
        }
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
            var mapper = new DateTimePickerMapper(new WinFormsDateTimePickerAdapter(GenerateStub<DateTimePicker>()), "FakeDateProp", false, GetControlFactory());
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
        public void Test_ValueChanged_OnAdaptedControl_ShouldRaiseEventOnAdapter()
        {
            //---------------Set up test pack-------------------
            var dtp = new DateTimePicker();
            var adapter = new WinFormsDateTimePickerAdapter(dtp);
            bool valueChangedCalled = false;
            adapter.ValueChanged += (sender, args) => valueChangedCalled = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(dtp, adapter.WrappedControl);
            Assert.IsFalse(valueChangedCalled);
            //---------------Execute Test ----------------------
            dtp.Value = DateTime.Today.AddDays(44);
            //---------------Test Result -----------------------
            Assert.IsTrue(valueChangedCalled);
        }
        [Ignore("I dont know how to force teh Enter event to fire on the dtpicker")]
        [Test]
        public void Test_Enter_OnAdaptedControl_ShouldRaiseEventOnAdapter()
        {
            //---------------Set up test pack-------------------
            var dtp = new DateTimePicker();
            var adapter = new WinFormsDateTimePickerAdapter(dtp);
            bool enterCalled = false;
            adapter.Enter += (sender, args) => enterCalled = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(dtp, adapter.WrappedControl);
            Assert.IsFalse(enterCalled);
            //---------------Execute Test ----------------------
            dtp.Select();
            //---------------Test Result -----------------------
            Assert.IsTrue(enterCalled);
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