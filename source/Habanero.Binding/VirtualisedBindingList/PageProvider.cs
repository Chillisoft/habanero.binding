using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Binding
{
    public class PageProvider<T> : IPageProvider<T> where T : class, IBusinessObject, new()
    {

        #region Fields

        /// <summary>
        /// Web Service request
        /// </summary>
        private readonly BindingListRequest<T> _bindingListRequest;


        #endregion

        #region Constructor

        public PageProvider(): this(new BindingListRequest<T>())
        {
        }
        public PageProvider(BindingListRequest<T> bindingListRequest)
        {
            if (bindingListRequest == null) throw new ArgumentNullException("bindingListRequest");
            _bindingListRequest = bindingListRequest;
            SetRowsCount();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Filter
        /// </summary>
        public string Filter
        {
            set
            {
                _bindingListRequest.Filter = value;
                SetRowsCount();
            }
            get
            {
                return _bindingListRequest.Filter;
            }
        }

        public int RowsPerPage
        {
            get { return _bindingListRequest.RowsPerPage; }
        }

        /// <summary>
        /// The Total number of rows in the datasource that match the filter criteria
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets or sets current sort expression
        /// </summary>
        public string Sort
        {
            set
            {
                _bindingListRequest.Sort = value;
            }
            get
            {
                return _bindingListRequest.Sort;
            }
        }

        #endregion

        #region IPageProvider implementation

        /// <summary>
        /// Gets a objects list for the specified page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        protected virtual IList<T> GetDataPage(int pageNumber, int rowsPerPage)
        {
            _bindingListRequest.PageNumber = pageNumber;
            _bindingListRequest.RowsPerPage = rowsPerPage;

            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            int recordCount;
            col.LoadWithLimit(Filter, Sort, _bindingListRequest.StartIndex, _bindingListRequest.RowsPerPage, out recordCount);
            this.RowCount = recordCount;
            return col;
        }

        #endregion

        #region private methods

        private void SetRowsCount()
        {

            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            int recordCount;
            col.LoadWithLimit("", Sort, 0, 0, out recordCount);
            this.RowCount = recordCount;
        }

        #endregion
        /// <summary>
        /// Returns a list of BusinessObjects for the page identified by pageNumber
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public IList<T> GetDataPage(int pageNumber)
        {
            return GetDataPage(pageNumber, _bindingListRequest.RowsPerPage);
        }
    }
}
