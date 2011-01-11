using System;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.Util;

namespace HabaneroProgramaticBinding.UI
{
    /// <summary>
    /// Wraps a TextBox control in order to display and capture a property of the business object.
    /// There are some  limitations with using a TextBox for numbers.  For greater control 
    /// of user input with numbers, you should consider using a NumericUpDown 
    /// control, failing this the appropriate <see cref="ITextBoxMapperStrategy"/> can be used.
    /// </summary>
    public class TextBoxMapperWinForms : ControlMapper
    {
        private readonly TextBox _textBox;
        private string _oldText;

        /// <summary>
        /// Constructor to initialise a new instance of the mapper
        /// </summary>
        /// <param name="tb">The TextBox to map</param>
        /// <param name="propName">The property name</param>
        /// <param name="isReadOnly">Whether this control is read only</param>
        /// <param name="factory">the control factory to be used when creating the controlMapperStrategy</param>
        public TextBoxMapperWinForms(TextBox tb, string propName, bool isReadOnly, IControlFactory factory)
            : base(new TextBoxWrapperWinForms(tb), propName, isReadOnly, factory)
        {
            _textBox = tb;
            _oldText = "";
        }

        /// <summary>
        /// Gets and sets the business object that has a property
        /// being mapped by this mapper.  In other words, this property
        /// does not return the exact business object being shown in the
        /// control, but rather the business object shown in the
        /// form.  Where the business object has been amended or
        /// altered, the <see cref="ControlMapper.UpdateControlValueFromBusinessObject"/> method is automatically called here to 
        /// implement the changes in the control itself.
        /// </summary>
        public override IBusinessObject BusinessObject
        {
            get { return base.BusinessObject; }
            set
            {
                base.BusinessObject = value;
                AddKeyPressEventHandler();
                AddUpdateBoPropOnTextChangedHandler();
            }
        }

        private void AddKeyPressEventHandler()
        {
            TextBoxControl.KeyPress += KeyPressEventHandler;
        }

        private void AddUpdateBoPropOnTextChangedHandler()
        {
            TextBoxControl.TextChanged += UpdateBoPropWithTextFromTextBox;
        }

        private void UpdateBoPropWithTextFromTextBox(object sender, EventArgs e)
        {
            try
            {
                this.ApplyChangesToBusinessObject();
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }

        private TextBox TextBoxControl
        {
            get { return _textBox; }
        }

        private void KeyPressEventHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                if (!IsValidCharacter(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }


        /// <summary>
        /// Indicates if the given character being typed is valid, based on the
        /// text already entered in the textbox.  For instance, if the property
        /// type is an integer, this method will return false for a non-numeric
        /// character (apart from a negative sign).
        /// </summary>
        /// <param name="character">The character being input</param>
        /// <returns>Returns true if valid</returns>
        private bool IsValidCharacter(char character)
        {
            if (base.CurrentBOProp() == null) return true;
            if (TextBoxControl == null) return true;

            if (base.CurrentBOProp().PropertyType.IsInteger())
            {
                if ((character < '0' || character > '9') && character != 8 && character != '-')
                {
                    return false;
                }
                if (character == '-' && TextBoxControl.SelectionStart != 0)
                {
                    return false;
                }
            }
            else if (base.CurrentBOProp().PropertyType.IsDecimal())
            {
                if ((character < '0' || character > '9') && character != '.' && character != 8 && character != '-')
                {
                    return false;
                }
                if (character == '.' && TextBoxControl.Text.Contains("."))
                {
                    return false;
                }
                // In fact the char is valid, but we want the event to get handled in order to prevent double dots
                if (character == '.' && TextBoxControl.SelectionStart == 0)
                {
                    TextBoxControl.Text = "0." + TextBoxControl.Text;
                    TextBoxControl.SelectionStart = 2;
                    TextBoxControl.SelectionLength = 0;
                    return false;
                }
                if (character == '-' && TextBoxControl.SelectionStart != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the interface when the value has been changed in the
        /// object being represented
        /// </summary>
        protected override void InternalUpdateControlValueFromBo()
        {
            TextBoxControl.Text = Convert.ToString(GetPropertyValue());
        }

        /// <summary>
        /// Updates the properties on the represented business object
        /// </summary>
        public override void ApplyChangesToBusinessObject()
        {
            string value = TextBoxControl.Text;

            if (!IsEditable) return;

            try
            {
                SetPropertyValue(value);
            }
            catch (FormatException)
            {
                TextBoxControl.Text = _oldText;
                throw new BusObjectInAnInvalidStateException("The business object '" +
                                                             this._businessObject.ClassDef.ClassName + "' - '" +
                                                             this._businessObject +
                                                             "' could not be updated since the value '" + value +
                                                             "' is not valid for the property '" + PropertyName + "'");
            }
            _oldText = TextBoxControl.Text;
        }
    }

    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class TextBoxWrapperWinForms : ControlWrapperWinForms, ITextBox
    {
        private readonly TextBox _txtBox;
        public TextBoxWrapperWinForms(TextBox control)
            : base(control)
        {
            _txtBox = control;
        }

        public void SelectAll()
        {
            _txtBox.SelectAll();
        }

        public bool Multiline
        {
            get { return _txtBox.Multiline; }
            set { _txtBox.Multiline = value; }
        }

        public bool AcceptsReturn
        {
            get { return _txtBox.AcceptsReturn; }
            set { _txtBox.AcceptsReturn = value; }
        }

        public char PasswordChar
        {
            get { return _txtBox.PasswordChar; }
            set { _txtBox.PasswordChar = value; }
        }

        Habanero.Faces.Base.ScrollBars ITextBox.ScrollBars
        {
            get { return (Habanero.Faces.Base.ScrollBars)_txtBox.ScrollBars; }
            set { _txtBox.ScrollBars = (System.Windows.Forms.ScrollBars)value; }
        }

        Habanero.Faces.Base.HorizontalAlignment ITextBox.TextAlign
        {
            get { return (Habanero.Faces.Base.HorizontalAlignment)_txtBox.TextAlign; }
            set { _txtBox.TextAlign = (System.Windows.Forms.HorizontalAlignment)value; }
        }
    }
}