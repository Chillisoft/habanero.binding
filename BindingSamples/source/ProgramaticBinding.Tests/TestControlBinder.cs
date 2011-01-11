using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Habanero.Base;
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
    public class TestControlBinder
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
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            GlobalUIRegistry.ControlFactory = new ControlFactoryManualBindingWin();
        }
        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        #region AddTextboxMapper ForWin

        [Test]
        public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldCreateMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBoxWin>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, "FirstName");
            //---------------Test Result -----------------------
            Assert.IsNotNull(txtMapper);
        }

        [Test]
        public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldSetMappers_ControlAndPropName()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBoxWin>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(txtMapper);
            var controlWrapper = txtMapper.Control as ControlWrapperWinForms;
            Assert.IsNotNull(controlWrapper);
            Assert.AreSame(txtBox, controlWrapper.WrappedControl, "Should set TextBox");
            Assert.AreEqual(propName, txtMapper.PropertyName, "Should set PropName");
        }  

        [Test]
        public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldSetMappers_ReadOnlyFalse()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBoxWin>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsFalse(txtMapper.IsReadOnly, "IsReadOnly should Be False By Default");
        }  
        [Test]
        public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
        {
            //---------------Set up test pack-------------------
            var expectedControlFactory = GenerateStub<IControlFactory>();
            GlobalUIRegistry.ControlFactory = expectedControlFactory;
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBoxWin>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedControlFactory,  GlobalUIRegistry.ControlFactory);
            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedControlFactory, txtMapper.ControlFactory);
        }

        [Test]
        public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldAddToControlMappersCollection()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBoxWin>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            var addedMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(haberoControlBinder.ControlMappers.Contains(addedMapper), "Should contain added control mapper");
        }
        #endregion

        #region AddTextboxMapper

        [Test]
        public void Test_AddTextBoxMapper_ShouldCreateMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBox>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, "FirstName");
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(txtMapper);
            Assert.IsNotNull(txtMapper);
        }

        [Test]
        public void Test_AddTextBoxMapper_ShouldSetMappers_ControlAndPropName()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBox>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(txtMapper);
            var dtControlWrapper = txtMapper.Control as ControlWrapperWinForms;
            Assert.IsNotNull(dtControlWrapper);
            var wrappedControl = dtControlWrapper.WrappedControl;
            Assert.AreSame(txtBox, wrappedControl, "Should set TextBox");
            Assert.AreEqual(propName, txtMapper.PropertyName, "Should set PropName");
        }
        [Test]
        public void Test_AddTextBoxMapper_ShouldSetMappers_ReadOnlyFalse()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBox>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(txtMapper);
            Assert.IsFalse(txtMapper.IsReadOnly, "IsReadOnlyshouldBeFalseByDefault");
        }
        [Test]
        public void Test_AddTextBoxMapper_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
        {
            //---------------Set up test pack-------------------
            var expectedControlFactory = GenerateStub<IControlFactory>();
            GlobalUIRegistry.ControlFactory = expectedControlFactory;
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBox>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedControlFactory, GlobalUIRegistry.ControlFactory);
            //---------------Execute Test ----------------------
            var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(txtMapper);
            Assert.AreSame(expectedControlFactory, txtMapper.ControlFactory);
        }
        
        [Test]
        public void Test_AddTextBoxMapper_ShouldAddToControlMappersCollection()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var txtBox = GenerateStub<TextBox>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            var addedMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<TextBoxMapperWinForms>(addedMapper);
            Assert.IsTrue(haberoControlBinder.ControlMappers.Contains(addedMapper), "Should contain added control mapper");
        }
        #endregion

        #region DateTimePickerMapper

        [Test]
        public void Test_AddDateTimePicker_ShouldCreateMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var dtPicker = GenerateStub<DateTimePicker>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var dtpMapper = haberoControlBinder.AddDateTimePicker(dtPicker, "FirstName");
            //---------------Test Result -----------------------
            Assert.IsNotNull(dtpMapper);
        }

        [Test]
        public void Test_AddDateTimePicker_ShouldSetMappers_ControlAndPropName()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var dtPicker = GenerateStub<DateTimePicker>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var dtpMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
            //---------------Test Result -----------------------
            var dtControlWrapper = dtpMapper.Control as ControlWrapperWinForms;
            Assert.IsNotNull(dtControlWrapper);
            var wrappedControl = dtControlWrapper.WrappedControl;
            Assert.AreSame(dtPicker, wrappedControl, "Should set DateTimePicker");
            Assert.AreEqual(propName, dtpMapper.PropertyName, "Should set PropName");
        }
        [Test]
        public void Test_AddDateTimePicker_ShouldSetMappers_ReadOnlyFalse()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var dtPicker = GenerateStub<DateTimePicker>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var dtpMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
            //---------------Test Result -----------------------
            Assert.IsFalse(dtpMapper.IsReadOnly, "IsReadOnlyshouldBeFalseByDefault");
        }
        [Test]
        public void Test_AddDateTimePicker_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
        {
            //---------------Set up test pack-------------------
            var expectedControlFactory = GenerateStub<IControlFactory>();
            GlobalUIRegistry.ControlFactory = expectedControlFactory;
            var haberoControlBinder = new HabaneroControlBinder();
            var dtPicker = GenerateStub<DateTimePicker>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreSame(expectedControlFactory, GlobalUIRegistry.ControlFactory);
            //---------------Execute Test ----------------------
            var dtpMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedControlFactory, dtpMapper.ControlFactory);
        }

        [Test]
        public void Test_AddDateTimePicker_ShouldAddToControlMappersCollection()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();
            var dtPicker = GenerateStub<DateTimePicker>();
            var propName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
            //---------------Execute Test ----------------------
            var addedMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(haberoControlBinder.ControlMappers.Contains(addedMapper), "Should contain added control mapper");
        }

        #endregion

        [Test]
        public void Test_SetBusinessObject_ShouldSetBusinessObjectOnAllControlMappers()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();

            var dtpMapper = haberoControlBinder.AddDateTimePicker(GenerateStub<DateTimePicker>(), "FakeDateProp");
            var txtMapper = haberoControlBinder.AddTextBoxMapper(GenerateStub<TextBox>(), "FakeStringProp");
            var expectedBO = new FakeBo();
            //---------------Assert Precondition----------------
            Assert.IsNull(dtpMapper.BusinessObject, "The BusinessObject has not yet been set");
            Assert.IsNull(txtMapper.BusinessObject, "The BusinessObject has not yet been set");
            //---------------Execute Test ----------------------
            haberoControlBinder.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.IsNotNull(expectedBO);
            Assert.AreSame(expectedBO, dtpMapper.BusinessObject, "BO should be set on the Mapper");
            Assert.AreSame(expectedBO, txtMapper.BusinessObject, "BO should be set on the Mapper");
        }

        [Test]
        public void Test_SetBusinessObject_WhenOneMapper_ShouldSetBusinessObjectOnMapper()
        {
            //---------------Set up test pack-------------------
            var haberoControlBinder = new HabaneroControlBinder();

            var mapper = haberoControlBinder.AddDateTimePicker(GenerateStub<DateTimePicker>(), "FakeDateProp");
            var expectedBO = new FakeBo();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(expectedBO);
            Assert.IsNull(mapper.BusinessObject, "The BusinessObject has not yet been set");
            Assert.AreNotSame(expectedBO, mapper.BusinessObject);
            //---------------Execute Test ----------------------
            haberoControlBinder.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.IsNotNull(expectedBO);
            Assert.AreSame(expectedBO, haberoControlBinder.BusinessObject);
            Assert.AreSame(expectedBO, mapper.BusinessObject, "BO should be set on the Mapper");
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

    public class FakeBo:BusinessObject<FakeBo>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string FakeStringProp
        {
            get { return ((string) (base.GetPropertyValue("FakeStringProp"))); }
            set { base.SetPropertyValue("FakeStringProp", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? FakeDateProp
        {
            get { return ((DateTime?) (base.GetPropertyValue("FakeDateProp"))); }
            set { base.SetPropertyValue("FakeDateProp", value); }
        }
    }
}
