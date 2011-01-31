using System.Windows.Forms;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;
// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests.ControlAdaptors
{
    [TestFixture]
    public class TestWinFormsReadOnlyGridAdapter
    {

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [Test]
        public void Test_Constuct_ShouldSetWrappedControl()
        {
            //---------------Set up test pack-------------------
            var expectedWrappedControl = GenerateStub<DataGridView>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IWinFormsReadOnlyGridAdapter adapter = new WinFormsReadOnlyGridAdapter(expectedWrappedControl);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedWrappedControl, adapter.WrappedControl);
        }




        private static T GenerateStub<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }
}