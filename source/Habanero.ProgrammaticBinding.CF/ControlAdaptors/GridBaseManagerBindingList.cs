using System;
using System.ComponentModel;
using System.Diagnostics;
using Habanero.Base;
using Habanero.Base.Logging;
using Habanero.Binding;
using Habanero.BO;
using Habanero.Faces.Base;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    public class GridBaseManagerBindingList : GridBaseManager
    {
        private static readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger("Habanero.ProgrammaticBinding.GridBaseManagerBindingList");

        private IBindingListView _bindingListView;

        public GridBaseManagerBindingList(IGridBase gridBase) : base(gridBase)
        {
        }

        protected override IBindingListView GetBindingListView(IBusinessObjectCollection businessObjectCollection)
        {
            if (businessObjectCollection == null) throw new ArgumentNullException("businessObjectCollection");
            var classType = GetClassType(businessObjectCollection);
            if (this.ClassDef == null || this.ClassDef != businessObjectCollection.ClassDef)
            {
                this.ClassDef = businessObjectCollection.ClassDef;
            }
            _logger.Log("Start CreateBindingListView : classType : " + classType, LogCategory.Debug);
            _logger.Log(GetStackTrace(), LogCategory.Debug);

            //Needs this code 
            //            var uiDef = ((ClassDef) this.ClassDef).GetUIDef(UiDefName);
//            if (uiDef == null)
//            {
//                throw new ArgumentException
//                    (String.Format
//                         ("You cannot Get the data for the grid {0} since the uiDef {1} cannot be found for the classDef {2}",
//                          this._gridBase.Name, UiDefName, ((ClassDef)this.ClassDef).ClassName));
//            }
            IViewBuilder viewBuilder = null;
            try
            {
                Type defaultViewBuilderType = typeof (DefaultViewBuilder<>).MakeGenericType(classType);
                viewBuilder = (IViewBuilder) Activator.CreateInstance(defaultViewBuilderType);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogCategory.Exception);
                Console.WriteLine(e);
            }

            var bindingListType = typeof (BindingListView<>).MakeGenericType(classType);
            _bindingListView =
                (IBindingListView) Activator.CreateInstance(bindingListType, businessObjectCollection, viewBuilder);
            return _bindingListView;
        }

        private static string GetStackTrace()
        {
            var stack = new StackTrace();
            return stack.ToString();
        }

        /// <summary>
        /// See <see cref="IBOColSelectorControl.GetBusinessObjectAtRow"/>
        /// </summary>
        public override IBusinessObject GetBusinessObjectAtRow(int rowIndex)
        {
            if (_bindingListView == null) return base.GetBusinessObjectAtRow(rowIndex);
            if (rowIndex >= 0 && rowIndex < _bindingListView.Count)
                return _bindingListView[rowIndex] as IBusinessObject;

            _logger.Log("IN GetBusinessObjectAtRow No Bo not found in binding list at index '" + rowIndex + "'", LogCategory.Debug);
            return null;
        }

        private static Type GetClassType(IBusinessObjectCollection businessObjectCollection)
        {
            return businessObjectCollection.ClassDef.ClassType;
        }

        /// <summary>
        /// See <see cref="IGridBase.GetBusinessObjectRow"/>
        /// </summary>
        public override IDataGridViewRow GetBusinessObjectRow(IBusinessObject businessObject)
        {
            if (businessObject == null) return null;
            if (_bindingListView == null) return null;
            var indexOf = _bindingListView.IndexOf(businessObject);
            if (indexOf < 0)
            {
                _logger.Log("Bo not found in binding list : " + businessObject, LogCategory.Warn);
                return null;
            }
            return _gridBase.Rows[indexOf];
        }

        /// <summary>
        /// See <see cref="IBOColSelectorControl.SelectedBusinessObject"/>
        /// </summary>
        public override IBusinessObject SelectedBusinessObject
        {
            get
            {
                //                int rownum = -1;
                //                for (int i = 0; i < _gridBase.Rows.Count; i++)
                //                    if (_gridBase.Rows[i].Selected) rownum = i;
                for (int i = _gridBase.Rows.Count - 1; i >= 0; i--)
                {
                    if (_gridBase.Rows[i].Selected)
                    {
                        return this.GetBusinessObjectAtRow(i);
                    }
                }
                return null;
            }
            set
            {
                if (_boCol == null && value != null)
                {
                    throw new GridBaseInitialiseException
                        ("You cannot call SelectedBusinessObject if the collection is not set");
                }
                if (_bindingListView == null && value != null)
                {
                    base.SelectedBusinessObject = value;
                    return;
                }
                var gridRows = _gridBase.Rows;
                try
                {
                    _gridBase.SelectionChanged -= _gridBaseOnSelectionChangedHandler;
                    _fireBusinessObjectSelectedEvent = false;
                    ClearAllSelectedRows(gridRows);
                    if (value == null)
                    {
                        _gridBase.CurrentCell = null;
                    }
                }
                finally
                {
                    _fireBusinessObjectSelectedEvent = true;
                    _gridBase.SelectionChanged += _gridBaseOnSelectionChangedHandler;
                }
                var bo = value as BusinessObject;
                if (bo == null)
                {
                    FireBusinessObjectSelected();
                    return;
                }
                var indexOf = _bindingListView.IndexOf(bo);
                var boFoundAndHighlighted = false;
                if (indexOf >= 0 && indexOf < gridRows.Count)
                {
                    gridRows[indexOf].Selected = true;
                    boFoundAndHighlighted = true;
                    _gridBase.ChangeToPageOfRow(indexOf);
                }

                if (boFoundAndHighlighted && indexOf >= 0 && indexOf < gridRows.Count)
                {
                    if (_gridBase != null)
                    {
                        var row = _gridBase.Rows[indexOf];
                        if (row != null)
                        {
                            if (_gridBase.ColumnCount > 0)
                            {
                                var cell = row.Cells[0];
                                if (cell != null && cell.RowIndex >= 0) _gridBase.CurrentCell = cell;
                            }
                        }
                    }
                    if (_gridBase != null)
                        if (_gridBase.CurrentRow != null && !_gridBase.CurrentRow.Displayed)
                        {
                            try
                            {
                                _gridBase.FirstDisplayedScrollingRowIndex = _gridBase.Rows.IndexOf(_gridBase.CurrentRow);
                                gridRows[indexOf].Selected = true; //Getting turned off for some reason
                            }
                            catch (InvalidOperationException)
                            {
                                //Do nothing - designed to catch error "No room is available to display rows"
                                //  when grid height is insufficient
                            }
                        }
                }
            }
        }
    }
}