using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.Faces.Base;
using Habanero.Faces.CF;
using Habanero.ProgrammaticBinding;
using Habanero.ProgrammaticBinding.CF;
using Habanero.ProgrammaticBinding.CF.ControlAdaptors;
using NUnit.Framework;
using Rhino.Mocks;
// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
	[TestFixture]
	public class TestHabaneroControlBinder
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

		#region Constuctor

		[Test]
		public void Test_Construct_SHouldSetControlMappers()
		{
			//---------------Set up test pack-------------------
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var controlBinder = new HabaneroControlBinder<FakeBo>();
			//---------------Test Result -----------------------
			Assert.IsNotNull(controlBinder.ControlMappers);
		}
		
		[Test]
		public void Test_Construct_WithNamingConvention_ShouldSetUpControlMappers()
		{
			//---------------Set up test pack-------------------
			var namingConvention = GenerateStub<IControlNamingConvention>();
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var controlBinder = new HabaneroControlBinder<FakeBo>(namingConvention);
			//---------------Test Result -----------------------
			Assert.IsNotNull(controlBinder.ControlMappers);
		}
		[Test]
		public void Test_Construct_WithNamingConvention_ShouldSetConvention()
		{
			//---------------Set up test pack-------------------
			var namingConvention = GenerateStub<IControlNamingConvention>();
			//---------------Assert Precondition----------------
			//---------------Execute Test ----------------------
			var controlBinderSpy = new HabaneroControlBinderSpy<FakeBo>(namingConvention);
			//---------------Test Result -----------------------
			Assert.AreSame(namingConvention, controlBinderSpy.GetNamingConvention());
		}

		#endregion


		#region AddTextboxMapper HabaneroForWin

		[Test]
		public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldCreateMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(txtMapper);
			var controlWrapper = txtMapper.Control as WinFormsControlAdapter;
			Assert.IsNull(controlWrapper, "Since we are using a TextBox the control does not need adapter");
			Assert.AreSame(txtBox, txtMapper.Control, "Should set TextBox");
			Assert.AreEqual(propName, txtMapper.PropertyName, "Should set PropName");
		}  

		[Test]
		public void Test_AddTextBoxMapper_WithHabaneroTB_ShouldSetMappers_ReadOnlyFalse()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
			//---------------Execute Test ----------------------
			var addedMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(addedMapper, controlMapper, "Should have added the newly created control mapper");
		}
		#endregion

		#region AddTextboxMapper

		[Test]
		public void Test_AddTextBoxMapper_ShouldCreateMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, "FirstName");
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(txtMapper);
			Assert.IsNotNull(txtMapper);
		}

		[Test]
		public void Test_AddTextBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(txtMapper);
			var dtControlWrapper = txtMapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(txtBox, wrappedControl, "Should set TextBox");
			Assert.AreEqual(propName, txtMapper.PropertyName, "Should set PropName");
		}
		[Test]
		public void Test_AddTextBoxMapper_ShouldSetMappers_ReadOnlyFalse()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(txtMapper);
			Assert.IsFalse(txtMapper.IsReadOnly, "IsReadOnlyshouldBeFalseByDefault");
		}
		[Test]
		public void Test_AddTextBoxMapper_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
		{
			//---------------Set up test pack-------------------
			var expectedControlFactory = GenerateStub<IControlFactory>();
			GlobalUIRegistry.ControlFactory = expectedControlFactory;
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreSame(expectedControlFactory, GlobalUIRegistry.ControlFactory);
			//---------------Execute Test ----------------------
			var txtMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(txtMapper);
			Assert.AreSame(expectedControlFactory, txtMapper.ControlFactory);
		}
		
		[Test]
		public void Test_AddTextBoxMapper_ShouldAddToControlMappersCollection()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var txtBox = GenerateStub<TextBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
			//---------------Execute Test ----------------------
			var addedMapper = haberoControlBinder.AddTextBoxMapper(txtBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<TextBoxMapper>(addedMapper);

			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(addedMapper, controlMapper, "Should have added the newly created control mapper");
		}
		#endregion

		#region DateTimePickerMapper

		[Test]
		public void Test_AddDateTimePicker_ShouldCreateMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
			var dtPicker = GenerateStub<DateTimePicker>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var dtpMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
			//---------------Test Result -----------------------
			var dtControlWrapper = dtpMapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(dtPicker, wrappedControl, "Should set DateTimePicker");
			Assert.AreEqual(propName, dtpMapper.PropertyName, "Should set PropName");
		}
		[Test]
		public void Test_AddDateTimePicker_ShouldSetMappers_ReadOnlyFalse()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
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
			var haberoControlBinder = CreateHabaneroControlBinder();
			var dtPicker = GenerateStub<DateTimePicker>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
			//---------------Execute Test ----------------------
			var addedMapper = haberoControlBinder.AddDateTimePicker(dtPicker, propName);
			//---------------Test Result -----------------------
			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(addedMapper, controlMapper, "Should have added the newly created control mapper");
		}


		#endregion


/*CF Not yet ported
		#region CheckBoxMapper

		[Test]
		public void Test_AddCheckBoxMapper_ShouldCreateMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBox>();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddCheckBoxMapper(chkBox, "FirstName");
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(mapper);
			Assert.IsNotNull(mapper);
		}

		[Test]
		public void Test_AddCheckBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddCheckBoxMapper(chkBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(chkBox, wrappedControl, "Should set CheckBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}


		[Test]
		public void Test_AddCheckBoxMapper_WithHabaneroCheckBox_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBoxWin>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddCheckBoxMapper(chkBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper, "Since we are using a CheckBoxWin the control does not need adapter");
			Assert.AreSame(chkBox, mapper.Control, "Should set CheckBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}  
		[Test]
		public void Test_AddCheckBoxMapper_ShouldSetMappers_ReadOnlyFalse()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddCheckBoxMapper(chkBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(mapper);
			Assert.IsFalse(mapper.IsReadOnly, "IsReadOnlyshouldBeFalseByDefault");
		}
		[Test]
		public void Test_AddCheckBoxMapper_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
		{
			//---------------Set up test pack-------------------
			var expectedControlFactory = GenerateStub<IControlFactory>();
			GlobalUIRegistry.ControlFactory = expectedControlFactory;
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreSame(expectedControlFactory, GlobalUIRegistry.ControlFactory);
			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddCheckBoxMapper(chkBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(mapper);

			Assert.AreSame(expectedControlFactory, mapper.ControlFactory);
		}

		[Test]
		public void Test_AddCheckBoxMapper_ShouldAddToControlMappersCollection()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var chkBox = GenerateStub<CheckBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
			//---------------Execute Test ----------------------
			var addedMapper = haberoControlBinder.AddCheckBoxMapper(chkBox, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<CheckBoxMapper>(addedMapper);
			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(addedMapper, controlMapper, "Should have added the newly created control mapper");
		}
		#endregion



		#region EnumComboBoxMapper

		[Test]
		public void Test_AddEnumComboBoxMapper_ShouldCreateMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, "FirstName");
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<ComboBoxMapper>(mapper);
			Assert.IsNotNull(mapper);
		}

		[Test]
		public void Test_AddEnumComboBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<EnumComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(cmb, wrappedControl, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}


		[Test]
		public void Test_AddEnumComboBoxMapper_WithHabaneroComboBox_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBoxWin>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<EnumComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper, "Since we are using a ComboBoxWin the control does not need adapter");
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}
		[Test]

		public void Test_AddEnumComboBoxMapper_ShouldSetMappers_ReadOnlyFalse()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<EnumComboBoxMapper>(mapper);
			Assert.IsFalse(mapper.IsReadOnly, "IsReadOnlyshouldBeFalseByDefault");
		}
		[Test]
		public void Test_AddEnumComboBoxMapper_ShouldSetMappers_ControlfactoryTo_GlobalUIRegistry_ControlFactory()
		{
			//---------------Set up test pack-------------------
			var expectedControlFactory = GenerateStub<IControlFactory>();
			GlobalUIRegistry.ControlFactory = expectedControlFactory;
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreSame(expectedControlFactory, GlobalUIRegistry.ControlFactory);
			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<EnumComboBoxMapper>(mapper);
			Assert.AreSame(expectedControlFactory, mapper.ControlFactory);
		}

		[Test]
		public void Test_AddEnumComboBoxMapper_ShouldAddToControlMappersCollection()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------
			Assert.AreEqual(0, haberoControlBinder.ControlMappers.Count);
			//---------------Execute Test ----------------------
			var addedMapper = haberoControlBinder.AddEnumComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<ComboBoxMapper>(addedMapper);
			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(addedMapper, controlMapper, "Should have added the newly created control mapper");
		}
		#endregion

		#region LookupComboBoxMapper

		[Test]
		public void Test_AddLookupComboBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddLookupComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<LookupComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(cmb, wrappedControl, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}
		[Test]
		public void Test_AddLookupComboBoxMapper_WithHabaneroControl_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBoxWin>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddLookupComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<LookupComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper);
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}

		#endregion


		#region ListComboBoxMapper

		[Test]
		public void Test_AddListComboBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBox();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddListComboBoxMapper(cmb, propName, "");
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<ListComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(cmb, wrappedControl, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}
		[Test]
		public void Test_AddListComboBoxMapper_WithHabaneroControl_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBoxWin();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddListComboBoxMapper(cmb, propName, "");
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<ListComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper);
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}

		[Test]
		public void Test_AddListComboBoxMapper_ShouldSetComboItemsToList()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBox();
			var propName = GetRandomString();
			const string list = "|Mr|Mrs|Miss";
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			haberoControlBinder.AddListComboBoxMapper(cmb, propName, list);
			//---------------Test Result -----------------------
			Assert.AreEqual(4, cmb.Items.Count);
		}

		[Test]
		public void Test_AddListComboBoxMapper_ShouldAddMapperToMapperCollection()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBox();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddListComboBoxMapper(cmb, propName, "");
			//---------------Test Result -----------------------
			var controlMapper = haberoControlBinder.ControlMappers[propName];
			Assert.AreSame(mapper, controlMapper, "Should have added the newly created control mapper");
		}
		#endregion

		#region RelationshipComboBoxMapper

		[Test]
		public void Test_AddRelationshipComboBoxMapper_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBox>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddAutoLoadingRelationshipComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<AutoLoadingRelationshipComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper);
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(cmb, wrappedControl, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}

		[Test]
		public void Test_AddRelationshipComboBoxMapper_WithHabaneroControl_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = GenerateStub<ComboBoxWin>();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.AddAutoLoadingRelationshipComboBoxMapper(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<AutoLoadingRelationshipComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper);
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}

		#endregion
        */
        #region AddGenericMethod
        /*CF Not yet ported
        [Test]
		public void Test_AddGeneric_WithWinFormsControl_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBox();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<AutoLoadingRelationshipComboBoxMapper, ComboBox>(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<AutoLoadingRelationshipComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper, "Since this is a WinForms Control it needs an adapter to be used by Habanero");
			var wrappedControl = dtControlWrapper.WrappedControl;
			Assert.AreSame(cmb, wrappedControl, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}
		[Test]
		public void Test_AddGeneric_WithHabaneroControl_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBoxWin();
			var propName = GetRandomString();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<AutoLoadingRelationshipComboBoxMapper, ComboBox>(cmb, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<AutoLoadingRelationshipComboBoxMapper>(mapper);
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}

		[Test]
		public void Test_AddGeneric_WithHabaneroControl_UsingLambda_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBoxWin();
			const string propName = "FakeStringProp";
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<AutoLoadingRelationshipComboBoxMapper, ComboBox>(cmb, x => x.FakeStringProp);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<AutoLoadingRelationshipComboBoxMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNull(dtControlWrapper, "If the control being placed on the form is a Habanero Control (i.e. ComboBoxWin) then it will not have an adapter");
			Assert.AreSame(cmb, mapper.Control, "Should set ComboBox");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
			Assert.IsFalse(mapper.IsReadOnly);
		}
		[Test]
		public void Test_AddGeneric_WithHabaneroControl_UsingLambda_WithReadOnlyTrue_ShouldSetMapperReadOnly()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBoxWin();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<AutoLoadingRelationshipComboBoxMapper, ComboBox>(cmb, x => x.FakeStringProp, true);
			//---------------Test Result -----------------------
			Assert.IsTrue(mapper.IsReadOnly);
		}

		[Test]
		public void Test_AddGeneric_WithLabel_UsingLambda_WithReadOnlyTrue_ShouldSetMapperReadOnly()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var cmb = new ComboBoxWin();
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<AutoLoadingRelationshipComboBoxMapper, ComboBox>(new Label(),  cmb, x => x.FakeStringProp, true);
			//---------------Test Result -----------------------
			Assert.IsTrue(mapper.IsReadOnly);
		}

		[Test]
		public void Test_AddGeneric_NumericUpDown_UsingLambda_ShouldSetMappers_ControlAndPropName()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var numericUpDown = new NumericUpDown();
			const string propName = "FakeIntProp";
			//---------------Assert Precondition----------------

			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<NumericUpDownIntegerMapper, NumericUpDown>(numericUpDown, x => x.FakeIntProp);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<NumericUpDownMapper>(mapper);
			Assert.IsInstanceOf<NumericUpDownIntegerMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper,"Since this is a WinForms Control it needs an adapter to be used by Habanero");
			Assert.AreSame(numericUpDown, dtControlWrapper.WrappedControl, "Should set NumericUpDown");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");
		}
        */
		#endregion

		#region AddReadUpdateControl With Label
        /*CF Not yet ported
		[Test]
		public void TestAccept_AddGeneric_WithLabel_WithCompulsory_UsingLambda_ShouldSetMappersControlAndPropName_ShouldSetLabelCompIndicator()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var numericUpDown = new NumericUpDown();
			var labelText = GetRandomString();
			var label = new Label { Text = labelText };
			const string propName = "CompIntProp";
			//---------------Assert Precondition----------------
			Assert.AreEqual(labelText, label.Text);
			Assert.IsTrue(GetPropDef(propName).Compulsory);
			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<NumericUpDownIntegerMapper, NumericUpDown>(label, numericUpDown, x => x.CompIntProp);
			//---------------Test Result -----------------------
			//---------------Test Control Mapper -----------------------
			Assert.IsInstanceOf<NumericUpDownMapper>(mapper);
			Assert.IsInstanceOf<NumericUpDownIntegerMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper, "Since this is a WinForms Control it needs an adapter to be used by Habanero");
			Assert.AreSame(numericUpDown, dtControlWrapper.WrappedControl, "Should set NumericUpDown");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");

			//------------------Check Label ------------------------------------
			Assert.AreEqual(labelText + " *", label.Text, "Should append star");
			Assert.IsTrue(label.Font.Bold, "Should be bold");
		}
		[Test]
		public void TestAccept_AddGeneric_WithLabel_WithCompulsory_UsingPropertyNameString_ShouldSetMappers_ControlAndPropName_ShouldSetLabelCompIndicato()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();

			var numericUpDown = new NumericUpDown();
			var labelText = GetRandomString();
			var label = new Label { Text = labelText };
			const string propName = "CompIntProp";
			//---------------Assert Precondition----------------
			Assert.AreEqual(labelText, label.Text);
			Assert.IsTrue(GetPropDef(propName).Compulsory);
			//---------------Execute Test ----------------------
			var mapper = haberoControlBinder.Add<NumericUpDownIntegerMapper, NumericUpDown>(label, numericUpDown, propName);
			//---------------Test Result -----------------------
			Assert.IsInstanceOf<NumericUpDownMapper>(mapper);
			Assert.IsInstanceOf<NumericUpDownIntegerMapper>(mapper);
			var dtControlWrapper = mapper.Control as WinFormsControlAdapter;
			Assert.IsNotNull(dtControlWrapper, "Since this is a WinForms Control it needs an adapter to be used by Habanero");
			Assert.AreSame(numericUpDown, dtControlWrapper.WrappedControl, "Should set NumericUpDown");
			Assert.AreEqual(propName, mapper.PropertyName, "Should set PropName");

			//------------------Check Label ------------------------------------
			Assert.AreEqual(labelText + " *", label.Text, "Should append star");
			Assert.IsTrue(label.Font.Bold, "Should be bold");
		}

		[Test]
		public void Test_AddGeneric_WithLabel_WhenCompulsory_ShouldAddStarAndBold()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var control = new NumericUpDown();
			var labelText = RandomValueGen.GetRandomString(10);
			var label = new Label { Text = labelText };
			//---------------Assert Precondition----------------
			Assert.AreEqual(labelText, label.Text);
			Assert.IsTrue(GetPropDef("CompIntProp").Compulsory);
			//---------------Execute Test ----------------------
			haberoControlBinder.Add<NumericUpDownIntegerMapper, NumericUpDown>(label, control, x => x.CompIntProp);
			//---------------Test Result -----------------------
			Assert.AreEqual(labelText + " *", label.Text, "Should append star");
			Assert.IsTrue(label.Font.Bold, "Should be bold");
		}

		[Test]
		public void Test_AddGeneric_WithLabel_WhenNotCompulsory_ShouldNotAddStarAndNotBeBold()
		{
			//---------------Set up test pack-------------------
			GlobalUIRegistry.ControlFactory = new ControlFactoryCF();
			var haberoControlBinder = CreateHabaneroControlBinder();
			var numericUpDown = new NumericUpDown();
			var labelText = RandomValueGen.GetRandomString(10);
			var label = new Label { Text = labelText };
			const string propName = "FakeIntProp";
			//---------------Assert Precondition----------------
			Assert.AreEqual(labelText, label.Text);
			Assert.IsFalse(GetPropDef(propName).Compulsory);
			//---------------Execute Test ----------------------
			haberoControlBinder.Add<NumericUpDownIntegerMapper, NumericUpDown>(label, numericUpDown, x => x.FakeIntProp);
			//---------------Test Result -----------------------
			Assert.AreEqual(labelText, label.Text, "Should Not append star");
			Assert.IsFalse(label.Font.Bold, "Should Not be bold");
		}
        */
		#endregion


		[Test]
		public void Test_SetBusinessObject_WhenOneMapper_ShouldSetBusinessObjectOnMapper()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();

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

		[Test]
		public void Test_SetBusinessObject_ShouldSetBusinessObjectOnAllControlMappers()
		{
			//---------------Set up test pack-------------------
			var haberoControlBinder = CreateHabaneroControlBinder();

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

	   

		private static IPropDef GetPropDef(string propName)
		{
			var classDef = ClassDef.Get<FakeBo>();
			return classDef.GetPropDef(propName);
		}

		private static HabaneroControlBinder<FakeBo> CreateHabaneroControlBinder()
		{
			return new HabaneroControlBinder<FakeBo>();
		}

		// ReSharper restore InconsistentNaming
		private static T GenerateStub<T>() where T : class
		{
			return MockRepository.GenerateStub<T>();
		}

		private static string GetRandomString()
		{
			return RandomValueGen.GetRandomString();
		}
	}

	public class HabaneroControlBinderSpy<TBo> : HabaneroControlBinder<TBo> where TBo : class, IBusinessObject
	{
		public HabaneroControlBinderSpy(IControlNamingConvention namingConvention) : base(namingConvention)
		{
		}
		public IControlNamingConvention GetNamingConvention()
		{
			return _controlNamingConvention;
		}
	}
}
