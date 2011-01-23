using System;
using System.ComponentModel;
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Helpers;
using Habanero.Util;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestPropertyDescriptorPropInfo
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

            var classDefs = typeof(FakeBO).MapClasses();
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
        /// <summary>
        /// Unfortunately I cannot avoid this null reference error since
        /// the PropertyDescriptor does not have a constructor that can take
        /// a null string etc for propertyName and does not implement an interface.
        /// </summary>
        [Test]
        public void Test_Construct_WithNullPropDef_ShouldRaiseError()
        {

            //---------------Set up test pack-------------------
            PropertyInfo propertyInfo = null;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new PropertyDescriptorPropInfo(propertyInfo);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (NullReferenceException ex)
            {
                Assert.IsTrue(true, "IF raised this error then OK. I cannot raise more meaningfull error");
            }
        }

        [Test]
        public void Test_Construct_WithPropInfor_ShouldSetPropInfo()
        {
            //---------------Set up test pack-------------------
            PropertyInfo propertyInfo = new Habanero.Testability.Helpers.FakePropertyInfo();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDescriptor);
            Assert.AreSame(propertyInfo, propDescriptor.GetPropertyInfo());
        }
        [Test]
        public void Test_Construct_WithPropInfo_ShouldSetName()
        {
            //---------------Set up test pack-------------------
            var propName = GetRandomString();
            PropertyInfo propertyInfo = new FakePropertyInfo(propName);
            //---------------Assert Precondition----------------
            Assert.AreEqual(propName, propertyInfo.Name);
            //---------------Execute Test ----------------------
            var propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Test Result -----------------------
            Assert.AreEqual(propName, propDescriptor.Name);
        }


        [Test]
        public void Test_DisplayName_ShouldBePropertyName_CamelCaseDelimited()
        {
            //---------------Set up test pack-------------------
            const string propName = "ThisIsCamelCased";
            var propertyInfo = new FakePropertyInfo(propName);
            PropertyDescriptor propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.AreEqual(propName, propertyInfo.Name);
            //---------------Execute Test ----------------------
            var actualPropDescriptorDisplayName = propDescriptor.DisplayName;
            //---------------Test Result -----------------------
            Assert.AreEqual("This Is Camel Cased", actualPropDescriptorDisplayName);
        }

        [Test]
        public void Test_PropertyType_ShouldReturnPropertyTypePropInfo()
        {
            //---------------Set up test pack-------------------
            var expectedPropType = typeof(int);
            var propertyInfo = new FakePropertyInfo(GetRandomString(), expectedPropType);

            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedPropType, propertyInfo.PropertyType);
            //---------------Execute Test ----------------------
            var propertyType = propDescriptor.PropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropType, propertyType);
        }
        
        [Test]
        public void Test_IsReadOnly_WhenHasSetMethod_AndReadOnlyAttributeTrue_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            FakePropertyInfo propertyInfo = GetMockPropertyInfo();
            propertyInfo.Stub(info => info.GetCustomAttributes(typeof(ReadOnlyAttribute), true)).Return(new object[]{new ReadOnlyAttribute(true)});
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo.GetSetMethod());
            Assert.IsTrue(propertyInfo.GetAttribute<ReadOnlyAttribute>().IsReadOnly);
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsTrue(isReadOnly, "If has setter but ReadOnlyAttribute is true should be ReadOnly.");
        }


        [Test]
        public void Test_IsReadOnly_WhenHasSetMethod_AndReadOnlyAttributeFalse_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = GetMockPropertyInfo();
            propertyInfo.Stub(info => info.GetCustomAttributes(typeof(ReadOnlyAttribute), true)).Return(new object[]{new ReadOnlyAttribute(false)});
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo.GetSetMethod());
            Assert.IsFalse(propertyInfo.GetAttribute<ReadOnlyAttribute>().IsReadOnly);
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsFalse(isReadOnly, "If has setter and ReadOnlyAttribute false so should not be ReadOnly.");
        }        
        [Test]
        public void Test_IsReadOnly_WhenNotHasSetMethod_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateStub<FakePropertyInfo>();
            propertyInfo.Stub(info => info.Name).Return(GetRandomString());
            propertyInfo.Stub(info => info.GetSetMethod(false)).Return(null);
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNull(propertyInfo.GetSetMethod());
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsTrue(isReadOnly, "If has NO setter should be ReadOnly.");
        }
        [Test]
        public void Test_ComponentType_ShouldReturnGridColumnsClassDefsClassType()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = new FakePropertyInfo();
            var expectedComponentType = typeof (FakeBO);
            propertyInfo.SetReflectedType(expectedComponentType);
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propertyInfo.ReflectedType);
            //---------------Execute Test ----------------------
            var componentType = propDescriptor.ComponentType;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedComponentType, componentType);
        }


        [Test]
        public void Test_ShouldSerializeValue_WhenReadOnly_ShouldRetFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = MockRepository.GenerateStub<FakePropertyInfo>();
            propertyInfo.Stub(info => info.Name).Return(GetRandomString());
            propertyInfo.Stub(info => info.GetSetMethod(false)).Return(null);
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.IsNull(propertyInfo.GetSetMethod(false));
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
            var propertyInfo = typeof (FakeBO).GetProperty("FakeBOName");
            var expectedPropValue = RandomValueGen.GetRandomString();
            var fakeBO = new FakeBO { FakeBOName = expectedPropValue };
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedPropValue,  propertyInfo.GetValue(fakeBO, null));
            Assert.AreEqual(expectedPropValue, fakeBO.FakeBOName);
            //---------------Execute Test ----------------------
            var actualValue = propDescriptor.GetValue(fakeBO);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropValue, actualValue);
        }

        [Test]
        public void Test_GetValue_WhenComponentNotCorrectType_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
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
        // ReSharper disable AssignNullToNotNullAttribute
        [Test]
        public void Test_GetValue_WhenComponentNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = new FakePropertyInfo();
            PropertyDescriptorPropInfo propDescriptor = new PropertyDescriptorPropInfoSpy(propertyInfo);
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
        // ReSharper restore AssignNullToNotNullAttribute




        [Test]
        public void Test_SetValue_ShouldSetValueOnBO()
        {
            //---------------Set up test pack-------------------

            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            var initialPropValue = GetRandomString();
            var fakeBO = new FakeBO { FakeBOName = initialPropValue };
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);

            
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialPropValue, propertyInfo.GetValue(fakeBO, null));
            Assert.AreEqual(initialPropValue, fakeBO.FakeBOName);
            //---------------Execute Test ----------------------
            var expectedNewPropValue = GetRandomString();
            propDescriptor.SetValue(fakeBO, expectedNewPropValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNewPropValue, fakeBO.FakeBOName);
        }


        [Test]
        public void Test_SetValue_WhenComponentNotCorrectType_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
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
        // ReSharper disable AssignNullToNotNullAttribute
        [Test]
        public void Test_SetValue_WhenComponentNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
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
        // ReSharper restore AssignNullToNotNullAttribute


        [Test]
        public void Test_CanResetValue_WhenNotDirty_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
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
            var propertyInfo = typeof(FakeBO).GetProperty("FakeBOName");
            var propDescriptor = new PropertyDescriptorPropInfo(propertyInfo);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var fakeBO = new FakeBO { FakeBOName = RandomValueGen.GetRandomString() };
            var canResetValue = propDescriptor.CanResetValue(fakeBO);
            //---------------Test Result -----------------------
            Assert.IsFalse(canResetValue);
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static FakePropertyInfo GetMockPropertyInfo()
        {
            var propertyInfo = MockRepository.GenerateStub<FakePropertyInfo>();
            propertyInfo.Stub(info => info.GetSetMethod()).Return(new FakeMethodInfo());
            propertyInfo.Stub(info => info.Name).Return(GetRandomString());
            return propertyInfo;
        }
    }

    public class PropertyDescriptorPropInfoSpy : PropertyDescriptorPropInfo
    {
        public PropertyDescriptorPropInfoSpy(PropertyInfo propertyInfo): base(propertyInfo)
        {
            
        }
        public PropertyInfo GetPropertyInfo()
        {
            return this.PropInfo;
        }
    }
}