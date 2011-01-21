using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Naked;
using Habanero.Testability;
using NUnit.Framework;
using Habanero.Smooth;

namespace Habanero.Binding.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestUIDefViewBuilder
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
            IViewBuilder viewBuilder = new UIDefViewBuilder<FakeBO>();
             //---------------Test Result -----------------------
            Assert.IsNotNull(viewBuilder);
        }

        [Test]
        public void Test_GetGridView_ShouldAddIDDescriptor()
        {
            //---------------Set up test pack-------------------
            IViewBuilder viewBuilder = new UIDefViewBuilder<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBO)));
            //---------------Execute Test ----------------------
            PropertyDescriptorCollection descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.IsNotNull(descriptorCollection);
            Assert.AreEqual(2, descriptorCollection.Count);
            var propertyDescriptor = descriptorCollection[1];
            Assert.IsInstanceOf<PropertyDescriptorID>(propertyDescriptor);
        }
        [Test]
        public void Test_GetGridView_WhenNoUIViewSpecified_ShouldReturnPropDescColForDefaultView()
        {
            //---------------Set up test pack-------------------
            IViewBuilder viewBuilder = new UIDefViewBuilder<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBO)));
            //---------------Execute Test ----------------------
            PropertyDescriptorCollection descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.IsNotNull(descriptorCollection);
            Assert.AreEqual(2, descriptorCollection.Count);
            var propertyDescriptor = descriptorCollection[0];
            Assert.IsInstanceOf<PropertyDescriptorPropDef>(propertyDescriptor);
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
        }
        [Test]
        public void Test_GetGridView_WithTwoProps_ShouldReturnPropDescColWithBothProps()
        {
            //---------------Set up test pack-------------------
            IViewBuilder viewBuilder = new UIDefViewBuilder<FakeBOW2Props>();
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBOW2Props)));
            //---------------Execute Test ----------------------
            PropertyDescriptorCollection descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.AreEqual(3, descriptorCollection.Count);
            var propertyDescriptor = descriptorCollection[0];
            Assert.AreEqual("Prop1", propertyDescriptor.Name);
            var propertyDescriptor2 = descriptorCollection[1];
            Assert.AreEqual("Prop2", propertyDescriptor2.Name);
        }
        [Test]
        public void Test_GetGridView_WithReflectiveProp_ShouldReturnPropDescColWithProp()
        {
            //---------------Set up test pack-------------------
            //ClassDef.ClassDefs = new ClassDefCol();
            ClassDef.ClassDefs.Remove(typeof(FakeBOWReflectiveProp));
            var classDef = typeof (FakeBOWReflectiveProp).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            Habanero.Naked.UIViewCreator viewCreator = new UIViewCreator();
            var defaultUiDef = viewCreator.GetDefaultUIDef(classDef);
            defaultUiDef.Name = RandomValueGen.GetRandomString();
            defaultUiDef.UIGrid.Add(new UIGridColumn(null, "ReflectiveProp",null, null, true, 100, PropAlignment.left, null));
            classDef.UIDefCol.Add(defaultUiDef);
            var viewBuilder = new UIDefViewBuilder<FakeBOWReflectiveProp>(defaultUiDef.Name);
            //---------------Assert Precondition----------------
//            Assert.IsTrue(ClassDef.ClassDefs.Contains(typeof(FakeBOWReflectiveProp)));
            Assert.AreEqual(1, defaultUiDef.UIDefCol.Count, "There should be only the reflective column");
            //---------------Execute Test ----------------------
            PropertyDescriptorCollection descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, descriptorCollection.Count);
            var propertyDescriptor = descriptorCollection[0];
            Assert.IsInstanceOf<PropertyDescriptorPropInfo>(propertyDescriptor);
            Assert.AreEqual("ReflectiveProp", propertyDescriptor.Name);
        }

        [Test]
        public void Test_Construct_WithSpecifiedView()
        {
            //---------------Set up test pack-------------------
            var classDef = ClassDef.Get<FakeBOWReflectiveProp>();
            Habanero.Naked.UIViewCreator viewCreator = new UIViewCreator();
            var uiDef = viewCreator.GetDefaultUIDef(classDef);
            const string noneDefaultUI = "NotDefaultUI";
            uiDef.Name = noneDefaultUI;
            uiDef.UIGrid.Add(new UIGridColumn(null, "ReflectiveProp", null, null, true, 100, PropAlignment.left, null));
            classDef.UIDefCol.Add(uiDef);
            
            //---------------Assert Precondition----------------
            Assert.IsFalse(classDef.UIDefCol.Contains("default"));
            Assert.IsTrue(classDef.UIDefCol.Contains(noneDefaultUI));
            Assert.AreEqual(1, uiDef.UIDefCol.Count, "There should be only the reflective column");
            //---------------Execute Test ----------------------
            var viewBuilder = new UIDefViewBuilder<FakeBOWReflectiveProp>(noneDefaultUI);
            var descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, descriptorCollection.Count, "Reflective column plus ID column");
            var propertyDescriptor = descriptorCollection[0];
            Assert.IsInstanceOf<PropertyDescriptorPropInfo>(propertyDescriptor);
            Assert.AreEqual("ReflectiveProp", propertyDescriptor.Name);
        }
        [Test]

        public void Test_Construct_WithSpecifiedView_ThatDoesNotExist_ShouldReturnDefaultView()
        {
            //---------------Set up test pack-------------------
            var classDef = ClassDef.Get<FakeBOWReflectiveProp>();
            Habanero.Naked.UIViewCreator viewCreator = new UIViewCreator();
            var uiDef = viewCreator.GetDefaultUIDef(classDef);
            const string noneExistantView = "NoneExistantView";
            uiDef.UIGrid.Add(new UIGridColumn(null, "ReflectiveProp", null, null, true, 100, PropAlignment.left, null));
            classDef.UIDefCol.Add(uiDef);
            
            //---------------Assert Precondition----------------
            Assert.AreEqual("default", uiDef.Name);
            Assert.IsTrue(classDef.UIDefCol.Contains("default"));
            Assert.IsFalse(classDef.UIDefCol.Contains(noneExistantView));
            //---------------Execute Test ----------------------
            var viewBuilder = new UIDefViewBuilder<FakeBOWReflectiveProp>(noneExistantView);
            var descriptorCollection = viewBuilder.GetPropertyDescriptors();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, descriptorCollection.Count);
            var propertyDescriptor = descriptorCollection[0];
            Assert.IsInstanceOf<PropertyDescriptorPropInfo>(propertyDescriptor);
            Assert.AreEqual("ReflectiveProp", propertyDescriptor.Name);
        }
    }

// ReSharper disable UnusedMember.Global
    // ReSharper disable ClassNeverInstantiated.Global
    public class FakeBO: BusinessObject
    {

        public FakeObjectNotABo FakeObjectNotABo { get; set; }

        public string FakeBOName
        {
            get { return GetPropertyValueString("FakeBOName"); }
            set { SetPropertyValue("FakeBOName", value); }
        }
    }
    public class FakeBoOnePropOneSingleRelationship: BusinessObject
    {
        /// <summary>
        /// The FakeBO this FakeBoOnePropOneSingleRelationship is for.
        /// </summary>
        public virtual FakeBO FakeBO
        {
            get { return Relationships.GetRelatedObject<FakeBO>("FakeBO"); }
            set { Relationships.SetRelatedObject("FakeBO", value); }
        }

        public string FakeBOName
        {
            get { return GetPropertyValueString("FakeBOName"); }
            set { SetPropertyValue("FakeBOName", value); }
        }
    }

    public class FakeBOW5Props : BusinessObject
    {
        public FakeObjectNotABo FakeObjectNotABo { get; set; }

        public string FakeBOName
        {
            get { return GetPropertyValueString("FakeBOName"); }
            set { SetPropertyValue("FakeBOName", value); }
        }
        public int FakeBONumber
        {
            get { return GetPropertyValue<int>("FakeBONumber"); }
            set { SetPropertyValue("FakeBONumber", value); }
        }
        public DateTime FakeBODate
        {
            get { return GetPropertyValue<DateTime>("FakeBODate"); }
            set { SetPropertyValue("FakeBODate", value); }
        }
        public Guid FakeBOID
        {
            get { return GetPropertyValue<Guid>("FakeBOID"); }
            set { SetPropertyValue("FakeBOID", value); }
        }
    }

    public class FakeBOW2Props: BusinessObject
    {
        public FakeObjectNotABo FakeObjectNotABo { get; set; }
        public string Prop1
        {
            get { return GetPropertyValueString("Prop1"); }
            set { SetPropertyValue("Prop1", value); }
        }
        public string Prop2
        {
            get { return GetPropertyValueString("Prop2"); }
            set { SetPropertyValue("Prop2", value); }
        }
        [AutoMapIgnore]
        public string ReflectiveProp { get; set; }
    }
    public class FakeBOW2HabiProps_TwoReflectiveProps: BusinessObject
    {
        public FakeObjectNotABo FakeObjectNotABo { get; set; }
        public string Prop1
        {
            get { return GetPropertyValueString("Prop1"); }
            set { SetPropertyValue("Prop1", value); }
        }
        public string Prop2
        {
            get { return GetPropertyValueString("Prop2"); }
            set { SetPropertyValue("Prop2", value); }
        }
        [AutoMapIgnore]
        public string ReflectiveProp { get; set; }
    }


    public class FakeBOWSingleRelationship : BusinessObject
    {
        public FakeObjectNotABo FakeObjectNotABo { get; set; }
        public string Prop1
        {
            get { return GetPropertyValueString("Prop1"); }
            set { SetPropertyValue("Prop1", value); }
        }

        public FakeBOW2Props FakeBOW2Props
        {
            get { return Relationships.GetRelatedObject<FakeBOW2Props>("FakeBOW2Props"); }
            set { Relationships.SetRelatedObject("FakeBOW2Props", value); }
        }
    }


    public class FakeBOWReflectiveProp: BusinessObject

    {
        public FakeObjectNotABo FakeObjectNotABo { get; set; }
        [AutoMapIgnore]
        public string ReflectiveProp { get; set; }
    }

    public class FakeObjectNotABo{}

    public class PropertyDescriptorStub: PropertyDescriptor{
        public PropertyDescriptorStub(string name, Attribute[] attrs) : base(name, attrs)
        {
        }

        public PropertyDescriptorStub(MemberDescriptor descr) : base(descr)
        {
        }

        public PropertyDescriptorStub(MemberDescriptor descr, Attribute[] attrs) : base(descr, attrs)
        {
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        /// <param name="component">The component to test for reset capability. </param>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        /// <param name="component">The component with the property for which to retrieve the value. </param>
        public override object GetValue(object component)
        {
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value. </param>
        public override void ResetValue(object component)
        {
            //Do Cancel edits I guess.
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set. </param><param name="value">The new value. </param>
        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        /// <param name="component">The component with the property to be examined for persistence. </param>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.
        /// </returns>
        public override Type ComponentType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <returns>
        /// true if the property is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the property.
        /// </returns>
        public override Type PropertyType
        {
            get { throw new NotImplementedException(); }
        }
    }
    // ReSharper restore UnusedMember.Global
    // ReSharper restore InconsistentNaming
    // ReSharper restore ClassNeverInstantiated.Global
}