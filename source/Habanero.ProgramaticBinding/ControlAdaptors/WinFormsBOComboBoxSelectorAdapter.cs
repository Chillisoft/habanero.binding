using System;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.Faces.Base;

// ReSharper disable SuggestUseVarKeywordEvident
// ReSharper disable MemberCanBePrivate.Global
namespace Habanero.ProgramaticBinding.ControlAdaptors
{
    public class WinFormsBOComboBoxSelectorAdapter : WinFormsComboBoxAdapter, IBOComboBoxSelector
    {
        private readonly ComboBoxCollectionSelector _manager;
        public WinFormsBOComboBoxSelectorAdapter(ComboBox control) : base(control)
        {
            _manager = new ComboBoxCollectionSelector(this, GlobalUIRegistry.ControlFactory) {IncludeBlankItem = true};
            _manager.BusinessObjectSelected += delegate { FireBusinessObjectSelected(); };
        }

        private void FireBusinessObjectSelected()
        {
            if (this.BusinessObjectSelected != null)
            {
                this.BusinessObjectSelected(this, new BOEventArgs(this.SelectedBusinessObject));
            }
        }

        /// <summary>
        /// Gets and Sets the business object collection displayed in the grid.  This
        /// collection must be pre-loaded using the collection's Load() command or from the
        /// <see cref="IBusinessObjectLoader"/>.
        /// The default UI definition will be used, that is a 'ui' element 
        /// without a 'name' attribute.
        /// </summary>
        public IBusinessObjectCollection BusinessObjectCollection
        {
            get { return _manager.Collection; }
            set { _manager.SetCollection(value); }
        }

        /// <summary>
        /// Gets and sets the currently selected business object in the grid
        /// </summary>
        public IBusinessObject SelectedBusinessObject
        {
            get { return _manager.SelectedBusinessObject; }
            set { _manager.SelectedBusinessObject = value; }
        }

        /// <summary>
        /// Gets and sets whether the Control is enabled or not
        /// </summary>
        public bool ControlEnabled
        {
            get { return this.ComboBox.Enabled; }
            set { this.ComboBox.Enabled = value; }
        }

        /// <summary>
        /// Event Occurs when a business object is selected
        /// </summary>
        public event EventHandler<BOEventArgs> BusinessObjectSelected;

        /// <summary>
        /// Clears the business object collection and the rows in the data table
        /// </summary>
        public void Clear()
        {
            _manager.Clear();
        }

        ///<summary>
        /// Returns the Underlying ComboBoxControl that is used by this selector
        ///</summary>
        public IComboBox ComboBox
        {
            get { return this; }
        }

        /// <summary>Gets the number of items displayed in the <see cref="IBOColSelector"></see>.</summary>
        /// <returns>The number of items in the <see cref="IBOColSelector"></see>.</returns>
        public int NoOfItems
        {
            get { return _manager.NoOfItems; }
        }

        /// <summary>
        /// Returns the business object at the specified row number
        /// </summary>
        /// <param name="row">The row number in question</param>
        /// <returns>Returns the busines object at that row, or null
        /// if none is found</returns>
        public IBusinessObject GetBusinessObjectAtRow(int row)
        {
            return _manager.GetBusinessObjectAtRow(row);
        }

        /// <summary>
        /// Gets and sets whether this selector autoselects the first item or not when a new collection is set.
        /// </summary>
        public bool AutoSelectFirstItem
        {
            get { return _manager.AutoSelectFirstItem; }
            set { _manager.AutoSelectFirstItem = value; }
        }

        ///<summary>
        /// Gets or sets whether the current <see cref="IBOColSelectorControl.SelectedBusinessObject">SelectedBusinessObject</see> should be preserved in the selector when the 
        /// <see cref="IBOColSelectorControl.BusinessObjectCollection">BusinessObjectCollection</see> 
        /// is changed to a new collection which contains the current <see cref="IBOColSelectorControl.SelectedBusinessObject">SelectedBusinessObject</see>.
        /// If the <see cref="IBOColSelectorControl.SelectedBusinessObject">SelectedBusinessObject</see> doesn't exist in the new collection then the
        /// <see cref="IBOColSelectorControl.SelectedBusinessObject">SelectedBusinessObject</see> is set to null.
        /// If the current <see cref="IBOColSelectorControl.SelectedBusinessObject">SelectedBusinessObject</see> is null then this will also be preserved.
        /// This overrides the <see cref="IBOColSelectorControl.AutoSelectFirstItem">AutoSelectFirstItem</see> property.
        ///</summary>
        public bool PreserveSelectedItem 
        {
            get { return _manager.PreserveSelectedItem; }
            set { _manager.PreserveSelectedItem = value; }
        }
    }

}
