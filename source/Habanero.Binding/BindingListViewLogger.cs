using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Base.Logging;
using Habanero.BO;

namespace Habanero.Binding
{
    /// <summary>
    /// This is a class that is used for the simple purposes of logging all event and method calls
    /// to the BindingListView. The reason for this is that the documentation is sparse (to say the least).
    /// And what each method needs to do is far from obvious.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingListViewLogger<T> : ITypedList, IBindingListView, ICancelAddNew, IRaiseItemChangedEvents
        where T : class, IBusinessObject, new()
    {
        private IViewBuilder _viewBuilder;

        private static readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger("Habanero.Binding.BindingListViewLogger");

        //private readonly IDictionary<int,T> _addedBOsCancelled = new Dictionary<int, T>();
        private readonly IDictionary<int,T> _addedBOs = new Dictionary<int, T>();

        /// <summary>
        /// 
        /// </summary>
        public event ListChangedEventHandler ListChanged;

        #region Constructors

        /// <summary>
        /// New object constructs a default BindingListView
        /// </summary>
        public BindingListViewLogger() : this(new BusinessObjectCollection<T>())
        {
        }

        /// <summary>
        /// This constructs a new BindingListView with a business object collection
        /// </summary>
        /// <param name="collection"></param>
        public BindingListViewLogger(BusinessObjectCollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            this.BusinessObjectCollection = collection;
            this.ViewOfBusinessObjectCollection = collection.Clone();
        }
        /// <summary>
        /// Returns the undelying <see cref="IBusinessObjectCollection"/>.
        /// </summary>
        public BusinessObjectCollection<T> BusinessObjectCollection { get; private set; }

        /// <summary>
        /// Returns the View (Copy) of the Underlying <see cref="IBusinessObjectCollection"/>
        /// </summary>
        protected BusinessObjectCollection<T> ViewOfBusinessObjectCollection { get; set; }

        #endregion

        #region ITypedList

        /// <summary>
        /// Returns the name of this list.
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            _logger.Log("GetListName"); 
            return this.GetType().Name;
        }

        /// <summary>
        /// Returns the collection of property descriptors for the <see cref="IBusinessObject"/> 
        ///   of type <see cref="T"/>
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            _logger.Log("GetItemProperties"); 
            return this.ViewBuilder != null
                       ? this.ViewBuilder.GetPropertyDescriptors()
                       : CreateDefaultViewBuilder().GetPropertyDescriptors();
        }

        /// <summary>
        /// 
        /// </summary>
        public IViewBuilder ViewBuilder
        {
            get
            {
                if (this._viewBuilder != null) return this._viewBuilder;
                return this._viewBuilder = CreateDefaultViewBuilder();
            }
            set { _viewBuilder = value; }
        }

        protected virtual UIDefViewBuilder<T> CreateDefaultViewBuilder()
        {
            return new UIDefViewBuilder<T>();
        }

        #endregion


        #region IBindingListView

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="IBusinessObjectCollection"/>.
        /// </summary>
        public object SyncRoot
        {
            get { return ((IBusinessObjectCollection)ViewOfBusinessObjectCollection).SyncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="IBusinessObjectCollection"/> is 
        /// synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((IBusinessObjectCollection)ViewOfBusinessObjectCollection).IsSynchronized; }
        }


        /// <summary>
        /// Gets the number of objects contained in the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        public int Count
        {
               //TODO brett 19 Jan 2011: get { return _filterIndices == null ? BusinessObjectCollection.Count : _filterIndices.Length; }
            get
            {
               // _logger.Log("Get Count "); This gets ht hundreds of times
                return ViewOfBusinessObjectCollection.Count; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set. 
        /// </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> 
        /// is not a valid index in the <see cref="T:System.Collections.IList"/>. 
        /// </exception><exception cref="T:System.NotSupportedException">The property is set and the 
        /// <see cref="T:System.Collections.IList"/> is read-only. 
        /// </exception><filterpriority>2</filterpriority>
        public object this[int index]
        {
            get
            {
                //_logger.Log("Get this ", LogCategory.Info); this fires many many times.
                //TODO brett 19 Jan 2011: This needs to return the item from the filtered list

                try
                {
                    return ViewOfBusinessObjectCollection[index];
                }
                catch (Exception e)
                {
                    _logger.Log("Get this. Fired Exception " + e.Message, LogCategory.Exception);
                    throw;
                }
            }
            set
            {
                _logger.Log("Set this ", LogCategory.Info);
                ViewOfBusinessObjectCollection[index] = (T)value;
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="IList"/>.  
        /// </summary>
        public void Clear()
        {

            _logger.Log("Clear ", LogCategory.Info);
            ViewOfBusinessObjectCollection.Clear();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }


        /// <summary>
        /// Determines whether the <see cref="IBusinessObjectCollection"/> contains a specific <see cref="BusinessObject"/>.
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns>True if the object is found in the collection; otherwise, false.</returns>
        public bool Contains(object value)
        {
            _logger.Log("Contains : " + value + " ", LogCategory.Info);
            T bo = (T)value;
            return ViewOfBusinessObjectCollection.Contains(bo);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            _logger.Log("IndexOf : " + value + " ", LogCategory.Info);
            return ViewOfBusinessObjectCollection.IndexOf((T)value);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="IBusinessObjectCollection"/>.</param>
        public void Remove(object value)
        {
            _logger.Log("Remove : " + value + " ", LogCategory.Info);
            var index = IndexOf(value);
            ViewOfBusinessObjectCollection.Remove((T)value);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index, index));
        }

        /// <summary>
        /// Removes the <see cref="IBusinessObjectCollection"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {

            _logger.Log("RemoveAt : " + index + " ", LogCategory.Info);
            ViewOfBusinessObjectCollection.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        /// <summary>
        /// Inserts an object to the <see cref="IBusinessObjectCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the collection.</param>
        public void Insert(int index, object value)
        {
            if (value != null && !typeof(T).IsAssignableFrom(value.GetType()))
                throw new HabaneroArgumentException("Given instance doesn't match needed type.");

            _logger.Log("Insert Index : " + index + " value : " + value, LogCategory.Info);
            ViewOfBusinessObjectCollection.Insert(index, (T)value);
            BusinessObjectCollection.Insert(index, (T) value);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        /// <summary>
        ///  Called when the list changes or an item in the list changes.
        /// </summary>
        protected void OnListChanged(ListChangedEventArgs args)
        {

            if (ListChanged != null) ListChanged(this, args);
        }

        /// <summary>
        /// Returns the index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor"/>.
        /// </summary>
        /// <returns>
        /// The index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor"/>.
        /// </returns>
        /// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor"/> to search on. </param>
        /// <param name="key">The value of the <paramref name="property"/> parameter to search for.</param>
        public int Find(PropertyDescriptor property, object key)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (key == null) throw new ArgumentNullException("key");

            _logger.Log("Find : " + property.Name + " value : " + key, LogCategory.Info);
            var foundBO = ViewOfBusinessObjectCollection.Find(obj => obj.GetPropertyValue(property.Name) == key);
            if (foundBO == null) return -1;
            return ViewOfBusinessObjectCollection.IndexOf(foundBO);
        }


        #endregion

        #region Basic Binding List Props

        /// <summary>
        /// Whether you can add items to the business object collection
        /// </summary>
        public bool AllowNew
        {
            get { return true; }
        }

        /// <summary>
        /// whether you can update items in the business object collection
        /// </summary>
        public bool AllowEdit
        {
            get { return true; }
        }


        /// <summary>
        /// Gets whether you can remove items from the list, using <see cref="M:System.Collections.IList.Remove(System.Object)"/> or <see cref="M:System.Collections.IList.RemoveAt(System.Int32)"/>.
        /// </summary>
        /// <returns>
        /// true if you can remove items from the list; otherwise, false.
        /// </returns>
        public bool AllowRemove
        {
            get { return true; }
        }
        /// <summary>
        /// Supports Change Notification
        /// </summary>
        public bool SupportsChangeNotification
        {
            get { return true; }
        }
        /// <summary>
        /// Gets whether the list supports searching using the Find method. (Inherited from IBindingList.)
        /// </summary>
        public bool SupportsSearching
        {
            get { return true; }
        }
        /// <summary>
        /// Supports Sorting
        /// </summary>
        public bool SupportsSorting
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
        ///  Gets a value indicating whether the  <see cref="IBusinessObjectCollection"/> has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports advanced sorting.
        /// </summary>
        public bool SupportsAdvancedSorting
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

        #endregion

        #region BindingList.Sort

        /// <summary>
        /// Sort descriptors
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions { get; set; }

        /// <summary>
        /// Sort direction.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get
            {
                _logger.Log("SortDirection Get", LogCategory.Info);
                if (SortDescriptions != null)
                    if (SortDescriptions.Count == 1) return SortDescriptions[0].SortDirection;
                return ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Current sort property.
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get
            {
                _logger.Log("Sort Property Get", LogCategory.Info);
                if (SortDescriptions != null)
                    if (SortDescriptions.Count == 1) return SortDescriptions[0].PropertyDescriptor;
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IBusinessObjectCollection"/> is sorted
        /// </summary>
        public bool IsSorted
        {
            get
            {
                _logger.Log("IsSorted", LogCategory.Info);
                if (SortDescriptions == null) return false;
                return SortDescriptions.Count > 0;
            }
        }



        /// <summary>
        /// Sorts the collection based on a PropertyDescriptor and a ListSortDirection.
        /// </summary>
        /// <param name="property">The <see cref="PropertyDescriptor"/> to sort by.</param>
        /// <param name="direction">One of the <see cref="ListSortDirection"/> values.</param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            try
            {
                _logger.Log("ApplySort - PropertyDescriptor : " + property.Name + " Direction : " + direction, LogCategory.Info);

                if (property is PropertyDescriptorPropDef)
                {
                    this.ViewOfBusinessObjectCollection.Sort(property.Name, true, direction == ListSortDirection.Ascending);
                }
                else
                {
                    this.ViewOfBusinessObjectCollection.Sort(property.Name, false, direction == ListSortDirection.Ascending);
                }
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptionCollection"></param>
        public void ApplySort(ListSortDescriptionCollection descriptionCollection)
        {
            _logger.Log("ApplySort - ListSortDescriptionCollection ", LogCategory.Info);
            SortDescriptions = descriptionCollection;
            ViewOfBusinessObjectCollection.Sort(new GenericComparer<T>(SortDescriptions));
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }


        #endregion

        /// <summary>
        /// Copies the objects of the <see cref="IBusinessObjectCollection"/> to an <see cref="Array"/>, 
        /// starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the objects copied from 
        /// <see cref="IBusinessObjectCollection"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (index < 0) throw new ArgumentOutOfRangeException("index");
            if (index >= array.Length) throw new ArgumentException("index");
            if (ViewOfBusinessObjectCollection != null) ViewOfBusinessObjectCollection.CopyTo(array, index);
            _logger.Log("CopyTo ", LogCategory.Info);
        }

        #region IEmunerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            _logger.Log("AddIndex ", LogCategory.Info);
            return this.ViewOfBusinessObjectCollection.GetEnumerator();
        }

        #endregion

        #region IList

        int IList.Add(object value)
        {
            _logger.Log("Add (" + value + ")", LogCategory.Info);
            this.Insert(this.Count, value);
            return this.Count;
        }

        #endregion

        #region IBindingList

        public void AddIndex(PropertyDescriptor property)
        {
            _logger.Log("AddIndex ", LogCategory.Info);
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            _logger.Log("RemoveIndex ", LogCategory.Info);
        }

        public void RemoveSort()
        {
            _logger.Log("RemoveSort ", LogCategory.Info);
        }

        #endregion

        #region Filter

        public void RemoveFilter()
        {
            _logger.Log("RemoveFilter ", LogCategory.Info);
        }

        private string _filter;
        public string Filter
        {
            get
            {
                _logger.Log("Get Filter ", LogCategory.Info);
                return _filter;
            }
            set
            {
                _logger.Log("Set Filter ", LogCategory.Info);
                _filter = value;
            }
        }

        #endregion

        #region Implementation of ICancelAddNew

        /// <summary>
        /// Adds a new item to the list. This method works very closely with the
        /// <see cref="ICancelAddNew"/> interface in the way it interacts with the 
        /// grid to manage permanent add (EndNew) and the Cancelling of the Add (CancelNew)
        /// </summary>
        /// <returns>
        /// The item added to the list.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException"><see cref="P:System.ComponentModel.IBindingList.AllowNew"/> is false. </exception>
        public object AddNew()
        {
            _logger.Log("AddNew ", LogCategory.Info);
            var addedBO = this.ViewOfBusinessObjectCollection.CreateBusinessObject();
            _addedBOs.Add(this.Count - 1, addedBO);
            return addedBO;
        }

        /// <summary>
        /// Discards a pending new item from the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        public void CancelNew(int itemIndex)
        {
            _logger.Log("Start CancelNew (" + itemIndex + ")", LogCategory.Info);
            var addedBO = this.ViewOfBusinessObjectCollection[itemIndex];

            if (_addedBOs.ContainsKey(itemIndex) && addedBO != null)
            {
                _addedBOs.Remove(itemIndex);
                _logger.Log("In CancelNew CancelEdits (" + addedBO + ")", LogCategory.Info);
                addedBO.CancelEdits();
                this.ViewOfBusinessObjectCollection.Remove(addedBO);


                _logger.Log("In CancelNew B4 ListChanged (" + itemIndex + ")", LogCategory.Info);
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex, itemIndex));
                _logger.Log("In CancelNew AF ListChanged (" + itemIndex + ")", LogCategory.Info);
            }
            //OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex, itemIndex));
            _logger.Log("End CancelNew (" + itemIndex + ")", LogCategory.Info);
        }

        //TODO brett 23 Jan 2011:  This should update the underlying collection for added bos
        /// <summary>
        /// Commits a pending new item to the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        public void EndNew(int itemIndex)
        {
            _logger.Log("Start EndNew (" + itemIndex + ")", LogCategory.Info);
/*            //this did not solve the error revert this to the simple code of doing this in the CancelNew?
            if (_addedBOsCancelled.ContainsKey(itemIndex))
            {
                var bo = _addedBOsCancelled[itemIndex];
                _logger.Log("In EndNew CancelEdits (" + bo + ")", LogCategory.Info);
                bo.CancelEdits();
                this.ViewOfBusinessObjectCollection.Remove(bo);
                _addedBOsCancelled.Remove(itemIndex);

                _logger.Log("EndNew B4 ListChanged (" + itemIndex + ")", LogCategory.Info);
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex, itemIndex));
                _logger.Log("EndNew AF ListChanged (" + itemIndex + ")", LogCategory.Info);
            }*/

            _logger.Log("End EndNew (" + itemIndex + ")", LogCategory.Info);
        }

        #endregion

        #region Implementation of IRaiseItemChangedEvents

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.ComponentModel.IRaiseItemChangedEvents"/> object raises <see cref="E:System.ComponentModel.IBindingList.ListChanged"/> events.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.ComponentModel.IRaiseItemChangedEvents"/> object raises <see cref="E:System.ComponentModel.IBindingList.ListChanged"/> events when one of its property values changes; otherwise, false.
        /// </returns>
        public bool RaisesItemChangedEvents
        {
            //TODO brett 19 Jan 2011: What I am assuming this does is that when your BO.Prop changes this event is raised resulting in the DataGrid Updating?
            get { return true; }
        }

        #endregion
    }
}