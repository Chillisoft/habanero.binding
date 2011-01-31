using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.Faces.Win.Grid;
using AutoCompleteMode = Habanero.Faces.Base.AutoCompleteMode;
using AutoCompleteSource = Habanero.Faces.Base.AutoCompleteSource;
using DataGridViewSelectionMode = Habanero.Faces.Base.DataGridViewSelectionMode;

namespace Habanero.ProgramaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a Control Adapter for any <see cref="DataGridView"/> control.
    ///  It wraps the <see cref="DataGridView"/> control behind a standard interface.
    /// This allows Faces to interact with it as if it is a Habanero Control.
    /// </summary>
    public class WinFormsDataGridViewAdapter : WinFormsControlAdapter, IWinFormsDataGridViewAdapter
    {
        private readonly DataGridView _grid;
        private readonly DataGridViewManager _manager;

        private readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger(
                "Habanero.ProgramaticBinding.ControlAdaptors.WinFormsDataGridViewAdapter");
        public event EventHandler SelectionChanged;
        public WinFormsDataGridViewAdapter(DataGridView gridView)
            : base(gridView)
        {
            _grid = gridView;
            _manager = new DataGridViewManager(this);
            _grid.DataError +=(sender, args) => 
                        {
                            //do nothing  necessary because of strange behaviour in windows grid
                            _logger.Log("DataError from Grid " + args.Exception.Message);
                        };
            _grid.SelectionChanged += FireBusinessObjectSelected;
        }

        private void FireBusinessObjectSelected(object sender, EventArgs eventArgs)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(sender, eventArgs);
            }
        }

        public bool BeginEdit(bool selectAll)
        {
            return _grid.BeginEdit(selectAll);
        }

        public bool CancelEdit()
        {
            return _grid.CancelEdit();
        }

        public void Sort(IDataGridViewColumn dataGridViewColumn, ListSortDirection direction)
        {
            Sort(dataGridViewColumn.Name, direction == ListSortDirection.Ascending);
        }

        public void Sort(string columnName, bool ascending)
        {
            _manager.SetSortColumn(columnName, ascending);
        }

        public bool AllowUserToAddRows
        {
            get { return _grid.AllowUserToAddRows; }
            set { _grid.AllowUserToAddRows = value; }
        }

        public bool AllowUserToDeleteRows
        {
            get { return _grid.AllowUserToDeleteRows; }
            set { _grid.AllowUserToDeleteRows = value; }
        }

        public bool AutoGenerateColumns
        {
            get { return _grid.AutoGenerateColumns; }
            set { _grid.AutoGenerateColumns = value; }
        }

        public IDataGridViewColumnCollection Columns
        {
            get { return new DataGridViewWin.DataGridViewColumnCollectionWin(_grid.Columns); }
        }

        public IDataGridViewCell CurrentCell
        {
            get { return _grid.CurrentCell == null ? null : new DataGridViewCellWin(_grid.CurrentCell); }
            set { _grid.CurrentCell = value == null ? null : ((DataGridViewCellWin) value).DataGridViewCell; }
        }

        public IDataGridViewRow CurrentRow
        {
            get { return _grid.CurrentRow == null ? null : new DataGridViewWin.DataGridViewRowWin(_grid.CurrentRow); }
        }

        public object DataSource
        {
            get { return _grid.DataSource; }
            set { _grid.DataSource = value; }
        }

        public int FirstDisplayedScrollingRowIndex
        {
            get { return _grid.FirstDisplayedScrollingRowIndex; }
            set { _grid.FirstDisplayedScrollingRowIndex = value; }
        }

        public bool ReadOnly
        {
            get { return _grid.ReadOnly; }
            set { _grid.ReadOnly = value; }
        }

        #region Pagination Support

        /// <summary>
        /// Gets or sets the current page of the grid when the grid implements pagination. implements IDataGridView
        /// These are not implemented in the standard windows form DataGridView
        /// </summary>
        int IDataGridView.CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page used when the grid implements pagination. implements IDataGridView
        /// These are not implemented in the standard windows form DataGridView
        /// </summary>
        int IDataGridView.ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total pages. implements IDataGridView
        /// These are not implemented in the standard windows form DataGridView
        /// </summary>
        /// <value></value>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        /// <value></value>
        public int TotalItems { get; set; }

        /// <summary>
        /// When pagination is used, changes the current page to the one containing
        /// the given row number.  implements IDataGridView
        /// </summary>
        /// <param name="rowNum">The row that you wish to show the page of.  For example, if your grid has
        /// 30 rows and is set to 20 rows per page, calling ChangeToPageOfRow with an argument
        /// of 25 will set the page to page 2 since row 25 is on page 2.</param>
        public void ChangeToPageOfRow(int rowNum)
        {
            //do nothing, everything on the same page
        }

        #endregion

        public int RowCount
        {
            get { return _grid.RowCount; }
            set { _grid.RowCount = value; }
        }

        public bool RowHeadersVisible
        {
            get { return _grid.RowHeadersVisible; }
            set { _grid.RowHeadersVisible = value; }
        }

        public int RowHeadersWidth
        {
            get { return _grid.RowHeadersWidth; }
            set { _grid.RowHeadersWidth = value; }
        }

        public IDataGridViewRowCollection Rows
        {
            get { return new DataGridViewWin.DataGridViewRowCollectionWin(_grid.Rows); }
        }

        public IDataGridViewColumn SortedColumn
        {
            get { return _grid.SortedColumn == null ? null : new DataGridViewColumnWin(_grid.SortedColumn); }
        }

        public IDataGridViewSelectedRowCollection SelectedRows
        {
            get { return new DataGridViewWin.DataGridViewSelectedRowCollectionWin(_grid.SelectedRows); }
        }

        public IDataGridViewSelectedCellCollection SelectedCells
        {
            get { return new DataGridViewWin.DataGridViewSelectedCellCollectionWin(_grid.SelectedCells); }
        }

        public DataGridViewSelectionMode SelectionMode
        {
            get { return DataGridViewSelectionModeWin.GetDataGridViewSelectionMode(_grid.SelectionMode); }
            set { _grid.SelectionMode = DataGridViewSelectionModeWin.GetDataGridViewSelectionMode(value); }
        }

        /// <summary>
        /// Provides an indexer to get or set the cell located at the intersection of the column and row with the specified indexes.
        /// </summary>
        /// <param name="columnIndex">The index of the column containing the cell.</param>
        /// <param name="rowIndex">The index of the row containing the cell</param>
        /// <returns>The DataGridViewCell at the specified location</returns>
        public IDataGridViewCell this[int columnIndex, int rowIndex]
        {
            get { return new DataGridViewCellWin(_grid[columnIndex, rowIndex]); }
            set { _grid[columnIndex, rowIndex] = value == null ? null : ((DataGridViewCellWin) value).DataGridViewCell; }
        }

    }
}