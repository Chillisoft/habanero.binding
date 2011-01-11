using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BindingListViewSample.BO;
using BindingListViewSample.Test.BO;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;
using Habanero.Testability;

namespace BindingListViewSample.Logic
{
    public static class SampleDataGenerator
    {
        public static Employee CreateNewRandomEmployee()
        {
            BOTestFactoryRegistry.Instance.Register<Employee>(typeof(EmployeeNameFactory));

            DateTime randomDate = RandomValueGen.GetRandomDate("1980-03-03", "1990-05-05");
            Decimal randomSalary = RandomValueGen.GetRandomDecimal(1000, 15000);

            var employee =  TestUtilsEmployee.GetTestFactory()
                            .WithValue(employee1 => employee1.Salary, randomSalary)
                            .WithValue(employee2 => employee2.BirthDate, randomDate)
                            .CreateSavedBusinessObject();
            return employee;
        }

        public static BusinessObjectCollection<Employee> CreateNewRandomEmployees(int numberOfItems)
        {
            var boCol = new BusinessObjectCollection<Employee>();
            for (var i = 0; i < numberOfItems; i++)
            {
                boCol.Add(CreateNewRandomEmployee());
            }
            return boCol;
        }
    }

    internal class EmployeeNameFactory : BOTestFactory<Employee>
    {
        public EmployeeNameFactory()
        {
            this.SetValidValueGenerator(source => source.FirstName, typeof(EmployeeNameValidValueGenerator));
            this.SetValidValueGenerator(source => source.LastName, typeof(EmployeeLastNameValidValueGenerator));
        }

        public EmployeeNameFactory(Employee bo)
            : base(bo)
        {
            this.SetValidValueGenerator(source => source.FirstName, typeof(EmployeeNameValidValueGenerator));
            this.SetValidValueGenerator(source => source.LastName, typeof(EmployeeLastNameValidValueGenerator));
        }
    }

    internal class EmployeeNameValidValueGenerator : ValidValueGeneratorName
    {
        private static readonly List<string> _names = new List<string>
        {
            "Tom","Jack","Harry","Leslie","Mitch","Gary"
        };

        public EmployeeNameValidValueGenerator(IPropDef propDef)
            : base(propDef, _names)
        {
        }
    }

    internal class EmployeeLastNameValidValueGenerator : ValidValueGeneratorName
    {
        private static readonly List<string> _names = new List<string>
        {
            "Sawyer","Beanstalk","Gwala","Smith","Dawson","Player"
        };

        public EmployeeLastNameValidValueGenerator(IPropDef propDef)
            : base(propDef, _names)
        {
        }
    }
}
