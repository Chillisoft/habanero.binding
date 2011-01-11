using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BindingListViewSample.BO;
using BindingListViewSample.Test.BO;
using BindingListViewSample.UI;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using NUnit.Framework;

namespace BindingListViewSample.Test.UI
{
    [TestFixture]
    public class TestEmployeeAddRemoveForm
    {
        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            TestBaseUI.SetupTestFixture();
        }

        [SetUp]
        public void Setup()
        {
            TestBaseUI.SetupTest();
        }

        public IControlFactory GetControlFactory()
        {
            return GlobalUIRegistry.ControlFactory;
        }

        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var form = new EmployeeAddRemoveForm();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<FormWin>(form);
            Assert.AreSame(form.ControlFactory, GetControlFactory());
            Assert.AreEqual(2, form.Controls.Count);
            Assert.IsInstanceOf<IReadOnlyGridControl>(form.Controls[0]);
        }

        [Test]
        public void Test_EmployeeGridControl_Construct()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var gridControl = (IReadOnlyGridControl)form.Controls[0];
            //---------------Test Result -----------------------
            Assert.AreSame(form.EmployeeGridControl, gridControl);
            Assert.IsFalse(gridControl.Buttons["Add"].Visible);
            Assert.IsFalse(gridControl.Buttons["Edit"].Visible);
            Assert.IsFalse(gridControl.Buttons["Delete"].Visible);
            Assert.AreEqual(3, gridControl.Buttons.Controls.Count);
        }

        [Test]
        public void Test_ButtonGroupControl_Construct()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var buttonGroupControl = (IButtonGroupControl)form.Controls[1];
            //---------------Test Result -----------------------
            Assert.IsNotNull(buttonGroupControl);
            Assert.IsInstanceOf<IButtonGroupControl>(buttonGroupControl);
            Assert.AreSame(buttonGroupControl, form.ButtonGroupControl);
        }

        [Test]
        public void Test_AddNewEmployeeButton_Construct()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            var buttonGroupControl = (IButtonGroupControl)form.Controls[1];
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var addNewEmpButton = buttonGroupControl.Controls[0];
            //---------------Test Result -----------------------
            Assert.IsNotNull(addNewEmpButton);
            Assert.IsInstanceOf<IButton>(addNewEmpButton);
            Assert.AreSame(addNewEmpButton, form.AddNewEmployeeButton);
            Assert.AreEqual("Add New Employee", addNewEmpButton.Text);
        }

        [Test]
        public void Test_RemoveEmployeeButton_Construct()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            var buttonGroupControl = (IButtonGroupControl)form.Controls[1];
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var removeEmpButton = buttonGroupControl.Controls[1];
            //---------------Test Result -----------------------
            Assert.IsNotNull(removeEmpButton);
            Assert.IsInstanceOf<IButton>(removeEmpButton);
            Assert.AreSame(removeEmpButton, form.RemoveEmployeeButton);
            Assert.AreEqual("Remove Employee", removeEmpButton.Text);
        }

        [Test]
        public void Test_BindingListView_Construct()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BindingListView<Employee> employeeBindList = form.EmployeeBindList;
            //---------------Test Result -----------------------
            Assert.IsNotNull(employeeBindList);
            Assert.AreSame(employeeBindList, form.EmployeeBindList);
            Assert.IsInstanceOf<BindingListView<Employee>>(employeeBindList);
        }

        [Test]
        public void Test_BindListView_Add_ShouldAddEmployeeToTheBusinessObjectCollection()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            Employee employee = CreateNewEmployee();
            BindingListView<Employee> employees = form.EmployeeBindList;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(employee);
            Assert.AreEqual(0, employees.Count);
            Assert.AreEqual(0, employees.BusinessObjectCollection.Count());
            //---------------Execute Test ----------------------
            employees.Add(employee);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, employees.Count);
            Assert.AreEqual(1, employees.BusinessObjectCollection.Count());
        }

        [Test]
        public void Test_Add_WhenNewEmployeeAdd_ShouldAddEmployeeToEmployeeGridControlCollection()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            Employee employee = CreateNewEmployee();
            BindingListView<Employee> employees = form.EmployeeBindList;
            employees.Add(employee);
            IReadOnlyGridControl employeeGridControl = form.EmployeeGridControl;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(employee);
            Assert.AreEqual(1, employees.Count);
            Assert.AreEqual(1, employees.BusinessObjectCollection.Count);
            Assert.AreEqual(1, employeeGridControl.BusinessObjectCollection.Count);
            //---------------Execute Test ----------------------
            employees.Add(CreateNewEmployee());
            //---------------Test Result -----------------------
            Assert.AreEqual(2, employeeGridControl.BusinessObjectCollection.Count);
        }

        [Test]
        public void Test_AddNewEmployee_ShouldAddNewEmployee()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            BindingListView<Employee> employees = form.EmployeeBindList;
            employees.Add(CreateNewEmployee());
            employees.Add(CreateNewEmployee());
            IReadOnlyGridControl employeeGridControl = form.EmployeeGridControl;
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, employees.Count);
            Assert.AreEqual(2, employeeGridControl.BusinessObjectCollection.Count);
            //---------------Execute Test ----------------------
            form.AddNewEmployee();
            //---------------Test Result -----------------------
            Assert.AreEqual(3, employees.Count);
            Assert.AreEqual(3, employeeGridControl.BusinessObjectCollection.Count);
        }

        [Test]
        public void Test_RemoveEmployee_ShouldRemoveEmployee()
        {
            //---------------Set up test pack-------------------
            var form = new EmployeeAddRemoveForm();
            Employee employee1 = CreateNewEmployee();
            BindingListView<Employee> employees = form.EmployeeBindList;
            employees.Add(employee1);
            employees.Add(CreateNewEmployee());
            IReadOnlyGridControl employeeGridControl = form.EmployeeGridControl;
            form.SelectedEmployee = employee1;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(employee1);
            Assert.Contains(employee1, employeeGridControl.BusinessObjectCollection);
            Assert.AreEqual(2, employees.Count);
            Assert.AreEqual(2, employeeGridControl.BusinessObjectCollection.Count);
            //---------------Execute Test ----------------------
            form.RemoveEmployee();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, employees.Count);
            Assert.AreEqual(1, employeeGridControl.BusinessObjectCollection.Count);
        }

        private static Employee CreateNewEmployee()
        {
            return TestUtilsEmployee.CreateSavedEmployee();
        }
    }
}
