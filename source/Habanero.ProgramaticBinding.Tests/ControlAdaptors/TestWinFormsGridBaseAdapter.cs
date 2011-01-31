using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Test.Win.Grid;
using Habanero.Faces.Win;
using Habanero.ProgramaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Test;
using Habanero.Testability;
using Habanero.Testability.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using IWinFormsGridBaseAdapter = Habanero.ProgramaticBinding.ControlAdaptors.IWinFormsGridBaseAdapter;

// ReSharper disable InconsistentNaming

namespace Habanero.ProgramaticBinding.Tests.ControlAdaptors
{
    [TestFixture]
    public class TestWinFormsGridBaseAdapter : TestGridBaseWin
    {
/*        private static IControlFactory GetControlFactory()
        {
            return new ControlFactoryWin();
        }*/
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
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
            IWinFormsGridBaseAdapter gridBaseAdapter = new WinFormsGridBaseAdapter(expectedWrappedControl);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedWrappedControl, gridBaseAdapter.WrappedControl);
        }


        [Test]
        public void Test_SelectItem_ShouldFireSelectionChanged()
        {
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            //---------------Set up test pack-------------------
            var gridView = new DataGridView();
            var gridBaseAdapter = GetGridBaseAdapter(gridView);
            gridBaseAdapter.BusinessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var selectionChangedEventFired = false;
            PlaceGridOnForm(gridView);
            gridBaseAdapter.SelectionChanged += (sender, args) => selectionChangedEventFired = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(gridView, gridBaseAdapter.WrappedControl);
            Assert.AreEqual(3, gridBaseAdapter.RowCount);
            Assert.AreEqual(3, gridView.RowCount, "Should have 3 items in the grid");
            Assert.IsFalse(selectionChangedEventFired);
            //---------------Execute Test ----------------------
            gridView.Rows[1].Selected = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(selectionChangedEventFired, "Selected Event Should Fire");
        }

        [Test]
        public void Test_SelectItem_ShouldFireBusinessObjectSelected()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            var gridView = new DataGridView();
            var gridBaseAdapter = GetGridBaseAdapter(gridView);
            gridBaseAdapter.BusinessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var businessObjectSelectedFired = false;
            PlaceGridOnForm(gridView);
            gridBaseAdapter.BusinessObjectSelected += (sender, args) => businessObjectSelectedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(gridView, gridBaseAdapter.WrappedControl);
            Assert.AreEqual(3, gridBaseAdapter.RowCount);
            Assert.AreEqual(3, gridView.RowCount, "Should have 3 items in the grid");
            Assert.IsFalse(businessObjectSelectedFired);
            //---------------Execute Test ----------------------
            gridView.Rows[1].Selected = true;
            //---------------Test Result -----------------------
            Assert.IsTrue(businessObjectSelectedFired, "Selected Event Should Fire");
        }
        [Test]
        public void Test_ResetCollection_ShouldFireCollectionChanged()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            var gridView = new DataGridView();
            var gridBaseAdapter = GetGridBaseAdapter(gridView);
            gridBaseAdapter.BusinessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var collectionChangedFired = false;
            PlaceGridOnForm(gridView);
            gridBaseAdapter.CollectionChanged += (sender, args) => collectionChangedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(gridView, gridBaseAdapter.WrappedControl);
            Assert.IsFalse(collectionChangedFired);
            //---------------Execute Test ----------------------
            gridBaseAdapter.BusinessObjectCollection = new BusinessObjectCollection<FakeBo>();
            //---------------Test Result -----------------------
            Assert.IsTrue(collectionChangedFired, "Selected Event Should Fire");
        }

        [Test]
        public void Test_ApplySearch_ShouldFireFilterUpdated()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Add(typeof(FakeBo).MapClasses());
            var gridView = new DataGridView();
            var gridBaseAdapter = GetGridBaseAdapter(gridView) as WinFormsGridBaseAdapter;
            Assert.IsNotNull(gridBaseAdapter);
            gridBaseAdapter.BusinessObjectCollection = GetBusinessObjectCollectionWith3Items();
            var filterUpdatedFired = false;
            PlaceGridOnForm(gridView);
            gridBaseAdapter.FilterUpdated += (sender, args) => filterUpdatedFired = true;
            //---------------Assert Precondition----------------
            Assert.AreSame(gridView, gridBaseAdapter.WrappedControl);
            Assert.IsFalse(filterUpdatedFired);
            //---------------Execute Test ----------------------
            gridBaseAdapter.ApplySearch("FakeStringProp = fdasf", "FakeStringProp");
            //---------------Test Result -----------------------
            Assert.IsTrue(filterUpdatedFired, "Selected Event Should Fire");
        }

        [Test]
        public void TestClearFilter1()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            string filterString = col[2].TestProp.ToString().Substring(0, 10);
            IFilterClauseFactory factory = new DataViewFilterClauseFactory();
            IFilterClause filterClause =
                factory.CreateStringFilterClause("TestProp", FilterClauseOperator.OpLike, filterString);
            gridBase.ApplyFilter(filterClause);

            //---------------Verify PreConditions --------------
            Assert.AreEqual(1, gridBase.Rows.Count, "Filtered Grid should have 1 item");

            //---------------Execute Test ----------------------
            gridBase.ApplyFilter(null);

            //---------------Test Result -----------------------

            Assert.AreEqual(4, gridBase.Rows.Count);
            //---------------Tear Down -------------------------
        }

        #region temp


        [Test]
        public override void TestGetBusinessObjectAtRow()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            //---------------Execute Test ----------------------
            IBusinessObject businessObject2 = gridBase.GetBusinessObjectAtRow(2);
            IBusinessObject businessObject3 = gridBase.GetBusinessObjectAtRow(3);
            //---------------Test Result -----------------------
            Assert.AreSame(col[2], businessObject2);
            Assert.AreSame(col[3], businessObject3);
        }
        [Test]
        public override void TestGetBusinessObjectAtRow_WhenCustomLoad_ShouldReturnBO()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectCollection<MyBO> col = CreateCollectionWith_4_Objects();
            IGridBase gridBase = CreateGridBaseStub();
            gridBase.GridLoader = GridLoaderDelegate;
            SetupGridColumnsForMyBo(gridBase);
            gridBase.BusinessObjectCollection = col;
            //---------------Assert Preconditions---------------
            Assert.IsNull(gridBase.DataSetProvider);
            Assert.IsNull(gridBase.DataSource);
            Assert.AreEqual(4, gridBase.RowCount);
            //---------------Execute Test ----------------------
            IBusinessObject businessObject2 = gridBase.GetBusinessObjectAtRow(2);
            IBusinessObject businessObject3 = gridBase.GetBusinessObjectAtRow(3);
            //---------------Test Result -----------------------
            Assert.AreSame(col[2], businessObject2);
            Assert.AreSame(col[3], businessObject3);
        }
        [Test]
        public override void TestGetBusinessObjectAtRow_WhenDataSourceNotNullButDataSetProviderNull_ShouldReturnBO()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectCollection<MyBO> col = CreateCollectionWith_4_Objects();
            IGridBase gridBase = CreateGridBaseStub();
            gridBase.GridLoader = GridLoaderDelegateSetDataSource;
            SetupGridColumnsForMyBo(gridBase);
            gridBase.BusinessObjectCollection = col;
            //---------------Assert Preconditions---------------
            Assert.IsNull(gridBase.DataSetProvider);
            Assert.IsNotNull(gridBase.DataSource);
            //---------------Execute Test ----------------------
            IBusinessObject businessObject2 = gridBase.GetBusinessObjectAtRow(2);
            IBusinessObject businessObject3 = gridBase.GetBusinessObjectAtRow(3);
            //---------------Test Result -----------------------
            Assert.AreSame(col[2], businessObject2);
            Assert.AreSame(col[3], businessObject3);
        }

        [Test]
        public override void TestGetBusinessObjectAtRow_WhenGridHasObjectIDButBOColNotHasObject_ShouldLoadBO()
        {
            //This is a fairly specific situation but can occur when you are using
            // a CachedBindingList or a paginaged BindingList where the BOCol that the
            // grid has reference to does not have any BusinessObjects.
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectCollection<MyBO> col = CreateCollectionWith_4_SavedObjects();

            IGridBase gridBase = CreateGridBaseStub();
            var loaderClass = new CustomDelegateLoaderClass(col);
            gridBase.GridLoader = loaderClass.GridLoaderDelegateLoadFromDiffCol;
            SetupGridColumnsForMyBo(gridBase);
            gridBase.BusinessObjectCollection = new BusinessObjectCollection<MyBO>();
            //---------------Assert Preconditions---------------
            Assert.IsNull(gridBase.DataSetProvider);
            Assert.IsNotNull(gridBase.DataSource);
            col.Refresh();
            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(4, gridBase.RowCount);
            //---------------Execute Test ----------------------
            IBusinessObject businessObject2 = gridBase.GetBusinessObjectAtRow(2);
            IBusinessObject businessObject3 = gridBase.GetBusinessObjectAtRow(3);
            //---------------Test Result -----------------------
            Assert.AreSame(col[2], businessObject2);
            Assert.AreSame(col[3], businessObject3);
        }

        [Test]
        public override void TestSetSelectedBO_WhenGridHasObjectIDButBOColNotHasObject_ShouldSetBO()
        {
            //This is a fairly specific situation but can occur when you are using
            // a CachedBindingList or a paginaged BindingList where the BOCol that the
            // grid has reference to does not have any BusinessObjects.
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectCollection<MyBO> col = CreateCollectionWith_4_SavedObjects();

            IGridBase gridBase = CreateGridBaseStub();
            gridBase.GridLoader = new CustomDelegateLoaderClass(col).GridLoaderDelegateLoadFromDiffCol;
            SetupGridColumnsForMyBo(gridBase);
            gridBase.BusinessObjectCollection = new BusinessObjectCollection<MyBO>();
            //---------------Assert Preconditions---------------
            Assert.IsNull(gridBase.DataSetProvider);
            Assert.IsNotNull(gridBase.DataSource);
            col.Refresh();
            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(4, gridBase.RowCount);
            //---------------Execute Test ----------------------
            var expectedSelectedBo = col[2];
            gridBase.SelectedBusinessObject = expectedSelectedBo;
            var actualSelectedBO = gridBase.SelectedBusinessObject;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedSelectedBo, actualSelectedBO);
        }


        private static void GridLoaderDelegate(IGridBase grid, IBusinessObjectCollection col)
        {
            //grid.Rows.Clear();
            if (col == null)
            {
                grid.DataSource = null;
                return;
            }
            foreach (IBusinessObject businessObject in col)
            {
                MyBO asset = (MyBO)businessObject;
                grid.Rows.Add(
                    asset.ID.ObjectID,
                    asset.TestProp);
            }
        }
        private void GridLoaderDelegateSetDataSource(IGridBase grid, IBusinessObjectCollection col)
        {
            if (col == null)
            {
                grid.DataSource = null;
                return;
            }
            var classType = GetClassType(col);
            var bindingListType = typeof(BindingListView<>).MakeGenericType(classType);
            var bindingListView = (IBindingListView) Activator.CreateInstance(bindingListType, col);
/*            var dataSetProvider = new ReadOnlyDataSetProvider(col);
            IUIDef uiDef = ((ClassDef)col.ClassDef).GetUIDef(grid.UiDefName);*/
            grid.DataSource = bindingListView;
        }
        private static Type GetClassType(IBusinessObjectCollection businessObjectCollection)
        {
            return businessObjectCollection.ClassDef.ClassType;
        }
        [Test]
        public override void TestGetBusinessObjectRow()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            MyBO myBO2 = col[2];
            const int expectedIndex = 2;
            //-------------Assert Preconditions -------------
            Assert.AreSame(myBO2, gridBase.GetBusinessObjectAtRow(expectedIndex));
            //---------------Execute Test ----------------------
            IDataGridViewRow dataGridViewRow = gridBase.GetBusinessObjectRow(myBO2);
            //---------------Test Result -----------------------
            Assert.IsNotNull(dataGridViewRow, "Should return related row");
            Assert.AreEqual(expectedIndex, dataGridViewRow.Index);
        }

        [Test]
        public override void TestGetBusinessObjectRow_NullBO()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            IDataGridViewRow dataGridViewRow = gridBase.GetBusinessObjectRow(null);
            //---------------Test Result -----------------------
            Assert.IsNull(dataGridViewRow);
        }

        [Test]
        public override void TestGetBusinessObjectRow_ReturnsNullIfNotFound()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            MyBO notFoundBO = new MyBO();
            //-------------Assert Preconditions -------------
            //---------------Execute Test ----------------------
            IDataGridViewRow dataGridViewRow = gridBase.GetBusinessObjectRow(notFoundBO);
            //---------------Test Result -----------------------
            Assert.IsNull(dataGridViewRow);
        }


        [Test]
        public void TestSelectedBusinessObject_SetsCurrentRow_2()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            SetupGridColumnsForMyBo(gridBase);
            MyBO firstBO = col[0];
            MyBO secondBO = col[1];
            //---------------Assert Precondition----------------
            Assert.AreEqual(firstBO, gridBase.SelectedBusinessObject);
            Assert.IsNull(gridBase.CurrentRow);
            //Assert.AreEqual(0, gridBase.Rows.IndexOf(gridBase.CurrentRow));   //surely the currentrow should be active on setCol?
            //---------------Execute Test ----------------------
            gridBase.SelectedBusinessObject = secondBO;
            //---------------Test Result -----------------------
            Assert.AreEqual(secondBO, gridBase.SelectedBusinessObject);
            Assert.AreEqual(1, gridBase.Rows.IndexOf(gridBase.CurrentRow));
        }


        [Test]
        public void TestGetBusinessObjectAtRow_2()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            //---------------Execute Test ----------------------
            IBusinessObject businessObject2 = gridBase.GetBusinessObjectAtRow(2);
            IBusinessObject businessObject3 = gridBase.GetBusinessObjectAtRow(3);
            //---------------Test Result -----------------------
            Assert.AreSame(col[2], businessObject2);
            Assert.AreSame(col[3], businessObject3);
        }

        //TODO brett 30 Jan 2011: this is not yet implemented for binding grid
/*        [Test]
        public void TestRemoveItemFromCollectionRemovesItemFromGrid_2()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            IGridBase gridBase = GetGridBaseWith_4_Rows(out col);
            MyBO bo = col[1];
            //---------------Verify precondition----------------
            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(4, gridBase.Rows.Count);
            //---------------Execute Test ----------------------
            col.Remove(bo);
            //---------------Test Result -----------------------
            Assert.AreEqual(3, col.Count);
            Assert.AreEqual(3, gridBase.Rows.Count);
        }*/

        [Test]
        public override void TestRefreshBusinessObjectRow()
        {
            //This test does not apply when using a binding list since the UI is immediately updated when a bo is updated and visa versa.
        }
            
        #endregion


        [Test]
        public override void TestWinApplyFilterFiresFilterUpdatedEvent()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            var gridBase = GetGridBaseWith_4_Rows(out col) as WinFormsGridBaseAdapter;
            Assert.IsNotNull(gridBase);
            string filterString = col[2].TestProp.ToString().Substring(1, 10);
            IFilterClauseFactory factory = new DataViewFilterClauseFactory();
            IFilterClause filterClause =
                factory.CreateStringFilterClause("TestProp", FilterClauseOperator.OpLike, filterString);
            bool filterUpdatedFired = false;
            gridBase.FilterUpdated += delegate { filterUpdatedFired = true; };
            //---------------Execute Test ----------------------
            gridBase.ApplyFilter(filterClause);
            //---------------Test Result -----------------------
            Assert.IsTrue(filterUpdatedFired);
        }
        [Test]
        public override void TestWinApplySearch_ShouldFireFilterUpdatedEvent()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            var gridBase = GetGridBaseWith_4_Rows(out col) as WinFormsGridBaseAdapter;
            Assert.IsNotNull(gridBase);
            bool filterUpdatedFired = false;
            gridBase.FilterUpdated += delegate { filterUpdatedFired = true; };
            //---------------Execute Test ----------------------
            gridBase.ApplySearch("", "");
            //---------------Test Result -----------------------
            Assert.IsTrue(filterUpdatedFired);
        }
        [Test]
        public override void TestWinApplySearch_WithFilterClause_ShouldFireFilterUpdatedEvent()
        {
            //---------------Set up test pack-------------------
            BusinessObjectCollection<MyBO> col;
            var gridBase = GetGridBaseWith_4_Rows(out col) as WinFormsGridBaseAdapter;
            Assert.IsNotNull(gridBase);
            string filterString = col[2].ID.ToString().Substring(5, 30);
            IFilterClauseFactory factory = new DataViewFilterClauseFactory();
            IFilterClause filterClause =
                factory.CreateStringFilterClause("TestProp", FilterClauseOperator.OpLike, filterString);

            bool filterUpdatedFired = false;
            gridBase.FilterUpdated += delegate { filterUpdatedFired = true; };
            //---------------Execute Test ----------------------
            gridBase.ApplySearch(filterClause, "");
            //---------------Test Result -----------------------
            Assert.IsTrue(filterUpdatedFired);
        }

        [Ignore("Working on this")] //TODO Brett 30 Jan 2011: Ignored Test - Working on this
        [Test]
        public override void Test_SetDateToGridCustomFormat_LoadViaCollection()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadClassDefWithDateTime();
            IDataGridViewColumn column;
            const string requiredFormat = "dd.MMM.yyyy";
            IGridBase gridBase = CreateGridBaseWithDateCustomFormatCol(out column, requiredFormat);
            BusinessObjectCollection<MyBO> col = new BusinessObjectCollection<MyBO>();
            MyBO bo = new MyBO();
            const string dateTimeProp = "TestDateTime";
            DateTime expectedDate = DateTime.Now;
            bo.SetPropertyValue(dateTimeProp, expectedDate);
            col.Add(bo);
            //--------------Assert PreConditions----------------      
            Assert.AreEqual(expectedDate, bo.GetPropertyValue(dateTimeProp));
            Assert.AreEqual(1, gridBase.ColumnCount);
            Assert.AreEqual(dateTimeProp, gridBase.Columns[0].Name);
            Assert.AreEqual(dateTimeProp, gridBase.Columns[0].DataPropertyName);
            //---------------Execute Test ----------------------
            gridBase.BusinessObjectCollection = col;
            //---------------Test Result -----------------------
            IDataGridViewCell dataGridViewCell = gridBase.Rows[0].Cells[0];
            Assert.AreEqual(expectedDate.ToString(requiredFormat), dataGridViewCell.FormattedValue);
        }

        [Test]
        public override void Test_SetDateToGridCustomFormat_LoadViaDataTable()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadClassDefWithDateTime();
            IDataGridViewColumn column;
            const string requiredFormat = "dd.MMM.yyyy";
            IGridBase gridBase = CreateGridBaseWithDateCustomFormatCol(out column, requiredFormat);
            DataTable dataTable = new DataTable();
            const string dateTimeProp = "TestDateTime";
            dataTable.Columns.Add(dateTimeProp, typeof(DateTime));
            DateTime expectedDate = DateTime.Now;
            dataTable.Rows.Add(expectedDate);
            //--------------Assert PreConditions----------------            
            //---------------Execute Test ----------------------
            gridBase.DataSource = dataTable.DefaultView;
            //---------------Test Result -----------------------
            IDataGridViewCell dataGridViewCell = gridBase.Rows[0].Cells[0];
            Assert.AreEqual(expectedDate.ToString(requiredFormat), dataGridViewCell.FormattedValue);
        }

        private static IWinFormsGridBaseAdapter GetGridBaseAdapter(DataGridView gridView)
        {
            IWinFormsGridBaseAdapter gridBaseAdapter = new WinFormsGridBaseAdapter(gridView);
            gridView.Columns.Add("ff", "fdfd");
            return gridBaseAdapter;
        }

        /// <summary>
        /// This is essential since the events binding and stuff only works when 
        /// the grid is on a form
        /// </summary>
        /// <param name="gridView"></param>
        private static void PlaceGridOnForm(DataGridView gridView)
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


        protected override IGridBase CreateGridBaseStub()
        {
            var dataGridView = new DataGridView();
            System.Windows.Forms.Form frm = new System.Windows.Forms.Form();
            frm.Controls.Add(dataGridView);
            return new WinFormsGridBaseAdapterStub(dataGridView);
        }
    }

    internal class WinFormsGridBaseAdapterStub : WinFormsGridBaseAdapter
    {
        public override bool CheckUserWantsToDelete()
        {
            return true;
        }
        public WinFormsGridBaseAdapterStub(DataGridView gridView) : base(gridView)
        {
        }
    }
}