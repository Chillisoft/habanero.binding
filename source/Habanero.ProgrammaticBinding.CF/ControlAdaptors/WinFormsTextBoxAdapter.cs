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
    public class WinFormsTextBoxAdapter : WinFormsControlAdapter, IWinFormsTextBoxAdapter
    {
        private readonly TextBox _txtBox;
        public WinFormsTextBoxAdapter(TextBox control)
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