using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.CF;
using Habanero.ProgrammaticBinding;
using Habanero.ProgrammaticBinding.CF;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestHabaneroSelectorControlBinder
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
        public void Test_Construct_ShouldCreate()
        {
            //---------------Set up test pack-------------------
            var lstBox = new ListBox();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            new HabaneroSelectorControlBinder<FakeBo, ListBox>(lstBox);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it constructed all is good.");
        }

        [Test]
        public void Test_BindBusinessObjectCollection_ToListBox_ShouldAddToControlList()
        {
            //---------------Set up test pack-------------------
            var lstBox = new ListBox();
            var selectorBinder = new HabaneroSelectorControlBinder<FakeBo, ListBox>(lstBox);
            var businessObjectCollection = new BusinessObjectCollection<FakeBo>();
            businessObjectCollection.CreateBusinessObject();
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, lstBox.Items.Count);
            //---------------Execute Test ----------------------
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, lstBox.Items.Count, "The business object collection's items should be in list");
        }
        
        [Test]
        public void Test_AddToBusinessObjectCollection_ShouldAddToControlList()
        {
            //---------------Set up test pack-------------------
            var lstBox = new ListBox();
            IHabaneroSelectorControlBinder<FakeBo> selectorBinder = new HabaneroSelectorControlBinder<FakeBo, ListBox>(lstBox);
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, lstBox.Items.Count);
            //---------------Execute Test ----------------------
            businessObjectCollection.Add(new FakeBo());
            //---------------Test Result -----------------------
            Assert.AreEqual(4, lstBox.Items.Count, "should have added new item");
        }

        [Test]
        public void Test_RemvoveToBusinessObjectCollection_ShouldRemoveFromControlList()
        {
            //---------------Set up test pack-------------------
            var lstBox = new ListBox();
            var selectorBinder = new HabaneroSelectorControlBinder<FakeBo, ListBox>(lstBox);
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, lstBox.Items.Count);
            //---------------Execute Test ----------------------
            businessObjectCollection.Remove(businessObjectCollection[1]);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, lstBox.Items.Count, "should have removed item");
        }

        [Test]
        public void Test_SetSelectedBusinessObject_SetsSelectedBO()
        {
            //---------------Set up test pack-------------------
            IHabaneroSelectorControlBinder<FakeBo> selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var boToSelect = businessObjectCollection[2];

            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boToSelect);
            Assert.AreNotSame(boToSelect, selectorBinder.SelectedBusinessObject);
            //---------------Execute Test ----------------------
            selectorBinder.SelectedBusinessObject = boToSelect;
            //---------------Test Result -----------------------
            Assert.AreSame(boToSelect, selectorBinder.SelectedBusinessObject);
        }
        [Test]
        public void Test_SetSelectedBusinessObject_RaisesBusinessObjectSelectedEvent()
        {
            //---------------Set up test pack-------------------
            var selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var boToSelect = businessObjectCollection[2];

            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            bool boSelectedEventCalled  =false;
            selectorBinder.BusinessObjectSelected += (sender, args) => boSelectedEventCalled = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(boSelectedEventCalled);
            //---------------Execute Test ----------------------
            selectorBinder.SelectedBusinessObject = boToSelect;
            //---------------Test Result -----------------------
            Assert.IsTrue(boSelectedEventCalled);
        }
        [Test]
        public void Test_SetSelectedBusinessObject_SetsSelectedBO_OnControl()
        {
            //---------------Set up test pack-------------------
            var selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var boToSelect = businessObjectCollection[2];
            var listControl = selectorBinder.ListControl;

            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boToSelect);
            Assert.AreNotSame(boToSelect, listControl.SelectedItem);
            //---------------Execute Test ----------------------
            selectorBinder.SelectedBusinessObject = boToSelect;
            //---------------Test Result -----------------------
            Assert.AreSame(boToSelect, listControl.SelectedItem);
        }
        [Test]
        public void Test_BindHabaneroControlBinder_ThenSetSelectedItem_ShouldSetBusinessObjectControlBinder()
        {
            //---------------Set up test pack-------------------
            IHabaneroSelectorControlBinder<FakeBo> selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            var boToSelect = businessObjectCollection[2];

            var controlBinder = new HabaneroControlBinder<FakeBo>();
            //---------------Assert Precondition----------------
            Assert.IsNull(controlBinder.BusinessObject);
            Assert.IsNotNull(boToSelect);
            //---------------Execute Test ----------------------
            selectorBinder.BindEditorControlBinder(controlBinder);
            selectorBinder.SelectedBusinessObject = boToSelect;
            //---------------Test Result -----------------------
            Assert.AreSame(boToSelect, controlBinder.BusinessObject);
        }
        [Test]
        public void Test_BindHabaneroControlBinder_ShouldReturnHabaneroControlBinder()
        {
            //---------------Set up test pack-------------------
            IHabaneroSelectorControlBinder<FakeBo> selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            var boToSelect = businessObjectCollection[2];

            var controlBinder = new HabaneroControlBinder<FakeBo>();
            //---------------Assert Precondition----------------
            Assert.IsNull(controlBinder.BusinessObject);
            Assert.IsNotNull(boToSelect);
            //---------------Execute Test ----------------------
            var returnedBinder = selectorBinder.BindEditorControlBinder(controlBinder);
            //---------------Test Result -----------------------
            Assert.AreSame(returnedBinder, controlBinder);
        }

        [Test]
        public void Test_BindHabaneroControlBinder_ThenSelectItemViaControl_ShouldSetBusinessObjectControlBinder()
        {
            //---------------Set up test pack-------------------
            IHabaneroSelectorControlBinder<FakeBo> selectorBinder = CreateHabaneroSelectorControlBinder();
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            var boToSelect = businessObjectCollection[2];

            var controlBinder = new HabaneroControlBinder<FakeBo>();
            //---------------Assert Precondition----------------
            Assert.IsNull(controlBinder.BusinessObject);
            Assert.IsNotNull(boToSelect);
            //---------------Execute Test ----------------------
            selectorBinder.BindEditorControlBinder(controlBinder);
            selectorBinder.ListControl.SelectedItem = boToSelect;
            //---------------Test Result -----------------------
            Assert.AreSame(boToSelect, controlBinder.BusinessObject);
        }

        [Test]
        public void Test_SetSelectedBusinessObject_WithComboBox_SetsSelectedBO_OnControl()
        {
            //---------------Set up test pack-------------------
            var comboBox = new ComboBox();
            var selectorBinder = new HabaneroSelectorControlBinder<FakeBo, ComboBox>(comboBox);
            var businessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var boToSelect = businessObjectCollection[2];
            var listControl = selectorBinder.ListControl;

            selectorBinder.SetBusinessObjectCollection(businessObjectCollection);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boToSelect);
            Assert.AreNotSame(boToSelect, listControl.SelectedItem);
            //---------------Execute Test ----------------------
            selectorBinder.SelectedBusinessObject = boToSelect;
            //---------------Test Result -----------------------
            Assert.AreSame(boToSelect, listControl.SelectedItem);
        }

        private static BusinessObjectCollection<FakeBo> GetBusinessObjectCollectionWith3Items()
        {
            var businessObjectCollection = new BusinessObjectCollection<FakeBo>();
            businessObjectCollection.CreateBusinessObject();
            businessObjectCollection.CreateBusinessObject();
            businessObjectCollection.CreateBusinessObject();
            return businessObjectCollection;
        }

        private static HabaneroSelectorControlBinder<FakeBo, ListBox> CreateHabaneroSelectorControlBinder()
        {
            var lstBox = new ListBox();
            return new HabaneroSelectorControlBinder<FakeBo, ListBox>(lstBox);
        }

    }
}
// ReSharper restore InconsistentNaming