using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;

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
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingListView<T> : IBindingListView, ITypedList, ICancelAddNew
        where T : class, IBusinessObject, new()
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public event ListChangedEventHandler ListChanged;
        private DataTable _filterTable;
        private PropertyDescriptorCollection _descriptorCollection;
        /// <summary>
        /// 
        /// </summary>
        public IViewBuilder ViewBuilder { get; set; }
        private string _currentFilterExpression = null;

        #endregion

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
        /// <param name="collection"></param>
        public BindingListView(BusinessObjectCollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            BusinessObjectCollection = collection;
            RemoveSort();
            InitializeFiltering();
        }

        private void InitializeFiltering()
        {
            _descriptorCollection = TypeDescriptor.GetProperties(typeof (T));
            _filterTable = new DataTable("FilterTable");
            foreach (PropertyDescriptor property in _descriptorCollection)
            {
                Type colType = property.PropertyType;
                //TODO brett 19 Jan 2011: This should be refactored to use common syntax e.g. from wrappers.
                //  there is code in Smooth to do this I would sugges either using this or moving this all into reflection utils.
                if (colType.IsGenericType && colType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
                    colType = colType.GetGenericArguments()[0];
                _filterTable.Columns.Add(property.Name, colType);
            }
        }

        #endregion

        /// <summary>
        /// Gets the business object collection
        /// </summary>
        public BusinessObjectCollection<T> BusinessObjectCollection { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate 
        /// through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
                yield return this[i];
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
            if (BusinessObjectCollection != null) BusinessObjectCollection.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of objects contained in the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        public int Count
        { //TODO brett 19 Jan 2011: get { return _filterIndices == null ? BusinessObjectCollection.Count : _filterIndices.Length; }
            get { return BusinessObjectCollection.Count; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="IBusinessObjectCollection"/>.
        /// </summary>
        public object SyncRoot
        {
            get { return ((IBusinessObjectCollection) BusinessObjectCollection).SyncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="IBusinessObjectCollection"/> is 
        /// synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((IBusinessObjectCollection) BusinessObjectCollection).IsSynchronized; }
        }

        /// <summary>
        /// Adds an item to the <see cref="IBusinessObjectCollection"/>.  
        /// </summary>
        /// <param name="value">The instance to add to the <see cref="IBusinessObjectCollection"/>.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(object value)
        {
            if (value != null && !typeof (T).IsAssignableFrom(value.GetType()))
                throw new HabaneroArgumentException("Given instance doesn't match needed type.");

            T bo = (T) value;
            BusinessObjectCollection.Add(bo);
            int newIndex = IndexOf(bo);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
            return newIndex;
        }

        /// <summary>
        /// Determines whether the <see cref="IBusinessObjectCollection"/> contains a specific <see cref="BusinessObject"/>.
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns>True if the object is found in the collection; otherwise, false.</returns>
        public bool Contains(object value)
        {
            T bo = (T) value;
            return BusinessObjectCollection.Contains(bo);
        }

        /// <summary>
        /// Removes all items from the <see cref="IList"/>.  
        /// </summary>
        public void Clear()
        {

            BusinessObjectCollection.Clear();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        /// <param name="value">The object to locate in the collection.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            return BusinessObjectCollection.IndexOf((T) value);
        }

        /// <summary>
        /// Inserts an object to the <see cref="IBusinessObjectCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The object to insert into the collection.</param>
        public void Insert(int index, object value)
        {
            if (value != null && !typeof (T).IsAssignableFrom(value.GetType()))
                throw new HabaneroArgumentException("Given instance doesn't match needed type.");

            BusinessObjectCollection.Insert(index, (T) value);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IBusinessObjectCollection"/>. 
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="IBusinessObjectCollection"/>.</param>
        public void Remove(object value)
        {
            int index = IndexOf(value);
            BusinessObjectCollection.Remove((T) value);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        /// <summary>
        /// Removes the <see cref="IBusinessObjectCollection"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            BusinessObjectCollection.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
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
                //TODO brett 19 Jan 2011: This needs to return the item from the filtered list
                return BusinessObjectCollection[index];
            }
            set
            {
                BusinessObjectCollection[index] = (T) value;
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
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
            get { return true; }
        }

        public object AddNew()
        {
            var bo = new T();
            BusinessObjectCollection.Add(bo);
            return bo;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="property"></param>
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
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
            var foundBO = BusinessObjectCollection.Find(obj => obj.GetPropertyValue(property.Name) == key);
            if (foundBO == null) return -1;
            return BusinessObjectCollection.IndexOf(foundBO);
        }

        /// <summary>
        /// Removes the System.ComponentModel.PropertyDescriptor from the indexes used for searching.
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="property"> PropertyDescriptor to remove from the indexes used for searching.</param>
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Removes any sort applied using <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"><see cref="P:System.ComponentModel.IBindingList.SupportsSorting"/> is false. </exception>
        public void RemoveSort()
        {
            this.BusinessObjectCollection.Sort();
        }

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
            get { return false; }
        }
        /// <summary>
        /// Supports Sorting
        /// </summary>
        public bool SupportsSorting
        {
            get { return true; }
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
        /// Sorts the collection based on a PropertyDescriptor and a ListSortDirection.
        /// </summary>
        /// <param name="property">The <see cref="PropertyDescriptor"/> to sort by.</param>
        /// <param name="direction">One of the <see cref="ListSortDirection"/> values.</param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            try
            {
                if (property is PropertyDescriptorPropDef)
                {
                    this.BusinessObjectCollection.Sort(property.Name, true, direction == ListSortDirection.Ascending);
                }
                else
                {
                    this.BusinessObjectCollection.Sort(property.Name, false, direction == ListSortDirection.Ascending);
                }
/*
                _sortedList = new ArrayList();
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                var listSortDescription = new ListSortDescription(property, direction);
                var listSortDescriptions = new[] { listSortDescription };
                var sorts = new ListSortDescriptionCollection(listSortDescriptions);

                // Check to see if the property type we are sorting by implements
                // the IComparable interface.
                Type interfaceType = property.PropertyType.GetInterface("IComparable");
                if (interfaceType != null)
                {
                    _unsortedItems = new ArrayList(this.Count);
                    this.BusinessObjectCollection.Sort();
                    // Loop through each item, adding it the the sortedItems ArrayList.
                    foreach (T item in this.BusinessObjectCollection)
                    {
                        if (item == null) continue;
                        var propertyValue = property.GetValue(item);
                        if (propertyValue != null) _sortedList.Add(propertyValue);
                        _unsortedItems.Add(item);
                    }
                    // Call Sort on the ArrayList. 
                    _sortedList.Sort();
                    if (direction == ListSortDirection.Descending)
                        _sortedList.Reverse();

                    for (int i = 0; i < this.Count; i++)
                    {
                        int position = Find(property, _sortedList[i]);
                        if (position > 0 && position != i)
                        {
                            T temp = this.BusinessObjectCollection[i];
                            this.BusinessObjectCollection[i] = this.BusinessObjectCollection[position];
                            this.BusinessObjectCollection[position] = temp;
                        }
                    }
                    //Raise the ListChanged event so bound controls refresh their values.
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
                else
                {
                    ApplySort(sorts);
                }*/
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "Error", "Error");
            }
        }

        public void ApplySort(ListSortDescriptionCollection descriptionCollection)
        {
            SortDescriptions = descriptionCollection;
            /*              _sortIndices = new int[BusinessObjectCollection.Count];
                      object[] items = new object[BusinessObjectCollection.Count];
                      for (int i = 0; i < _sortIndices.Length; i++)
                      {
                          _sortIndices[i] = i;//Index for the sorted Collection allows you to create a sorted collection without
                          // a real collection underneath.
                          items[i] = BusinessObjectCollection[i];
                      }*/
            BusinessObjectCollection.Sort(new GenericComparer<T>(SortDescriptions));
            Filter = _currentFilterExpression;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Removes the current filter applied to the collection.
        /// </summary>
        public void RemoveFilter()
        {
            this.Filter = string.Empty;
        }

        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the collection of items returned by the data source
        /// </summary>
        /// <returns>
        /// The string used to filter items out in the item collection returned by the data source. 
        /// </returns>
        public string Filter
        {
            get { return _currentFilterExpression; }
            set
            {
                if (_currentFilterExpression == value) return;

                // If the value is not null or empty, but doesn't
                // match expected format, throw an exception.
                if (!string.IsNullOrEmpty(value) &&
                    !Regex.IsMatch(value, BuildRegExForFilterFormat(), RegexOptions.Singleline))
                    throw new ArgumentException("Filter is not in the format: propName[<>=]'value'.");


                // If the value is null or empty, reset list.
                if (string.IsNullOrEmpty(value))
                    ResetList();
                else
                {
                    int count = 0;
                    string[] matches = value.Split(new[] {" AND "}, StringSplitOptions.RemoveEmptyEntries);

                    while (count < matches.Length)
                    {
                        string filterPart = matches[count];

                        // Check to see if the filter was set previously.
                        // Also, check if current filter is a subset of 
                        // the previous filter.
                        if (!String.IsNullOrEmpty(_currentFilterExpression) && !value.Contains(_currentFilterExpression))
                            ResetList();

                        // Parse and apply the filter.
                        SingleFilterInfo filterInfo = ParseFilter(filterPart);
                        ApplyFilter(filterInfo);
                        count++;
                    }
                }
                // Set the filter value and turn on list changed events.
                _currentFilterExpression = value;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }


        private void ApplyFilter(SingleFilterInfo filterParts)
        {
            // Check to see if the property type we are filtering by implements
            // the IComparable interface.
            Type interfaceType =
                TypeDescriptor.GetProperties(typeof (T))[filterParts.PropName].PropertyType.GetInterface("IComparable");

            if (interfaceType == null)
                throw new InvalidOperationException("Filtered property" + " must implement IComparable.");

            var results = new BusinessObjectCollection<T>();
            // Check each value and add to the results list.
            foreach (T item in BusinessObjectCollection)
            {
                if (filterParts.PropDesc.GetValue(item) == null) continue;
                var compareValue = filterParts.PropDesc.GetValue(item) as IComparable;
                if (compareValue == null) continue;
                int result = compareValue.CompareTo(filterParts.CompareValue);
                if (filterParts.OperatorValue == FilterOperator.EqualTo && result == 0)
                    results.Add(item);
                if (filterParts.OperatorValue == FilterOperator.GreaterThan && result > 0)
                    results.Add(item);
                if (filterParts.OperatorValue == FilterOperator.LessThan && result < 0)
                    results.Add(item);
                if (filterParts.OperatorValue == FilterOperator.Like && IsMatch(filterParts, compareValue))
                    results.Add(item);
            }
            //TODO brett 19 Jan 2011:  this needs to be looked at
            TempBusinessObjectCollection = BusinessObjectCollection.Clone();
            BusinessObjectCollection.Clear();
            foreach (T itemFound in results)
                BusinessObjectCollection.Add(itemFound);
        }

        private static bool IsMatch(SingleFilterInfo filterParts, IComparable compareValue)
        {
            return compareValue.ToString().Contains(filterParts.CompareValue.ToString());
        }

        //TODO brett 19 Jan 2011: This needs to be looked at
        protected BusinessObjectCollection<T> TempBusinessObjectCollection { get; set; }

        private SingleFilterInfo ParseFilter(string filterPart)
        {
            var filterInfo = new SingleFilterInfo
                                 {
                                     OperatorValue = DetermineFilterOperator(filterPart)
                                 };

            string[] filterStringParts = filterInfo.OperatorValue == FilterOperator.Like
                                             ? filterPart.Split(new[] {" Like "},
                                                                StringSplitOptions.RemoveEmptyEntries)
                                             : filterPart.Split(new char[] {(char) filterInfo.OperatorValue});

            filterInfo.PropName = filterStringParts[0].Replace("[", "").Replace("]", "").Replace(" AND ", "").Trim();

            // Get the property descriptor for the filter property name.
            var filterPropDesc = TypeDescriptor.GetProperties(typeof (T))[filterInfo.PropName];

            // Convert the filter compare value to the property type.
            if (filterPropDesc == null)
                throw new InvalidOperationException("Specified property to " + "filter " + filterInfo.PropName +
                                                    " on does not exist on type: " + typeof (T).Name);

            filterInfo.PropDesc = filterPropDesc;

            string comparePartNoQuotes = StripOffQuotes(filterStringParts[1]);
            try
            {
                var converter = TypeDescriptor.GetConverter(filterPropDesc.PropertyType);
                if (converter != null) filterInfo.CompareValue = converter.ConvertFromString(comparePartNoQuotes);
            }
            catch (NotSupportedException)
            {
                throw new InvalidOperationException("Specified filter" +
                                                    "value " + comparePartNoQuotes + " can not be converted" +
                                                    "from string. Implement a type converter for " +
                                                    filterPropDesc.PropertyType);
            }
            return filterInfo;
        }

        /// <summary>
        /// Used to determine the filter's Operators.
        /// </summary>
        /// <param name="filterPart"></param>
        /// <returns></returns>
        public FilterOperator DetermineFilterOperator(string filterPart)
        {
            // Determine the filter's operator.
            if (Regex.IsMatch(filterPart, "[^>^<]=")) return FilterOperator.EqualTo;
            if (Regex.IsMatch(filterPart, "<[^>^=]")) return FilterOperator.LessThan;
            if (Regex.IsMatch(filterPart, "[^<]>[^=]")) return FilterOperator.GreaterThan;
            if (filterPart.Contains("Like")) return FilterOperator.Like;
            return FilterOperator.None;
        }

        private static string StripOffQuotes(string filterPart)
        {
            // Strip off quotes in compare value if they are present.
            if (Regex.IsMatch(filterPart, "'.+'"))
            {
                int quote = filterPart.IndexOf('\'');
                filterPart = filterPart.Remove(quote, 1);
                quote = filterPart.LastIndexOf('\'');
                filterPart = filterPart.Remove(quote, 1);
                filterPart = filterPart.Trim();
            }
            return filterPart;
        }

        private void ResetList()
        {
            BusinessObjectCollection.Clear();
            foreach (T t in TempBusinessObjectCollection)
            {
                BusinessObjectCollection.Add(t);
            }
        }

        // Build a regular expression to determine if 
        // filter is in correct format.
        private static string BuildRegExForFilterFormat()
        {
            var regex = new StringBuilder();
            // Look for optional literal brackets, 
            // followed by word characters or space.
            regex.Append(@"\[?[\w\s]+\]?\s?");
            // Add the operators: > < or =.
            regex.Append(@"[><=^Like^]");
            //Add optional space followed by optional quote and
            // any character followed by the optional quote.
            regex.Append(@"\s?'?.+'?");
            return regex.ToString();
        }

        /// <summary>
        /// Sort descriptors
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions { get; set; }

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

        /// <summary>
        /// Returns the name of the list
        /// </summary>
        /// <param name="listAccessors">
        /// An array of <see cref="PropertyDescriptor"/> objects, for which the list 
        /// name is returned. This can be a null reference.
        /// </param>
        /// <returns>The name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return this.GetType().Name;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (this.ViewBuilder != null) return this.ViewBuilder.GetPropertyDescriptors();
            return TypeDescriptor.GetProperties(typeof (T));
        }

        #region private methods

        /// <summary>
        ///  Called when the list changes or an item in the list changes.
        /// </summary>
        protected void OnListChanged(ListChangedEventArgs args)
        {
            if (ListChanged != null) ListChanged(this, args);
        }

        #endregion

        #region ICancelAddNew

        public void CancelNew(int itemIndex)
        {
//TODO brett 19 Jan 2011: this should just cancel the added object
            throw new NotImplementedException();
        }

        public void EndNew(int itemIndex)
        {
            //TODO brett 19 Jan 2011: 
            // this is a commit from the grids perspective of a new object.
            // this could be a save of the BO or a nothing. depending on whether the 
            // developer wants to save when the user moves off of a new Row or wants a seperate 
            // save button.
            throw new NotImplementedException();
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
}