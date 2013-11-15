using System;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsListViewAdapter : WinFormsControlAdapter, IListView
    {
        private readonly ListView _lstBox;
/*        private readonly ListBoxWin.ListBoxSelectedObjectCollectionWin _selectedObjectCollection;
        private readonly ListBoxWin.ListBoxObjectCollectionWin _objectCollection;*/
        public WinFormsListViewAdapter(ListView control)
            : base(control)
        {
            _lstBox = control;
            _lstBox.SelectedIndexChanged += RaiseSelectedIndexChanged;
/*            _objectCollection = new ListBoxWin.ListBoxObjectCollectionWin(_lstBox.Items);
            _selectedObjectCollection = new ListBoxWin.ListBoxSelectedObjectCollectionWin(_lstBox.SelectedItems);
            
            _lstBox.SelectedValueChanged += RaiseSelectedValueChanged;*/
        }
        private void RaiseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e);
        }
#pragma warning disable
        public event EventHandler SelectedValueChanged;
#pragma warning enable
        /*
        public event EventHandler SelectedValueChanged;
        public event EventHandler SelectedIndexChanged;
        
        private void RaiseSelectedValueChanged(object sender, EventArgs e)
        {
            if (SelectedValueChanged != null) SelectedValueChanged(sender, e);
        }
        public string GetItemText(object item)
        {
            return _lstBox.GetItemText(item);
        }

        public object DataSource
        {
            get { return _lstBox.DataSource; }
            set { _lstBox.DataSource = value; }
        }

        public string DisplayMember
        {
            get { return _lstBox.DisplayMember; }
            set { _lstBox.DisplayMember = value; }
        }

        public int SelectedIndex
        {
            get { return _lstBox.SelectedIndex; }
            set { _lstBox.SelectedIndex = value; }
        }

        public object SelectedValue
        {
            get { return _lstBox.SelectedValue; }
            set { _lstBox.SelectedValue = value; }
        }

        public string ValueMember
        {
            get { return _lstBox.ValueMember; }
            set { _lstBox.ValueMember = value; }
        }

        public void ClearSelected()
        {
            _lstBox.ClearSelected();
        }

        public void SetSelected(int index, bool value)
        {
            _lstBox.ClearSelected();
        }

        public int FindString(string strValue)
        {
            return _lstBox.FindString(strValue);
        }

        public int FindString(string strValue, int intStartIndex)
        {
            return _lstBox.FindString(strValue, intStartIndex);
        }*/
/*

        /// <summary>
        /// Gets the items of the ListBox
        /// </summary>
        public IListControlObjectCollection Items
        {
            get { return _objectCollection; }
        }

        public object SelectedItem
        {
            get { return _lstBox.SelectedItem; }
            set { _lstBox.SelectedItem = value; }
        }


        /// <summary>
        /// Gets a collection containing the currently selected items in the ListBox
        /// </summary>
        public ISelectedObjectCollection SelectedItems
        {
            get { return _selectedObjectCollection; }
        }

        /// <summary>
        /// Gets or sets the method in which items are selected in the ListBox
        /// </summary>
        public ListBoxSelectionMode SelectionMode
        {
            get { return (ListBoxSelectionMode)Enum.Parse(typeof(ListBoxSelectionMode), _lstBox.SelectionMode.ToString()); }
            set { _lstBox.SelectionMode = (SelectionMode)Enum.Parse(typeof(SelectionMode), value.ToString()); }
        }


        public bool Sorted
        {
            get { return _lstBox.Sorted; }
            set { _lstBox.Sorted = value; }
        }*/


        public ISelectedObjectCollection SelectedItems
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler SelectedIndexChanged;
        public string GetItemText(object item)
        {
            return item == null ? "" : item.ToString();
        }

        public object DataSource
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string DisplayMember
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int SelectedIndex
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public object SelectedValue
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object SelectedItem
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string ValueMember
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IListControlObjectCollection Items
        {
            get { throw new NotImplementedException(); }
        }
    }
}