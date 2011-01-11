using System;
using BindingListViewSample.BO;
using Habanero.BO;
using NUnit.Framework;
using Habanero.Testability;

namespace BindingListViewSample.Test.BO
{
    /// <summary>
    /// Provides a place to write custom tests for Employee objects.
    /// This file is only written once and can be changed.  The Def file
    /// attached to this as a dependent is rewritten with each regeneration
    /// and contains the standard tests for Employee.
    /// Regenerate this test project whenever there have been changes to the
    /// business objects.
    /// If tests are failing due to a unique setup in your application,
    /// you can either override the Create methods in TestUtils, or you
    /// can add the test to the ignore list below and reimplement it here.
    /// </summary>
    public partial class TestEmployee
    {
        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            TestBase.SetupTestFixture();

            //------------------------------------------------------------
            // Use this list to ignore generated tests that are failing
            // due to a unique condition in your application.
            // Remember to reimplement the test here.
            //------------------------------------------------------------
            //_ignoreList.Add("TestMethodName", "Reason for ignoring it");
        }

        [SetUp]
        public void Setup()
        {
            TestBase.SetupTest();
        }
        
       [Test]
        public void Test_ToString()
        {
            //---------------Set up test pack-------------------
            BORegistry.DataAccessor = new DataAccessorInMemory();
            Employee employee = TestUtilsEmployee.CreateUnsavedValidEmployee();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            string toStringValue = employee.ToString();
            //---------------Test Result -----------------------
            Assert.AreEqual(String.Format("{0} {1}",employee.FirstName, employee.LastName), toStringValue);
        }
    }
}