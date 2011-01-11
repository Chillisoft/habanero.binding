using BindingListViewSample.Test.BO;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace BindingListViewSample.Test.UI
{
    public class TestBaseUI
    {
        public static void SetupTestFixture()
        {
            TestBase.SetupTestFixture();
        }

        public static void RefreshClassDefs()
        {
            TestBase.RefreshClassDefs();
        }

        public static void SetupTest()
        {
            TestBase.SetupTest();
            GlobalUIRegistry.ControlFactory = new ControlFactoryWin();
        }
    }
}
