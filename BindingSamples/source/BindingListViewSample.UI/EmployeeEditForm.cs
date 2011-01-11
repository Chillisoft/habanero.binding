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
using Habanero.Faces.Base;

namespace BindingListViewSample.UI
{
    public partial class EmployeeEditForm : Form
    {
        protected BindingListView<Employee> EmployeeBindingList { get; set; }
        //protected Employee SelectedEmployee { get; set; }
        
        public EmployeeEditForm()
        {
            InitializeComponent();
            var employees = Broker.GetBusinessObjectCollection<Employee>("", "");
            EmployeeBindingList = new BindingListView<Employee>(employees);

            employeeBindingSource.DataSource = EmployeeBindingList;
            
            //-----You can also set the Bindings programmatically------

            // employeeListBox.DataSource = employeeBindingSource;
            // errorProvider1.DataSource = employeeBindingSource;
            
            //firstNameTxtBox.DataBindings.Add("Text", employeeBindingSource, "FirstName");
            //lastNameTxtBox.DataBindings.Add("Text", employeeBindingSource, "LastName");
            //salaryTxtBox.DataBindings.Add("Text", employeeBindingSource, "Salary");
            //birthDatedateTimePicker.DataBindings.Add("Text", employeeBindingSource, "BirthDate");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedEmployee = employeeBindingSource.Current as Employee;
                if (selectedEmployee != null) selectedEmployee.Save();

                employeeBindingSource.ResetBindings(true);
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }
    }

  
}

