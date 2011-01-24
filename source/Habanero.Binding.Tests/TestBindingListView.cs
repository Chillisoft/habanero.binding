using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Rules;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;

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
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.        
            //ClassDef.ClassDefs.Clear();
            ClassDef.ClassDefs.Add(typeof(FakeBO).MapClasses());
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [Test]
        public void Test_Construct_WithEmptyConstructor_ShouldNotRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BindingListView<FakeBO> bindingListView = new BindingListView<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
            Assert.IsTrue(bindingListView.AllowEdit);
            Assert.IsTrue(bindingListView.AllowNew);
            Assert.IsTrue(bindingListView.AllowRemove);
            Assert.IsTrue(bindingListView.IsFixedSize);
            Assert.IsTrue(bindingListView.IsReadOnly);
            Assert.IsTrue(bindingListView.SupportsAdvancedSorting);
            Assert.IsTrue(bindingListView.SupportsChangeNotification);
            Assert.IsTrue(bindingListView.SupportsFiltering);
            Assert.IsFalse(bindingListView.SupportsSearching);
            Assert.IsTrue(bindingListView.SupportsSorting);
        }
        [Test]
        public void Test_Construct_WithCollection_ShouldNotRaiseError()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BindingListView<FakeBO> bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView);
            Assert.IsTrue(bindingListView.AllowEdit);
            Assert.IsTrue(bindingListView.AllowNew);
            Assert.IsTrue(bindingListView.AllowRemove);
            Assert.IsTrue(bindingListView.IsFixedSize);
            Assert.IsTrue(bindingListView.IsReadOnly);
            Assert.IsTrue(bindingListView.SupportsAdvancedSorting);
            Assert.IsTrue(bindingListView.SupportsChangeNotification);
            Assert.IsTrue(bindingListView.SupportsFiltering);
            Assert.IsFalse(bindingListView.SupportsSearching);
            Assert.IsTrue(bindingListView.SupportsSorting);
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
            var listViewSpy = new BindingListViewSpy<FakeBO>(collection) { ViewBuilder = viewBuilder };
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
            var propertyInfos = typeof(FakeBO).GetProperties();
            Assert.AreEqual(propertyInfos.Length, pds.Count);
        }

        [Test]
        public void Test_IsSynchronized_ShouldReturn_BusinessObjectCollectionIsSynchronized()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IBindingListView bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var isSynchronized = bindingListView.IsSynchronized;
            //---------------Test Result -----------------------
            Assert.AreEqual(((IBusinessObjectCollection)collection).IsSynchronized, isSynchronized);
        }

        [Test]
        public void Test_SyncRoot_ShouldReturn_BusinessObjectCollectionSyncRoot()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            IBindingListView bindingListView = new BindingListView<FakeBO>(collection);
            var expectedSyncRoot = ((IBusinessObjectCollection)collection).SyncRoot;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var syncRoot = bindingListView.SyncRoot;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedSyncRoot, syncRoot);
        }



        [Test]
        public void Test_GetBOIndex3_WhenCollectionHas5_ShouldReturnFourthBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            var fakeBO = bindingListView[3];
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }

        [Test]
        public void Test_GetBO_WhenCollectioHas5And3Loaded_ShouldReturnThirdBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            collection.LoadWithLimit("","", 0, 3, out noOfRecords);
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(5, noOfRecords);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            var fakeBO = bindingListView[2];
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }

#pragma warning disable 168
        [Test]
        public void Test_GetFourthBO_WhenCollectioHas5And3Loaded_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            int noOfRecords;
            collection.LoadWithLimit("","", 0, 3, out noOfRecords);
            var bindingListView = new BindingListView<FakeBO>(collection);
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
                StringAssert.Contains("Index was out of range. Must be non-negative and less than the size of the collection.", ex.Message);
            }
        }
#pragma warning restore 168
        [Test]
        public void Test_GetSixthBO_WhenCollectioHas5_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
                StringAssert.Contains("Index was out of range. Must be non-negative and less than the size of the collection.", ex.Message);
            }
        }

        [Test]
        public void Test_Clear_WhenCollectionHas5_ShouldRemoveItemsFromCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Clear();
            //---------------Test Result -----------------------
            Assert.AreEqual(0, collection.Count);    
        }

        [Test]
        public void Test_Contains_WhenCollectionHas5AndThirdSelected_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            //---------------Execute Test ----------------------
            bool contains = bindingListView.Contains(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.IsTrue(contains);
        }

        [Test]
        public void Test_Remove_WhenColHas1_ShouldRemoveBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(1);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            int indexOf = bindingListView.IndexOf(bindingListView[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, indexOf);
        }

        [Test]
        public void Test_IndexOf_WhenColHas5AndThirdSelected_ShouldReturnIndexTwoOfBO()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            int indexOf = bindingListView.IndexOf(bindingListView[2]);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, indexOf);
        }
        [Test]
        public void Test_RemoveAt_WhenColHas1_ShouldRemoveBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(1);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(5);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual(5, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.RemoveAt(2);
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }



        [Test]
        public void Test_Insert_WhenColHas3_ShouldInsertObjectAtIndex3()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            FakeBO bo = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(3, bo);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView[3]);
            Assert.AreEqual(3, bindingListView.IndexOf(bo));
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3_ShouldInsertObjectAtIndex1()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            FakeBO bo = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            bindingListView.Insert(1, bo);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingListView[1]);
            Assert.IsNotNull(bindingListView[3]);
            Assert.AreEqual(1, bindingListView.IndexOf(bo));
            Assert.AreEqual(4, collection.Count);
            Assert.AreEqual(4, bindingListView.Count);
        }

        [Test]
        public void Test_Insert_WhenColHas3AndObjectInsertedAtIndex5_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            FakeBO bo = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(3, bindingListView.Count);
            //---------------Execute Test ----------------------
            try
            {
                bindingListView.Insert(5, bo);
                Assert.Fail("Expected to throw an ArgumentOutOfRangeException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentOutOfRangeException ex)
            {
                StringAssert.Contains("Index must be within the bounds of the List.", ex.Message);
            }
        }

        [Test]
        public void Test_CopyTo_WhenArrayNull_ShouldThrowArgumentNullException()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            object[] array = new object[]{};
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            object[] array = new object[] { };
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var array = new object[] { };
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

        [Ignore("This fails because Line 1266 in BusinessObjectCollection.cs sets a new BO in IList.CopyTo()")] 
        [Test]
        public void Test_CopyTo_WhenColHas3Objets_ShouldCopyObjectsToArray()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            object[] array = new object[4];
            int index = 1;
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

        [Test]
        public void Test_Find_WhenPropertyDescriptorIsNull_ShoudRaiseError()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            string key = "FakeBONotABo";
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
                StringAssert.Contains("The given property name '" + propertyDescriptor.Name + "' does not exist in the collection of properties for the class", ex.Message);
            }
        }

        [Test]
        public void Test_Find_ShouldReturnIndexOfRowContainingPropertyDescriptor()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            const int propDescriptorIndex = 1;
            PropertyDescriptor propertyDescriptor = descriptorCollection[propDescriptorIndex];
            object key = propertyDescriptor.GetValue(collection[1]);
            //---------------Assert Precondition----------------
            Assert.AreEqual(9, descriptorCollection.Count);
            //---------------Execute Test ----------------------
            int index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, propDescriptorIndex);
        }

        [Test]
        public void Test_Find_WhenKeyNotFound_ShouldReturnIndexOfNegativeOne()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bo = new FakeBO();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            const int propDescriptorIndex = 1;
            PropertyDescriptor propertyDescriptor = descriptorCollection[propDescriptorIndex];
            object key = propertyDescriptor.GetValue(bo);
            //---------------Assert Precondition----------------
            Assert.AreEqual(9, descriptorCollection.Count);
            //---------------Execute Test ----------------------
            int index = bindingListView.Find(propertyDescriptor, key);
            //---------------Test Result -----------------------
            Assert.AreEqual(index, -1);
        }

        #region BindingList.Sort

        [Test]
        public void Test_SortDirection_WhenListSortDescriptionCollectionHasZero_ShouldSetSortDirectionAscending()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            PropertyDescriptor descriptor = null;
            var listSortDescription = new ListSortDescription(descriptor, ListSortDirection.Descending);
            ListSortDescription[] descriptions = new[] { listSortDescription };
            ListSortDescriptionCollection descriptionCollection = new ListSortDescriptionCollection(descriptions);
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            PropertyDescriptor descriptor = null;
            var listSortDescription1 = new ListSortDescription(descriptor, ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptor, ListSortDirection.Descending);
            var descriptions = new[] { listSortDescription1, listSortDescription2 };
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
            CreateSavedBOs(3);
            BusinessObjectCollection<FakeBO> collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
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
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] { listSortDescription };
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
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription1 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] { listSortDescription1, listSortDescription2 };
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
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            var listSortDescription1 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var listSortDescription2 = new ListSortDescription(descriptorCollection[0], ListSortDirection.Descending);
            var descriptions = new[] { listSortDescription1, listSortDescription2 };
            var descriptionCollection = new ListSortDescriptionCollection(descriptions);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(9, bindingListView.SortDescriptions.Count);
            //---------------Execute Test ----------------------
            bool isSorted = bindingListView.IsSorted;
            //---------------Test Result -----------------------
            Assert.True(isSorted);
        }
        [Test]
        public void Test_IsSorted_WhenSortDescriptionsNull_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            bindingListView.SortDescriptions = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(bindingListView.SortDescriptions);
            //---------------Execute Test ----------------------
            bool isSorted = bindingListView.IsSorted;
            //---------------Test Result -----------------------
            Assert.False(isSorted);
        }

        [Test]
        public void Test_IsSorted_WhenSortDescriptionsAreLessThanOrEqualToZero_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var collection = new BusinessObjectCollection<FakeBO>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptionCollection = new ListSortDescriptionCollection(null);
            bindingListView.SortDescriptions = descriptionCollection;
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, bindingListView.SortDescriptions.Count);
            //---------------Execute Test ----------------------
            bool isSorted = bindingListView.IsSorted;
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
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(new PropertyDescriptor[0]);
            PropertyDescriptor propertyDescriptor1 = descriptorCollection[1];
            string fakeBo1 = collection[0].FakeBOName;
            string fakeBo2 = collection[1].FakeBOName;
            string fakeBo3 = collection[2].FakeBOName;
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBo1, collection[0].FakeBOName);
            Assert.AreSame(fakeBo2, collection[1].FakeBOName);
            Assert.AreSame(fakeBo3, collection[2].FakeBOName);
            Assert.AreEqual(9, descriptorCollection.Count);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(propertyDescriptor1, ListSortDirection.Ascending);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, collection[0].FakeBOName);
            Assert.AreSame(fakeBo2, collection[1].FakeBOName);
            Assert.AreSame(fakeBo1, collection[2].FakeBOName);
        }

        [Test]
        public void Test_ApplySort_WhenListSortDirectionDescscending_WithPropertyDescriptor_ShouldSortByPropertyDescriptor()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.Load("", "FakeBOName ASC");
            var bindingListView = new BindingListView<FakeBO>(collection);
            var descriptorCollection = bindingListView.GetItemProperties(null);
            PropertyDescriptor propertyDescriptor1 = descriptorCollection[1];
            string fakeBo1 = collection[0].FakeBOName;
            string fakeBo2 = collection[1].FakeBOName;
            string fakeBo3 = collection[2].FakeBOName;
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBo1, collection[0].FakeBOName);
            Assert.AreSame(fakeBo2, collection[1].FakeBOName);
            Assert.AreSame(fakeBo3, collection[2].FakeBOName);
            Assert.AreEqual(9, descriptorCollection.Count);
            //---------------Execute Test ----------------------
            bindingListView.ApplySort(propertyDescriptor1, ListSortDirection.Descending);
            //---------------Test Result -----------------------
            Assert.AreSame(fakeBo3, collection[0].FakeBOName);
            Assert.AreSame(fakeBo2, collection[1].FakeBOName);
            Assert.AreSame(fakeBo1, collection[2].FakeBOName);
        }
        #endregion

/*
        [Test]
        public void Test_AddNew_ShouldAddNewObjectToCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.AddNew();
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
        }

        [Test]
        public void Test_AddNew_ShouldReturnNewBusinessObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(3);
            var collection = new BusinessObjectCollection<FakeBO>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            object bo = bindingListView.AddNew();
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
            Assert.IsTrue(((BusinessObject)bo).Status.IsNew);
        }*/


        #region Filtering

        [Test]
        public void Test_SetGetFilter_ShouldRetFilter()
        {
            //---------------Set up test pack-------------------
            const string expectedFilter = "FakeBOName = 1";
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var list = new BindingListView<FakeBO> { Filter = expectedFilter };
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilter, list.Filter);
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
        public void Test_DetermineFilterOperator_WhenEquals_ShouldGetFilterOperator()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            FilterOperator filterOperator = bindingListView.DetermineFilterOperator("FakeBO='SomeStuff'");
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterOperator.EqualTo, filterOperator);
        }

        [Test]
        public void Test_DetermineFilterOperator_WhenGreaterThan_ShouldGetFilterOperator()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            FilterOperator filterOperator = bindingListView.DetermineFilterOperator("FakeBO>'SomeStuff'");
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterOperator.GreaterThan, filterOperator);
        }

        [Test]
        public void Test_DetermineFilterOperator_WhenLessThan_ShouldGetFilterOperator()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            FilterOperator filterOperator = bindingListView.DetermineFilterOperator("FakeBO<'SomeStuff'");
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterOperator.LessThan, filterOperator);
        }

        [Test]
        public void Test_DetermineFilterOperator_WhenNoOperator_ShouldReturmNoneFilterOperator()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            FilterOperator filterOperator = bindingListView.DetermineFilterOperator("FakeBO<>'SomeStuff'");
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterOperator.None, filterOperator);
        }

        [Test]
        public void Test_DetermineFilterOperator_WhenLikeOperator_ShouldReturnLikeFilterOperator()
        {
            //---------------Set up test pack-------------------
            var collection = MockRepository.GenerateStub<BusinessObjectCollection<FakeBO>>();
            var bindingListView = new BindingListView<FakeBO>(collection);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            FilterOperator filterOperator = bindingListView.DetermineFilterOperator("FakeBO Like 'SomeStuff'");
            //---------------Test Result -----------------------
            Assert.AreEqual(FilterOperator.Like, filterOperator);
        }

        [Test]
        public void Test_Filter_WithNoFilter_ShouldResetCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", DateTime.Today, Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = null;
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenEqualWithStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName='SomeName'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenEqualToWithDateTimeMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate='2010/01/01'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenEqualToWithIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", new DateTime(2010, 1, 1), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber='3'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenEqualToWithGuidMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", new DateTime(2010, 1, 1), new Guid("7AD655A7-DF06-451B-848A-25CBEBDBBC8A"), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(4, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOID='7AD655A7-DF06-451B-848A-25CBEBDBBC8A'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenLessThanWithStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName<'B'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }


        [Test]
        public void Test_Filter_WhenLessThanWithDateTimeMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate<'2010/01/02'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenLessThanWithIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber<'2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenLessThanWithGuidMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), new Guid("7AD655A7-DF06-451B-848A-25CBEBDBBC8A"), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), new Guid("8AD655A7-DF06-451B-848A-25CBEBDBBC8A"), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), new Guid("9AD655A7-DF06-451B-848A-25CBEBDBBC8A"), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOID<'8AD655A7-DF06-451B-848A-25CBEBDBBC8A'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName>'BBB'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithDateTimeMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate>'2010/01/02'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenGreaterThanWithIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber>'2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }


        [Test]
        public void Test_Filter_WhenLikeStringMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("ABB", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName Like 'B'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenLikeIntMatch_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber Like '2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenFilterValueIncludesAND_ShouldReturnMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName='AAA' AND FakeBONumber='1'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenFilterValueIncludesANDWithNoMatch_ShouldReturnNoMatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBOName='AAA' AND FakeBONumber='3'";
            //---------------Test Result -----------------------
            Assert.AreEqual(0, collection.Count);
        }

        #region ToBeImplemented

        /* [Test]
        public void Test_Filter_WhenGreaterThanEqualToWithStringMatch_ShouldReturn2MatchedObject()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "[FakeBOName]>='BBB'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Test_Filter_WhenGreaterThanEqualWithDateTimeMatch_ShouldReturn2MatchedObjects()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBODate>='2010/01/02'";
            //---------------Test Result -----------------------
            Assert.AreEqual(1, collection.Count);
        }
        
        [Test]
        public void Test_Filter_WhenGreaterThanEqualToWithIntMatch_ShouldReturn2MatchedObjects()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Prop("AAA", new DateTime(2010, 1, 1), Guid.NewGuid(), 1);
            CreateSavedFakeBOW5Prop("BBB", new DateTime(2010, 1, 2), Guid.NewGuid(), 2);
            CreateSavedFakeBOW5Prop("CCC", new DateTime(2010, 1, 3), Guid.NewGuid(), 3);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "FakeBONumber>='2'";
            //---------------Test Result -----------------------
            Assert.AreEqual(2, collection.Count);
        }*/

        #endregion

        [Ignore, Test]//TODO: To check what happens when a collection is reset
        public void Test_Filter_WithAMatchThenEmptyFilter_ShouldResetCollection()
        {
            //---------------Set up test pack-------------------
            CreateSavedFakeBOW5Props(3);
            CreateSavedFakeBOW5Prop("SomeName", DateTime.Today, Guid.NewGuid(), 1);
            var collection = new BusinessObjectCollection<FakeBOW5Props>();
            collection.LoadAll();
            var bindingListView = new BindingListView<FakeBOW5Props>(collection) { Filter = "FakeBOName='SomeName'" };
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, collection.Count);
            //---------------Execute Test ----------------------
            bindingListView.Filter = "";
            //---------------Test Result -----------------------
            Assert.AreEqual(4, collection.Count);
        }

        #endregion


        private static void CreateSavedBOs(int numberToCreate)
        {
            for (int i = 0; i < numberToCreate; i++)
            {
                FakeBO businessObject = new FakeBO();
                businessObject.FakeBOName = "A" + RandomValueGen.GetRandomString();
                businessObject.Save();
            }
        }

        private static void CreateSavedFakeBOW5Props(int numberToCreate)
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

        private static void CreateSavedFakeBOW5Prop(string fakeBOName, DateTime fakeBODate, Guid fakeBOId, int fakeBONumber)
        {
            var businessObject = new FakeBOW5Props
                                     {
                                         FakeBOName = fakeBOName,
                                         FakeBODate = fakeBODate,
                                         FakeBOID = fakeBOId,
                                         FakeBONumber = fakeBONumber
                                     };
            businessObject.Save();
        }
    }

    public class BindingListViewSpy<T> : BindingListView<T> where T : class, IBusinessObject, new()
    {
        public BindingListViewSpy(BusinessObjectCollection<T> collection):base(collection)
        {
            
        }
    }
}
