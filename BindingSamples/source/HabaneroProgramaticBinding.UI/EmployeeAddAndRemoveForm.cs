using System;
using System.Windows.Forms;
using BindingListViewSample.BO;

namespace HabaneroProgramaticBinding.UI
{
    public partial class EmployeeAddAndRemoveForm : Form
    {
     //   private BindingListView<Employee> EmployeeBindingList { get; set; }

        public EmployeeAddAndRemoveForm()
        {
            InitializeComponent();
/*            EmployeeBindingList = new BindingListView<Employee>();

            employeeBindingSource.DataSource = EmployeeBindingList;*/
        }

        private void addButton_Click(object sender, EventArgs e)
        {
          //  EmployeeBindingList.Add(SampleDataGenerator.CreateNewRandomEmployee());
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var selectedEmployee = employeeBindingSource.Current as Employee;
            //if (selectedEmployee != null) EmployeeBindingList.Remove(selectedEmployee);
        }
    }
}
