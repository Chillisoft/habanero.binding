using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BindingListViewSample.BO;
using Habanero.Binding;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace BindingListViewSample.UI
{
    public class EmployeeGridControl : FormWin
    {
        public EmployeeGridControl()
        {
            IControlFactory factory = GlobalUIRegistry.ControlFactory;
            var gridControl = factory.CreateReadOnlyGridControl();
            SetupGrid(gridControl);

            var layoutManager = factory.CreateBorderLayoutManager(this);
            layoutManager.AddControl(gridControl, BorderLayoutManager.Position.Centre);
        }

        private static void SetupGrid(IReadOnlyGridControl gridControl)
        {
            gridControl.Initialise(ClassDef.Get<Employee>(), "default");
            var bindingListView = new BindingListView<Employee>();

            //Testing Add Method.
            bindingListView.Add(new Employee{LastName = "Haas",FirstName = "Jonathan", Salary = 54788, BirthDate = new DateTime(2001, 12, 12)});
            bindingListView.Add(new Employee{LastName = "Tom",FirstName = "Lesg", Salary = 324543, BirthDate = new DateTime(2008, 12, 12)});
            bindingListView.Add(new Employee{LastName = "fdsf",FirstName = "sad", Salary = 547456, BirthDate = new DateTime(2010, 12, 12)});
            bindingListView.Add(new Employee{LastName = "gggg",FirstName = "ee", Salary = 3456, BirthDate = new DateTime(2005, 12, 12)});

            var employee = new Employee { LastName = "Test", FirstName = "Test", Salary = 55555, BirthDate = new DateTime(1999, 12, 12) };
            //bindingListView.Insert(2, employee);
            bindingListView.Add(employee);
            bindingListView.RemoveAt(3);

            //Testing ApplySort
            PropertyDescriptorCollection descriptorCollection = TypeDescriptor.GetProperties(bindingListView[0]);
            PropertyDescriptor descriptionCollection = descriptorCollection[4];
            bindingListView.ApplySort(descriptionCollection, ListSortDirection.Ascending);

            bindingListView.ListChanged += ListChangedHandler;
//            var find = bindingListView.Find(descriptorCollection[1], "Lesg");
//            Console.WriteLine("Found " + find);

            //bindingListView.Filter = "LastName = 'Haas'";
            //Console.WriteLine("Is included " + bindingListView.Contains(employee));
            //var bindingSource2 = new BindingSource();
            
            /*var employees = new Employee[];
            bindingListView.CopyTo(employees, 0);
            Console.WriteLine("Employee Name: {0}.", employees[0].FirstName);*/

            foreach (PropertyDescriptor prop in descriptorCollection)
            {
                Console.WriteLine(prop.Name);  
            }

            gridControl.BusinessObjectCollection = bindingListView.BusinessObjectCollection;
        }

        private static void ListChangedHandler(Object sender, ListChangedEventArgs e)
        {
            var employees = (BindingListView<Employee>) sender;
            Console.WriteLine("Collection item count: {0}", employees.Count);
            // List change type.
            Console.WriteLine("Change Type: {0}", e.ListChangedType);

            // Affected index.
            Console.WriteLine("Affected Index: {0}", e.NewIndex);

            // Old index.
            Console.WriteLine("Old Index: {0}", e.OldIndex);

            // Item changed.
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                if (e.PropertyDescriptor != null)
                {
                    Console.WriteLine("Changed Property: {0}",
                                     e.PropertyDescriptor.Name);
                    Console.WriteLine("New Property Value: {0}",
                                     e.PropertyDescriptor.GetValue(employees[e.NewIndex]));
                }
            }
        }

    }
}
