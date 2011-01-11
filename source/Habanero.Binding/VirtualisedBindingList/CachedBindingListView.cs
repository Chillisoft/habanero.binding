using System;
using System.ComponentModel;
using System.Collections;
using Habanero.Base;

// ReSharper disable CheckNamespace
namespace Habanero.Binding

{
    public class CachedBindingListView<T> : IBindingListView, ITypedList where T : class, IBusinessObject, new()
    {

        #region fields

        /// <summary>
        /// Called if list was changed.
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// Inner data cache
        /// </summary>
        private Cache<T> InnerCache { get; set; }

        /// <summary>
        /// Page provider
        /// </summary>
        private IPageProvider<T> PageProvider { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// New object constructs a default BindingListRequest and Page provider
        /// </summary>
        public CachedBindingListView():this(new PageProvider<T>(new BindingListRequest<T>()))
        {
        }
        /// <summary>
        /// New object
        /// </summary>
        /// <param name="bindingListRequest">Request for a particular set of Business Objects</param>
        public CachedBindingListView(BindingListRequest<T> bindingListRequest):this(new PageProvider<T>(bindingListRequest))
        {
        }
        /// <summary>
        /// This constructor is provided for the purposes of testing so that test doubles can be injected into this.
        /// </summary>
        /// <param name="pageProvider"></param>
        protected CachedBindingListView(IPageProvider<T> pageProvider)
        {
            SetupSortDescriptionsCol();
            PageProvider = pageProvider;
            InnerCache = new Cache<T>(PageProvider);
        }
        #endregion

        #region properties

        /// <summary>
        /// whether you can update items in the list..
        /// </summary>
        public bool AllowEdit
        {
            get { return false; }
        }

        /// <summary>
        /// whether you can add items to the list using System.ComponentModel.IBindingList.AddNew().
        /// </summary>
        public bool AllowNew
        {
            get { return false; }
        }

        /// <summary>
        /// whether you can remove items from the list, using System.Collections.IList.Remove(System.Object)
        /// </summary>
        public bool AllowRemove
        {
            get { return false; }
        }

        /// <summary>
        /// the number of elements contained in the System.Collections.ICollection.
        /// In this case it is the total number of records contained in the DataSource
        /// matching the FilterCriteria and not the number of records currently in the
        /// collection.
        /// </summary>
        public int Count
        {
            get 
            {
                return PageProvider.RowCount;
            }
        }

        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the collection of
        //     items returned by the data source
        /// </summary>
        public string Filter
        {
            get 
            {
                return PageProvider.Filter; 
            }
            set
            {
                Filtering(value);
                if (PageProvider.RowCount > 0)
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
                else
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the System.Collections.IList has a fixed
        //     size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        ///  Gets a value indicating whether object is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether list is sorted
        /// </summary>
        public bool IsSorted
        {
            get { return SortDescriptions.Count > 0; }
        }


        /// <summary>
        /// Gets a value indicating whether list is synchronized. 
        /// </summary>
        public bool IsSynchronized
        {
            get { return InnerCache.IsSynchronized; }
        }

        /// <summary>
        /// Sort descriptors.
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions { get; private set; }


        /// <summary>
        /// Sort direction.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return SortDescriptions.Count == 1 ? SortDescriptions[0].SortDirection : ListSortDirection.Ascending; }
        }

        /// <summary>
        /// Current sort property.
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get { return SortDescriptions.Count == 1 ? SortDescriptions[0].PropertyDescriptor : null; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports advanced sorting.
        /// </summary>
        public bool SupportsAdvancedSorting
        {
            get { return true; }
        }


        /// <summary>
        /// Gets whether a System.ComponentModel.IBindingList.ListChanged event is raised
        //     when the list changes or an item in the list changes.
        /// </summary>
        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports filtering.
        /// </summary>
        public bool SupportsFiltering
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports searching.
        /// </summary>
        public bool SupportsSearching
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports sorting.
        /// </summary>
        public bool SupportsSorting
        {
            get { return true; }
        }

        /// <summary>
        /// Synchronization root. 
        /// </summary>
        public object SyncRoot
        {
            get { return InnerCache.SyncRoot; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Element.</returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                //this[index] = (T)value;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return InnerCache.RetrieveElement(index);
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        private void Filtering(string filter)
        {
            PageProvider.Filter = filter;
            InnerCache.LoadFirstTwoPages();
        }

        /// <summary>
        ///  Called when the list changes or an item in the list changes.
        /// </summary>
        protected virtual void OnListChanged(ListChangedEventArgs args)
        {
            if (ListChanged != null)
                ListChanged(this, args);
        }

        #endregion

        #region public methods
        /// <summary>
        /// Adds an item to the <see cref="IList"/>.  
        /// </summary>
        /// <param name="value">The instance to add to the <see cref="IList"/>.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(object value)
        {
            if (value != null && !typeof(T).IsAssignableFrom(value.GetType()))
                throw new ArgumentException("Given instance doesn't match needed type.");
            //mPageProvider.
            
            //int result = _innerList.Add(value);
            int result = 0;
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, result));
            return result;  

        }

        /// <summary>
        /// Sorts the data source based on the given <see cref="T:System.ComponentModel.ListSortDescriptionCollection"/>.
        /// </summary>
        /// <param name="sorts">The <see cref="T:System.ComponentModel.ListSortDescriptionCollection"/> containing the sorts to apply to the data source.
        /// </param>
        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            SortDescriptions = sorts;
            string sort = String.Empty;
            foreach (ListSortDescription descr in sorts)
            {
                sort += descr.PropertyDescriptor.Name + 
                    " " + (descr.SortDirection == ListSortDirection.Ascending ? "Asc" : "Desc") + ", ";
            }
            sort = sort.Trim().Trim(new[] {','});
            PageProvider.Sort = sort;
            InnerCache.Reset();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// Sorts the list based on a <see cref="T:System.ComponentModel.PropertyDescriptor"/> and a <see cref="T:System.ComponentModel.ListSortDirection"/>.
        /// </summary>
        /// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor"/> to sort by. 
        /// </param><param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection"/> values. 
        /// </param><exception cref="T:System.NotSupportedException"><see cref="P:System.ComponentModel.IBindingList.SupportsSorting"/> is false. 
        /// </exception>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            ListSortDescription[] descriptions = new[] { new ListSortDescription(property, direction) };
            SortDescriptions = new ListSortDescriptionCollection(descriptions);
            PageProvider.Sort =  property.Name + " " + (direction == ListSortDirection.Ascending ? "Asc" : "Desc");
            InnerCache.Reset();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// Removes all items from the <see cref="IList"/>.  
        /// </summary>
        public void Clear()
        {
            //_innerList.Clear();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
            throw new NotImplementedException();
        }
        /// <summary>
        /// Determines whether the <see cref="IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The instance to locate in the <see cref="IList"/>.</param>
        /// <returns>true if the instance is found in the <see cref="IList"/>; otherwise, false.</returns>
        public bool Contains(object value)
        {
            //return _innerList.Contains(value);
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
                yield return this[i];
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (this.ViewBuilder != null) return this.ViewBuilder.GetGridView();
            return TypeDescriptor.GetProperties(typeof(T));
            //return ListBindingHelper.GetListItemProperties(typeof(T));
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return this.GetType().Name;
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList"/>. 
        /// </summary>
        /// <param name="value">The instance to locate in the <see cref="IList"/>.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>

        public int IndexOf(object value)
        {
            //return _innerList.IndexOf(value);
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IList"/>. 
        /// </summary>
        /// <param name="value">The instance to remove from the <see cref="IList"/>.</param>
        public void Remove(object value)
        {
            int index = IndexOf(value);
           // _innerList.Remove(value);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the <see cref="IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            //_innerList.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the current filter applied to the data source.
        /// </summary>
        public void RemoveFilter()
        {
            this.Filter = string.Empty;
        }

        /// <summary>
        /// Remove all current sorts.
        /// </summary>
        public void RemoveSort()
        {
            SetupSortDescriptionsCol();
        }

        private void SetupSortDescriptionsCol()
        {
            SortDescriptions = new ListSortDescriptionCollection();
        }

        #endregion

        #region Not implemented

        /// <summary>
        /// Adds the System.ComponentModel.PropertyDescriptor to the indexes used for
        //     searching.
        /// </summary>
        /// <param name="property">
        /// The System.ComponentModel.PropertyDescriptor to add to the indexes used for
        //     searching.
        /// </param>
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///  Adds a new item to the list.
        /// </summary>
        /// <returns>New item</returns>
        public object AddNew()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the index of the row that has the given System.ComponentModel.PropertyDescriptor.
        /// </summary>
        /// <param name="property">PropertyDescriptor to search on.</param>
        /// <param name="key">Key.</param>
        /// <returns>.</returns>
        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the System.ComponentModel.PropertyDescriptor from the indexes used
        ///     for searching.
        /// </summary>
        /// <param name="property">
        /// PropertyDescriptor to remove from the indexes used
        //     for searching.
        /// </param>
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public IViewBuilder ViewBuilder { get; set; }

        #endregion

    }

}
// ReSharper restore CheckNamespace
