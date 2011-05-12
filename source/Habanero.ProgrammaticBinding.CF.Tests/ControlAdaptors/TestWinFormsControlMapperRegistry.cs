using System;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.CF;
using Habanero.ProgrammaticBinding.CF.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability.CF;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestWinFormsControlMapperRegistry
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.     

            ClassDef.ClassDefs.Add(typeof (FakeBo).MapClasses());
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

        [Test]
        public void Test_Construct_WithNullNamingConvention_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IControlNamingConvention namingConvention = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(namingConvention);
            //---------------Execute Test ----------------------
            try
            {
                new WinFormsControlMapperRegistry(namingConvention);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
            }
        }
        /*AssertWasNotCalled not supported by CF
                [Test]
                public void Test_Construct_WithNonDefaultNamingConvention_WhenComboBox_ShouldUseNonDefaultNamingConvention()
                {
                    //---------------Set up test pack-------------------
                    //-----Note_: the Naming convention is only needed in the registry to deal with
                    // ------comboboxes since they could be enum, Relationship or lookups so have to 
                    // ------get the property name to resolve.
                    var namingConvention = MockRepository.GenerateStub<IControlNamingConvention>();
                    var control = new ComboBox {Name = "cmbSomeName"};
                    namingConvention.Stub(nc => nc.GetPropName(control)).Return("SomeName");
                    var mapperRegistry = new WinFormsControlMapperRegistry(namingConvention);
                    //---------------Assert Precondition----------------
                    Assert.IsNotNull(namingConvention);
                    Assert.IsInstanceOf<ComboBox>(control);
                    namingConvention.AssertWasNotCalled(nc => nc.GetPropName(control));
                    //---------------Execute Test ----------------------
                    mapperRegistry.GetMapperType<FakeBo>(control);
                    //---------------Test Result -----------------------
                    namingConvention.AssertWasCalled(nc => nc.GetPropName(control));
                }*/
   /*CF Not yet ported
    * [Test]
        public void Test_GetMapperType_WhenControlTypeCheckBox_ShouldReturnCheckBoxMapper()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var checkBox = new CheckBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(checkBox);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(checkBox);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mapperType);
            Assert.AreEqual(typeof (CheckBoxMapper), mapperType, "Should be CheckBoxMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenControlTypeTextBox_ShouldReturnTextBoxMapper()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var textBox = new TextBox();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(textBox);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(textBox);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mapperType);
            Assert.AreEqual(typeof(TextBoxMapper), mapperType, "Should be TextBoxMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenControlTypeDateTimePicker_ShouldReturnDateTimePickerMapper()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var dateTimePicker = new DateTimePicker();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dateTimePicker);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(dateTimePicker);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mapperType);
            Assert.AreEqual(typeof(DateTimePickerMapper), mapperType, "Should be DateTimePickerMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenControlTypeDateTimePickerWin_ShouldReturnDateTimePickerMapper()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var dateTimePicker = new DateTimePickerWin(new ControlFactoryCF());
            //---------------Assert Precondition----------------
            Assert.IsNotNull(dateTimePicker);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(dateTimePicker);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mapperType);
            Assert.AreEqual(typeof(DateTimePickerMapper), mapperType, "Should be DateTimePickerMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenControlTypeComboBox_WhenPropTypeIsEnum_ShouldReturnEnumComboBoxMapper()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var comboBox = new ComboBox {Name = "cmbFakeEnumProp"};
            var classDef = ClassDef.Get<FakeBo>();
            var propDef = classDef.GetPropDef("FakeEnumProp", false);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(comboBox);
            Assert.IsTrue(propDef.PropertyType.ToTypeWrapper().IsEnumType());
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(comboBox);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(EnumComboBoxMapper), mapperType, "Should be EnumComboBoxMapper");
        }


        [Test]
        public void Test_GetMapperType_WhenComboBox_WhenPropTypeIsLookupList_ShouldReturnLookupComboBoxMapper()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(FakeBoWithLookupListProp.GetClassDef());
            var registry = CreateWinFormsControlMapperRegistry();
            var comboBox = new ComboBox { Name = "cmbLookupListProp" };
            var classDef = ClassDef.Get<FakeBoWithLookupListProp>();
            var propDef = classDef.GetPropDef("LookupListProp", false);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(comboBox);
            Assert.IsTrue(propDef.HasLookupList());
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBoWithLookupListProp>(comboBox);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(LookupComboBoxMapper), mapperType, "Should be LookupComboBoxMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenComboBox_WhenIsSingleRelationship_ShouldReturnAutoLoadingRelationshipComboBoxMapper()

        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            var comboBox = new ComboBox { Name = "cmbFakeBoRel" };
            var classDef = ClassDef.Get<FakeBoWithSingleRelationship>();
            var propDef = classDef.GetPropDef("FakeBoRel", false);
            var relationshipDef = classDef.GetRelationship("FakeBoRel");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(comboBox);
            Assert.IsNull(propDef, "This is a single relationship not a property");
            Assert.IsNotNull(relationshipDef, "This is a single relationship");
            Assert.IsInstanceOf<SingleRelationshipDef>(relationshipDef);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBoWithSingleRelationship>(comboBox);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(AutoLoadingRelationshipComboBoxMapper), mapperType, "Should be AutoLoadingRelationshipComboBoxMapper");
        }*/
        [Test]
        public void Test_GetMapperType_WhenControlTypeCheckBoxAndControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var registry = CreateWinFormsControlMapperRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                registry.GetMapperType<FakeBo>(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
            }
        }

        #region NumericUpDown
        [Test]
        public void Test_GetMapperType_WhenControlTypeNumericUpDown_WhenPropertyIsInteger_ShouldReturnNumericUpDownIntegerMapper()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "FakeIntProp";
            var registry = CreateWinFormsControlMapperRegistry();
            var numericUpDown = new NumericUpDown { Name = "nud" + propertyName };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(numericUpDown);
            Assert.AreEqual(typeof(int), GetPropDef(propertyName).PropertyType);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(numericUpDown);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(NumericUpDownIntegerMapper), mapperType, "Should be NumericUpDownIntegerMapper");
        }
        [Test]
        public void Test_GetMapperType_WhenControlTypeNumericUpDown_WhenPropertyIsLong_ShouldReturnNumericUpDownIntegerMapper()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "FakeLongProp";
            var registry = CreateWinFormsControlMapperRegistry();
            var numericUpDown = new NumericUpDown { Name = "nud" + propertyName };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(numericUpDown);
            Assert.AreEqual(typeof(long), GetPropDef(propertyName).PropertyType);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(numericUpDown);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(NumericUpDownIntegerMapper), mapperType, "Should be NumericUpDownIntegerMapper");
        }
        [Test]
        public void Test_GetMapperType_WhenControlTypeNumericUpDown_WhenPropertyIsDecimal_ShouldReturnNumericUpDownCurrencyMapper()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "DecimalProp";
            var registry = CreateWinFormsControlMapperRegistry();
            var numericUpDown = new NumericUpDown { Name = "nud" + propertyName };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(numericUpDown);
            Assert.AreEqual(typeof(decimal), GetPropDef(propertyName).PropertyType);
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(numericUpDown);
            //---------------Test Result -----------------------
            Assert.AreEqual(typeof(NumericUpDownCurrencyMapper), mapperType, "Should be NumericUpDownCurrencyMapper");
        }

        [Test]
        public void Test_GetMapperType_WhenNumericUpdown_WhenPropDoesNotExistOnBO_ShouldReturnNull()
        {
            string propertyName = GetRandomString();
            var registry = CreateWinFormsControlMapperRegistry();
            var numericUpDown = new NumericUpDown { Name = "nud" + propertyName };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(numericUpDown);
            Assert.IsNull( GetPropDef(propertyName), "Prop should not exist");
            //---------------Execute Test ----------------------
            var mapperType = registry.GetMapperType<FakeBo>(numericUpDown);
            //---------------Test Result -----------------------
            Assert.IsNull(mapperType);
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static IPropDef GetPropDef(string propertyName)
        {
            var classDef = ClassDef.Get<FakeBo>();
            return classDef.GetPropDef(propertyName, false);
        }

        #endregion

        private static WinFormsControlMapperRegistry CreateWinFormsControlMapperRegistry()
        {
            return new WinFormsControlMapperRegistry();
        }
    }
}// ReSharper restore InconsistentNaming