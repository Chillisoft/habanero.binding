using System;
using System.Collections;
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
    public partial class EmployeeFilterForm : Form
    {
        private BindingListView<Employee> EmployeeBindList { get; set; }

        public EmployeeFilterForm()
        {
            InitializeComponent();
            EmployeeBindList = new BindingListView<Employee>(SampleDataGenerator.CreateNewRandomEmployees(5));
            var employees = Broker.GetBusinessObjectCollection<Employee>("", "");
            EmployeeBindList.BusinessObjectCollection = employees;
            LoadOperatorComboBoxSelector();
            LoadpropertyComboBoxSelector();

            employeeBindingSource.DataSource = EmployeeBindList;
            //You can also set the Bindings programmatically
            //employeeDataGridView.DataSource = EmployeeBindList;

        }

        private void ApplyFilterButtonClick(object sender, EventArgs e)
        {
            try
            {
                ApplyFilter();
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        /// <summary>
        /// This filters the <see cref="BindingListView{T}"/>
        ///  using the <see cref="BindingListView{T}.Filter"/> method.
        /// </summary>
        public void ApplyFilter()
        {
            EmployeeBindList.Filter = GetFilterString();
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            try
            {
                ClearFilter();
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }

        /// <summary>
        /// This removes the filter on the <see cref="BindingListView{T}"/> 
        /// using the <see cref="BindingListView{T}.RemoveFilter"/> method.
        /// </summary>
        public void ClearFilter()
        {
            EmployeeBindList.RemoveFilter();
        }

        #region Supporting methods for the demo project

        private void LoadpropertyComboBoxSelector()
        {
            PropertyDescriptorCollection descriptorCollection = TypeDescriptor.GetProperties(EmployeeBindList[0]);
            IList list = new List<string>();
            foreach (PropertyDescriptor prop in descriptorCollection)
            {
                if (IsComparable(prop) && (IsNotSpecialBOProp(prop)))
                    list.Add(prop.Name);
            };
            propertyComboBox.DataSource = list;
        }

        private static bool IsNotSpecialBOProp(PropertyDescriptor prop)
        {
            return prop.Name != "DirtyXML" && prop.Name != "Error";
        }

        private bool IsComparable(PropertyDescriptor prop)
        {
            return prop.PropertyType.GetInterface("IComparable", true) != null;
        }

        private void LoadOperatorComboBoxSelector()
        {
            var operators = new string[] { "<", "=", ">" };
            operatorComboBox.DataSource = operators;
        }
        
        private string GetFilterString()
        {
            var query = new StringBuilder();
            if (propertyComboBox.SelectedItem != null)
                query.Append((string)propertyComboBox.SelectedItem);
            if (operatorComboBox.SelectedItem != null)
                query.Append((string)operatorComboBox.SelectedItem);
            query.Append("'" + filterValueTextBox.Text + "'");
            return query.ToString();
        }

        #endregion


    }
}
