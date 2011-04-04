using System;
using System.Drawing;
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
    public class WinFormsCheckBoxAdapter : WinFormsControlAdapter, IWinFormsCheckBoxAdapter
    {
        private readonly CheckBox _chkBox;
        public WinFormsCheckBoxAdapter(CheckBox control)
            : base(control)
        {
            _chkBox = control;
            _chkBox.CheckedChanged += RaiseCheckChanged;
        }

        private void RaiseCheckChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null) CheckedChanged(sender, e);
        }

        public bool Checked
        {
            get { return _chkBox.Checked; }
            set { _chkBox.Checked = value; }
        }

        public ContentAlignment CheckAlign
        {
            get { return _chkBox.CheckAlign; }
            set { _chkBox.CheckAlign = value; }
        }

        public event EventHandler CheckedChanged;
    }
}