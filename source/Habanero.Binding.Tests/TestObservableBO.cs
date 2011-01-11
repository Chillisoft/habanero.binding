using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Testers;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestObservableBO
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
        
        #region IDataErrorInfo

        [Test]
        public void Test_FakeObservableBO_ShouldImplementIDataErrorInfo()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var FakeObservableBO = new FakeObservableBO();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<IDataErrorInfo>(FakeObservableBO);
        }

        [Test]
        public void Test_Error_WhenPropHasError_ShouldShowError()
        {
            //---------------Set up test pack-------------------
            var FakeObservableBO = new FakeObservableBO();
            var boTester = new BOTester<FakeObservableBO>();
            //---------------Assert Precondition----------------
            boTester.ShouldBeCompulsory(emp => emp.Prop1);
            boTester.ShouldBeCompulsory(emp => emp.Prop2);
            //---------------Execute Test ----------------------
            var error = FakeObservableBO.Error;
            //---------------Test Result -----------------------
            StringAssert.Contains("FakeObservableBO.Prop 1' is a compulsory field and has no value", error);
            StringAssert.Contains("FakeObservableBO.Prop 2' is a compulsory field and has no value", error);
        }
        [Test]
        public void Test_this_WithFirstName_WhenPropHasError_ShouldShowError()
        {
            //---------------Set up FakeObservableBO pack-------------------
            var FakeObservableBO = new FakeObservableBO();
            var boTester = new BOTester<FakeObservableBO>();
            //---------------Assert Precondition----------------
            boTester.ShouldBeCompulsory(emp => emp.Prop1);
            //---------------Execute Test ----------------------
            var errorForFirstName = FakeObservableBO["Prop1"];
            //---------------Test Result -----------------------
            StringAssert.Contains("FakeObservableBO.Prop 1' is a compulsory field and has no value", errorForFirstName);
        }

        #endregion

        #region INotifyPropertyChanged

        [Test]
        public void Test_FakeObservableBO_ShouldImplementINotifyPropertyChangedEvent()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var FakeObservableBO = new FakeObservableBO();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<INotifyPropertyChanged>(FakeObservableBO);
        }

        [Test]
        public void Test_SetPropertyValue_ShouldRaiseINotifyPropertyChangedEvent()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            var FakeObservableBO = new FakeObservableBO();
            var propertyThatChanged = "";
            FakeObservableBO.PropertyChanged += (sender, args) => propertyThatChanged = args.PropertyName;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<INotifyPropertyChanged>(FakeObservableBO);
            Assert.IsEmpty(propertyThatChanged, "No property has changed yet");
            //---------------Execute Test ----------------------
            FakeObservableBO.Prop1 = GetRandomString();
            //---------------Test Result -----------------------
            Assert.AreEqual("Prop1", propertyThatChanged,
                            "PropertyChanged Event for FirstName should have been fired");
        }

        [Test]
        public void Test_SetPropertyValue_ViaBOProp_ShouldRaiseINotifyPropertyChangedEvent()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            var FakeObservableBO = new FakeObservableBO();
            var propertyThatChanged = "";
            FakeObservableBO.PropertyChanged += (sender, args) => propertyThatChanged = args.PropertyName;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<INotifyPropertyChanged>(FakeObservableBO);
            Assert.IsEmpty(propertyThatChanged, "No property has changed yet");
            //---------------Execute Test ----------------------
            FakeObservableBO.SetPropertyValue("Prop2", GetRandomString());
            //---------------Test Result -----------------------
            Assert.AreEqual("Prop2", propertyThatChanged,
                            "PropertyChanged Event for Prop2 should have been fired");
        }

        [Test]
        public void Test_SetPropertyValue_WhenConstructedWithClassDef_ShouldRaiseINotifyPropertyChangedEvent()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            var classDef = ClassDef.Get<FakeObservableBO>();
            var FakeObservableBO = new FakeObservableBO(classDef);
            var propertyThatChanged = "";
            FakeObservableBO.PropertyChanged += (sender, args) => propertyThatChanged = args.PropertyName;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<INotifyPropertyChanged>(FakeObservableBO);
            Assert.IsEmpty(propertyThatChanged, "No property has changed yet");
            //---------------Execute Test ----------------------
            FakeObservableBO.Prop1 = GetRandomString();
            //---------------Test Result -----------------------
            Assert.AreEqual("Prop1", propertyThatChanged,
                            "PropertyChanged Event for FirstName should have been fired");
        }
 
        #endregion

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }

    public class FakeObservableBO : ObservableBusinessObject<FakeObservableBO>
    {
        public FakeObservableBO(IClassDef def) : base(def)
        {
        }

        public FakeObservableBO()
        {
        }


        [AutoMapCompulsory]
        public string Prop1
        {
            get { return GetPropertyValueString("Prop1"); }
            set { SetPropertyValue("Prop1", value); }
        }
        [AutoMapCompulsory]
        public string Prop2
        {
            get { return GetPropertyValueString("Prop2"); }
            set { SetPropertyValue("Prop2", value); }
        }
    }
}
// ReSharper restore InconsistentNaming