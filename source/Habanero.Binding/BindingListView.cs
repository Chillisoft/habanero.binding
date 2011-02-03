using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;

namespace Habanero.Binding
{
    /// <summary>
    /// A filterable, sortable view of a <see cref="BusinessObjectCollection{TBusinessObject}"/>.
    /// This can be used for binding to an Editable Grid.
    /// It can also obviously be used to bind to any other collecton bindable control such as 
    /// a combo box, Listbox, readonly grid etc.
    /// Note_ this does not support hierachical binding so it will not support binding to a
    /// tree view.
    /// If you want to bind a <see cref="BusinessObjectCollection{TBusinessObject}"/>
    ///  to a tree view where its compositional or aggregate children are automatically bound then
    ///  please look at Habanero.Faces.
    /// 
    /// A very inportant concept of a BindingListView is that it is a a View of the underlying collection.
    /// It references the same instance of the <see cref="IBusinessObject"/>s but does so
    /// in a seperate collection. Filtering, Sorting should not affect the
    /// underlying Business Object Collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingListView<T> : ITypedList, ICancelAddNew, IBindingListView, IRaiseItemChangedEvents
        where T : class, IBusinessObject, new()
    {
        private IViewBuilder _viewBuilder;
        private readonly IDictionary<int, T> _addedBOs = new Dictionary<int, T>();
        private static readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger("Habanero.Binding.BindingListViewNew");
        /// <summary>
        /// 
        /// </summary>
        public event ListChangedEventHandler ListChanged;
        #region Constructors

        /// <summary>
        /// New object constructs a default BindingListView
        /// </summary>
        public BindingListView() : this(new BusinessObjectCollection<T>())
        {
        }

        /// <summary>
        /// This constructs a new BindingListView with a business object collection
        /// </summary>
        /// <param name="boCol"></param>
        public BindingListView(BusinessObjectCollection<T> boCol)
        {
            if (boCol == null) throw new ArgumentNullException("boCol");
            this.BusinessObjectCollection = boCol;
            this.ViewOfBusinessObjectCollection = boCol.Clone();
            this.BusinessObjectCollection.BusinessObjectAdded += OnBusinessObjectAdded;
            this.BusinessObjectCollection.BusinessObjectRemoved += OnBusinessObjectRemoved;
        }

        private void OnBusinessObjectRemoved(object sender, BOEventArgs<T> args)
        {
            var boToBeRemoved = args.BusinessObject;
            if (boToBeRemoved == null) return;
            var removedIndex = this.ViewOfBusinessObjectCollection.IndexOf(boToBeRemoved);
            this.ViewOfBusinessObjectCollection.Remove(boToBeRemoved);
            FireListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, removedIndex));        
        }

        private void OnBusinessObjectAdded(object sender, BOEventArgs<T> args)
        {
            var boToBeAdded = args.BusinessObject;
            if (boToBeAdded == null) return;
            var currentCriteria = this.ViewOfBusinessObjectCollection.SelectQuery.Criteria;
            if (currentCriteria == null || currentCriteria.IsMatch(boToBeAdded))
            {
                this.ViewOfBusinessObjectCollection.Add(boToBeAdded);
                FireListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.ViewOfBusinessObjectCollection.Count));              
            }
        }

        /// <summary>
        /// This constructs a new BindingListView with a business object collection
        /// </summary>
        /// <param name="boCol"></param>
        /// <param name="viewBuilder"></param>
        public BindingListView(BusinessObjectCollection<T> boCol, IViewBuilder viewBuilder): this(boCol)
        {
            this.ViewBuilder = viewBuilder;
        }
        /// <summary>
        /// Returns the undelying <see cref="IBusinessObjectCollection"/>.
        /// </summary>
        public BusinessObjectCollection<T> BusinessObjectCollection { get; private set; }

        private BusinessObjectCollection<T>  _viewOfBusinessObjectCollection;

        /// <summary>
        /// Returns the View (Copy) of the Underlying <see cref="IBusinessObjectCollection"/>
        /// </summary>
        protected BusinessObjectCollection<T> ViewOfBusinessObjectCollection
        {
            get { return _viewOfBusinessObjectCollection; }
            private set
            {
                _viewOfBusinessObjectCollection = value;
                FireListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        #endregion

        #region ITypedList

        /// <summary>
        /// Returns the name of this list.
        /// </summary>
        /// <param name="listAccessors"></param>
        /// <returns></returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
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
            get { return ViewOfBusinessObjectCollection.Count; }
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
                return ViewOfBusinessObjectCollection[index];
            }
            set
            {
                var indexInUnderlyingCollection = this.BusinessObjectCollection.IndexOf(this.ViewOfBusinessObjectCollection[index]);
                this.ViewOfBusinessObjectCollection[index] = (T)value;
                this.BusinessObjectCollection[indexInUnderlyingCollection] = (T) value;
                FireListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="IList"/>.  
        /// </summary>
        public void Clear()
        {
            this.ViewOfBusinessObjectCollection.Clear();
            FireListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        public int Add(object value)
        {
            var bo = value as T;
            this.ViewOfBusinessObjectCollection.Add(bo);
            this.BusinessObjectCollection.Add(bo);
            return ViewOfBusinessObjectCollection.Count -1;
        }

        /// <summary>
        /// Determines whether the <see cref="IBusinessObjectCollection"/> contains a specific <see cref="BusinessObject"/>.
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns>True if the object is found in the collection; otherwise, false.</returns>
        public bool Contains(object value)
        {
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
            return ViewOfBusinessObjectCollection.IndexOf((T)value);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="IBusinessObjectCollection"/>.</param>
        public void Remove(object value)
        {
            var index = IndexOf(value);
            ViewOfBusinessObjectCollection.Remove((T)value);
            FireListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index, index));
        }

        /// <summary>
        /// Removes the <see cref="IBusinessObjectCollection"/> item at the specified index.
        /// This is the code that is actually hit when the Delete Key is pressed from a Data Grid View.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            var boToDelete = ViewOfBusinessObjectCollection[index];
            boToDelete.MarkForDelete();
            boToDelete.Save();
            FireListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
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

            ViewOfBusinessObjectCollection.Insert(index, (T)value);
            BusinessObjectCollection.Insert(index, (T) value);
            FireListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

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
        }
        /// <summary>
        ///  Called when the list changes or an item in the list changes.
        /// </summary>
        protected void FireListChanged(ListChangedEventArgs args)
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
            var foundBO = ViewOfBusinessObjectCollection.Find(obj => obj.GetPropertyValue(property.Name) == key);
            if (foundBO == null) return -1;
            return ViewOfBusinessObjectCollection.IndexOf(foundBO);
        }

        /// <summary>
        /// Removes the <see cref="T:System.ComponentModel.PropertyDescriptor"/> from the indexes used for searching.
        /// </summary>
        /// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor"/> to remove from the indexes used for searching. </param>
        public void RemoveIndex(PropertyDescriptor property)
        {
            //Do Nothing See tests for full explanation of why
        }

        /// <summary>
        /// Adds the <see cref="T:System.ComponentModel.PropertyDescriptor"/> to the indexes used for searching.
        /// </summary>
        /// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor"/> to add to the indexes used for searching. </param>
        public void AddIndex(PropertyDescriptor property)
        {
            //Do Nothing See tests for full explanation of why
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
                if (SortDescriptions == null) return false;
                return SortDescriptions.Count > 0;
            }
        }


        /// <summary>
        /// Removes any sort applied using <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"><see cref="P:System.ComponentModel.IBindingList.SupportsSorting"/> is false. </exception>
        public void RemoveSort()
        {
            this.ViewOfBusinessObjectCollection = this.BusinessObjectCollection.Clone();
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
                this.SortDescriptions = CreateSortDescriptions(property, direction);
                if (property is PropertyDescriptorPropDef)
                {
                    this.ViewOfBusinessObjectCollection.Sort(property.Name, true, direction == ListSortDirection.Ascending);
                }
                else
                {
                    this.ViewOfBusinessObjectCollection.Sort(property.Name, false, direction == ListSortDirection.Ascending);
                }
                FireListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }

        private static ListSortDescriptionCollection CreateSortDescriptions(PropertyDescriptor property, ListSortDirection direction)
        {
            var listSortDescription = new ListSortDescription(property, direction);
            return new ListSortDescriptionCollection(new[] { listSortDescription });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptionCollection"></param>
        public void ApplySort(ListSortDescriptionCollection descriptionCollection)
        {
            SortDescriptions = descriptionCollection;
            ViewOfBusinessObjectCollection.Sort(new GenericComparer<T>(SortDescriptions));
            FireListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        #endregion


        #region ICancelAddNew
        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <returns>
        /// The item added to the list.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException"><see cref="P:System.ComponentModel.IBindingList.AllowNew"/> is false. </exception>
        public object AddNew()
        {
            try
            {
                _logger.Log("AddNew ", LogCategory.Info);
                var addedBO = this.ViewOfBusinessObjectCollection.CreateBusinessObject();
                _addedBOs.Add(this.Count - 1, addedBO);
                return addedBO;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Exception);
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
            return null;
        }

        /// <summary>
        /// Discards a pending new item from the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        public void CancelNew(int itemIndex)
        {
            try
            {
                _logger.Log("Start CancelNew (" + itemIndex + ")", LogCategory.Info);
                var addedBO = this.ViewOfBusinessObjectCollection[itemIndex];
                if (_addedBOs.ContainsKey(itemIndex) && addedBO != null)
                {
                    _addedBOs.Remove(itemIndex);
                    if (addedBO.Status.IsNew)
                    {
                        addedBO.MarkForDelete();
                        _logger.Log("In CancelNew B4 OnListChanged (" + itemIndex + ")", LogCategory.Info);
                        FireListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, itemIndex, itemIndex));
                    }
                }
                _logger.Log("End CancelNew (" + itemIndex + ")", LogCategory.Info);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Exception);
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        /// <summary>
        /// Commits a pending new item to the collection.
        /// </summary>
        /// <param name="itemIndex">The index of the item that was previously added to the collection. </param>
        public void EndNew(int itemIndex)
        {
            try
            {
                _logger.Log("Start EndNew (" + itemIndex + ")", LogCategory.Info);
                var addedBO = this.ViewOfBusinessObjectCollection[itemIndex];
                if (_addedBOs.ContainsKey(itemIndex) && addedBO != null)
                {
                    this.BusinessObjectCollection.Add(addedBO);
                    addedBO.Save();
                    _addedBOs.Remove(itemIndex);
                }
                _logger.Log("End EndNew (" + itemIndex + ")", LogCategory.Info);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogCategory.Exception);
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }
        #endregion

        #region IBindingListView.Filter

        private string _filter;


        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the list of items returned by the data source
        /// </summary>
        /// <returns>
        /// The string used to filter items out in the item list returned by the data source. 
        /// </returns>
        public string Filter
        {
            get { return _filter; }
            set
            {
                try
                {
                    _filter = value;
                    if (!String.IsNullOrEmpty(value))
                    {
                        var col = new BusinessObjectCollection<T>();
                        var criteria = GetCriteriaObject(ClassDef.ClassDefs[typeof(T)], _filter);

                        var matchingBos = this.BusinessObjectCollection.Where(bo => criteria.IsCriteriaMatch(bo));
                        col.Add(matchingBos);
                        col.SelectQuery.Criteria = criteria;
                        this.ViewOfBusinessObjectCollection = col;
                    }
                    else
                    {
                        ResetViewCollection();   
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message, LogCategory.Exception);
                    GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
                }
            }
        }

        private void ResetViewCollection()
        {
            this.ViewOfBusinessObjectCollection = this.BusinessObjectCollection.Clone();
        }

        private static Criteria GetCriteriaObject(IClassDef classDef, string criteriaString)
        {
            var criteria = CriteriaParser.CreateCriteria(criteriaString);
            QueryBuilder.PrepareCriteria(classDef, criteria);
            return criteria;
        }

        /// <summary>
        /// Removes the current filter applied to the List.
        /// </summary>
        public void RemoveFilter()
        {
            this.Filter = string.Empty;
        }
        #endregion

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ViewOfBusinessObjectCollection.GetEnumerator();
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
            get { return true; }
        }

        #endregion
    }

    /// <summary>
    /// A generic comparer for List&lt;T&gt;.Sort(IComparer).
    /// For strings, use <see cref="System.StringComparer"/>
    /// </summary>
    public class GenericComparer<T> : IComparer<T>
    {
        private readonly ListSortDescriptionCollection _sortDescriptions;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="sortDescriptions">
        /// The <see cref="ListSortDescriptionCollection"/> which should be
        /// used as the bassi for comparison.
        /// </param>
        public GenericComparer(ListSortDescriptionCollection sortDescriptions)
        {
            _sortDescriptions = sortDescriptions;
        }

        #region IComparer<T> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        public int Compare(T x, T y)
        {
            for (int i = 0; i < _sortDescriptions.Count; i++)
            {
                var propertyDescriptor = _sortDescriptions[i].PropertyDescriptor;
                object valueX = propertyDescriptor.GetValue(x);
                object valueY = propertyDescriptor.GetValue(y);

                bool xIsNull = valueX == DBNull.Value || valueX == null;
                bool yIsNull = valueY == DBNull.Value || valueY == null;

                int result;
                if (xIsNull)
                {
                    if (yIsNull) result = 0;
                    else result = -1;
                }
                else
                {
                    if (yIsNull) result = 1;
                    else
                    {
                        IComparable comparableX = valueX as IComparable;
                        IComparable comparableY = valueY as IComparable;
                        result = comparableX.CompareTo(comparableY);
                    }
                }
                if (result != 0)
                    return _sortDescriptions[i].SortDirection == ListSortDirection.Ascending ? result : -result;
            }
            return 0;
        }

        #endregion
    }
    internal static class CriteriaExtensions
    {

        internal static bool IsCriteriaMatch(this Criteria criteria, IBusinessObject bo)
        {
            return criteria == null || criteria.IsMatch(bo, false);
        }
    }
}