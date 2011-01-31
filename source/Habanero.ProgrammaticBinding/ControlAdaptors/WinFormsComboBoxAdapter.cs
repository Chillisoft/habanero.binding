using System;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using AutoCompleteMode = Habanero.Faces.Base.AutoCompleteMode;
using AutoCompleteSource = Habanero.Faces.Base.AutoCompleteSource;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsComboBoxAdapter : WinFormsControlAdapter, IWinFormsComboBoxAdapter
    {
        private readonly ComboBox _cmb;
        public WinFormsComboBoxAdapter(ComboBox control)
            : base(control)
        {
            _cmb = control;
            _cmb.SelectedIndexChanged += RaiseSelectedIndexChanged;
            _cmb.SelectedValueChanged += RaiseSelectedValueChanged;
        }

        private void RaiseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e);
        }
        private void RaiseSelectedValueChanged(object sender, EventArgs e)
        {
            if (SelectedValueChanged != null) SelectedValueChanged(sender, e);
        }

        public event EventHandler SelectedValueChanged;

        public ISelectedObjectCollection SelectedItems
        {
            get { return new NullSelectedObjectCollection(); }
        }

        public event EventHandler SelectedIndexChanged;

        public string GetItemText(object item)
        {
            return _cmb.GetItemText(item);
        }

        public string DisplayMember
        {
            get { return _cmb.DisplayMember; }
            set { _cmb.DisplayMember = value; }
        }

        public int SelectedIndex
        {
            get { return _cmb.SelectedIndex; }
            set { _cmb.SelectedIndex = value; }
        }

        public object SelectedValue
        {
            get { return _cmb.SelectedValue; }
            set { _cmb.SelectedValue = value; }
        }

        public string ValueMember
        {
            get { return _cmb.ValueMember; }
            set { _cmb.ValueMember = value; }
        }

        public IListControlObjectCollection Items
        {
            get
            {
                // The two collections get out of synch without a re-instantiation
                return new ComboBoxWin.ComboBoxObjectCollectionWin(_cmb.Items);
            }
        }

        public object SelectedItem
        {
            get { return _cmb.SelectedItem; }
            set { _cmb.SelectedItem = value; }
        }

        public int DropDownWidth
        {
            get { return _cmb.DropDownWidth; }
            set { _cmb.DropDownWidth = value; }
        }

        public object DataSource
        {
            get { return _cmb.DataSource; }
            set { _cmb.DataSource = value; }
        }

        public AutoCompleteMode AutoCompleteMode
        {
            get { return ComboBoxAutoCompleteModeWin.GetAutoCompleteMode(_cmb.AutoCompleteMode); }
            set { _cmb.AutoCompleteMode = ComboBoxAutoCompleteModeWin.GetAutoCompleteMode(value); }
        }

        public AutoCompleteSource AutoCompleteSource
        {
            get { return ComboBoxAutoCompleteSourceWin.GetAutoCompleteSource(_cmb.AutoCompleteSource); }
            set { _cmb.AutoCompleteSource = ComboBoxAutoCompleteSourceWin.GetAutoCompleteSource(value); }
        }
    }
}