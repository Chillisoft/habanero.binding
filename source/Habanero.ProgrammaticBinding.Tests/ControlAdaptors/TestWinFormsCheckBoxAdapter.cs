using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgrammaticBinding;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;
using System.Drawing;

namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestWinFormsCheckBoxAdapter
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
        public void Test_SetChecked_SetsCheckedOnWrappedCheckBox()
        {
            //---------------Set up test pack-------------------
            var checkBox = GenerateStub<CheckBox>();
            var adapter = new WinFormsCheckBoxAdapter(checkBox);

            //---------------Assert Precondition----------------
            Assert.AreSame(checkBox, adapter.WrappedControl);
            Assert.IsFalse(checkBox.Checked);
            //---------------Execute Test ----------------------
            adapter.Checked = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(checkBox.Checked);
        }
        [Test]
        public void Test_SetCheckedWrappedCheckBox_SetsCheckedOnAdapter()
        {
            //---------------Set up test pack-------------------
            var checkBox = GenerateStub<CheckBox>();
            var adapter = new WinFormsCheckBoxAdapter(checkBox);

            //---------------Assert Precondition----------------
            Assert.AreSame(checkBox, adapter.WrappedControl);
            Assert.IsFalse(checkBox.Checked);
            //---------------Execute Test ----------------------
            checkBox.Checked = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(adapter.Checked);
        }

        [Test]
        public void Test_SetCheckedAlign_SetsCheckedOnWrappedCheckBox()
        {
            //---------------Set up test pack-------------------
            var checkBox = new CheckBox();
            var adapter = new WinFormsCheckBoxAdapter(checkBox);
            checkBox.CheckAlign = ContentAlignment.BottomRight;
            //---------------Assert Precondition----------------
            Assert.AreSame(checkBox, adapter.WrappedControl);
            Assert.AreEqual(ContentAlignment.BottomRight, checkBox.CheckAlign);
            Assert.AreEqual(ContentAlignment.BottomRight, adapter.CheckAlign);
            //---------------Execute Test ----------------------
            adapter.CheckAlign = ContentAlignment.TopLeft;
            //---------------Test Result -----------------------
            Assert.AreEqual(ContentAlignment.TopLeft, checkBox.CheckAlign);
            Assert.AreEqual(ContentAlignment.TopLeft, adapter.CheckAlign);
        }
        [Test]
        public void Test_CheckedChanged_OnAdaptedControl_ShouldRaiseEventOnAdapter()
        {
            //---------------Set up test pack-------------------
            var checkBox = new CheckBox();
            var adapter = new WinFormsCheckBoxAdapter(checkBox);
            bool checkChangedCalled = false;
            adapter.CheckedChanged += (sender, args) => checkChangedCalled = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(checkBox, adapter.WrappedControl);
            Assert.IsFalse(checkBox.Checked);
            Assert.IsFalse(checkChangedCalled);
            //---------------Execute Test ----------------------
            adapter.Checked = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(checkChangedCalled);
            Assert.IsTrue(checkBox.Checked);
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