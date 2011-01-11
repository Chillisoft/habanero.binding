
using NUnit.Framework;

namespace Habanero.Binding.Tests
{
    [TestFixture]
    public class TestBindingListRequest
    {
        [Test]
        public void Test_Construct_BindingRequest_ShouldDefaultRowsPerPageAndPageNo()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            //---------------Test Result -----------------------
            Assert.AreEqual(20, request.RowsPerPage);
            Assert.AreEqual(0, request.PageNumber);
        }

        [Test]
        public void Test_Construct_BindingRequest_ShouldDefaultFilterAndSortToEmpty()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsEmpty(request.Filter);
            Assert.IsEmpty(request.Sort);
        }

        [Test]
        public void Test_CalculateStartIndex_ShouldSetToPageNoXRowsPerPage()
        {
            //---------------Set up test pack-------------------
            BindingListRequest<FakeBO> request = new BindingListRequest<FakeBO> 
                    {RowsPerPage = 12, PageNumber = 3};
            const int expectedStartIndex = 36;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            int startIndex = request.StartIndex;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedStartIndex, startIndex);
        }
    }
}
