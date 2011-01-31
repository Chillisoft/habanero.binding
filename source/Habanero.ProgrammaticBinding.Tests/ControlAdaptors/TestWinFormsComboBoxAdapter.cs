using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;
// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests.ControlAdaptors
{
    [TestFixture]
    public class TestWinFormsComboBoxAdapter
    {
/*        private static IControlFactory GetControlFactory()
        {
            return new ControlFactoryWin();
        }*/
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
        public void Test_Constuct_ShouldSetWrappedControl()
        {
            //---------------Set up test pack-------------------
            var expectedWrappedControl = GenerateStub<ComboBox>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var adapter = new WinFormsComboBoxAdapter(expectedWrappedControl);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedWrappedControl, adapter.WrappedControl);
        }
/* 

        [Test]
        public void Test_SetSelectedValue_SetsSelectedValueOnWrappedComboBox()
        {
            //---------------Set up test pack-------------------
            var comboBox = GenerateStub<ComboBox>();
            var adapter = new WinFormsEditableGridAdapter(comboBox);

            //---------------Assert Precondition----------------
            Assert.AreSame(comboBox, adapter.WrappedControl);
            Assert.IsNull(comboBox.SelectedValue);
            //---------------Execute Test ----------------------
            var expectedSelectedValue = new object();
            adapter.SelectedValue = expectedSelectedValue;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedSelectedValue, adapter.SelectedValue);
            Assert.AreSame(expectedSelectedValue, comboBox.SelectedValue);
        }
        [Test]
        public void Test_SelectedIndexChanged_OnAdaptedControl_ShouldRaiseEventOnAdapter()
        {
            //---------------Set up test pack-------------------
            var comboBox = new ComboBox();
            comboBox.Items.Add(new object());
            comboBox.Items.Add(new object());
            comboBox.Items.Add(new object());
            var adapter = new WinFormsEditableGridAdapter(comboBox);
            bool selectedIndexChangedCalled = false;
            adapter.SelectedIndexChanged += (sender, args) => selectedIndexChangedCalled = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(comboBox, adapter.WrappedControl);
            Assert.AreNotEqual(1, comboBox.SelectedIndex);
            Assert.IsFalse(selectedIndexChangedCalled);
            //---------------Execute Test ----------------------
            adapter.SelectedIndex = 1;
            //---------------Test Result -----------------------
            Assert.AreEqual(1, adapter.SelectedIndex);
            Assert.AreEqual(1, comboBox.SelectedIndex);
            Assert.IsTrue(selectedIndexChangedCalled, "Should have raised adaptors event");
        }

        [Test]
        public void Test_SelectedValueChanged_OnAdaptedControl_ShouldRaiseEventOnAdapter()
        {
            //---------------Set up test pack-------------------
            var comboBox = new ComboBox();
            comboBox.Items.Add(new object());
            comboBox.Items.Add(new object());
            comboBox.Items.Add(new object());
            var adapter = new WinFormsEditableGridAdapter(comboBox);
            bool selectedValueChangedCalled = false;
            adapter.SelectedValueChanged += (sender, args) => selectedValueChangedCalled = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(comboBox, adapter.WrappedControl);
            Assert.AreNotEqual(1, comboBox.SelectedIndex);
            Assert.IsFalse(selectedValueChangedCalled);
            //---------------Execute Test ----------------------
            adapter.SelectedIndex = 1;//Change the selected idex changes the selected value.
            //---------------Test Result -----------------------
            Assert.AreEqual(1, adapter.SelectedIndex);
            Assert.AreEqual(1, comboBox.SelectedIndex);
            Assert.IsTrue(selectedValueChangedCalled, "Should have raised adaptors event");
        }
*/



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