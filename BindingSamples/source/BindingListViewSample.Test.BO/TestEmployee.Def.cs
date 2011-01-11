// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
//
// If tests are failing due to a unique condition in your application, use the
// ignore feature in the stub class SetupTestFixture() method.  Reimplement the
// test in the stub class.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BindingListViewSample.BO;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using Habanero.Testability;

namespace BindingListViewSample.Test.BO
{
    // Provides the part of the test class that tests Employee objects
    [TestFixture]
    public partial class TestEmployee
    {
        private readonly Dictionary<string, string> _ignoreList = new Dictionary<string, string>();

        /// <summary>
        /// Checks if the developer has put this test on the ignore list.
        /// If your application has a unique condition that is causing a
        /// generated test to fail, you would lose test repairs when this
        /// class is regenerated.
        /// Simply add the test name to the ignore list in the TestFixtureSetup
        /// of the once-off-generated part of this test class, and then
        /// reimplement the test in that class.
        /// </summary>
        private void CheckIfTestShouldBeIgnored()
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            if (_ignoreList.ContainsKey(methodName))
            {
                Assert.Ignore("The developer has chosen to ignore this test: " + methodName +
                    ", Reason: " + _ignoreList[methodName]);
            }
        }

        [Ignore("Changed Salary and Birthdate Property(Not Nullable)")] //TODO Wajeeda 12 Nov 2010: Ignored Test - Changed Salary and Birthdate Property(Not Nullable)
        [Test]  // Ensures that the defaults have not been tampered
        public void Test_CreateEmployeeWithDefaults()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Employee employee = new Employee();

            //---------------Test Result -----------------------
                        Assert.IsNotNull(employee.EmployeeID);
            Assert.IsInstanceOf(employee.Props["EmployeeID"].PropertyType, employee.EmployeeID);
                        Assert.IsNull(employee.FirstName);
                        Assert.IsNull(employee.LastName);
                        Assert.IsNull(employee.Salary);
                        Assert.IsNull(employee.BirthDate);
        }

        [Test]  // Ensures that a class can be successfully saved
        public void Test_SaveEmployee()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = TestUtilsEmployee.CreateUnsavedValidEmployee();

            //---------------Assert Precondition----------------
            Assert.IsTrue(employee.Status.IsNew);
            BusinessObjectCollection<Employee> col = new BusinessObjectCollection<Employee>();
            col.LoadAll();
            Assert.AreEqual(0, col.Count);

            //---------------Execute Test ----------------------
            employee.Save();

            //---------------Test Result -----------------------
            Assert.IsFalse(employee.Status.IsNew);
            col.LoadAll();
            Assert.AreEqual(1, col.Count);
	    
        }
        
        [Test]  // Ensures that a saved class can be loaded
        public void Test_LoadEmployee()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = TestUtilsEmployee.CreateSavedEmployee();

            //---------------Execute Test ----------------------
            Employee loadedEmployee = Broker.GetBusinessObject<Employee>(employee.ID);

            //---------------Test Result -----------------------
                        Assert.AreEqual(employee.EmployeeID, loadedEmployee.EmployeeID);
                        Assert.AreEqual(employee.FirstName, loadedEmployee.FirstName);
                        Assert.AreEqual(employee.LastName, loadedEmployee.LastName);
                        Assert.AreEqual(employee.Salary, loadedEmployee.Salary);
                        Assert.AreEqual(employee.BirthDate, loadedEmployee.BirthDate);
        }
        
        [Test]  // Ensures that a class can be deleted
        public void Test_DeleteEmployee()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = TestUtilsEmployee.CreateSavedEmployee();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            employee.MarkForDelete();
            employee.Save();
            //---------------Test Result -----------------------
            try
            {
                Employee retrievedEmployee = Broker.GetBusinessObject<Employee>(employee.ID);
                Assert.Fail("expected Err");
            }
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been deleted by another user", ex.Message);
                StringAssert.Contains("There are no records in the database for the Class: Employee", ex.Message);
            }
        }

        [Test]  // Ensures that updates to property values are stored and can be retrieved
        public void Test_UpdateEmployee()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = TestUtilsEmployee.CreateSavedEmployee();
            BOTestFactory<Employee> factory = BOTestFactoryRegistry.Instance.Resolve<Employee>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueForFirstName = factory.GetValidPropValue(employee1=>employee1.FirstName);
            employee.FirstName = valueForFirstName;
            var valueForLastName = factory.GetValidPropValue(employee1=>employee1.LastName);
            employee.LastName = valueForLastName;
            var valueForSalary = factory.GetValidPropValue(employee1=>employee1.Salary);
            employee.Salary = valueForSalary;
            var valueForBirthDate = factory.GetValidPropValue(employee1=>employee1.BirthDate);
            employee.BirthDate = valueForBirthDate;
            employee.Save();

            //---------------Test Result -----------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            TestUtilsShared.WaitForGC();
            Employee retrievedEmployee =
                    Broker.GetBusinessObject<Employee>(employee.ID);
            
            Assert.AreEqual(valueForFirstName, retrievedEmployee.FirstName);
            Assert.AreEqual(valueForLastName, retrievedEmployee.LastName);
            Assert.AreEqual(valueForSalary, retrievedEmployee.Salary);
            Assert.AreEqual(valueForBirthDate, retrievedEmployee.BirthDate);
        }
        
        [Test]  // Ensures that gets and sets in the code refer to the same property
        public void Test_PropertyGetters()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = new Employee();
            BOTestFactory<Employee> factory = BOTestFactoryRegistry.Instance.Resolve<Employee>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueForFirstName = factory.GetValidPropValue(employee1=>employee1.FirstName);
            employee.FirstName = valueForFirstName;
            var valueForLastName = factory.GetValidPropValue(employee1=>employee1.LastName);
            employee.LastName = valueForLastName;
            var valueForSalary = factory.GetValidPropValue(employee1=>employee1.Salary);
            employee.Salary = valueForSalary;
            var valueForBirthDate = factory.GetValidPropValue(employee1=>employee1.BirthDate);
            employee.BirthDate = valueForBirthDate;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForFirstName, employee.FirstName);
            Assert.AreEqual(valueForLastName, employee.LastName);
            Assert.AreEqual(valueForSalary, employee.Salary);
            Assert.AreEqual(valueForBirthDate, employee.BirthDate);
        }
        
        [Test]  // Ensures that property getters in the code point to the correct property
        public void Test_PropertyGettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = new Employee();
            BOTestFactory<Employee> factory = BOTestFactoryRegistry.Instance.Resolve<Employee>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueForFirstName = factory.GetValidPropValue(employee1=>employee1.FirstName);
            employee.FirstName = valueForFirstName;
            var valueForLastName = factory.GetValidPropValue(employee1=>employee1.LastName);
            employee.LastName = valueForLastName;
            var valueForSalary = factory.GetValidPropValue(employee1=>employee1.Salary);
            employee.Salary = valueForSalary;
            var valueForBirthDate = factory.GetValidPropValue(employee1=>employee1.BirthDate);
            employee.BirthDate = valueForBirthDate;
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForFirstName, employee.GetPropertyValue("FirstName"));
            Assert.AreEqual(valueForLastName, employee.GetPropertyValue("LastName"));
            Assert.AreEqual(valueForSalary, employee.GetPropertyValue("Salary"));
            Assert.AreEqual(valueForBirthDate, employee.GetPropertyValue("BirthDate"));
        }
        
        [Test]  // Ensures that property setters in the code point to the correct property
        public void Test_PropertySettersUseCorrectPropertyNames()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            Employee employee = new Employee();
            BOTestFactory<Employee> factory = BOTestFactoryRegistry.Instance.Resolve<Employee>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueForFirstName = factory.GetValidPropValue(employee1=>employee1.FirstName);            
            employee.SetPropertyValue("FirstName", valueForFirstName);
            var valueForLastName = factory.GetValidPropValue(employee1=>employee1.LastName);            
            employee.SetPropertyValue("LastName", valueForLastName);
            var valueForSalary = factory.GetValidPropValue(employee1=>employee1.Salary);            
            employee.SetPropertyValue("Salary", valueForSalary);
            var valueForBirthDate = factory.GetValidPropValue(employee1=>employee1.BirthDate);            
            employee.SetPropertyValue("BirthDate", valueForBirthDate);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(valueForFirstName, employee.FirstName);
            Assert.AreEqual(valueForLastName, employee.LastName);
            Assert.AreEqual(valueForSalary, employee.Salary);
            Assert.AreEqual(valueForBirthDate, employee.BirthDate);
        }


        [Ignore("Changed Salary and Birthdate Property(Not Nullable)")] //TODO Wajeeda 12 Nov 2010: Ignored Test - Changed Salary and Birthdate Property(Not Nullable)
        [Test]
        public void Test_NotSettingCompulsoryPropertiesThrowsException()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            Employee employee = TestUtilsEmployee.CreateUnsavedValidEmployee();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            employee.EmployeeID = null;
            employee.FirstName = null;
            employee.LastName = null;
//            employee.Salary = null;
//            employee.BirthDate = null;
            
            try
            {
                employee.Save();
                Assert.Fail("Should throw an exception when compulsory properties are null");
            }
            //---------------Test Result -----------------------
            catch (BusObjectInAnInvalidStateException ex)
            {
                StringAssert.Contains(".Employee ID' is a compulsory field and has no value", ex.Message);
                StringAssert.Contains(".First Name' is a compulsory field and has no value", ex.Message);
                StringAssert.Contains(".Last Name' is a compulsory field and has no value", ex.Message);
                StringAssert.Contains(".Salary' is a compulsory field and has no value", ex.Message);
                StringAssert.Contains(".Birth Date' is a compulsory field and has no value", ex.Message);
            }
        }
        
        [Test]  // Checks that the read-write rules have not been changed in the class defs
        public void Test_ReadWriteRules()
        {
            CheckIfTestShouldBeIgnored();
            //---------------Set up test pack-------------------
            ClassDef classDef = ClassDef.Get<Employee>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            
            //---------------Test Result -----------------------
			Assert.AreEqual("WriteNew",classDef.PropDefColIncludingInheritance["EmployeeID"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["FirstName"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["LastName"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["Salary"].ReadWriteRule.ToString());
			Assert.AreEqual("ReadWrite",classDef.PropDefColIncludingInheritance["BirthDate"].ReadWriteRule.ToString());
        }
        
        
        
        
        [Test]  // Checks that classes using primary keys that are not an ID cannot have duplicate primary key values
        public void Test_NonIDPrimaryKey_ChecksForUniqueness()
        {
            CheckIfTestShouldBeIgnored();
            // Test does not apply to this class since the primary key is an ID
        }
        
        
                
        
        
        
        
        
        
       
    }
}
