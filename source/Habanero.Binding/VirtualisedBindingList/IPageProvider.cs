using System.Collections.Generic;

namespace Habanero.Binding
{
    public interface IPageProvider<T>
    {
        IList<T> GetDataPage(int pageNumber);
        string Filter { get; set;}
        string Sort { get; set;}
        int RowsPerPage { get; }
        int RowCount { get;}
    }
}
