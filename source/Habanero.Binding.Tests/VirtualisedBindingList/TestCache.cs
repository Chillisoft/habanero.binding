using Habanero.BO;
using NUnit.Framework;
using Rhino.Mocks;
using Habanero.Smooth;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestCache
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
            typeof (FakeBO).MapClasses();
/*            ClassDef.ClassDefs.Clear();
            XmlClassDefsLoader loader = new XmlClassDefsLoader(new StreamReader("Classdefs.xml").ReadToEnd(), new DtdLoader());
            ClassDef.ClassDefs.Add(loader.LoadClassDefs());*/
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }
        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
        }

        [Test]
        public void Test_Construct_ShouldSetRowsPerPage()
        {
            //---------------Set up test pack-------------------
            IPageProvider<FakeBO> pageProvider = MockRepository.GenerateStub<IPageProvider<FakeBO>>();
            const int expectedRowsPerpage = 93;
            pageProvider.Stub(provider => provider.RowsPerPage).Return(expectedRowsPerpage);
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedRowsPerpage, pageProvider.RowsPerPage);
            //---------------Execute Test ----------------------
            Cache<FakeBO> cache = new Cache<FakeBO>(pageProvider);
            //---------------Test Result -----------------------
            //Assert.AreEqual(expectedRowsPerpage, cache.P);
            //Assert.Fail("Not Yet Implemented");
        }
    }
}