using System;
using System.Collections.Generic;
using System.IO;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestPageProvider
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
            ClassDef.ClassDefs.Add(typeof(FakeBO).MapClasses());
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }
        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
            BusinessObjectManager.Instance.ClearLoadedObjects();
        }
        [Test]
        public void Test_Construct_PageProvider_ShouldSetupSortString()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            const string expectedSortString = "FakeBOName";
            request.Sort = expectedSortString;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedSortString, request.Sort);
            //---------------Execute Test ----------------------
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedSortString, pageProvider.Sort);
        }
        [Test]
        public void Test_Construct_PageProvider_ShouldSetupFilterString()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            const string expectedFilterString = "FakeBOName Like 'w%'";
            request.Filter = expectedFilterString;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedFilterString, request.Filter);
            //---------------Execute Test ----------------------
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilterString, pageProvider.Filter);
        }

        [Test]
        public void Test_Construct_WithNoBindingListRequest_ShouldCreateRequest()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(pageProvider.Filter);
        }
        [Test]
        public void Test_Construct_PageProvider_ShouldSetupRowsPerPage()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            const int expectedRowsPerPage = 99;
            request.RowsPerPage = expectedRowsPerPage;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedRowsPerPage, request.RowsPerPage);
            //---------------Execute Test ----------------------
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedRowsPerPage, pageProvider.RowsPerPage);
        }
        [Test]
        public void Test_SetSort_ShouldSetSortString()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            const string expectedSortString = "FakeBOName";
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(pageProvider.Sort);
            //---------------Execute Test ----------------------
            pageProvider.Sort = expectedSortString;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedSortString, pageProvider.Sort);
        }
        [Test]
        public void Test_SetFilter_ShouldSetFilterString()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            const string expectedFilterString = "FakeBOName Like 'w%'";
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(pageProvider.Filter);
            //---------------Execute Test ----------------------
            pageProvider.Filter = expectedFilterString;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilterString, pageProvider.Filter);
        }
        [Test]
        public void Test_GetdataPage_WhenRecordsInDataSource_ShouldReturnRecords()
        {
            //---------------Set up test pack-------------------         
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            BOTestFactory<FakeBO> assetFactory = BOTestFactoryRegistry.Instance.Resolve<FakeBO>();
            assetFactory.CreateManySavedBusinessObject(40);
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(pageProvider.Filter);
            Assert.AreEqual(0, request.PageNumber);
            Assert.AreEqual(20, request.RowsPerPage);
            //---------------Execute Test ----------------------
            IList<FakeBO> dataPage = pageProvider.GetDataPage(0);
            //---------------Test Result -----------------------
            Assert.AreEqual(request.RowsPerPage, dataPage.Count);
            //dataPage.
            
        }
/*TODO brett 25 Jun 2010:         [Test]
        public void Test_SetFilter_ShouldSetFilterString()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            PageProvider<FakeBO> pageProvider = new PageProvider<FakeBO>(request);
            const string expectedFilterString = "FakeBOName Like 'w%'";
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(pageProvider.Filter);
            //---------------Execute Test ----------------------
            pageProvider.Filter = expectedFilterString;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedFilterString, pageProvider.Filter);
        }*/
    }

    public class BusinessObjectManagerFake : BusinessObjectManager
    {
    }
}