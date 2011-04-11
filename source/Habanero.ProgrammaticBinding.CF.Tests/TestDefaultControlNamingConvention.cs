using System;
using System.Windows.Forms;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgrammaticBinding.ControlAdaptors;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming
namespace Habanero.ProgrammaticBinding.Tests
{
    [TestFixture]
    public class TestDefaultControlNamingConvention
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.     

            ClassDef.ClassDefs.Add(typeof (FakeBo).MapClasses());
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalUIRegistry.ControlFactory = new ControlFactoryWin();
        }

        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [Test]
        public void Test_IsValidControl_WhenControlNull_ReturnFailingResult()
        {
            //---------------Set up test pack-------------------
            IControlNamingConvention namingConvention = GetNamingConvention();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            //---------------Execute Test ----------------------
            var isValidControl = namingConvention.IsValidControl(null);
            //---------------Test Result -----------------------
            Assert.IsFalse(isValidControl.Successful, "Should be invalid");
            StringAssert.Contains("control cannot be null", isValidControl.Message);
        }

        [Test]
        public void Test_IsValidControl_WhenControlNameEmpty_ReturnFailingResult()
        {
            //---------------Set up test pack-------------------
            var namingConvention = GetNamingConvention();
            var control = new Control { Name = "" };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            Assert.IsEmpty(control.Name);
            //---------------Execute Test ----------------------
            var isValidControl = namingConvention.IsValidControl(control);
            //---------------Test Result -----------------------
            Assert.IsFalse(isValidControl.Successful, "Should be invalid");
            StringAssert.Contains("control.Name cannot be empty", isValidControl.Message);
        }

        [Test]
        public void Test_IsValidControl_WhenControlNameEQ3Letters_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var namingConvention = GetNamingConvention();
            var control = new Control { Name = "ddf" };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            Assert.IsNotEmpty(control.Name);
            Assert.LessOrEqual(control.Name.Length, 3);
            //---------------Execute Test ----------------------
            var isValidControl = namingConvention.IsValidControl(control);
            //---------------Test Result -----------------------
            Assert.IsFalse(isValidControl.Successful, "Should be invalid");
            StringAssert.Contains("control.Name must be greater than 3 characters", isValidControl.Message);
        }

        [Test]
        public void Test_GetPropName_WhenControlNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var namingConvention = GetNamingConvention();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            //---------------Execute Test ----------------------
            try
            {
                namingConvention.GetPropName(null);
                Assert.Fail("expected HabaneroArgumentException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains("control cannot be null", ex.Message);
                StringAssert.Contains("control", ex.ParameterName);
            }
        }

        [Test]
        public void Test_GetPropName_WhenControlNameEmpty_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var namingConvention = GetNamingConvention();
            var control = new Control { Name = "" };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            Assert.IsEmpty(control.Name);
            //---------------Execute Test ----------------------
            try
            {
                namingConvention.GetPropName(control);
                Assert.Fail("expected HabaneroArgumentException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains("control.Name cannot be empty", ex.Message);
                StringAssert.Contains("control", ex.ParameterName);
            }
        }

        [Test]
        public void Test_GetPropName_WhenControlNameEQ3Letters_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var namingConvention = GetNamingConvention();
            var control = new Control { Name = "ddf" };
            //---------------Assert Precondition----------------
            Assert.IsNotNull(namingConvention);
            Assert.IsNotEmpty(control.Name);
            Assert.LessOrEqual(control.Name.Length, 3);
            //---------------Execute Test ----------------------
            try
            {
                namingConvention.GetPropName(control);
                Assert.Fail("expected HabaneroArgumentException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains("The control.Name must be greater than 3 characters", ex.Message);
                StringAssert.Contains("control", ex.ParameterName);
            }
        }

        private static DefaultControlNamingConvention GetNamingConvention()
        {
            return new DefaultControlNamingConvention();
        }
    }
}
// ReSharper restore InconsistentNaming