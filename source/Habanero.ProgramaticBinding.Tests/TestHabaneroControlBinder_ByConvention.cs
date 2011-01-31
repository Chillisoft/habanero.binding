using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgramaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgramaticBinding.Tests
{
    [TestFixture]
    public class TestHabaneroControlBinder_ByConvention
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalUIRegistry.ControlFactory = new ControlFactoryWin();
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
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

        #region Acceptance Tests

        [Test]
        public void TestAccept_AddMappersByConventions_WithMultipleControls_ShouldAddAllControls()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox { Name = "txtFakeStringProp" };
            var controls = CreateControlCollection();
            controls.Add(textBox);
            var label = new Label { Name = "lblFakeStringProp" };
            controls.Add(label);
            var checkBox = new CheckBox { Name = "chkFakeBoolProp" };
            controls.Add(checkBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, controls.Count, "Should have both controls");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(2, haberoControlBinder.ControlMappers.Count, "Should not create mapper for the label");
            //---------------First Control ------------------------------------
            var textBoxMapper = haberoControlBinder.ControlMappers[0];
            var textBoxAdapter = textBoxMapper.Control as IWinFormsControlAdapter;
            Assert.IsNotNull(textBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(textBox, textBoxAdapter.WrappedControl);
            Assert.AreEqual("FakeStringProp", textBoxMapper.PropertyName, "The prop name should be derived from the control.Name");

            //---------------Second Control ------------------------------------
            var checkBoxMapper = haberoControlBinder.ControlMappers[1];
            var checkBoxAdapter = checkBoxMapper.Control as IWinFormsControlAdapter;
            Assert.IsNotNull(checkBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(checkBox, checkBoxAdapter.WrappedControl);
            Assert.AreEqual("FakeBoolProp", checkBoxMapper.PropertyName, "The prop name should be derived from the control.Name");
        }

        #endregion


        #region Add Mappers by Convetion without labels

        [Test]
        public void Test_AddMappersByConventions_WhenNull_ShouldRaiseError()
        {
            var haberoControlBinder = CreateHabaneroControlBinder();
            IList<Control> controls = null;
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            try
            {
                haberoControlBinder.AddMappersByConvention(controls);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("controls", ex.ParamName);
            }
        }

        [Test]
        public void Test_AddMappersByConventions_WhenEmptyCollection_ShouldDoNothing()
        {
            var haberoControlBinder = CreateHabaneroControlBinder();
            var controls = CreateControlCollection();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, controls.Count);
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
        }

        [Test]
        public void Test_AddMappersByConventions_WhenOneItem_WhenUsesConvention_ShouldCreateMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox {Name = "txtFakeStringProp"};
            var controls = CreateControlCollection();
            controls.Add(textBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have textbox");
            Assert.Contains(textBox, controls, "Should have textbox");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
        }
        [Test]
        public void Test_AddMappersByConventions_WhenTextBox_WhenUsesConvention_ShouldAddTextBoxMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox {Name = "txtFakeStringProp"};
            var controls = CreateControlCollection();
            controls.Add(textBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have textbox");
            Assert.Contains(textBox, controls, "Should have textbox");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var textBoxMapper = haberoControlBinder.ControlMappers[0];
            Assert.IsInstanceOf<TextBoxMapper>(textBoxMapper);
        }
         
        [Test]
        public void Test_AddMappersByConventions_WhenTextBox_WhenUsesConvention_ShouldAddTextBoxMapper_WithCorrectControlAndProperty()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox {Name = "txtFakeStringProp"};
            var controls = CreateControlCollection();
            controls.Add(textBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have textbox");
            Assert.Contains(textBox, controls, "Should have textbox");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var textBoxMapper = haberoControlBinder.ControlMappers[0];
            var textBoxAdapter = textBoxMapper.Control as IWinFormsTextBoxAdapter;
            Assert.IsNotNull(textBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(textBox, textBoxAdapter.WrappedControl);
            Assert.AreEqual("FakeStringProp", textBoxMapper.PropertyName, "The prop name should be derived from the control.Name");
        }

        [Test]
        public void Test_AddMappersByConventions_WhenCheckBox_WhenUsesConvention_ShouldAddCheckBoxMapper_WithCorrectControlAndProperty()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = CreateHabaneroControlBinder();
            var checkBox = new CheckBox { Name = "chkFakeBoolProp" };
            Control.ControlCollection controls = GetControlsCollection(checkBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have CheckBox");
            Assert.Contains(checkBox, controls, "Should have CheckBox");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var checkBoxMapper = haberoControlBinder.ControlMappers[0];
            var checkBoxAdapter = checkBoxMapper.Control as IWinFormsCheckBoxAdapter;
            Assert.IsNotNull(checkBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(checkBox, checkBoxAdapter.WrappedControl);
            Assert.AreEqual("FakeBoolProp", checkBoxMapper.PropertyName, "The prop name should be derived from the control.Name");
        }

        [Test]
        public void Test_AddMappersByConventions_WhenComboBox_WhenUsesConvention_ShouldAddLookupComboBoxMapper_WithCorrectControlAndProperty()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(FakeBoWithLookupListProp.GetClassDef());
            var haberoControlBinder = new HabaneroControlBinder<FakeBoWithLookupListProp>();
            var comboBox = new ComboBox { Name = "cmbLookupListProp" };
            Control.ControlCollection controls = GetControlsCollection(comboBox);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have ComboBox");
            Assert.Contains(comboBox, controls, "Should have ComboBox");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var comboBoxMapper = haberoControlBinder.ControlMappers[0];
            var comboBoxAdapter = comboBoxMapper.Control as IWinFormsComboBoxAdapter;
            Assert.IsNotNull(comboBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(comboBox, comboBoxAdapter.WrappedControl);
            Assert.AreEqual("LookupListProp", comboBoxMapper.PropertyName, "The prop name should be derived from the control.Name");
        }


        [Test]
        public void Test_AddMappersByConventions_WhenHasLabel_WithLabelsFalse_ShouldNotMapLabel()
        {
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox { Name = "txtFakeStringProp" };
            var controls = CreateControlCollection();
            controls.Add(textBox);
            var label = new Label { Name = "lblFakeStringProp" };
            controls.Add(label);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, controls.Count, "Should have both controls");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var textBoxMapper = haberoControlBinder.ControlMappers[0];
            var textBoxAdapter = textBoxMapper.Control as IWinFormsControlAdapter;
            Assert.IsNotNull(textBoxAdapter, "Should adapt the winforms control");
            Assert.AreSame(textBox, textBoxAdapter.WrappedControl);
            Assert.AreEqual("FakeStringProp", textBoxMapper.PropertyName, "The prop name should be derived from the control.Name");
        }


        [Test]
        public void Test_AddMappersByConventions_WhenOneControlAlreadyMapped_ShouldNotMapMappeControl()
        {
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox { Name = "txtFakeStringProp" };
            var controls = CreateControlCollection();
            controls.Add(textBox);
            haberoControlBinder.Add<TextBoxMapperStub, TextBox> (textBox, bo => bo.FakeStringProp);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have controls");
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var textBoxMapper = haberoControlBinder.ControlMappers[0];
            Assert.IsInstanceOf<TextBoxMapperStub>(textBoxMapper, "The original mapper used should still be used");
        }

        //NumericUpDownIntegerMapper
        [Test]
        public void Test_NumericUpdownMapper_WhenPropIsInteger_ShouldCreateNumericUpDownIntegerMapper()
        {
            var haberoControlBinder = CreateHabaneroControlBinder();
            var numericUpDown = new NumericUpDown { Name = "nudFakeIntProp" };
            var controls = CreateControlCollection();
            controls.Add(numericUpDown);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, controls.Count, "Should have control");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            Assert.AreEqual(typeof(int), GetPropDef("FakeIntProp").PropertyType);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>());
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            var numericUpdownMapper = haberoControlBinder.ControlMappers[0];
            Assert.IsInstanceOf<NumericUpDownIntegerMapper>(numericUpdownMapper, "The original mapper used should still be used");
        }

        #endregion

        #region Add Mappers by Convetion with labels

        [Test]
        public void Test_AddMappersByConventions_WithLabelsTrue_WhenHasLabel_ShouldMapLabel()
        {
            const string compulsoryPropSuffix = " *";
            const string propName = "CompIntProp";
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox { Name = "txt" + propName };
            var controls = CreateControlCollection();
            controls.Add(textBox);
            var label = new Label {Name = "lbl" + propName, Text = propName};
            controls.Add(label);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, controls.Count, "Should have both controls");
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            Assert.AreEqual(propName, label.Text);
            Assert.IsTrue(GetPropDef(propName).Compulsory);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>(), true);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            Assert.AreEqual(propName + compulsoryPropSuffix, label.Text, "Should have updated label as with compulsory std");
            Assert.IsTrue(label.Font.Bold, "Should be bold");
        }
        [Test]
        public void Test_AddMappersByConventions_WithLabelsTrue_WhenHasLabel_WhenControlAlreadyMapped_ShouldNotMapLabelTwice_FixBug1489()
        {
            const string compulsoryPropSuffix = " *";
            const string propName = "CompIntProp";
            var haberoControlBinder = CreateHabaneroControlBinder();
            var textBox = new TextBox { Name = "txt" + propName };
            var controls = CreateControlCollection();
            controls.Add(textBox);
            var label = new Label {Name = "lbl" + propName, Text = propName};
            controls.Add(label);
            haberoControlBinder.Add<TextBoxMapper, TextBox>(label, textBox, propName);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, controls.Count, "Should have both controls");
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            Assert.AreEqual(propName + compulsoryPropSuffix, label.Text);
            Assert.IsTrue(GetPropDef(propName).Compulsory);
            //---------------Execute Test ----------------------
            haberoControlBinder.AddMappersByConvention(controls.Cast<Control>(), true);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, haberoControlBinder.ControlMappers.Count);
            Assert.AreEqual(propName + compulsoryPropSuffix, label.Text, "Should have updated label as with compulsory std");
        }

        private static IPropDef GetPropDef(string propName)
        {
            var classDef = ClassDef.Get<FakeBo>();
            return classDef.GetPropDef(propName);
        }
        #endregion

        private static Control.ControlCollection GetControlsCollection(Control checkBox)
        {
            var controls = CreateControlCollection();
            controls.Add(checkBox);
            return controls;
        }

        private static Control.ControlCollection CreateControlCollection()
        {
            return new Control.ControlCollection(new Control());
        }


        private static HabaneroControlBinder<FakeBo> CreateHabaneroControlBinder()
        {
            return new HabaneroControlBinder<FakeBo>();
        }
    }
    internal class TextBoxMapperStub: TextBoxMapper{
        public TextBoxMapperStub(ITextBox tb, string propName, bool isReadOnly, IControlFactory factory) : base(tb, propName, isReadOnly, factory)
        {
        }
    }
}// ReSharper restore InconsistentNaming