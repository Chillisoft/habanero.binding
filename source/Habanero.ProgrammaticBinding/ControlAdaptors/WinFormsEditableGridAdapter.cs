using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Base.Logging;
using Habanero.BO;
using Habanero.BO.Exceptions;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.Faces.Win.Grid;
using DataGridViewSelectionMode = Habanero.Faces.Base.DataGridViewSelectionMode;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is an interface used specificaly for adapting a any control that inherits from System.Windows.Control 
    /// so that it can be treated as an IControlHabanero and can therefore be used by Faces for Habanero.Binding,
    /// <see cref="PanelBuilder"/>
    /// or any other such required behaviour.
    /// </summary>
    public interface IWinFormsEditableGridAdapter : IEditableGrid, IWinFormsControlAdapter
    {
    }
    /// <summary>
    /// This is a Control Adapter for any <see cref="DataGridView"/> control.
    ///  It wraps the <see cref="DataGridView"/> control behind a standard interface.
    /// This allows Faces to interact with it as if it is a Habanero Control.
    /// </summary>
    public class WinFormsEditableGridAdapter : WinFormsGridBaseAdapter, IWinFormsEditableGridAdapter
    {
        private readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger(
                "Habanero.ProgrammaticBinding.ControlAdaptors.WinFormsEditableGridAdapter");
        private DeleteKeyBehaviours _deleteKeyBehaviour;
        public WinFormsEditableGridAdapter(DataGridView gridView) : base(gridView)
        {
            ConfirmDeletion = false;
            AllowUserToAddRows = true;
            _deleteKeyBehaviour = DeleteKeyBehaviours.DeleteRow;
            //            SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ComboBoxClickOnce = true;
            _gridView.UserDeletingRow += ConfirmRowDeletion;
            CheckUserConfirmsDeletionDelegate = CheckUserWantsToDelete;
            _gridView.UserDeletedRow += ((sender, e) => ResetBOCollection());
            _gridView.CellClick += CellClickHandler;
        }

        #region Events

#pragma warning disable 168
        /// <summary>
        /// Carries out additional actions when a cell is clicked.  Specifically, if
        /// a combobox cell is clicked, the cell goes into edit mode immediately.
        /// </summary>
        public void CellClickHandler(object sender, DataGridViewCellEventArgs e)
        {
            var setToEditMode = CheckIfComboBoxShouldSetToEditMode(e.ColumnIndex, e.RowIndex);
            if (!setToEditMode) return;
            var dataGridViewColumn =
                ((DataGridViewColumnWin)Columns[e.ColumnIndex]).DataGridViewColumn;
            ControlsHelper.SafeGui(_gridView, () => BeginEdit(true));
            if (_gridView.EditingControl is DataGridViewComboBoxEditingControl)
            {
                ((DataGridViewComboBoxEditingControl)_gridView.EditingControl).DroppedDown = true;
            }
        }
#pragma warning restore 168
 
        #endregion



        private void ResetBOCollection()
        {
            IBusinessObjectCollection col = this.BusinessObjectCollection;
            IBusinessObject bo = this.SelectedBusinessObject;
            //TODO brett 17 Jan 2011: This should be changed to use a sensible datasource 
            // e.g. an IBindingListView
            if (this.DataSource != null)
            {
                ((DataView)this.DataSource).Table.RejectChanges();
            }
            BusinessObjectCollection = null;
            BusinessObjectCollection = col;
            SelectedBusinessObject = bo;
        }

        /// <summary>
        /// Restore the objects in the grid to their last saved state
        /// </summary>
        public void RejectChanges()
        {
            try
            {
                if (this.BusinessObjectCollection != null)
                {
                    this.BusinessObjectCollection.CancelEdits();
                    ResetBOCollection();
                }
                //TODO brett 17 Jan 2011: This should be changed to use a sensible datasource 
                // e.g. an IBindingListView
                else if (this.DataSource is DataView)
                {
                    ((DataView)this.DataSource).Table.RejectChanges();
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        /// <summary>
        /// Saves the changes made to the data in the grid
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                if (this.BusinessObjectCollection != null)
                {
                    this.BusinessObjectCollection.SaveAll();
                }
                    //TODO brett 17 Jan 2011: This should be changed to use a sensible datasource 
                    // e.g. an IBindingListView
                else if (this.DataSource is DataView)
                {
                    ((DataView)this.DataSource).Table.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        #region Delete

        /// <summary>
        /// Carries out actions when the delete key on the keyboard is pressed
        /// </summary>
        public void DeleteKeyHandler()
        {
            if (CurrentCell == null || CurrentCell.IsInEditMode || SelectedRows.Count != 0) return;
            if (_deleteKeyBehaviour == DeleteKeyBehaviours.DeleteRow && AllowUserToDeleteRows)
            {
                if (MustDelete())
                {
                    ArrayList rowIndexes = new ArrayList();
                    foreach (IDataGridViewCell cell in SelectedCells)
                    {
                        if (!rowIndexes.Contains(cell.RowIndex) && cell.RowIndex != _gridView.NewRowIndex)
                        {
                            rowIndexes.Add(cell.RowIndex);
                        }
                    }
                    rowIndexes.Sort();

                    for (int row = rowIndexes.Count - 1; row >= 0; row--)
                    {
                        Rows.RemoveAt((int)rowIndexes[row]);
                    }
                }
            }
            else if (_deleteKeyBehaviour == DeleteKeyBehaviours.ClearContents)
            {
                foreach (IDataGridViewCell cell in SelectedCells)
                {
                    cell.Value = null;
                }
            }
        }


        /// <summary>
        /// If deletion is to be confirmed, checks deletion with the user before continuing.
        /// This applies only to the default delete behaviour where a full row is selected
        /// by clicking on the column.
        /// </summary>
        private void ConfirmRowDeletion(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                CheckRowEvent(e);
                if (ConfirmDeletion && !CheckUserConfirmsDeletionDelegate())
                {
                    e.Cancel = true;
                    return;
                }
                var rowObjectIDValue = GetRowObjectIDValue(e.Row);
                var businessObject = this.DataSetProvider.Find(rowObjectIDValue);
                if (businessObject == null)
                {
                    _logger.Log("ConfirmRowDeletion - Row Index :" + e.Row.Index + " - No business object found", LogCategory.Debug);
                    e.Cancel = true;
                    return;
                }
                string message;
                if (!businessObject.IsDeletable(out message))
                {
                    e.Cancel = true;
                    throw new BusObjDeleteException(businessObject, message);
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        private Guid GetRowObjectIDValue(DataGridViewRow row)
        {
            var dataGridViewRowWin = new DataGridViewWin.DataGridViewRowWin(row);
            return this.GridBaseManager.GetRowObjectIDValue(dataGridViewRowWin);
        }
        private void CheckRowEvent(DataGridViewRowCancelEventArgs e)
        {
            if (e.Row == null)
            {
                throw new HabaneroApplicationException("The row cannot be deleted since the row event has a null row");
            }
        }

        /// <summary>
        /// Indicates what action should be taken when a selection of
        /// cells is selected and the Delete key is pressed.
        /// This has no correlation to how DataGridView handles the
        /// Delete key when the full row has been selected.
        /// </summary>
        public DeleteKeyBehaviours DeleteKeyBehaviour
        {
            get { return _deleteKeyBehaviour; }
            set { _deleteKeyBehaviour = value; }
        }
        #endregion

        #region ComboBox
        /// <summary>
        /// Gets or sets whether clicking on a ComboBox cell causes the drop-down to
        /// appear immediately.  Set this to false if the user should click twice
        /// (first to select, then to edit), which is the default behaviour.
        /// </summary>
        public bool ComboBoxClickOnce { get; set; }
        /// <summary>
        /// Checks whether this is a comboboxcolumn and whether it should
        /// begin edit immediately (to circumvent the pain of having to click
        /// a cell multiple times to edit the value).  This method is typically
        /// called by the cell click handler.
        /// </summary>
        /// <remarks>
        /// This method was extracted from the handler in order to make testing
        /// possible, since calling BeginEdit at testing time causes an STA thread
        /// error.
        /// </remarks>
        public bool CheckIfComboBoxShouldSetToEditMode(int columnIndex, int rowIndex)
        {
            if (columnIndex > -1 && rowIndex > -1 && !CurrentCell.IsInEditMode && this.ComboBoxClickOnce)
            {
                DataGridViewColumn dataGridViewColumn =
                    ((DataGridViewColumnWin)Columns[columnIndex]).DataGridViewColumn;
                if (dataGridViewColumn is DataGridViewComboBoxColumn)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}