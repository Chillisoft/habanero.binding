using System;

namespace Habanero.Binding
{
    public class BindingListRequest<T>
    {
        public BindingListRequest()
        {
            PageNumber = 0;
            RowsPerPage = 20;
            Filter = String.Empty;
            Sort = String.Empty;
        }

        public string Filter { get; set;}
        public string Sort { get; set; }
        //TODO brett 09 Jun 2010: Add Tests Page number must be >= 0
        public int PageNumber { get; set; }
        //TODO brett 09 Jun 2010: Add tests RowsPerPage must be > 0
        public int RowsPerPage { get; set; }

        public int StartIndex
        {
            get { return this.RowsPerPage*this.PageNumber; }
        }
    }
}
