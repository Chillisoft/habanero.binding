using System;
using System.Windows.Forms;
using BindingListViewSample.BO;
using BindingListViewSample.Logic;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;
using Habanero.Faces.Base;

namespace HabaneroProgramaticBinding.UI
{
    public partial class EmployeeEditForm : Form
    {
        private HabaneroControlBinder _habaneroControlBinder;

        //private BindingListView<Employee> EmployeeBindingList { get; set; }
        protected Employee SelectedEmployee { get; set; }
        
        public EmployeeEditForm()
        {
            InitializeComponent();


            _habaneroControlBinder = new HabaneroControlBinder();
            _habaneroControlBinder.AddTextBoxMapper(txtFirstName, "FirstName");
            _habaneroControlBinder.AddTextBoxMapper(txtLastName, "LastName");
            _habaneroControlBinder.AddTextBoxMapper(txtSalary, "Salary");
            _habaneroControlBinder.AddDateTimePicker(dtpBirthDate, "BirthDate");


            var employees = Broker.GetBusinessObjectCollection<Employee>("","");
            lstEmployees.DataSource = employees;
            //-----------------The list could also be bound using the BindingList ----------------
            //EmployeeBindingList = new BindingListView<Employee>(employees);
            //lstEmployees.DataSource = EmployeeBindingList;

            lstEmployees.SelectedIndexChanged +=LstEmployeesOnSelectedIndexChanged;
        }

        private void LstEmployeesOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            SelectedEmployee = lstEmployees.SelectedItem as Employee;
            _habaneroControlBinder.BusinessObject = SelectedEmployee;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedEmployee != null) SelectedEmployee.Save();
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }

    }
}

