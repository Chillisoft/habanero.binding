using System.IO;

namespace BindingListViewSample.BO
{
    public static class BOBroker
    {
        public static string GetClassDefsXml()
        {
            StreamReader classDefStream = new StreamReader(
                typeof(BOBroker).Assembly.GetManifestResourceStream("BindingListViewSample.BO.ClassDefs.xml"));
            return classDefStream.ReadToEnd();
        }
    }
}