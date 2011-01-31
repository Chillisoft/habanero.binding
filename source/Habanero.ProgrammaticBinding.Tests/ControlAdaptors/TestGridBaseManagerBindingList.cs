using System.Windows.Forms;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Win;
using Habanero.ProgramaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgramaticBinding.Tests.ControlAdaptors
{
    [TestFixture]
    public class TestGridBaseManagerBindingList
    {

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof (FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalRegistry.LoggerFactory = new HabaneroConsoleLoggerFactory();
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
            IWinFormsDataGridViewAdapter dataGridViewAdapter = new WinFormsDataGridViewAdapter(expectedWrappedControl);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedWrappedControl, dataGridViewAdapter.WrappedControl);
        }

        [Test]
        public void Test_SelectItem_ShouldFireSelectionChanged()
        {
            //---------------Set up test pack-------------------
            var gridView = new DataGridView();
            IWinFormsDataGridViewAdapter dataGridViewAdapter = new WinFormsDataGridViewAdapter(gridView);
            var bindingListView = new BindingListView<FakeBo>(GetBusinessObjectCollectionWith3Items());
            gridView.DataSource = bindingListView;
            bool selectionChangedEventFired = false;
            PlaceGridOnForm(gridView);
            dataGridViewAdapter.SelectionChanged += (sender, args) => selectionChangedEventFired = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(gridView, dataGridViewAdapter.WrappedControl);
            Assert.AreEqual(3, bindingListView.Count);
            Assert.AreEqual(3 + 1, gridView.RowCount, "Should have 3 items in the grid plus AddRow");
            Assert.IsFalse(selectionChangedEventFired);
            //---------------Execute Test ----------------------
            gridView.Rows[1].Selected = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(selectionChangedEventFired, "Selected Event Should Fire");
        }


        [Test]
        public void Test_SetSelectedBOToNull_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var gridView = new DataGridView();
            gridView.Columns.Add("FakeStringProp", "FakeStringProp");
            var dataGridViewAdapter = new WinFormsGridBaseAdapter(gridView);
            var gridBaseManager = new GridBaseManagerBindingList(dataGridViewAdapter);

            var boCol = GetBusinessObjectCollectionWith3Items();
            gridBaseManager.SetBusinessObjectCollection(boCol);
            PlaceGridOnForm(gridView);
            gridView.Rows[0].Selected = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(boCol[0], gridBaseManager.SelectedBusinessObject);
            //---------------Execute Test ----------------------
            gridBaseManager.SelectedBusinessObject = null;
            //---------------Test Result -----------------------
            Assert.IsNull(gridBaseManager.SelectedBusinessObject);
            Assert.IsFalse(gridView.Rows[0].Selected);
        }
        [Test]
        public void Test_SetSelectedBOToNull_ShouldRaiseSelectedEvent()
        {
            //---------------Set up test pack-------------------
            var gridView = new DataGridView();
            gridView.Columns.Add("FakeStringProp", "FakeStringProp");
            var dataGridViewAdapter = new WinFormsGridBaseAdapter(gridView);
            var gridBaseManager = new GridBaseManagerBindingList(dataGridViewAdapter);

            var boCol = GetBusinessObjectCollectionWith3Items();
            gridBaseManager.SetBusinessObjectCollection(boCol);
            PlaceGridOnForm(gridView);
            gridView.Rows[0].Selected = true;
            IBusinessObject selectedBO = boCol[0];
            bool selectedEventFired = false;
            gridBaseManager.BusinessObjectSelected += (sender, args) =>
                                                          {
                                                              selectedEventFired = true;
                                                              selectedBO = args.BusinessObject;
                                                          }; 
            //---------------Assert Precondition----------------
            Assert.AreSame(boCol[0], selectedBO);
            Assert.IsFalse(selectedEventFired);
            //---------------Execute Test ----------------------
            gridBaseManager.SelectedBusinessObject = null;
            //---------------Test Result -----------------------
            Assert.IsTrue(selectedEventFired);
            Assert.IsNull(selectedBO);
        }

        /// <summary>
        /// This is essential since the events binding and stuff only works when 
        /// the grid is on a form
        /// </summary>
        /// <param name="gridView"></param>
        private void PlaceGridOnForm(DataGridView gridView)
        {
            var form = new Form();
            form.Controls.Add(gridView);
            form.Show();
        }


        private static BusinessObjectCollection<FakeBo> GetBusinessObjectCollectionWith3Items()
        {
            var businessObjectCollection = new BusinessObjectCollection<FakeBo>();
            businessObjectCollection.CreateBusinessObject();
            businessObjectCollection.CreateBusinessObject();
            businessObjectCollection.CreateBusinessObject();
            return businessObjectCollection;
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