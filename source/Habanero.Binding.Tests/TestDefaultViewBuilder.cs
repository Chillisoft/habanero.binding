using System.ComponentModel;
using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Naked;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Habanero.Testability.Helpers;

// ReSharper disable InconsistentNaming
namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestDefaultViewBuilder
    {
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerFake();
            ClassDef.ClassDefs.Clear();
            ClassDef.ClassDefs.Add(typeof(FakeBO).MapClasses());
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once. 
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
            IViewBuilder viewBuilder = new DefaultViewBuilder<FakeBoOnePropOneSingleRelationship>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(viewBuilder);
        }

        [Test]
        public void Test_GetGridView_ShouldNotAddBusinessObjectPropsmarkedWithTypeDescriptorIgnoreAttribute()
        {
            //---------------Set up test pack-------------------
            IViewBuilder viewBuilder = new DefaultViewBuilder<FakeBoOnePropOneSingleRelationship>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBoOnePropOneSingleRelationship)));
            //---------------Execute Test ----------------------
            PropertyDescriptorCollection descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.IsNotNull(descriptorCollection);
            Assert.AreEqual(2, descriptorCollection.Count, "Prop, Single Rel");
        }

        [Test]
        public void Test_GetGridView_WithTwoHabiProps_TwoReflectiveProps_ShouldReturnPropDescColWithAllProps()
        {
            //---------------Set up test pack-------------------
            IViewBuilder viewBuilder = new DefaultViewBuilder<FakeBOW2Props>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBOW2Props)));
            //---------------Execute Test ----------------------
            var descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.AreEqual(4, descriptorCollection.Count);
            descriptorCollection.Cast<PropertyDescriptor>().ShouldContain(prop => prop.Name == "Prop1");
            descriptorCollection.Cast<PropertyDescriptor>().ShouldContain(prop => prop.Name == "Prop2");
            descriptorCollection.Cast<PropertyDescriptor>().ShouldContain(prop => prop.Name == "ReflectiveProp");
            descriptorCollection.Cast<PropertyDescriptor>().ShouldContain(prop => prop.Name == "FakeObjectNotABo");
        }
        
    }

}