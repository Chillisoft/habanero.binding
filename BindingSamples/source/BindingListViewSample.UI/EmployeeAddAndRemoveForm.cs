using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BindingListViewSample.BO;
using BindingListViewSample.Logic;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;

namespace BindingListViewSample.UI
{
    public partial class EmployeeAddAndRemoveForm : Form
    {
        private BindingListView<Employee> EmployeeBindingList { get; set; }

        public EmployeeAddAndRemoveForm()
        {
            InitializeComponent();
            EmployeeBindingList = new BindingListView<Employee>();
            BusinessObjectCollection<Employee> employees = new BusinessObjectCollection<Employee>();
            employees.LoadAll();
            EmployeeBindingList.BusinessObjectCollection = employees;
            employeeBindingSource.DataSource = EmployeeBindingList;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var employee = SampleDataGenerator.CreateNewRandomEmployee();
            EmployeeBindingList.Add(employee);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var selectedEmployee = employeeBindingSource.Current as Employee;
            if (selectedEmployee != null) EmployeeBindingList.Remove(selectedEmployee);
        }
    }
}
