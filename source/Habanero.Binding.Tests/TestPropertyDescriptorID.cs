using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestPropertyDescriptorID
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
        public void Test_Construct_ShouldConstruct()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            PropertyDescriptor propDescriptor = new PropertyDescriptorID();
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDescriptor);           
        }

        [Test]
        public void Test_Name_ShouldBeGridColumnPropertyName()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptor propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var actualPropDescriptorName = propDescriptor.Name;
            //propDescriptor.DisplayName
            //---------------Test Result -----------------------
            Assert.AreEqual("HABANERO_OBJECTID", actualPropDescriptorName);
        }
        [Test]
        public void Test_DisplayName_ShouldBeGridColumnPropertyName()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptor propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var actualDisplayName = propDescriptor.DisplayName;
            //---------------Test Result -----------------------
            Assert.AreEqual("HABANERO_OBJECTID", actualDisplayName);
        }
        [Test]
        public void Test_LookupList_ShouldBeNull()
        {
            //---------------Set up test pack-------------------

            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ILookupList lookupList = propDescriptor.LookupList;
            //---------------Test Result -----------------------
            Assert.IsNull(lookupList);
        }

        [Test]
        public void Test_Width_ShouldReturnWidthFromColumn()
        {
            //---------------Set up test pack-------------------
            const int expectedWidth = 0;
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var width = propDescriptor.Width;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedWidth, width);
        }

        [Test]
        public void Test_Alignment_ShouldReturnAlignmentFromColumn()
        {
            //---------------Set up test pack-------------------
            const PropAlignment expectedAlignment = PropAlignment.left;
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var alignment = propDescriptor.Alignment;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedAlignment, alignment);
        }


        [Test]
        public void Test_PropertyType_ShouldReturnObject()
        {
            //---------------Set up test pack-------------------
            Type expectedPropType = typeof(object);
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var propertyType = propDescriptor.PropertyType;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropType, propertyType);
        }

        [Test]
        public void Test_IsReadOnly_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isReadOnly = propDescriptor.IsReadOnly;
            //---------------Test Result -----------------------
            Assert.IsTrue(isReadOnly, "ID Descriptor should always be read only");
        }

        [Test]
        public void Test_ComponentType_ShouldReturnIBusinessObject()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var componentType = propDescriptor.ComponentType;
            //---------------Test Result -----------------------
            Assert.AreSame(typeof(IBusinessObject), componentType);
        }

        [Test]
        public void Test_GetValue_ShouldGetValueFromBO()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            FakeBO fakeBO = new FakeBO { FakeBOName = RandomValueGen.GetRandomString() };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(fakeBO.ID.ObjectID);
            //---------------Execute Test ----------------------
            var actualValue = propDescriptor.GetValue(fakeBO);
            //---------------Test Result -----------------------
            Assert.AreEqual(fakeBO.ID.ObjectID, actualValue);
        }
        [Test]
        public void Test_GetValue_WhenComponentNotCorrectType_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
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
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
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
        public void Test_SetValue_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            PropertyDescriptorID propDescriptor = new PropertyDescriptorID();
            FakeBO fakeBO = new FakeBO { FakeBOName = RandomValueGen.GetRandomString() };
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(fakeBO.FakeBOName);
            //---------------Execute Test ----------------------
            var expectedValue = RandomValueGen.GetRandomString();
            try
            {
                propDescriptor.SetValue(fakeBO, expectedValue);
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The PropertyDescriptorID cannot set value since the objectID is ReadOnly", ex.Message);
            }
        }

        [Test]
        public void Test_CanResetValue_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var propDescriptor = new PropertyDescriptorID();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var canResetValue = propDescriptor.CanResetValue(new FakeBO());
            //---------------Test Result -----------------------
            Assert.IsFalse(canResetValue);
        }
    }
}