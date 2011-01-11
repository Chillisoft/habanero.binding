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
    public class TestPropertyDescriptorRelatedPropDef
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

        private IUIGridColumn GetGridColumnStub(IClassDef classDef)
        {
            IUIGridColumn gridColumn = MockRepository.GenerateStub<IUIGridColumn>();
            gridColumn.PropertyName = RandomValueGen.GetRandomString();
            gridColumn.Stub(column => column.ClassDef).Return(classDef);
            return gridColumn;
        }
        [Test]
        public void Test_GetValue_ShouldGetValueFromBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBOWSingleRelationship);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOW2Props.Prop1";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBOW2Props fakeBO = new FakeBOW2Props {Prop1 = RandomValueGen.GetRandomString()};
            FakeBOWSingleRelationship fakeBOwSingleRelationship = new FakeBOWSingleRelationship {FakeBOW2Props = fakeBO};
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef, gridColumn.ClassDef);
            Assert.AreSame(typeof(FakeBOWSingleRelationship), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.Prop1);
            //---------------Execute Test ----------------------
            var actualValue = propDescriptor.GetValue(fakeBOwSingleRelationship);
            //---------------Test Result -----------------------
            Assert.AreEqual(fakeBO.Prop1, actualValue);
        }

        [Test]
        public void Test_SetValue_ShouldSetValueOnBO()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = MockRepository.GenerateStub<IClassDef>();
            classDef.ClassType = typeof(FakeBOWSingleRelationship);
            var gridColumn = GetGridColumnStub(classDef);
            gridColumn.PropertyName = "FakeBOW2Props.Prop1";
            PropertyDescriptorPropDef propDescriptor = new PropertyDescriptorPropDef(gridColumn);
            FakeBOW2Props fakeBO = new FakeBOW2Props { Prop1 = RandomValueGen.GetRandomString() };
            FakeBOWSingleRelationship fakeBOwSingleRelationship = new FakeBOWSingleRelationship { FakeBOW2Props = fakeBO };
            //---------------Assert Precondition----------------
            Assert.AreSame(classDef, gridColumn.ClassDef);
            Assert.AreSame(typeof(FakeBOWSingleRelationship), gridColumn.ClassDef.ClassType);
            Assert.IsNotNullOrEmpty(fakeBO.Prop1);
            //---------------Execute Test ----------------------
            var expectedValue = RandomValueGen.GetRandomString();
            propDescriptor.SetValue(fakeBOwSingleRelationship, expectedValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedValue, fakeBO.Prop1);
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