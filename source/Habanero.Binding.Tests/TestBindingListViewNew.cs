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

// ReSharper disable InconsistentNaming

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestBindingListViewNew
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
            //ClassDef.ClassDefs.Clear();
            ClassDef.ClassDefs.Add(typeof (FakeBO).MapClasses());
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        #region Constructor

        [Test]
        public void Test_Construct_WithEmptyConstructor_ShouldNotRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
        }

        [Test]
        public void Test_Construct_ShouldSetUpProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
            Assert.IsTrue(bindingListView.AllowEdit);
            Assert.IsTrue(bindingListView.AllowNew);
            Assert.IsTrue(bindingListView.AllowRemove);
            Assert.IsFalse(bindingListView.IsFixedSize);
            Assert.IsTrue(bindingListView.IsReadOnly);
            Assert.IsTrue(bindingListView.SupportsAdvancedSorting);
            Assert.IsTrue(bindingListView.SupportsChangeNotification);
            Assert.IsTrue(bindingListView.SupportsFiltering);
            Assert.IsTrue(bindingListView.SupportsSearching);
            Assert.IsTrue(bindingListView.SupportsSorting);
        }

        [Test]
        public void Test_Construct_WithCollection_ShouldConstructWithCollection()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
            Assert.AreSame(collection, bindingListView.BusinessObjectCollection);
        }

        [Test]
        public void Test_Construct_WhenBusinessObjectCollectionIsNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new BindingListViewNew<FakeBO>(null);
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null.", ex.Message);
            }
        }

        #endregion

        #region TypedList

        /// <summary>
        /// This BindingListview is non heirachical i.e. it does not support
        /// having sub lists that are also bindable.
        /// </summary>
        [Test]
        public void Test_GetListName_ShouldReturnBindingListName()
        {
            //---------------Set up test pack-------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var listName = bindingListView.GetListName(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual("BindingListViewNew`1", listName);
        }

        [Test]
        public void Test_GetItemProperties_WhenNoViewBulderSet_ShouldReturnPropertiesFromTheDefaultViewBuilder()
        {
            //---------------Set up test pack-------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            var viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            var expectedPropDescriptors = new PropertyDescriptorCollectionStub();
            viewBuilder.Stub(builder => builder.GetPropertyDescriptors()).Return(expectedPropDescriptors);
            bindingListView.ViewBuilder = viewBuilder;
            //---------------Assert Precondition----------------
            Assert.AreSame(viewBuilder, bindingListView.ViewBuilder);
            //---------------Execute Test ----------------------
            var actualPropertyDescriptors = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropDescriptors, actualPropertyDescriptors);
        }

        #endregion

        #region ViewBuilder

        [Test]
        public void Test_ViewBuilder_WhenNoViewBulderSet_ShouldReturnUIDefViewBuilder()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<UIDefViewBuilder<FakeBO>>(bindingListView.ViewBuilder);
        }

        [Test]
        public void Test_ViewBuilder_WhenViewBulderSetToNull_ShouldReturnUIDefViewBuilder()
        {
            //---------------Set up test pack-------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingListView.ViewBuilder = null;
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<UIDefViewBuilder<FakeBO>>(bindingListView.ViewBuilder);
        }

        [Test]
        public void Test_SetViewBuilder_ShouldSetBuilder()
        {
            //---------------Set up test pack-------------------
            var bindingListView = new BindingListViewNew<FakeBO>();
            var expectedViewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            //---------------Assert Precondition----------------
            Assert.AreNotSame(expectedViewBuilder, bindingListView.ViewBuilder);
            //---------------Execute Test ----------------------
            bindingListView.ViewBuilder = expectedViewBuilder;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedViewBuilder, bindingListView.ViewBuilder);
        }

        [Test]
        public void Test_SetViewBuilder_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            var list = new BindingListViewSpy<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.IsNull(list.ViewBuilder);
            //---------------Execute Test ----------------------
            list.ViewBuilder = viewBuilder;
            //---------------Test Result -----------------------
            Assert.AreSame(viewBuilder, list.ViewBuilder);
        }

        [Test]
        public void Test_GetItemProperties_WhenHasViewBuilder_ShouldReturnViewBuidlersGetGridView()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            viewBuilder.Stub(builder => builder.GetPropertyDescriptors()).Return(
                new PropertyDescriptorCollection(new PropertyDescriptor[0]));
            var listViewSpy = new BindingListViewSpy<FakeBO>(collection) {ViewBuilder = viewBuilder};
            //---------------Assert Precondition----------------
            Assert.AreSame(viewBuilder, listViewSpy.ViewBuilder);
            //---------------Execute Test ----------------------
            var pds = listViewSpy.GetItemProperties(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, pds.Count);
        }

        [Test]
        public void Test_GetItemProperties_WhenNotHasViewBuilder_TypeDescriptorGetProperties()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var listViewSpy = new BindingListViewSpy<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.IsNull(listViewSpy.ViewBuilder);
            //---------------Execute Test ----------------------
            var pds = listViewSpy.GetItemProperties(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            var propertyInfos = typeof (FakeBO).GetProperties();
            Assert.AreEqual(propertyInfos.Length, pds.Count);
        }

        #endregion

        #region IBindingListView

        [Test]
        public void Test_IsSynchronized_ShouldReturn_BusinessObjectCollectionIsSynchronized()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isSynchronized = bindingListView.IsSynchronized;
            //---------------Test Result -----------------------
            Assert.AreEqual(((IBusinessObjectCollection) collection).IsSynchronized, isSynchronized);
        }

        [Test]
        public void Test_SyncRoot_ShouldReturn_BusinessObjectCollectionSyncRoot()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var expectedSyncRoot = ((IBusinessObjectCollection) collection).SyncRoot;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var syncRoot = bindingListView.SyncRoot;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedSyncRoot, syncRoot);
        }

        #region Indexer

        [Test]
        public void Test_GetBOIndex3_WhenCollectionHas5_ShouldReturnFourthBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            var returnedBO = bindingListView[3];
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedBO);
            Assert.AreSame(collection[3], returnedBO);
        }

        [Test]
        public void Test_GetBO_WhenCollectioHas5And3Loaded_ShouldReturnThirdBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            collection.LoadWithLimit("", "", 0, 3, out noOfRecords);
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(5, noOfRecords);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            var returnedBO = bindingListView[2];
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedBO);
            Assert.AreSame(collection[2], returnedBO);
        }

#pragma warning disable 168
        [Test]
        public void Test_GetFourthBO_WhenCollectioHas5And3Loaded_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            collection.LoadWithLimit("", "", 0, 3, out noOfRecords);
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(5, noOfRecords);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            try
            {
                var fakeBO = bindingListView[3];
                Assert.Fail("Expected to throw an ArgumentOutOfRangeException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentOutOfRangeException ex)
            {
                StringAssert.Contains(
                    "Index was out of range. Must be non-negative and less than the size of the collection.", ex.Message);
            }
        }

        [Test]
        public void Test_GetSixthBO_WhenCollectioHas5_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            try
            {
                var fakeBO = bindingListView[6];
                Assert.Fail("Expected to throw an ArgumentOutOfRangeException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentOutOfRangeException ex)
            {
                StringAssert.Contains(
                    "Index was out of range. Must be non-negative and less than the size of the collection.", ex.Message);
            }
        }


        [Test]
        public void Test_Set_Item_ShouldSetItemInCollection()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            //---------------Execute Test ----------------------
            var newFakeBo = new FakeBO();
            bindingListView[2] = newFakeBo;
            //---------------Test Result -----------------------
            Assert.AreSame(newFakeBo, bindingListView[2]);
            Assert.AreSame(newFakeBo, collection[2]);
        }

        [Test]
        public void Test_Set_Item_ShouldRaise_ListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            var newFakeBo = new FakeBO();
            bindingListView[2] = newFakeBo;
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

#pragma warning restore 168

        #endregion Indexer

        [Test]
        public void Test_Clear_WhenCollectionHas5_ShouldRemoveItemsFromCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Clear();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, bindingListView.Count);
        }

        [Test]
        public void Test_Clear_ShouldRiaseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.Clear();
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        [Test]
        public void Test_Contains_WhenCollectionHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            //---------------Execute Test ----------------------
            bool contains = bindingListView.Contains(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.IsTrue(contains);
        }

        [Test]
        public void Test_Contains_WhenCollectionNotHas_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            //---------------Execute Test ----------------------
            var contains = bindingListView.Contains(new FakeBO());
            //---------------Test Result -----------------------
            Assert.IsFalse(contains);
        }

        [Test]
        public void Test_Remove_WhenColHas1_ShouldRemoveBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(1, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, bindingListView.Count);
        }

        [Test]
        public void Test_Remove_WhenColHas5_ShouldRemoveOneBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_IndexOf_WhenColHas1_ShouldReturnIndexOfBO()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWithOneItem();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var itemAtZeroIndex = bindingListView[0];
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var indexOf = bindingListView.IndexOf(itemAtZeroIndex);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, indexOf);
        }

        [Test]
        public void Test_IndexOf_WhenColHas5AndThirdSelected_ShouldReturnIndexTwoOfBO()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var itemAtIndex2 = bindingListView[2];
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var indexOf = bindingListView.IndexOf(itemAtIndex2);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, indexOf);
        }

        [Test]
        public void Test_Remove_ShouldFireListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        [Test]
        public void Test_Remove_ShouldFireListChangedEvent_WithCurrentIndex()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var indexRemoved = -1;
            bindingListView.ListChanged += (sender, args) => indexRemoved = args.NewIndex;
            const int expectedIndex = 2;
            var itemToRemove = bindingListView[expectedIndex];
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            Assert.AreEqual(-1, indexRemoved);
            //---------------Execute Test ----------------------
            bindingListView.Remove(itemToRemove);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedIndex, indexRemoved);
        }

        [Test]
        public void Test_RemoveAt_WhenColHas1_ShouldRemoveBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(1, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(0);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(0, bindingListView.Count);
        }

        [Test]
        public void Test_RemoveAt_WhenColHas5_ShouldRemoveOneBusinessObject()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var itemToRemove = bindingListView[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(2);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.IsFalse(bindingListView.Contains(itemToRemove), "Should not contain removed item");
        }

        [Test]
        public void Test_RemoveAt_ShouldFireListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith5Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(2);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        [Test]
        public void Test_Insert_WhenColHas3_ShouldInsertObjectAtIndex3()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var boToInsert = new FakeBO();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(3, boToInsert);
            //---------------Test Result -----------------------
            Assert.AreSame(boToInsert, bindingListView[3]);
            Assert.AreEqual(3, bindingListView.IndexOf(boToInsert));
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3_ShouldInsertObjectAtIndex1()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var boToInsert = new FakeBO();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(1, boToInsert);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView[1]);
            Assert.IsNotNull(bindingListView[3]);
            Assert.AreEqual(1, bindingListView.IndexOf(boToInsert));
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3AndObjectInsertedAtIndex5_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var itemToInsert = new FakeBO();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            try
            {
                bindingListView.Insert(5, itemToInsert);
                Assert.Fail("Expected to throw an ArgumentOutOfRangeException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentOutOfRangeException ex)
            {
                StringAssert.Contains("Index must be within the bounds of the List.", ex.Message);
            }
        }

        [Test]
        public void Test_Insert_ShouldFireListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var itemToInsert = new FakeBO();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.Insert(3, itemToInsert);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        #endregion

        #region IList

        [Test]
        public void Test_CopyTo_WhenArrayNull_ShouldThrowArgumentNullException()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            try
            {
                bindingListView.CopyTo(null, 0);
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("array", ex.Message);
            }
        }

        [Test]
        public void Test_CopyTo_WhenArrayIndexLessThanZero_ShouldArgumentOutOfRangeException()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var array = new object[] {};
            //---------------Assert Precondition----------------
            try
            {
                bindingListView.CopyTo(array, -1);
                Assert.Fail("Expected to throw an ArgumentOutOfRangeException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentOutOfRangeException ex)
            {
                StringAssert.Contains("index", ex.Message);
            }
        }

        [Test]
        public void Test_CopyTo_WhenArrayIndexGreaterThanArrayLength_ShouldArgumentException()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var array = new object[] {};
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, array.Length);
            try
            {
                bindingListView.CopyTo(array, 1);
                Assert.Fail("Expected to throw an ArgumentException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("index", ex.Message);
            }
        }

        [Test]
        public void Test_CopyTo_WhenArrayIndexEqualToArrayLength_ShouldArgumentException()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var array = new object[] {};
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, array.Length);
            try
            {
                bindingListView.CopyTo(array, 0);
                Assert.Fail("Expected to throw an ArgumentException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("index", ex.Message);
            }
        }

        [Test]
        public void Test_CopyTo_WhenColHas3Objets_ShouldCopyObjectsToArray()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var array = new object[4];
            const int index = 1;
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, array.Length);
            Assert.Greater(array.Length, index);
            Assert.IsNotNull(collection);
            Assert.IsNull(array[1]);
            //---------------Execute Test ----------------------
            bindingListView.CopyTo(array, index);
            //---------------Test Result -----------------------
            Assert.IsNotNull(array[1]);
        }

        #endregion

        #region Find

        [Test]
        public void Test_Find_WhenPropertyDescriptorIsNull_ShoudRaiseError()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            try
            {
                string key = RandomValueGen.GetRandomString(2, 8);
                bindingListView.Find(null, key);
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("property", ex.Message);
            }
        }

        [Test]
        public void Test_Find_WhenKeyIsNull_ShoudRaiseError()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            const int propDescriptorIndex = 2;
            PropertyDescriptor propertyDescriptor = descriptorCollection[propDescriptorIndex];
            //---------------Assert Precondition----------------
            Assert.AreEqual(9, descriptorCollection.Count);
            try
            {
                bindingListView.Find(propertyDescriptor, null);
                Assert.Fail("Expected to throw an ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("key", ex.Message);
            }
        }

        [Test]
        public void Test_Find_WhenPropertyNameIsNotValid_ShoudRaiseError()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            const string key = "FakeBONotABo";
            const int propDescriptorIndex = 2;
            PropertyDescriptor propertyDescriptor = descriptorCollection[propDescriptorIndex];
            //---------------Assert Precondition----------------
            Assert.AreEqual(9, descriptorCollection.Count);
            try
            {
                bindingListView.Find(propertyDescriptor, key);
                Assert.Fail("Expected to throw an InvalidPropertyNameException");
            }
                //---------------Test Result -----------------------
            catch (InvalidPropertyNameException ex)
            {
                StringAssert.Contains(
                    "The given property name '" + propertyDescriptor.Name +
                    "' does not exist in the collection of properties for the class", ex.Message);
            }
        }

        [Test]
        public void Test_Find_ShouldReturnIndexOfRowContainingPropertyDescriptor()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            const int propDescriptorIndex = 1;
            var propertyDescriptor = descriptorCollection[propDescriptorIndex];
            var key = propertyDescriptor.GetValue(collection[1]);
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
            //---------------Execute Test ----------------------
            var index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, propDescriptorIndex);
        }

        [Test]
        public void Test_Find_WhenKeyNotFound_ShouldReturnIndexOfNegativeOne()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var boNotFound = new FakeBO();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            const int propDescriptorIndex = 1;
            var propertyDescriptor = descriptorCollection[propDescriptorIndex];
            var key = propertyDescriptor.GetValue(boNotFound);
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
            //---------------Execute Test ----------------------
            var index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, -1);
        }

        #endregion

        #region BindingList.Sort

        [Test]
        public void Test_SortDirection_WhenListSortDescriptionCollectionHasZero_ShouldSetSortDirectionAscending()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            var listSortDirection = bindingListView.SortDirection;
            //---------------Test Result -----------------------
            Assert.AreEqual(listSortDirection, ListSortDirection.Ascending);
        }

        [Test]
        public void Test_SortDirection_WhenListSortDescriptionCollectionHas1_ShouldSetSortDirection()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            PropertyDescriptor descriptor = null;
            var listSortDescription = new ListSortDescription(descriptor, ListSortDirection.Descending);
            var descriptions = new[] {listSortDescription};
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(bindingListView.SortDescriptions);
            Assert.AreEqual(1, descriptionCollection.Count);
            //---------------Execute Test ----------------------
            var listSortDirection = bindingListView.SortDirection;
            //---------------Test Result -----------------------
            Assert.AreEqual(descriptionCollection[0].SortDirection, listSortDirection);
        }

        [Test]
        public void Test_SortDirection_WhenListSortDescriptionCollectionHas2_ShouldSetSortDirectionAscending()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            PropertyDescriptor descriptor = null;
            var listSortDescription1 = new ListSortDescription(descriptor, ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptor, ListSortDirection.Descending);
            var descriptions = new[] {listSortDescription1, listSortDescription2};
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(bindingListView.SortDescriptions);
            Assert.AreEqual(2, descriptionCollection.Count);
            //---------------Execute Test ----------------------
            var listSortDirection = bindingListView.SortDirection;
            //---------------Test Result -----------------------
            Assert.AreEqual(listSortDirection, ListSortDirection.Ascending);
        }

        [Test]
        public void Test_SortProperty_WhenListSortDescriptionCollectionHasZero_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            var sortProperty = bindingListView.SortProperty;
            //---------------Test Result -----------------------
            Assert.IsNull(sortProperty);
        }

        [Test]
        public void Test_SortProperty_WhenListSortDescriptionCollectionHas1_ShouldReturnSortProperty()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] {listSortDescription};
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(bindingListView.SortDescriptions);
            Assert.AreEqual(1, descriptionCollection.Count);
            //---------------Execute Test ----------------------
            var sortProperty = bindingListView.SortProperty;
            //---------------Test Result -----------------------
            Assert.IsNotNull(sortProperty);
            Assert.AreSame(descriptorCollection[0], sortProperty);
        }

        [Test]
        public void Test_SortProperty_WhenListSortDescriptionCollectionHas2_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription1 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] {listSortDescription1, listSortDescription2};
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(bindingListView.SortDescriptions);
            Assert.AreEqual(2, descriptionCollection.Count);
            //---------------Execute Test ----------------------
            var sortProperty = bindingListView.SortProperty;
            //---------------Test Result -----------------------
            Assert.IsNull(sortProperty);
        }

        [Test]
        public void Test_IsSorted_WhenSortDescriptionsAreGreaterThanZero_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription1 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] {listSortDescription1, listSortDescription2};
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(9, bindingListView.SortDescriptions.Count);
            //---------------Execute Test ----------------------
            var isSorted = bindingListView.IsSorted;
            //---------------Test Result -----------------------
            Assert.True(isSorted);
        }

        [Test]
        public void Test_IsSorted_WhenSortDescriptionsNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var collection = GetCollectionWith3Items();
            var bindingListView = new BindingListViewNew<FakeBO>(collection) {SortDescriptions = null};
            //---------------Assert Precondition----------------
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            var isSorted = bindingListView.IsSorted;
            //---------------Test Result -----------------------
            Assert.False(isSorted);
        }

        [Test]
        public void Test_IsSorted_WhenSortDescriptionsAreLessThanOrEqualToZero_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var collection = new BusinessObjectCollection<FakeBO>();
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptionCollection = new ListSortDescriptionCollection(null);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, bindingListView.SortDescriptions.Count);
            //---------------Execute Test ----------------------
            var isSorted = bindingListView.IsSorted;
            //---------------Test Result -----------------------
            Assert.False(isSorted);
        }

        [Test]
        public void Test_ApplySort_WhenListSortDirectionAscending_WithPropertyDescriptor_ShouldSortByPropertyDescriptor()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName DESC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection)
                                      {ViewBuilder = new DefaultViewBuilder<FakeBO>()};
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var sortByDescriptor = descriptorCollection[1];
            var fakeBo1 = collection[0];
            var fakeBo2 = collection[1];
            var fakeBo3 = collection[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortByDescriptor, ListSortDirection.Ascending);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
        }

        [Test]
        public void
            Test_ApplySort_WhenListSortDirectionDescscending_WithPropertyDescriptor_ShouldSortByPropertyDescriptor()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName ASC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection)
                                      {ViewBuilder = new DefaultViewBuilder<FakeBO>()};
            var descriptorCollection = bindingListView.GetItemProperties(null);
            var propertyDescriptor1 = descriptorCollection[1];
            var fakeBo1 = collection[0];
            var fakeBo2 = collection[1];
            var fakeBo3 = collection[2];
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(propertyDescriptor1, ListSortDirection.Descending);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
        }

        [Test]
        public void Test_ApplySort_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName DESC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection)
                                      {ViewBuilder = new DefaultViewBuilder<FakeBO>()};
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var propertyDescriptor = descriptorCollection[1];

            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(propertyDescriptor, ListSortDirection.Ascending);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        [Test]
        public void Test_ApplySort_WithCollection_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName DESC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection)
                                      {ViewBuilder = new DefaultViewBuilder<FakeBO>()};
            var sortCol = new ListSortDescriptionCollection();
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortCol);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        [Test]
        public void Test_ApplySort_WhenListSortDirectionDescending_WithListSortDescriptionCollection_ShouldSortByPropertyDescriptors()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName ASC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            var sortByDescriptor = descriptorCollection[1];
            var listSortDescription = new ListSortDescription(sortByDescriptor, ListSortDirection.Descending);
            var sortDescriptions = new ListSortDescriptionCollection(new[]{ listSortDescription});
            var fakeBo1 = collection[0];
            var fakeBo2 = collection[1];
            var fakeBo3 = collection[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            Assert.AreEqual(2, descriptorCollection.Count);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
        }

        [Test]
        public void Test_ApplySort_WhenListSortDirectionAscending_WithListSortDescriptionCollection_ShouldSortByPropertyDescriptors()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName DESC");
            var bindingListView = new BindingListViewNew<FakeBO>(collection) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var sortByDescriptor = descriptorCollection[1];
            var listSortDescription = new ListSortDescription(sortByDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });
            var fakeBo1 = collection[0];
            var fakeBo2 = collection[1];
            var fakeBo3 = collection[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
            Assert.Fail("Test Not Yet Implemented");
        }
        [Test]
        public void Test_ApplySort_WithListSortDescriptionCollection_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }
        [Test]
        public void Test_ApplySort_ShouldNotSortUnderlyingCollection()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }
        #endregion

        private static BusinessObjectCollection<FakeBO> GetCollectionWith3Items()
        {
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            return collection;
        }

        private static BusinessObjectCollection<FakeBO> GetCollectionWithOneItem()
        {
            CreateSavedBOs(1);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            return collection;
        }

        private static BusinessObjectCollection<FakeBO> GetCollectionWith5Items()
        {
            CreateSavedBOs(5);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            return collection;
        }

        private static void CreateSavedBOs(int numberToCreate)
        {
            for (var i = 0; i < numberToCreate; i++)
            {
                var businessObject = new FakeBO
                                         {
                                             FakeBOName = "A" + RandomValueGen.GetRandomString()
                                         };
                businessObject.Save();
            }
        }
    }

    internal class PropertyDescriptorCollectionStub : PropertyDescriptorCollection
    {
        public PropertyDescriptorCollectionStub() : base(new PropertyDescriptor[0])
        {
        }
    }
}