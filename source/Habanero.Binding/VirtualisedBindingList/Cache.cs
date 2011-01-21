using System;
using System.Collections.Generic;

namespace Habanero.Binding
{
    /// <summary>
    /// Cache class
    /// </summary>
    /// <typeparam name="T">Object class cahce contains</typeparam>
    public class Cache<T>
    {

        #region Inner class

        /// <summary>
        /// Page of data
        /// </summary>
        private struct DataPage
        {
            /// <summary>
            /// List of data
            /// </summary>
            private readonly IList<T> _objects;
            private readonly int _lowestIndexValue;
            private readonly int _highestIndexValue;

            public DataPage(IList<T> list, int rowIndex, int rowsPerPage)
            {
                _lowestIndexValue = MapToLowestBoundary(rowIndex, rowsPerPage);
                _highestIndexValue = MapToHighestBoundary(rowIndex, rowsPerPage);
                _objects = list;
            }

            /// <summary>
            /// Lowest index
            /// </summary>
            public int LowestIndex
            {
                get
                {
                    return _lowestIndexValue;
                }
            }

            /// <summary>
            /// Highest index
            /// </summary>
            public int HighestIndex
            {
                get
                {
                    return _highestIndexValue;
                }
            }

            /// <summary>
            /// List of data
            /// </summary>
            public IList<T> Objects
            {
                get { return _objects; }
            }

            /// <summary>
            /// Map to page lowest boundery
            /// </summary>
            /// <param name="rowIndex">Row Index</param>
            /// <param name="rowsPerPage"></param>
            /// <returns>Page Lowest Boundary</returns>
            private static int MapToLowestBoundary(int rowIndex, int rowsPerPage)
            {
                return (rowIndex / rowsPerPage) * rowsPerPage;
            }

            /// <summary>
            /// Mao to page number
            /// </summary>
            /// <param name="rowIndex">Row Index</param>
            /// <param name="rowsPerPage"></param>
            /// <returns>Page Number</returns>
            public static int MapToPageNumber(int rowIndex, int rowsPerPage)
            {
                return (rowIndex / rowsPerPage);
            }

            /// <summary>
            /// Map to page highest boundary
            /// </summary>
            /// <param name="rowIndex">Row Index</param>
            /// <param name="rowsPerPage"></param>
            /// <returns>Page highest boundary</returns>
            private static int MapToHighestBoundary(int rowIndex, int rowsPerPage)
            {
                return MapToLowestBoundary(rowIndex, rowsPerPage) + rowsPerPage - 1;
            }
        }

        #endregion

        #region fields

        /// <summary>
        /// Rows per page
        /// </summary>
        private int RowsPerPage { get {return  _pageProvider.RowsPerPage; } }

        /// <summary>
        /// Array of pages
        /// </summary>
        private DataPage[] _cachePages;

        /// <summary>
        /// Провайдер страниц
        /// </summary>
        private readonly IPageProvider<T> _pageProvider;

        #endregion

        #region properties

        /// <summary>
        /// For a IBindingListView implementation
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return _cachePages.IsSynchronized;
            }
        }

        /// <summary>
        /// For a IBindingListView implementation
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return _cachePages.SyncRoot;
            }
        }

        #endregion

        #region Constructor

        ///<summary>
        ///</summary>
        ///<param name="pageProvider"></param>
        public Cache(IPageProvider<T> pageProvider)
        {
            _pageProvider = pageProvider;
            LoadFirstTwoPages();
        }

        #endregion

        #region  public methods

        /// <summary>
        /// Object by index
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns>Object</returns>
        public T RetrieveElement(int rowIndex)
        {

            T element = Activator.CreateInstance<T>();
            //If element is in cache
            if (IfPageCachedThenSetElement(rowIndex, ref element))
            {
                return element;
            }
            return RetrieveDataCacheItThenReturnElement(
                rowIndex);
        }

        /// <summary>
        /// Loads first two pages
        /// </summary>
        public void LoadFirstTwoPages()
        {
            _cachePages = new[] {
                new DataPage(_pageProvider.GetDataPage(0), 0, RowsPerPage),
                new DataPage(_pageProvider.GetDataPage(1), RowsPerPage, RowsPerPage)
            };
        }

        ///<summary>
        ///</summary>
        public void Reset()
        {
            LoadFirstTwoPages();
        }


        #endregion

        #region private methods
        /// <summary>
        /// Checks if index in page
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns></returns>
        private bool IsRowCachedInPage(int pageNumber, int rowIndex)
        {
            return rowIndex <= _cachePages[pageNumber].HighestIndex &&
                rowIndex >= _cachePages[pageNumber].LowestIndex;
        }

        /// <summary>
        /// Retrieves page corressponding to an index from a server and caches it
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Element</returns>
        private T RetrieveDataCacheItThenReturnElement(int rowIndex)
        {
            IList<T> list = _pageProvider.GetDataPage(DataPage.MapToPageNumber(rowIndex, RowsPerPage));
            _cachePages[GetIndexToUnusedPage(rowIndex)] = new DataPage(list, rowIndex, RowsPerPage);
            return RetrieveElement(rowIndex);
        }

        /// <summary>
        /// Retrieves element from the cache
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="element">Element</param>
        /// <returns>If cache contains an element with this index</returns>
        private bool IfPageCachedThenSetElement(int rowIndex, ref T element)
        {
            if (IsRowCachedInPage(0, rowIndex))
            {
                element = _cachePages[0].Objects[rowIndex % RowsPerPage];
                return true;
            }
            if (IsRowCachedInPage(1, rowIndex))
            {
                element = _cachePages[1].Objects[rowIndex % RowsPerPage];
                return true;
            }
            return false;
        }

        /// <summary>
        /// The farthest page from index
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <returns>Page number</returns>
        private int GetIndexToUnusedPage(int rowIndex)
        {
            if (rowIndex > _cachePages[0].HighestIndex &&
                rowIndex > _cachePages[1].HighestIndex)
            {
                int liOffsetFromPage0 = rowIndex - _cachePages[0].HighestIndex;
                int loOffsetFromPage1 = rowIndex - _cachePages[1].HighestIndex;
                if (liOffsetFromPage0 < loOffsetFromPage1)
                {
                    return 1;
                }
                return 0;
            }
            else
            {
                int liOffsetFromPage0 = _cachePages[0].LowestIndex - rowIndex;
                int liOffsetFromPage1 = _cachePages[1].LowestIndex - rowIndex;
                if (liOffsetFromPage0 < liOffsetFromPage1)
                {
                    return 1;
                }
                return 0;
            }
        }

        #endregion

    }
}
