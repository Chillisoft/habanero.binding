using System;
using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces;
using Habanero.Faces.Adapters;
using Habanero.Faces.Controls;
using Habanero.ProgrammaticBinding;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestControlAdaptorFactory
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
        }
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed

        }
        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }
/*CF Not yet ported
        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeListBox_ShouldReturnWinFormsListBoxAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var listBox = new ListBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(listBox);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(listBox);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsListBoxAdapter>(control);
        }

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeCheckBox_ShouldReturnWinFormCheckBoxAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var checkBox = new CheckBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(checkBox);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(checkBox);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsCheckBoxAdapter>(control);
        }*/

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeTextBox_ShouldReturnWinFormTextBoxAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var textBox = new TextBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(textBox);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(textBox);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsTextBoxAdapter>(control);
        }
/*CF not yet ported
        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeDateTimePicker_ShouldReturnWinFormDateTimePickerAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var dateTimePicker = new DateTimePicker();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dateTimePicker);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(dateTimePicker);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsDateTimePickerAdapter>(control);
        }

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeComboBox_ShouldReturnWinFormComboBoxPickerAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var comboBox = new ComboBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(comboBox);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(comboBox);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsComboBoxAdapter>(control);
        }
        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeNumericUpDown_ShouldReturnWinFormNumericUpDownAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var numericUpDown = new NumericUpDown();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(numericUpDown);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(numericUpDown);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsNumericUpDownAdapter>(control);
        }
        [Test]
        public void Test_GetHabaneroControl_WhenControlDataGridView_ShouldReturnWinFormDataGridViewAdapter()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            var dataGridView = new DataGridView();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dataGridView);
            //---------------Execute Test ----------------------
            var control = factory.GetHabaneroControl(dataGridView);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<WinFormsDataGridViewAdapter>(control);
        }

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeListBoxAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                factory.GetHabaneroControl<ListBox>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("control", ex.ParamName);
            }
        }

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeCheckBoxAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
               factory.GetHabaneroControl<CheckBox>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("control", ex.ParamName);
            }
        }*/

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeTextBoxAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                factory.GetHabaneroControl<TextBox>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
            }
        }
/*CF Not yet ported
        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeDateTimePickerAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                factory.GetHabaneroControl<DateTimePicker>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("control", ex.ParamName);
            }
        }

        [Test]
        public void Test_GetHabaneroControl_WhenControlTypeComboBoxAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = CreateWinFormsControlAdaptorFactory();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                factory.GetHabaneroControl<ComboBox>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("control", ex.ParamName);
            }
        }*/

        private static WinFormsControlAdaptorFactory CreateWinFormsControlAdaptorFactory()
        {
            return new WinFormsControlAdaptorFactory();
        }

    }
}// ReSharper restore InconsistentNaming