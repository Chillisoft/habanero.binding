using System;
using System.Collections.Generic;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;
using Habanero.Testability.Helpers;

// ReSharper disable InconsistentNaming

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestBindingListView
    {
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerFake();
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
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
            var bindingListView = new BindingListView<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
        }

        [Test]
        public void Test_Construct_ShouldSetUpProperties()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bindingListView = new BindingListView<FakeBO>();
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
            var boCol = GenerateStub<BusinessObjectCollection<FakeBO>>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
            Assert.AreSame(boCol, bindingListView.BusinessObjectCollection);
        }

        [Test]
        public void Test_Construct_WithViewBuilder_ShouldSetViewBuilder()
        {
            //---------------Set up test pack-------------------

            var boCol = GenerateStub<BusinessObjectCollection<FakeBO>>();
            var viewBuilder = GenerateStub<IViewBuilder>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var bindingListView = new BindingListView<FakeBO>(boCol, viewBuilder);
            //---------------Test Result -----------------------
            Assert.AreSame(viewBuilder, bindingListView.ViewBuilder);
        }
        [Test]
        public void Test_Construct_WhenBusinessObjectCollectionIsNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new BindingListView<FakeBO>(null);
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
            var bindingListView = new BindingListView<FakeBO>();
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
            var bindingListView = new BindingListView<FakeBO>();
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
            var bindingListView = new BindingListView<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<UIDefViewBuilder<FakeBO>>(bindingListView.ViewBuilder);
        }

        [Test]
        public void Test_ViewBuilder_WhenViewBulderSetToNull_ShouldReturnUIDefViewBuilder()
        {
            //---------------Set up test pack-------------------
            var bindingListView = new BindingListView<FakeBO>();
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
            var bindingListView = new BindingListView<FakeBO>();
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
            var boCol = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            var list = new BindingListViewSpy<FakeBO>(boCol);
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
            var boCol = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            viewBuilder.Stub(builder => builder.GetPropertyDescriptors()).Return(
                new PropertyDescriptorCollection(new PropertyDescriptor[0]));
            var listViewSpy = new BindingListViewSpy<FakeBO>(boCol) {ViewBuilder = viewBuilder};
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
            var boCol = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var listViewSpy = new BindingListViewSpy<FakeBO>(boCol);
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
            var boCol = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isSynchronized = bindingListView.IsSynchronized;
            //---------------Test Result -----------------------
            Assert.AreEqual(((IBusinessObjectCollection) boCol).IsSynchronized, isSynchronized);
        }

        [Test]
        public void Test_SyncRoot_ShouldReturn_BusinessObjectCollectionSyncRoot()
        {
            //---------------Set up test pack-------------------
            var boCol = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            BindingListViewSpy<FakeBO> bindingListView = new BindingListViewSpy<FakeBO>(boCol);
            var expectedSyncRoot = ((IBusinessObjectCollection)bindingListView.GetViewOfBusinessObjectCollection()).SyncRoot;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var syncRoot = bindingListView.SyncRoot;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedSyncRoot, syncRoot);
        }

        #region Indexer

        [Test]
        public void Test_GetBOIndex3_WhenCollectionHas5_ShouldReturnFourthBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            var returnedBO = bindingListView[3];
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedBO);
            Assert.AreSame(boCol[3], returnedBO);
        }

        [Test]
        public void Test_GetBO_WhenCollectioHas5And3Loaded_ShouldReturnThirdBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var boCol = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            boCol.LoadWithLimit("", "", 0, 3, out noOfRecords);
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
            Assert.AreEqual(5, noOfRecords);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            var returnedBO = bindingListView[2];
            //---------------Test Result -----------------------
            Assert.IsNotNull(returnedBO);
            Assert.AreSame(boCol[2], returnedBO);
        }

#pragma warning disable 168
        [Test]
        public void Test_GetFourthBO_WhenCollectioHas5And3Loaded_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var boCol = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            boCol.LoadWithLimit("", "", 0, 3, out noOfRecords);
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
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
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
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
        public void Test_Set_Item_ShouldSetItemInBindingList()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            var newFakeBo = new FakeBO();
            bindingListView[2] = newFakeBo;
            //---------------Test Result -----------------------
            Assert.AreSame(newFakeBo, bindingListView[2]);
        }
        [Test]
        public void Test_Set_Item_ShouldSetItemInUnderlyingCollection()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var descriptor = GetDescriptor(bindingListView, "FakeBOName");
            bindingListView.ApplySort(descriptor, ListSortDirection.Ascending);
            const int index = 2;
            var origItemAtIndex = bindingListView[index];
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            //---------------Execute Test ----------------------
            var newFakeBo = new FakeBO();
            bindingListView[index] = newFakeBo;
            //---------------Test Result -----------------------
            boCol.ShouldContain(newFakeBo);
            boCol.ShouldNotContain(origItemAtIndex);
        }
        [Test]
        public void Test_Set_Item_ShouldRaise_ListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
        public void Test_Clear_WhenCollectionHas5_ShouldRemoveItemsFromBindignListView()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Clear();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, bindingListView.Count);
            Assert.AreEqual(5, boCol.Count);
        }

        [Test]
        public void Test_Clear_ShouldRiaseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
        public void Test_Add_ShouldAddObjectToBindingListView()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var fakeBO = new FakeBO();
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            var addedIndex = bindingListView.Add(fakeBO);
            //---------------Test Result -----------------------
            Assert.AreEqual(6, bindingListView.Count);
            Assert.AreEqual(5, addedIndex, "Should add at end of collection");
        }

        [Test]
        public void Test_Add_ShouldAddObjectToUnderlyingBOCol()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var fakeBO = new FakeBO();
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Add(fakeBO);
            //---------------Test Result -----------------------
            Assert.AreEqual(6, bindingListView.Count);
            Assert.AreEqual(6, boCol.Count);
        }
        [Test]
        public void Test_Contains_WhenCollectionHas_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
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
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            //---------------Execute Test ----------------------
            var contains = bindingListView.Contains(new FakeBO());
            //---------------Test Result -----------------------
            Assert.IsFalse(contains);
        }


        [Test]
        public void Test_Remove_WhenColHas1_ShouldRemoveBusinessObjectFromBindingList()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, bindingListView.Count);
        }
        /// <summary>
        /// This does not affect the Business Object or the underlying Business Object Collection
        /// as the Data Grid does not call this method. If you want to remove or delete the Business Obect
        /// in the underlying collection, please use the RemoveAt instead. (See <see cref="Test_RemoveAt_ShouldDeleteBusinessObject"/>
        /// </summary>
        [Test]
        public void Test_Remove_WhenColHas1_ShouldNotRemoveFromUnderlyingCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, boCol.Count);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, boCol.Count);
        }

        [Test]
        public void Test_Remove_WhenColHas5_ShouldRemoveOneBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Remove(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.AreEqual(5, boCol.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Remove_ShouldFireListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
        public void Test_IndexOf_WhenColHas1_ShouldReturnIndexOfBO()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWithOneItem();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var itemAtIndex2 = bindingListView[2];
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var indexOf = bindingListView.IndexOf(itemAtIndex2);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, indexOf);
        }
        [Test]
        public void Test_RemoveAt_WhenColHas1_ShouldRemoveBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBO();
            var boCol = GetLoadedCollection<FakeBO>();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(0);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, bindingListView.Count);
        }
        /// <summary>
        /// The RemoveAt is called by the DataGridView when a BO is deleted from the 
        /// Data Grid using the Delete Key.
        /// </summary>
        [Test]
        public void Test_RemoveAt_ShouldDeleteBusinessObject()
        {
            var boToBeRemoved = CreateSavedBO();
            var boCol = GetLoadedCollection<FakeBO>();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, boCol.Count);
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsFalse(boToBeRemoved.Status.IsDeleted);
            Assert.IsFalse(boToBeRemoved.Status.IsNew);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(0);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, boCol.Count);
            Assert.IsTrue(boToBeRemoved.Status.IsDeleted, "Should be marked for delete");
            Assert.IsTrue(boToBeRemoved.Status.IsNew, "The Delete should be persisted - New Deleted Object has been deleted to DataStore");
            Assert.IsFalse(boToBeRemoved.Status.IsDirty, "The Delete should be persisted");
        }
        
        [Test]
        public void Test_RemoveAt_WhenColHas5_ShouldRemoveOneBusinessObject()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var itemToRemove = bindingListView[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, boCol.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(2);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, boCol.Count);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.IsFalse(bindingListView.Contains(itemToRemove), "Should not contain removed item");
        }

        [Test]
        public void Test_RemoveAt_ShouldFireListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith5Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var boToInsert = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(3, boToInsert);
            //---------------Test Result -----------------------
            Assert.AreSame(boToInsert, bindingListView[3]);
            Assert.AreEqual(3, bindingListView.IndexOf(boToInsert));
            Assert.AreEqual(4, boCol.Count, "Should insert item into underlying Business Object Collection");
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3_ShouldInsertObjectAtIndex1()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var boToInsert = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(1, boToInsert);
            //---------------Test Result -----------------------
            Assert.AreSame(boToInsert, bindingListView[1]);
            Assert.IsNotNull(bindingListView[3], "The last item should be moved down");
            Assert.AreEqual(1, bindingListView.IndexOf(boToInsert));
            Assert.AreEqual(4, boCol.Count, "Should insert item into underlying Business Object Collection");
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3_ShouldAddObjectTounderlyingCollection()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var boToInsert = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(1, boToInsert);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, boCol.Count);
            boCol.ShouldContain(boToInsert);
        }

        [Test]
        public void Test_Insert_WhenColHas3AndObjectInsertedAtIndex5_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var itemToInsert = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, boCol.Count);
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
            var boCol = GetCollectionWith3Items();
            var itemToInsert = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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

// ReSharper disable AssignNullToNotNullAttribute
        [Test]
        public void Test_CopyTo_WhenArrayNull_ShouldThrowArgumentNullException()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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

// ReSharper restore AssignNullToNotNullAttribute
        [Test]
        public void Test_CopyTo_WhenArrayIndexLessThanZero_ShouldArgumentOutOfRangeException()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var array = new object[4];
            const int index = 1;
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, array.Length);
            Assert.Greater(array.Length, index);
            Assert.IsNotNull(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var propertyDescriptor = descriptorCollection.Find("FakeBOName", true);
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            const string key = "SomeFindvalue";
            PropertyDescriptor propertyDescriptor = new PropertyDescriptorStub("InvalidPropName");
            //---------------Assert Precondition----------------
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
        public void Test_Find_WhenHasItem_ShouldReturnIndexOfRowContainingTheKey()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            var propertyDescriptor = descriptorCollection.Find("FakeBOName", true);
            var key = propertyDescriptor.GetValue(boCol[1]);
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
            //---------------Execute Test ----------------------
            var index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, 1);
        }

        [Test]
        public void Test_Find_WhenKeyNotFound_ShouldReturnIndexOfNegativeOne()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var boNotFound = new FakeBO {FakeBOName = GetRandomString()};
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            var propertyDescriptor = descriptorCollection.Find("FakeBOName", true);
            var key = propertyDescriptor.GetValue(boNotFound);
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", propertyDescriptor.Name);
            //---------------Execute Test ----------------------
            var index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, -1);
        }
        /// <summary>
        /// The AddIndex and Remove Index are an optimisation possibility. 
        /// Where an Index of the row is kept keyed on the value for the specified propDescriptor.
        /// This allows for optimising the Searching (Find) and Sorting (ApplySort).
        /// </summary>
        [Test]
        public void Test_AddIndex_DoesNothing()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingListView.AddIndex(new PropertyDescriptorStub("fdafdas"));
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "This does nothing");
        }
        /// <summary>
        /// See <see cref="Test_AddIndex_DoesNothing"/>
        /// </summary>
        [Test]
        public void Test_RemoveIndex_DoesNothing()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingListView.AddIndex(new PropertyDescriptorStub("fdafdas"));
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "This does nothing");
        }
        #endregion

        #region BindingList.Sort

        [Test]
        public void Test_SortDirection_WhenListSortDescriptionCollectionHasZero_ShouldSetSortDirectionAscending()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol) {SortDescriptions = null};
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
            var boCol = new BusinessObjectCollection<FakeBO>();
            var bindingListView = new BindingListView<FakeBO>(boCol);
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
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = CreateBindingListView(boCol);
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortByPropertyDescriptor, ListSortDirection.Ascending);
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
            var boCol = GetCollectionWith3Items("FakeBOName ASC");
            var bindingListView = CreateBindingListView(boCol);
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");

            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortByPropertyDescriptor, ListSortDirection.Descending);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
        }
        [Test]
        public void Test_ApplySort_WithPropertyDescriptor_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------

            var boCol = GetCollectionWith3Items();
            var bindingListView = CreateBindingListView(boCol);
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");

            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortByPropertyDescriptor, ListSortDirection.Ascending);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        //TODO brett 24 Jan 2011: 
        [Test]
        public void Test_ApplySort_WithPropertyDescriptor_Should_SetSortDescriptions()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortByPropertyDescriptor, ListSortDirection.Ascending);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView.SortDescriptions);
            Assert.AreEqual(1, bindingListView.SortDescriptions.Count);
            var listSortDescription = bindingListView.SortDescriptions[0];
            Assert.AreSame(sortByPropertyDescriptor, listSortDescription.PropertyDescriptor);
            Assert.AreEqual(ListSortDirection.Ascending, listSortDescription.SortDirection);
        }

        [Test]
        public void Test_ApplySort_WithSortDescriptionCollection_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = CreateBindingListView(boCol);

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
            var boCol = GetCollectionWith3Items("FakeBOName ASC");
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");

            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Descending);
            var sortDescriptions = new ListSortDescriptionCollection(new[]{ listSortDescription});
            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo1, bindingListView[2]);
        }

        [Test]
        public void Test_ApplySort_WithListSortDescriptionCollection_WhenListSortDirectionAscending_ShouldSortByPropertyDescriptors()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });
            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo1, bindingListView[2]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[0]);
        }
        [Test]
        public void Test_ApplySort_WithListSortDescriptionCollection_Should_SetSortDescriptions()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(sortDescriptions, bindingListView.SortDescriptions);
        }

        [Test]
        public void Test_ApplySort_WithListSortDescriptionCollection_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });

            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        /// <summary>
        /// This is fundamental to the Concept of a Bindig List View see (<see cref="BindingListView{T}"/>
        ///  i.e. the binding list view is a View of the underlying collection.
        /// It references the same <see cref="IBusinessObject"/>s but it does so
        /// in a seperate collection. Filtering, Sorting should not affect the
        /// underlying Business Object Collection.
        /// </summary>
        [Test]
        public void Test_ApplySort_WithListSortDescriptionCollection_ShouldNotSortUnderlyingCollection()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });
            //---------------Assert Precondition----------------
            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            //---------------Assert Precondition----------------
            Assert.AreEqual("FakeBOName", sortByPropertyDescriptor.Name);
            Assert.AreSame(fakeBo1, boCol[0]);
            Assert.AreSame(fakeBo2, boCol[1]);
            Assert.AreSame(fakeBo3, boCol[2]);

            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(sortDescriptions);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo1, bindingListView[2]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[0]);

            Assert.AreSame(fakeBo1, boCol[0]);
            Assert.AreSame(fakeBo2, boCol[1]);
            Assert.AreSame(fakeBo3, boCol[2]);
        }

        [Test]
        public void Test_RemoveSort_ShouldReturnToOriginallist()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items("FakeBOName DESC");
            var bindingListView = new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
            var sortByPropertyDescriptor = GetDescriptor(bindingListView, "FakeBOName");
            var listSortDescription = new ListSortDescription(sortByPropertyDescriptor, ListSortDirection.Ascending);
            var sortDescriptions = new ListSortDescriptionCollection(new[] { listSortDescription });

            var fakeBo1 = boCol[0];
            var fakeBo2 = boCol[1];
            var fakeBo3 = boCol[2];
            bindingListView.ApplySort(sortDescriptions);
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBo1, bindingListView[2]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[0]);

            Assert.AreSame(fakeBo1, boCol[0]);
            Assert.AreSame(fakeBo2, boCol[1]);
            Assert.AreSame(fakeBo3, boCol[2]);
            //---------------Execute Test ----------------------
            bindingListView.RemoveSort();
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo1, bindingListView[0]);
            Assert.AreSame(fakeBo2, bindingListView[1]);
            Assert.AreSame(fakeBo3, bindingListView[2]);
        }
        #endregion


        #region ICancelAddNew and Adding Objects In General
        /// <summary>
        /// This is a very tricky interface since it 
        /// </summary>
        [Test]
        public void Test_AddNew_ShouldCreateNewBo()
        {
            //---------------Set up test pack-------------------

            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            var newBO = bindingListView.AddNew();
            //---------------Test Result -----------------------
            Assert.IsNotNull(newBO);
            Assert.IsInstanceOf<FakeBO>(newBO);
        }
        [Test]
        public void Test_AddNew_ShouldAddNewBusinessObjectToList()
        {
            //---------------Set up test pack-------------------

            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            var newBO = bindingListView.AddNew();
            //---------------Test Result -----------------------
            Assert.AreSame(newBO, bindingListView[3]);
        }
        /// <summary>
        /// I think this is correct. I am creating the New BO from the
        /// ViewOfBusinessObjectCollection and it is therefore being created and added to the
        /// list.
        /// The AddNew however is called based on the grid event raised as a result of the user selecting a new row.
        /// The grid therefore appears to add the blank row automatically.
        /// </summary>
        [Test]
        public void Test_AddNew_ShouldNotRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------

            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.AddNew();
            //---------------Test Result -----------------------
            Assert.IsFalse(listChangedFired);
        }


        [Test]
        public void Test_EndNew_WhenNewItemAtIndex_ShouldAddNewItemTotheViewBusinessObjectCollection()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew();
            //---------------Assert Precondition----------------
            Assert.AreSame(newBO, bindingListView[3]);

            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreEqual(3, boCol.Count);
            boCol.ShouldNotContain(newBO);
            //---------------Execute Test ----------------------
            bindingListView.EndNew(3);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreEqual(4, boCol.Count);
            boCol.ShouldContain(newBO);
        }
        /// <summary>
        /// NNB ICancelAddnew Notes from msdn:
        /// In some scenarios, such as Windows Forms complex data binding, 
        /// the collection may receive CancelNew or EndNew calls for items other than the newly added item. 
        /// (Each item is typically a row in a data view.) Ignore these calls; cancel or 
        /// commit the new item only when that item's index is specified.
        /// </summary>
        [Test]
        public void Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew();
            //---------------Assert Precondition----------------
            Assert.AreSame(newBO, bindingListView[3]);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreEqual(3, boCol.Count);
            boCol.ShouldNotContain(newBO);
            //---------------Execute Test ----------------------
            bindingListView.EndNew(0);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreEqual(3, boCol.Count);
            boCol.ShouldNotContain(newBO);
        }

        [Test]
        public void Test_EndNew_WhenNewItemAtIndex_ShouldSaveNewItem()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(newBO);
            Assert.AreSame(newBO, bindingListView[3]);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.IsTrue(newBO.Status.IsNew);
            //---------------Execute Test ----------------------
            bindingListView.EndNew(3);
            //---------------Test Result -----------------------
            Assert.IsFalse(newBO.Status.IsNew);
        }
        /// <summary>
        /// see <see cref="Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing"/> for details of 
        /// why this test is needed.
        /// </summary>
        [Test]
        public void Test_EndNew_WhenNewItemNotAtIndex_ShouldNotSaveNewItem()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            const int endNewItemIndex = 2;
            var itemAtIndex = (FakeBO)bindingListView[endNewItemIndex];
            itemAtIndex.FakeBOName = GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(itemAtIndex);
            Assert.AreNotSame(newBO, itemAtIndex);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.IsTrue(itemAtIndex.Status.IsDirty);
            //---------------Execute Test ----------------------
            bindingListView.EndNew(endNewItemIndex);
            //---------------Test Result -----------------------
            Assert.IsTrue(itemAtIndex.Status.IsDirty);
        }

        [Test]
        public void Test_CancelNew_WhenNewItemAtIndex_ShouldMarkForDeleteBo()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(newBO);
            Assert.AreSame(newBO, bindingListView[3]);
            Assert.IsTrue(newBO.Status.IsNew);
            Assert.IsFalse(newBO.Status.IsDeleted);
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(3);
            //---------------Test Result -----------------------
            Assert.IsTrue(newBO.Status.IsNew);
            Assert.IsTrue(newBO.Status.IsDeleted, "Should be deleted");
        }

        [Test]
        public void Test_CancelNew_WhenNewItemAtIndex_ShouldRemoveFromBindingList()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            const int newItemIndex = 3;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(newBO);
            Assert.IsTrue(newBO.Status.IsNew);
            Assert.IsFalse(newBO.Status.IsDeleted);
            Assert.AreEqual(4, bindingListView.Count);

            Assert.AreSame(newBO, bindingListView[newItemIndex]);
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(newItemIndex);
            //---------------Test Result -----------------------
            Assert.AreEqual(3, bindingListView.Count);
            Assert.IsFalse(bindingListView.Contains(newBO), "Binding List view hsould not contain BO");
        }
        [Test]
        public void Test_CancelNew_WhenNewItemAtIndex_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            const int newItemIndex = 3;
            var newBO = bindingListView.AddNew() as FakeBO;
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(newBO, bindingListView[newItemIndex]);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(newItemIndex);
            //---------------Test Result -----------------------
            Assert.IsTrue(listChangedFired);
        }

        /// <summary>
        /// See Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing for an explanation 
        /// of why this needs to be explicitely implemented and tested.
        /// </summary>
        [Test]
        public void Test_CancelNew_WhenNewItemNotAtIndex_ShouldNotMarkForDeleteBo()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            const int cancelNewIndex = 2;
            var itemAtIndex = bindingListView[cancelNewIndex] as FakeBO;
            //---------------Assert Precondition----------------
            Assert.AreNotSame(newBO, itemAtIndex);
            Assert.IsNotNull(itemAtIndex);
            Assert.IsFalse(itemAtIndex.Status.IsDeleted);
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(cancelNewIndex);
            //---------------Test Result -----------------------
            Assert.IsFalse(itemAtIndex.Status.IsDeleted, "Should not be deleted");
        }

        /// <summary>
        /// See Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing for an explanation 
        /// of why this needs to be explicitely implemented and tested.
        /// </summary>
        [Test]
        public void Test_CancelNew_WhenNewItemNotAtIndex_ShouldNotRemoveFromBindingList()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;
            const int cancelNewIndex = 2;
            var itemAtIndex = bindingListView[cancelNewIndex] as FakeBO;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(itemAtIndex);
            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreNotSame(newBO, itemAtIndex);
            Assert.IsTrue(bindingListView.Contains(itemAtIndex), "Binding List view hsould not contain BO");
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(cancelNewIndex);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(itemAtIndex), "Binding List view hsould not contain BO");
        }
        /// <summary>
        /// See <see cref="Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing"/> for an explanation 
        /// of why this needs to be explicitely implemented and tested.
        /// </summary>
        [Test]
        public void Test_CancelNew_WhenNewItemNotAtIndex_ShouldNotRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            const int cancelNewIndex = 2;
            var newBO = bindingListView.AddNew() as FakeBO;
            var listChangedFired = false;
            bindingListView.ListChanged += (sender, args) => listChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreNotSame(newBO, bindingListView[cancelNewIndex]);
            Assert.IsFalse(listChangedFired);
            //---------------Execute Test ----------------------
            bindingListView.CancelNew(cancelNewIndex);
            //---------------Test Result -----------------------
            Assert.IsFalse(listChangedFired, "Should not raise event");
        }
        /// <summary>
        /// The End New has already accepted the change on this New BO. If Cancel is now called
        /// it should not Cancel.
        /// See <see cref="Test_EndNew_WhenNoNewItemAtThatIndex_ShouldDoNothing"/> for an explanation 
        /// </summary>
        [Test]
        public void Test_CancelNew_WhenNewItemWasEndNewed_ShouldNotMarkFromDelete()
        {
            var boCol = GetCollectionWith3Items();
            var bindingListView = new BindingListView<FakeBO>(boCol);
            var newBO = bindingListView.AddNew() as FakeBO;

            const int newItemIndex = 3;
            const int cancelNewItemIndex = newItemIndex;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(newBO);
            Assert.AreSame(newBO, bindingListView[cancelNewItemIndex]);
            Assert.IsTrue(newBO.Status.IsNew);
            Assert.IsFalse(newBO.Status.IsDeleted);
            Assert.AreEqual(4, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.EndNew(cancelNewItemIndex);
            bindingListView.CancelNew(cancelNewItemIndex);
            //---------------Test Result -----------------------
            Assert.IsFalse(newBO.Status.IsDeleted, "Should not be deleted");
            Assert.AreEqual(4, bindingListView.Count);
        }
        #endregion


        #region Filtering

        [Test]
        public void Test_GetFilter_ShouldRetFilter()
        {
            //---------------Set up test pack-------------------
            const string expectedFilter = "FakeBOName = 1";
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var list = new BindingListView<FakeBO> { Filter = expectedFilter };
            var actualFilter = list.Filter;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilter, actualFilter);
        }
        [Test]
        public void Test_SetFilter_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            const string filter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO {FakeBOName = "1"};
            var fakeBO2 = new FakeBO {FakeBOName = "2"};
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = filter;
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count);
            Assert.IsTrue(bindingList.Contains(fakeBO1));
            Assert.IsFalse(bindingList.Contains(fakeBO2));
        }
        [Test]
        public void Test_SetFilter_ShouldNotFilterUnderlyingBOCol()
        {
            //---------------Set up test pack-------------------
            const string filter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO {FakeBOName = "1"};
            var fakeBO2 = new FakeBO {FakeBOName = "2"};
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = filter;
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            Assert.IsTrue(boCol.Contains(fakeBO1));
            Assert.IsTrue(boCol.Contains(fakeBO2));
        }
        [Test]
        public void Test_SetFilter_ShouldRaiseListChangedEvent()
        {
            //---------------Set up test pack-------------------
            const string filter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO {FakeBOName = "1"};
            var fakeBO2 = new FakeBO {FakeBOName = "2"};
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = filter;
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_SetFilter_Null_ShouldClearFilter()
        {
            //---------------Set up test pack-------------------
            const string filter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO { FakeBOName = "1" };
            var fakeBO2 = new FakeBO { FakeBOName = "2" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol) {Filter = filter};
            //---------------Assert Precondition----------------
            Assert.AreEqual(filter, bindingList.Filter);
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = null;
            //---------------Test Result -----------------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
        }
        [Test]
        public void Test_SetFilter_EmptyString_ShouldClearFilter()
        {
            //---------------Set up test pack-------------------
            const string filter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO { FakeBOName = "1" };
            var fakeBO2 = new FakeBO { FakeBOName = "2" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol) {Filter = filter};
            //---------------Assert Precondition----------------
            Assert.AreEqual(filter, bindingList.Filter);
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "";
            //---------------Test Result -----------------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_SetFilter_Again_ShouldFilterBindingListAndReturnCorrectBO()
        {
            //---------------Set up test pack-------------------
            const string filter1 = "FakeBOName = 1";
            const string filter2 = "FakeBOName = 2";
            var fakeBO1 = new FakeBO { FakeBOName = "1" };
            var fakeBO2 = new FakeBO { FakeBOName = "2" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol) { Filter = filter1 };
            //---------------Assert Precondition----------------
            Assert.AreEqual(filter1, bindingList.Filter);
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            Assert.AreSame(fakeBO1, bindingList[0]);
            //---------------Execute Test ----------------------
            bindingList.Filter = filter2;
            //---------------Test Result -----------------------
            Assert.AreEqual(filter2, bindingList.Filter);
            Assert.AreEqual(1, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            Assert.AreSame(fakeBO2, bindingList[0]);
        }

        [Test]
        public void Test_Setfilter_WhenExactStringMatch_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBO { FakeBOName = "SomeFakeBO" };
            var fakeBO2 = new FakeBO { FakeBOName = "AnotherFakeBO" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBOName = 'SomeFakeBO'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count, "When filter expression exact string match, should return one item");
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_Setfilter_WhenExactIntegerMatch_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBOW5Props { FakeBONumber = 11 };
            var fakeBO2 = new FakeBOW5Props { FakeBONumber = 22 };
            var boCol = new BusinessObjectCollection<FakeBOW5Props> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBONumber = '11'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count, "When filter expression exact integer match, should return one item");
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_Setfilter_WhenExactDateMatch_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBOW5Props { FakeBODate = new DateTime(2000,1,1) };
            var fakeBO2 = new FakeBOW5Props { FakeBODate = new DateTime(2000,1,2) };
            var boCol = new BusinessObjectCollection<FakeBOW5Props> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBODate = '2000/01/01'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count, "When filter expression exact date match, should return one item");
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_Setfilter_WhenExactDeciamlMatch_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBOW5Props { FakeBODecimal = new decimal(10.00) };
            var fakeBO2 = new FakeBOW5Props { FakeBODecimal = new decimal(10.50) };
            var boCol = new BusinessObjectCollection<FakeBOW5Props> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBODecimal = '10.50'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingList.Count, "When filter expression exact decimal match, should return one item");
            Assert.AreEqual(2, boCol.Count);
        }

        [Test]
        public void Test_Setfilter_WhenLikeStringMatch_ShouldFilterBindingList()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBO { FakeBOName = "SomeFakeBO" };
            var fakeBO2 = new FakeBO { FakeBOName = "AnotherFakeBO" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var bindingList = new BindingListView<FakeBO>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, bindingList.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBOName Like Some%";
            //---------------Test Result --- --------------------
            Assert.AreEqual(1, bindingList.Count, "When filter expression like string match, should return one item");
            Assert.AreEqual(2, boCol.Count);
        }

        [Ignore("T")] //TODO Brett 24 Jan 2011: Ignored Test - T
        [Test]
        public void Test_Setfilter_WhenLikeOnReflectiveProp_ShouldFilterUnderlyinhBOCol()
        {
            //---------------Set up test pack-------------------
            var fakeBO1 = new FakeBOW5Props { FakeBONumber = 11 };
            var fakeBO2 = new FakeBOW5Props { FakeBONumber = 1 };
            var fakeBO3 = new FakeBOW5Props { FakeBONumber = 22 };
            var boCol = new BusinessObjectCollection<FakeBOW5Props> { fakeBO1, fakeBO2, fakeBO3 };
            var bindingList = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingList.Count);
            Assert.AreEqual(3, boCol.Count);
            //---------------Execute Test ----------------------
            bindingList.Filter = "FakeBONumberAsString LIKE 1%";
            //---------------Test Result -----------------------
            Assert.AreEqual(2, bindingList.Count, "When filter expression like integer match, should return one item");
            Assert.AreEqual(3, boCol.Count);
            Assert.Fail("Test Not Yet Implemented");
        }


        [Test]
        public void Test_Filter_WhenEqualToWithGuidMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOsW5Props(3);
            var fakeBOId = new Guid("7AD655A7-DF06-451B-848A-25CBEBDBBC8A");
            var boToFind = CreateSavedFakeBOW5Prop(fakeBOId);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, bindingListView.Count);
            Assert.AreEqual(fakeBOId, boToFind.FakeBOID);
            Assert.IsTrue(bindingListView.Contains(boToFind));
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOID='7AD655A7-DF06-451B-848A-25CBEBDBBC8A'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }


        [Test]
        public void Test_Filter_WhenLessThanWithStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            var boToFind = CreateSavedFakeBOW5Prop("AAA");
            CreateSavedFakeBOW5Prop("BBB");
            CreateSavedFakeBOW5Prop("CCC");
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName<'B'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }


        [Test]
        public void Test_Filter_WhenLessThanWithDateTimeMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            var boToFind = CreateSavedFakeBOW5Prop(new DateTime(2010, 1, 1));
            CreateSavedFakeBOW5Prop(new DateTime(2010, 1, 2));
            CreateSavedFakeBOW5Prop(new DateTime(2010, 1, 3));
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate<'2010/01/02'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenLessThanWithIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            var boToFind = CreateSavedFakeBOW5Prop(1);
            CreateSavedFakeBOW5Prop(2);
            CreateSavedFakeBOW5Prop(3);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber<'2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenLessThanWithGuidMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            var boToFind = CreateSavedFakeBOW5Prop(new Guid("7AD655A7-DF06-451B-848A-25CBEBDBBC8A"));
            CreateSavedFakeBOW5Prop(new Guid("8AD655A7-DF06-451B-848A-25CBEBDBBC8A"));
            CreateSavedFakeBOW5Prop(new Guid("9AD655A7-DF06-451B-848A-25CBEBDBBC8A"));
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOID<'8AD655A7-DF06-451B-848A-25CBEBDBBC8A'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA");
            CreateSavedFakeBOW5Prop("BBB");
            var boToFind = CreateSavedFakeBOW5Prop("CCC");
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName>'BBB'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithDateTimeMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), GetRandomGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), GetRandomGuid(), 1);
            var boToFind = CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), GetRandomGuid(), 1);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate>'2010/01/02'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop(1);
            CreateSavedFakeBOW5Prop(2);
            var boToFind = CreateSavedFakeBOW5Prop(3);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber>'2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenLikeStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA");
            var boToFind = CreateSavedFakeBOW5Prop("ABB");
            CreateSavedFakeBOW5Prop("CCC");
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            StringAssert.Contains("B", boToFind.FakeBOName);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName Like '%B%'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }
/*
        [Ignore("I cannot see any logical way for this to be applied")] //TODO Brett 24 Jan 2011: Ignored Test - I cannot see any logical way for this to be applied
        [Test]
        public void Test_Filter_WhenLikeIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop(1);
            CreateSavedFakeBOW5Prop(2);
            CreateSavedFakeBOW5Prop(3);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListViewNew<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber Like '2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
        }*/

        [Test]
        public void Test_Filter_WhenFilterValueIncludesAND_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            var boToFind = CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), GetRandomGuid(), 1);
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 2), GetRandomGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), GetRandomGuid(), 3);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName='AAA' AND FakeBONumber='1'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, bindingListView.Count);
            Assert.IsTrue(bindingListView.Contains(boToFind));
        }

        [Test]
        public void Test_Filter_WhenFilterValueIncludesANDWithNoMatch_ShouldReturnNoMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), GetRandomGuid(), 1);
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 2), GetRandomGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), GetRandomGuid(), 3);
            var boCol = GetLoadedCollection<FakeBOW5Props>();
            var bindingListView = new BindingListView<FakeBOW5Props>(boCol);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName='AAA' AND FakeBONumber='3'";
            //---------------Test Result -----------------------
            Assert.AreEqual(0, bindingListView.Count);
        }


        [Test]
        public void Test_RemoveFilter_ShouldSetFilterToEmpty()
        {
            //---------------Set up test pack-------------------
            const string initialFilter = "FakeBOName = 1";
            var list = new BindingListView<FakeBO> { Filter = initialFilter };
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialFilter, list.Filter);
            //---------------Execute Test ----------------------
            list.RemoveFilter();
            //---------------Test Result -----------------------
            Assert.IsEmpty(list.Filter);
        }

        [Test]
        public void Test_RemoveFilter_ShouldReturnBindingListToOriginal()
        {
            //---------------Set up test pack-------------------
            const string initialFilter = "FakeBOName = 1";
            var fakeBO1 = new FakeBO { FakeBOName = "1" };
            var fakeBO2 = new FakeBO { FakeBOName = "2" };
            var boCol = new BusinessObjectCollection<FakeBO> { fakeBO1, fakeBO2 };
            var list = new BindingListView<FakeBO>(boCol) { Filter = initialFilter };
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialFilter, list.Filter);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, boCol.Count);
            //---------------Execute Test ----------------------
            list.RemoveFilter();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, boCol.Count);
        }

        #endregion //Filtering

        #region IRaiseItemChangedEvents

        [Test]
        public void Test_RaisesItemChangedEvents_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var list = new BindingListView<FakeBO>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var raisesItemChangedEvents = list.RaisesItemChangedEvents;
            //---------------Test Result -----------------------
            Assert.IsTrue(raisesItemChangedEvents);
        }

        #endregion


        //TODO brett 24 Jan 2011: If you make an update to the Business Object or the underlying collection directly
        // then it should cause the ListChanged Event to fire.

        private static DateTime GetRandomDate()
        {
            return RandomValueGen.GetRandomDate();
        }
/*
        private static string GetRandomString(int maxLength)
        {
            return RandomValueGen.GetRandomString(maxLength);
        }*/

        private static Guid GetRandomGuid()
        {
            return RandomValueGen.GetRandomGuid();
        }
        private static int GetRandomInt()
        {
            return RandomValueGen.GetRandomInt(100);
        }
        private static BusinessObjectCollection<T> GetLoadedCollection<T>() where T : class, IBusinessObject, new()
        {
            var collection = new BusinessObjectCollection<T>();
            collection.LoadAll();
            return collection;
        }
        private static FakeBOW5Props CreateSavedFakeBOW5Prop(int fakeBONumber)
        {
            return CreateSavedFakeBOW5Prop(GetRandomString(), GetRandomDate(), GetRandomGuid(), fakeBONumber);
        }
        private static FakeBOW5Props CreateSavedFakeBOW5Prop(Guid fakeBOID)
        {
            return CreateSavedFakeBOW5Prop(GetRandomString(), GetRandomDate(), fakeBOID, GetRandomInt());
        }
        private static FakeBOW5Props CreateSavedFakeBOW5Prop(DateTime fakeBODate)
        {
            return CreateSavedFakeBOW5Prop(GetRandomString(), fakeBODate, GetRandomGuid(), GetRandomInt());
        }
        private static FakeBOW5Props CreateSavedFakeBOW5Prop(string fakeBOString)
        {
            return CreateSavedFakeBOW5Prop(fakeBOString, GetRandomDate(), GetRandomGuid(), GetRandomInt());
        }

        private static FakeBOW5Props CreateSavedFakeBOW5Prop(string fakeBOName, DateTime fakeBODate, Guid fakeBOId, int fakeBONumber)
        {
            var fakeBO = new FakeBOW5Props
                             {
                                 FakeBOName = fakeBOName,
                                 FakeBODate = fakeBODate,
                                 FakeBOID = fakeBOId,
                                 FakeBONumber = fakeBONumber
                             };
            fakeBO.Save();
            return fakeBO;
        }

        private static void CreateSavedFakeBOsW5Props(int numberToCreate)
        {
            for (int i = 0; i < numberToCreate; i++)
            {
                string fakeBOName = "A" + RandomValueGen.GetRandomString();
                DateTime fakeBODate = RandomValueGen.GetRandomDate();
                Guid fakeBOId = RandomValueGen.GetRandomGuid();
                int fakeBONumber = RandomValueGen.GetRandomInt();
                CreateSavedFakeBOW5Prop(fakeBOName, fakeBODate, fakeBOId, fakeBONumber);
            }
        }
        private static BusinessObjectCollection<FakeBO> GetCollectionWith3Items(string orderByClause)
        {
            CreateSavedBOs(3);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.Load("", orderByClause);
            return boCol;
        }
        private static PropertyDescriptor GetDescriptor(ITypedList bindingListView, string propName)
        {
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            return descriptorCollection.Find(propName, true);
        }

        private static BindingListView<FakeBO> CreateBindingListView(BusinessObjectCollection<FakeBO> boCol)
        {
            return new BindingListView<FakeBO>(boCol) { ViewBuilder = new DefaultViewBuilder<FakeBO>() };
        }
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static BusinessObjectCollection<FakeBO> GetCollectionWith3Items()
        {
            CreateSavedBOs(3);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            return boCol;
        }

        private static BusinessObjectCollection<FakeBO> GetCollectionWithOneItem()
        {
            CreateSavedBOs(1);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            return boCol;
        }

        private static BusinessObjectCollection<FakeBO> GetCollectionWith5Items()
        {
            CreateSavedBOs(5);
            var boCol = new BusinessObjectCollection<FakeBO>();
            boCol.LoadAll();
            return boCol;
        }

        private static void CreateSavedBOs(int numberToCreate)
        {
            for (var i = 0; i < numberToCreate; i++)
            {
                CreateSavedBO();
            }
        }

        private static FakeBO CreateSavedBO()
        {
            var businessObject = new FakeBO
                                     {
                                         FakeBOName = "A" + RandomValueGen.GetRandomString()
                                     };
            businessObject.Save();
            return businessObject;
        }

        private static T GenerateStub<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

    }
    internal class BindingListViewSpy<T> : BindingListView<T> where T : class, IBusinessObject, new()
    {
        public BindingListViewSpy(BusinessObjectCollection<T> boCol)
            : base(boCol)
        {
        }

        public BusinessObjectCollection<T> GetViewOfBusinessObjectCollection()
        {
            return base.ViewOfBusinessObjectCollection;
        }
    }

    internal class PropertyDescriptorCollectionStub : PropertyDescriptorCollection
    {
        public PropertyDescriptorCollectionStub()
            : base(new PropertyDescriptor[0])
        {
        }
    }

    // ReSharper disable ParameterTypeCanBeEnumerable.Global
    internal static class BLVTestingExtensions
    {

        public static void ShouldNotContain(this BusinessObjectCollection<FakeBO> boCol, object newBO)
        {
            ((IEnumerable<FakeBO>)boCol).ShouldNotContain(newBO);
        }
        public static void ShouldContain(this BusinessObjectCollection<FakeBO> boCol, object newBO)
        {
            ((IEnumerable<FakeBO>)boCol).ShouldContain(newBO);
        }
    }
// ReSharper restore ParameterTypeCanBeEnumerable.Global
}