using System;
using System.Windows.Forms;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.ProgrammaticBinding;
using Habanero.Testability;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming

namespace Habanero.ProgrammaticBinding.Tester.Tests
{
    /// <summary>
    /// For success these tests tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
    /// </summary>
    [TestFixture]
    public class TestBindingTester
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.       
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();

            GlobalUIRegistry.ControlFactory = new ControlFactoryWin();
        }

        [SetUp]
        public void Setup()
        {
            //Code that is executed before every test is run in this class. If multiple tests
            // are executed then will be executed every time.     
            BORegistry.DataAccessor = new DataAccessorInMemory();
        }

        [Test]
        public void Test_ConstructBindingTester_WithNullControlBindier_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            HabaneroControlBinder<FakeBoWithOnProp> controlBinder = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(controlBinder);
            //---------------Execute Test ----------------------
            try
            {
                new BindingTester<FakeBoWithOnProp>(controlBinder);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("habaneroControlBinder", ex.ParamName);
            }
        }

        [Test]
        public void Test_ConstructBindingTester_ShouldConstruct()
        {
            //---------------Set up test pack-------------------
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bindingTester);
        }

        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_AssertPropBound_WhenIs_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var textBox = new TextBox();
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";
            controlBinder.Add<TextBoxMapper, TextBox>(textBox, propName);
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingTester.AssertPropBoundTo(textBox, propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed.");
        }

        [Test]
        public void Test_AssertPropBound_WhenIsNotBoundToAnything_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                bindingTester.AssertPropBoundTo(GenerateStub<CheckBox>(), propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expectedMsg = string.Format("The Property '{0}' for class '{1}' should be mapped", propName, "FakeBoWithOnProp");
                StringAssert.Contains(expectedMsg, ex.Message);
            }
        }

        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_AssertPropBoundToTextBox_WhenIs_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var textBox = new TextBox();
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";
            controlBinder.Add<TextBoxMapper, TextBox>(textBox, propName);
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingTester.AssertPropBoundTo(textBox, propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed.");
        }

        [Test]
        public void Test_AssertPropBoundToTextBox_WhenIsNot_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var textBox = GenerateStub<TextBox>();
            var textBoxName = GetRandomString();
            textBox.Name = textBoxName;
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";

            var otherTextBox = GenerateStub<TextBox>();
            var textBoxMapper = controlBinder.Add<TextBoxMapper, TextBox>(otherTextBox, propName);
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------
            controlBinder.ControlMappers.Contains(textBoxMapper);
            Assert.AreNotSame(textBox, textBoxMapper.Control);
            //---------------Execute Test ----------------------
            try
            {
                bindingTester.AssertPropBoundTo(textBox, propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expectedMsg = string.Format("The Property '{0}' for class '{1}' should be mapped to '{2}'", propName, "FakeBoWithOnProp", textBoxName);
                StringAssert.Contains(expectedMsg, ex.Message);
            }
        }

        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_AssertPropBoundToTextBox_WithLambda_WhenIs_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var textBox = new TextBox();
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";
            controlBinder.Add<TextBoxMapper, TextBox>(textBox, propName);
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            bindingTester.AssertPropBoundTo(textBox, bo => bo.FakeBOName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed.");
        }

        [Test]
        public void Test_AssertPropBoundToTextBox_WithLambda_WhenIsNot_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var textBox = GenerateStub<TextBox>();
            var textBoxName = GetRandomString();
            textBox.Name = textBoxName;
            var controlBinder = new HabaneroControlBinder<FakeBoWithOnProp>();
            const string propName = "FakeBOName";

            var otherTextBox = GenerateStub<TextBox>();
            var textBoxMapper = controlBinder.Add<TextBoxMapper, TextBox>(otherTextBox, propName);
            var bindingTester = new BindingTester<FakeBoWithOnProp>(controlBinder);
            //---------------Assert Precondition----------------
            controlBinder.ControlMappers.Contains(textBoxMapper);
            Assert.AreNotSame(textBox, textBoxMapper.Control);
            //---------------Execute Test ----------------------
            try
            {
                bindingTester.AssertPropBoundTo(textBox, bo => bo.FakeBOName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expectedMsg = string.Format("The Property '{0}' for class '{1}' should be mapped to '{2}'", propName, "FakeBoWithOnProp", textBoxName);
                StringAssert.Contains(expectedMsg, ex.Message);
            }
        }
        private static T GenerateStub<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        [Ignore("Not sure how to implement this")] //TODO Brett 12 Dec 2010: need to check Faces have to do this for Reflective, Related and standard prop
        [Test]
        public void AssertFakeBONameMappedToTextBox_WhenPropNameNotAPropOfBO_ShouldReturnFalseTest()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var boPropertyMapper = new BOPropertyMapper("fdafads");
//            boPropertyMapper.
            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }

    }

    public class FakeBoWithOnProp : BusinessObject<FakeBoWithOnProp>
    {
        /// <summary>
        /// The Name of the FakeBO
        /// </summary>
        public virtual string FakeBOName
        {
            get { return ((string) (base.GetPropertyValue("FakeBOName"))); }
            set { base.SetPropertyValue("FakeBOName", value); }
        }
    }
}