using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestPropertyDescriptorPropDef
    {
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerFake();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once. 
       
            var classDefs = typeof (FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefs);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            //Code that is executed after all test run in this class. If multiple tests
            // are executed then it will still only be called once.        
            BORegistry.BusinessObjectManager = BusinessObjectManager.Instance;
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [Test]
        public void Test_Construct_WithNullColumn_ShouldRaiseError()
        {
            //Unfortunately I cannot avoid this null reference error since
            // the PropertyDescriptor does not have a constructor that can take
            // a null string etc for propertyName and does not implement an interface.
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new PropertyDescriptorPropDef(gridColumn);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (NullReferenceException )
            {
                Assert.IsTrue(true, "IF raised this error then OK");
            }            
        }

        [Test]
        public void Test_Construct_WithNotHasClassDef_ShouldRaiseError()
        {
            //This is a PropDescriptor that wraps a 
            //UIGridColumn that wraps a PropDef and if the 
            //PropDef is null this should therefore raise and error.
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = MockRepository.GenerateStub<IUIGridColumn>();
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsFalse(gridColumn.HasPropDef);
            //---------------Execute Test ----------------------
            try
            {
                new PropertyDescriptorPropDef(gridColumn);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                Assert.AreEqual("gridColumn.ClassDef", ex.ParameterName);
            }  
        }

        [Ignore("This is currently dealt with via the BOPropertyMapper")] //TODO Brett 10 Jul 2010: Ignored Test - This is currently dealt with via the BOPropertyMapper
        [Test]
        public void Test_Construct_WithNotHasPropDef_ShouldRaiseError()
        {
            //This is a PropDescriptor that wraps a 
            //UIGridColumn that wraps a PropDef and if the 
            //PropDef is null this should therefore raise and error.
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            IUIGridColumn gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsFalse(gridColumn.HasPropDef);
            //---------------Execute Test ----------------------
            try
            {
                new PropertyDescriptorPropDef(gridColumn);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                Assert.AreEqual("gridColumn.ClassDef", ex.ParameterName);
            }  
        }

        [Ignore("NotCurrentlyIMplemented")] //TODO Brett 10 Jul 2010: Ignored Test - NotCurrentlyIMplemented
        [Test]
        public void Test_Construct_WhenPropDefsClassDef_NotSameAs_GridColumnsClassDef_ShouldRaiseError()
        {
            //This is a PropDescriptor that wraps a 
            //UIGridColumn that wraps a PropDef if this 
            // PropDef is a related PropDef e.g. on Order using Customer.CustomerName
            // the the PropDefs ClassDef and the GridColumns ClassDef will not be the same.
            // The PropertyDescriptorRelatedPropDef should be used instead.
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = MockRepository.GenerateStub<IUIGridColumn>();
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsFalse(gridColumn.HasPropDef);
            //---------------Execute Test ----------------------
            try
            {
                new PropertyDescriptorPropDef(gridColumn);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                Assert.AreEqual("gridColumn.ClassDef", ex.ParameterName);
            }  
        }
        [Test]
        public void Test_Construct_ShouldConstruct()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            PropertyDescriptor propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDescriptor);           
        }

        [Test]
        public void Test_Name_ShouldBeGridColumnPropertyName()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            string expectedName = RandomValueGen.GetRandomString();
            gridColumn.PropertyName = expectedName;
            PropertyDescriptor propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var actualPropDescriptorName = propDescriptor.Name;
            //propDescriptor.DisplayName
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedName, actualPropDescriptorName);
        }

        [Test]
        public void Test_DisplayName_ShouldBeGridColumnDisplayName()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            var expectedDispalyName = RandomValueGen.GetRandomString();
            gridColumn.Stub(column => column.GetHeading()).Return(expectedDispalyName);
            PropertyDescriptor propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedDispalyName, gridColumn.GetHeading());
            //---------------Execute Test ----------------------
            var actualPropDescriptorDisplayName = propDescriptor.DisplayName;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedDispalyName, actualPropDescriptorDisplayName);
        }



        [Test]
        public void Test_LookupList_WhenNotSet_ShouldBeNull()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            IUIGridColumn gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsNull(classDef.GetLookupList(gridColumn.PropertyName));
            //---------------Execute Test ----------------------
            ILookupList lookupList = propDescriptor.LookupList;
            //---------------Test Result -----------------------
            Assert.IsNull(lookupList);
        }

        private IUIGridColumn GetGridColumnStub(IClassDef classDef)
        {
            IUIGridColumn gridColumn = MockRepository.GenerateStub<IUIGridColumn>();
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            gridColumn.Stub(column => column.ClassDef).Return(classDef);
            return gridColumn;
        }

        [Test]
        public void Test_LookupList_WhenSetOnProp_ShouldReturnList()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            ILookupList expectedLookupList = MockRepository.GenerateStub<ILookupList>();
            gridColumn.Stub(column => column.LookupList).Return(expectedLookupList);

            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(gridColumn.LookupList);
            //---------------Execute Test ----------------------
            ILookupList lookupList = propDescriptor.LookupList;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedLookupList, lookupList);
        }

        [Test]
        public void Test_Width_ShouldReturnWidthFromColumn()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            var expectedWidth = RandomValueGen.GetRandomInt(0, 33);
            gridColumn.Width = expectedWidth;

            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedWidth, gridColumn.Width);
            //---------------Execute Test ----------------------
            var width = propDescriptor.Width;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedWidth, width);
        }

        [Test]
        public void Test_Alignment_ShouldReturnAlignmentFromColumn()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            var expectedAlignment = RandomValueGen.GetRandomEnum<PropAlignment>();;
            gridColumn.Alignment = expectedAlignment;

            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedAlignment, gridColumn.Alignment);
            //---------------Execute Test ----------------------
            var alignment = propDescriptor.Alignment;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedAlignment, alignment);
        }

        [Test]
        public void Test_PropertyType_ShouldReturnPropertyTypeFromColumn()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            Type expectedPropType = typeof (DateTime);
            gridColumn.Stub(column => column.GetPropertyType()).Return(expectedPropType);

            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedPropType, gridColumn.GetPropertyType());
            //---------------Execute Test ----------------------
            var propertyType = propDescriptor.PropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropType, propertyType);
        }

        [Test]
        public void Test_IsReadOnly_WhenIsEditable_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var gridColumn = GetGridColumnStub();
            gridColumn.Editable = true;
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsTrue(gridColumn.Editable);
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsFalse(isReadOnly, "If grid Columnis editable PropDescriptor.ReadOnly should be false.");
        }
        [Test]
        public void Test_IsReadOnly_WhenNotIsEditable_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var gridColumn = GetGridColumnStub();

            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsFalse(gridColumn.Editable);
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsTrue(isReadOnly, "If gridColumn is not editable propDescriptor.ReadOnly should be true.");
        }

        [Test]
        public void Test_ComponentType_ShouldReturnGridColumnsClassDefsClassType()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof (FakeBO);
            IUIGridColumn gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(gridColumn.ClassDef);
            Assert.IsNotNull(gridColumn.ClassDef.ClassType);
            //---------------Execute Test ----------------------
            var componentType = propDescriptor.ComponentType;
            //---------------Test Result -----------------------
            Assert.AreSame(typeof(FakeBO), componentType);
        }

        [Test]
        public void Test_ShouldSerializeValue_WhenReadOnly_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            IUIGridColumn gridColumn = GetGridColumnStub();
            gridColumn.ClassDef.ClassType =  typeof(FakeBO);
            gridColumn.Editable = false;
            gridColumn.PropertyName = "FakeBOName";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDescriptor.IsReadOnly);
            //---------------Execute Test ----------------------
            var shouldSerializeValue = propDescriptor.ShouldSerializeValue(new FakeBO());
            //---------------Test Result -----------------------
            Assert.IsFalse(shouldSerializeValue);
        }

        [Test]
        public void Test_GetValue_ShouldGetValueFromBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOName";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBO fakeBO = new FakeBO {FakeBOName = RandomValueGen.GetRandomString()};
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef, gridColumn.ClassDef);
            Assert.AreSame(typeof(FakeBO), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.FakeBOName);
            //---------------Execute Test ----------------------
            var actualValue = propDescriptor.GetValue(fakeBO);
            //---------------Test Result -----------------------
            Assert.AreEqual(fakeBO.FakeBOName, actualValue);
        }
        [Test]
        public void Test_GetValue_WhenComponentNotCorrectType_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            try
            {
                propDescriptor.GetValue(100);
                Assert.Fail("Expected to throw an InvalidCastException");
            }
                //---------------Test Result -----------------------
            catch (InvalidCastException ex)
            {
                StringAssert.Contains("You cannot GetValue since the component is not of type ", ex.Message);
            }
        }
        [Test]
        public void Test_GetValue_WhenComponentNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            object x = null;
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            try
            {
                propDescriptor.GetValue(x);
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("component", ex.ParamName);
                StringAssert.Contains("Value cannot be null.", ex.Message);
            }
        }

        [Test]
        public void Test_SetValue_ShouldSetValueOnBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOName";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBO fakeBO = new FakeBO { FakeBOName = RandomValueGen.GetRandomString() };
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef, gridColumn.ClassDef);
            Assert.AreSame(typeof(FakeBO), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.FakeBOName);
            //---------------Execute Test ----------------------
            var expectedValue = RandomValueGen.GetRandomString();
            propDescriptor.SetValue(fakeBO, expectedValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedValue, fakeBO.FakeBOName);
        }
        [Test]
        public void Test_SetValue_WhenComponentNotCorrectType_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                propDescriptor.SetValue(100, "fdafd");
                Assert.Fail("Expected to throw an InvalidCastException");
            }
            //---------------Test Result -----------------------
            catch (InvalidCastException ex)
            {
                StringAssert.Contains("You cannot GetValue since the component is not of type ", ex.Message);
            }
        }
        [Test]
        public void Test_SetValue_WhenComponentNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBO);
            var gridColumn = GetGridColumnStub(classDef);
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            object x = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                propDescriptor.SetValue(x, "fdafd");
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("component", ex.ParamName);
                StringAssert.Contains("Value cannot be null.", ex.Message);
            }
        }

        [Test]
        public void Test_CanResetValue_WhenNotDirty_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var gridColumn = GetGridColumnStub();
            gridColumn.PropertyName = "FakeBOName";
            var propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var canResetValue = propDescriptor.CanResetValue(new FakeBO());
            //---------------Test Result -----------------------
            Assert.IsFalse(canResetValue);
        }
        [Test]
        public void Test_CanResetValue_When_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var gridColumn = GetGridColumnStub();
            gridColumn.PropertyName = "FakeBOName";
            var propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            FakeBO fakeBO = new FakeBO();
            fakeBO.FakeBOName = RandomValueGen.GetRandomString();
            var canResetValue = propDescriptor.CanResetValue(fakeBO);
            //---------------Test Result -----------------------
            Assert.IsFalse(canResetValue);
        }

        /// <summary>
        /// For more fully testing this look at TestPropertyDescriptorRelatedPropDef
        /// </summary>
        [Test]
        public void Test_GetValue_WhenUseRelatedBOProp_ShouldReturnValueFromBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBOWSingleRelationship);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOW2Props.Prop1";
            var propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBOW2Props fakeBO = new FakeBOW2Props { Prop1 = RandomValueGen.GetRandomString() };
            FakeBOWSingleRelationship fakeBowSingleRelationship = new FakeBOWSingleRelationship { FakeBOW2Props = fakeBO };
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef.ToString(), gridColumn.ClassDef.ToString());
            Assert.AreSame(typeof(FakeBOWSingleRelationship), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.Prop1);
            //---------------Execute Test ----------------------
            var actualValue = propDescriptor.GetValue(fakeBowSingleRelationship);
            //---------------Test Result -----------------------
            Assert.AreEqual(fakeBO.Prop1, actualValue);
        }

        /// <summary>
        /// For more fully testing this look at TestPropertyDescriptorRelatedPropDef
        /// </summary> 
        [Test]
        public void Test_SetValue__WhenUsingRelatedBOProp_ShouldSetValueOnBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBOWSingleRelationship);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOW2Props.Prop1";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBOW2Props fakeBO = new FakeBOW2Props { Prop1 = RandomValueGen.GetRandomString() };
            FakeBOWSingleRelationship fakeBowSingleRelationship = new FakeBOWSingleRelationship { FakeBOW2Props = fakeBO };
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef, gridColumn.ClassDef);
            Assert.AreSame(typeof(FakeBOWSingleRelationship), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.Prop1);
            //---------------Execute Test ----------------------
            var expectedValue = RandomValueGen.GetRandomString();
            propDescriptor.SetValue(fakeBowSingleRelationship, expectedValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedValue, fakeBO.Prop1);
        }
        

        private IUIGridColumn GetGridColumnStub()
        {
            IUIGridColumn gridColumn = MockRepository.GenerateStub<IUIGridColumn>();
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof (FakeBO);
            gridColumn.Stub(column => column.ClassDef).Return(classDef);
            return gridColumn;
        }

        // ReSharper disable UnusedMember.Local
        // Grants access to protected fields
        private class UIGridColumnSpy : UIGridColumn
        {
            public UIGridColumnSpy()
                : base("heading", null, null, null, true, 100,
                    PropAlignment.left, null)
            { }
            private IClassDef _setClassDef;

            public void SetHeading(string name)

            {
                Heading = name;
            }

            public void SetPropertyName(string name)
            {
                PropertyName = name;
            }

            public void SetGridControlType(Type type)
            {
                GridControlType = type;
            }

            public void SetEditable(bool editable)
            {
                Editable = editable;
            }

            public void SetWidth(int width)
            {
                Width = width;
            }

            public void SetAlignment(PropAlignment alignment)
            {
                Alignment = alignment;
            }

            public void SetClassDef(IClassDef classDef)
            {
                _setClassDef = classDef;
            }

            public override IClassDef ClassDef
            {
                get
                {
                    return _setClassDef ?? base.ClassDef;
                }
            }

            public new IPropDef PropDef
            {
                get { return _propDef; }
            }  

            public void SetPropDef(IPropDef propDef)
            {
                _propDef = propDef;
            }
        }
        // ReSharper restore UnusedMember.Local
    }
}