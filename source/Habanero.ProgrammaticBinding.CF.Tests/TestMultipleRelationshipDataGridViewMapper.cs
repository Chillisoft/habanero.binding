using System;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Binding;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Testability.Helpers;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestMultipleRelationshipDataGridViewMapper
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.     

            ClassDef.ClassDefs.Add(typeof (FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalUIRegistry.ControlFactory = new ControlFactoryWin();
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [SetUp]
        public void SetupTest()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }

        private static IControlFactory GetControlFactory()
        {
            return new ControlFactoryWin();
        }

        private static MultipleRelationshipDataGridViewMapper CreateMultipleRelationshipDataGridViewMapper(IDataGridView grid, string propName)
        {
            return new MultipleRelationshipDataGridViewMapper(grid, propName, false, GetControlFactory());
        }

        [Test]
        public void TestConstructor()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();
            const string propName = "asdfa";

            //---------------Execute Test ----------------------
            var mapper = new MultipleRelationshipDataGridViewMapper(grid, propName, false, GetControlFactory());

            //---------------Test Result -----------------------
            Assert.AreSame(grid, mapper.Control);
            Assert.AreEqual(propName, mapper.PropertyName);
        }
        // ReSharper disable PossibleNullReferenceException
        [Test]
        public void Test_BusinessObject()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();

            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);
            var expectedBO = new FakeBoWithMultipleRelationship();
            var fakeBos = expectedBO.FakeBos;
            //---------------Assert PreConditions---------------        
            Assert.IsNull(mapper.BusinessObject);

            //---------------Execute Test ----------------------
            mapper.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedBO, mapper.BusinessObject);
            Assert.IsInstanceOf<BindingListView<FakeBo>>(grid.DataSource);
            Assert.AreSame(fakeBos, GetBOColFromGrid(grid));
        }
        // ReSharper restore PossibleNullReferenceException

        [Test]
        public void Test_BusinessObject_WhenWasNotNull_WhenNull_ShouldClearDataSource()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();

            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);
            var expectedBO = new FakeBoWithMultipleRelationship();
            mapper.BusinessObject = expectedBO;
            var fakeBos = expectedBO.FakeBos;
            //---------------Assert PreConditions---------------        
            Assert.AreSame(expectedBO, mapper.BusinessObject);
            Assert.AreSame(fakeBos, GetBOColFromGrid(grid));
            //---------------Execute Test ----------------------
            mapper.BusinessObject = null;
            //---------------Test Result -----------------------
            Assert.IsNull(mapper.BusinessObject);
            Assert.IsNull(grid.DataSource);
        }

        [Test]
        public void Test_BusinessObject_WhenNullInitialValue_shouldSetDataSourceNull()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();

            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);
            //---------------Assert PreConditions---------------        
            //---------------Execute Test ----------------------
            mapper.BusinessObject = null;
            //---------------Test Result -----------------------
            Assert.IsNull(mapper.BusinessObject);
            Assert.IsNull(grid.DataSource);
        }

        [Test]
        public void Test_SetBO_ShouldPopulateGridWithRelatedItems()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();
            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);

            var expectedBO = new FakeBoWithMultipleRelationship();
            expectedBO.FakeBos.CreateBusinessObject();
            expectedBO.FakeBos.CreateBusinessObject();
            AddToForm(grid);
            //---------------Assert PreConditions---------------            
            Assert.AreEqual(2, expectedBO.FakeBos.Count);
            Assert.AreEqual(0, grid.Rows.Count);
            //---------------Execute Test ----------------------
            mapper.BusinessObject = expectedBO;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedBO.FakeBos, GetBOColFromGrid(grid));
            Assert.AreEqual(expectedBO.FakeBos.Count + 1, grid.Rows.Count, "BOs plus add row");
        }

        [Test]
        public void Test_SelectedBusinessObjects_WhenNoRowsSelected_ShouldReturnEmptyList()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();
            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);

            var expectedBO = new FakeBoWithMultipleRelationship();
            expectedBO.FakeBos.CreateBusinessObject();
            expectedBO.FakeBos.CreateBusinessObject();
            AddToForm(grid);
            mapper.BusinessObject = expectedBO;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBO.FakeBos.Count + 1, grid.Rows.Count, "Include add row");
            Assert.AreEqual(0, grid.SelectedRows.Count);
            //---------------Execute Test ----------------------
            var selectedBusinessObjects = mapper.SelectedBusinessObjects;
            //---------------Test Result -----------------------
            Assert.AreEqual(0, selectedBusinessObjects.Count);
        }
        [Test]
        public void Test_SelectedBusinessObjects_WhenOneRowsSelected_ShouldReturnItem()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();
            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);

            var expectedBO = new FakeBoWithMultipleRelationship();
            var selectedBo = expectedBO.FakeBos.CreateBusinessObject();
            expectedBO.FakeBos.CreateBusinessObject();
            AddToForm(grid);
            mapper.BusinessObject = expectedBO;
            grid.Rows[0].Selected = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBO.FakeBos.Count + 1, grid.Rows.Count, "Include add row");
            Assert.AreEqual(1, grid.SelectedRows.Count);
            //---------------Execute Test ----------------------
            var selectedBusinessObjects = mapper.SelectedBusinessObjects;
            //---------------Test Result -----------------------
            Assert.AreEqual(1, selectedBusinessObjects.Count);
            selectedBusinessObjects.ShouldContain(selectedBo);
        }
        [Test]
        public void Test_SelectedBusinessObjects_WhenTwoRowsSelected_ShouldReturnItem()
        {
            //---------------Set up test pack-------------------
            var grid = GetControlFactory().CreateDataGridView();
            const string propName = "FakeBos";
            var mapper = CreateMultipleRelationshipDataGridViewMapper(grid, propName);

            var expectedBO = new FakeBoWithMultipleRelationship();
            var selectedBo = expectedBO.FakeBos.CreateBusinessObject();
            var selectedBO2 = expectedBO.FakeBos.CreateBusinessObject();
            AddToForm(grid);
            mapper.BusinessObject = expectedBO;
            grid.Rows[0].Selected = true;
            grid.Rows[1].Selected = true;
            //---------------Assert Precondition----------------
            Assert.AreEqual(expectedBO.FakeBos.Count + 1, grid.Rows.Count, "Include add row");
            Assert.AreEqual(2, grid.SelectedRows.Count);
            //---------------Execute Test ----------------------
            var selectedBusinessObjects = mapper.SelectedBusinessObjects;
            //---------------Test Result -----------------------
            Assert.AreEqual(2, selectedBusinessObjects.Count);
            selectedBusinessObjects.ShouldContain(selectedBo);
            selectedBusinessObjects.ShouldContain(selectedBO2);
        }


        private static IBusinessObjectCollection GetBOColFromGrid(IDataGridView grid)
        {
            var blvFakes = grid.DataSource as BindingListView<FakeBo>;
            Assert.IsNotNull(blvFakes);
            return blvFakes.BusinessObjectCollection;
        }

        /// <summary>
        /// This is strange but is required for testing.
        /// All the events only wire up property when the grid is
        /// on the form.
        /// </summary>
        /// <param name="grid"></param>
        private static void AddToForm(IDataGridView grid)
        {
            var form = new Form();
            var dataGridView = (DataGridView)grid;
            form.Controls.Add( dataGridView);
            form.Show();
        }
    }
}