using System.ComponentModel;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestCachedBindingListView
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
            CachedBindingListView<FakeBO> list = new CachedBindingListView<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(list);
            Assert.IsFalse(list.AllowEdit);
            Assert.IsFalse(list.AllowNew);
            Assert.IsFalse(list.AllowRemove);
            Assert.IsTrue(list.IsFixedSize);
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsTrue(list.SupportsAdvancedSorting);
            Assert.IsTrue(list.SupportsChangeNotification);
            Assert.IsTrue(list.SupportsFiltering);
            Assert.IsFalse(list.SupportsSearching);
            Assert.IsTrue(list.SupportsSorting);
        }
        [Test]
        public void Test_Construct_ShouldNotRaiseError()
        {
            //---------------Set up test pack-------------------
            var bindingListRequest = MockRepository.GenerateStub<BindingListRequest<FakeBO>>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            CachedBindingListView<FakeBO> list = new CachedBindingListView<FakeBO>(bindingListRequest);
            //---------------Test Result -----------------------
            Assert.IsNotNull(list);
            Assert.IsFalse(list.AllowEdit);
            Assert.IsFalse(list.AllowNew);
            Assert.IsFalse(list.AllowRemove);
            Assert.IsTrue(list.IsFixedSize);
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsTrue(list.SupportsAdvancedSorting);
            Assert.IsTrue(list.SupportsChangeNotification);
            Assert.IsTrue(list.SupportsFiltering);
            Assert.IsFalse(list.SupportsSearching);
            Assert.IsTrue(list.SupportsSorting);
        }

        [Test]
        public void Test_SetGetFilter_ShouldRetFilter()
        {
            //---------------Set up test pack-------------------
            var bindingListRequest = MockRepository.GenerateStub<BindingListRequest<FakeBO>>();
            const string expectedFilter = "FakeBOName = 1";
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            CachedBindingListView<FakeBO> list = new CachedBindingListView<FakeBO>(bindingListRequest)
                                                    {Filter = expectedFilter};
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilter, list.Filter);
        }

        [Test]
        public void Test_RemoveFilter_ShouldSetFilterToEmpty()
        {
            //---------------Set up test pack-------------------
            var bindingListRequest = MockRepository.GenerateStub<BindingListRequest<FakeBO>>();
            const string initialFilter = "FakeBOName = 1";
            CachedBindingListView<FakeBO> list = new CachedBindingListView<FakeBO>(bindingListRequest) { Filter = initialFilter };
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialFilter, list.Filter);
            //---------------Execute Test ----------------------
            list.RemoveFilter();
            //---------------Test Result -----------------------
            Assert.IsEmpty(list.Filter);
        }

        [Test]
        public void Test_Filter_ReturnsThePageProviderFilter()
        {
            //---------------Set up test pack-------------------
            var mockPageProvider = MockRepository.GenerateStub<PageProvider<FakeBO>>();
            const string expectedFilter = "FakeBOName = 1";
            mockPageProvider.Filter = expectedFilter;
            var list = new CachedBindingListViewSpy<FakeBO>(mockPageProvider);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedFilter, mockPageProvider.Filter);
            //---------------Execute Test ----------------------
            string actualFilter = list.Filter;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilter, actualFilter);
        }

        [Test]
        public void Test_SetViewBuilder_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var mockPageProvider = MockRepository.GenerateStub<PageProvider<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            var list = new CachedBindingListViewSpy<FakeBO>(mockPageProvider);
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
            var mockPageProvider = MockRepository.GenerateStub<PageProvider<FakeBO>>();
            IViewBuilder viewBuilder = MockRepository.GenerateStub<IViewBuilder>();
            viewBuilder.Stub(builder => builder.GetGridView()).Return(
                new PropertyDescriptorCollection(new PropertyDescriptor[0]));
            var list = new CachedBindingListViewSpy<FakeBO>(mockPageProvider) {ViewBuilder = viewBuilder};
            //---------------Assert Precondition----------------
            Assert.AreSame(viewBuilder, list.ViewBuilder);
            //---------------Execute Test ----------------------
            var pds = list.GetItemProperties(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            Assert.AreEqual(0, pds.Count);
        }

        [Test]
        public void Test_GetItemProperties_WhenNotHasViewBuilder_TypeDescriptorGetProperties()
        {
            //---------------Set up test pack-------------------
            var mockPageProvider = MockRepository.GenerateStub<PageProvider<FakeBO>>();
            var list = new CachedBindingListViewSpy<FakeBO>(mockPageProvider);
            //---------------Assert Precondition----------------
            Assert.IsNull(list.ViewBuilder);
            //---------------Execute Test ----------------------
            var pds = list.GetItemProperties(new PropertyDescriptor[0]);
            //---------------Test Result -----------------------
            var propertyInfos = typeof(FakeBO).GetProperties();
            Assert.AreEqual(propertyInfos.Length, pds.Count);
        }

        [Test]
        public void Test_GetItemIndex3_When3Loaded_ShouldReturnItem()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var bindingListRequest = new BindingListRequest<FakeBO> {PageNumber = 0, RowsPerPage = 3};
            var list = new CachedBindingListView<FakeBO>(bindingListRequest);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, list.Count);
            //---------------Execute Test ----------------------
            var fakeBO = list[3];
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }
        [Test]
        public void Test_GetItemIndex2_When3Loaded_ShouldReturnItem()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(5);
            var bindingListRequest = new BindingListRequest<FakeBO> {PageNumber = 0, RowsPerPage = 3};
            var list = new CachedBindingListView<FakeBO>(bindingListRequest);
            //---------------Assert Precondition----------------
            Assert.AreEqual(5, list.Count);
            //---------------Execute Test ----------------------
            var fakeBO = list[2];
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }
        [Test]
        public void Test_GetItemIndex5_When3Loaded_ShouldReturnItem()
        {
            //---------------Set up test pack-------------------
            CreateSavedBOs(6);
            var bindingListRequest = new BindingListRequest<FakeBO> {PageNumber = 0, RowsPerPage = 3};
            var list = new CachedBindingListView<FakeBO>(bindingListRequest);
            //---------------Assert Precondition----------------
            Assert.AreEqual(6, list.Count);
            //---------------Execute Test ----------------------
            var fakeBO = list[5];
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }
        //[Test]
        //public void Test_GetItemIndex6_When3Loaded_ShouldReturnItem()
        //{
        //    //---------------Set up test pack-------------------
        //    CreateSavedBOs(5);
        //    var bindingListRequest = new BindingListRequest<FakeBO> {PageNumber = 0, RowsPerPage = 3};
        //    var list = new CachedBindingListView<FakeBO>(bindingListRequest);
        //    //---------------Assert Precondition----------------
        //    Assert.AreEqual(5, list.Count);
        //    //---------------Execute Test ----------------------
        //    var fakeBO = list[6];
        //    //---------------Test Result -----------------------
        //    Assert.IsNotNull(fakeBO);
        //}

        private static void CreateSavedBOs(int numberToCreate)
        {
            for (int i = 0; i < numberToCreate; i++)
            {
                new FakeBO().Save();
            }
        }
    }

    class CachedBindingListViewSpy<T> : CachedBindingListView<T> where T : class, IBusinessObject, new()
    {
/*        
        public CachedBindingListViewSpy(BindingListRequest<T> bindingListRequest) : base(bindingListRequest)
        {
        }*/

        public CachedBindingListViewSpy(IPageProvider<T> pageProvider): base(pageProvider)
        {
        }


    }
}