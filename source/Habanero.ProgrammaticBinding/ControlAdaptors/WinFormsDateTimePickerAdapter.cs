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
    public class WinFormsDateTimePickerAdapter : WinFormsControlAdapter, IWinFormsDateTimePickerAdapter
    {
        private readonly DateTimePicker _dtp;

        public WinFormsDateTimePickerAdapter(DateTimePicker control) : base(control)
        {
            _dtp = control;

            _dtp.ValueChanged += RaiseValueChanged;
            _dtp.Enter += RaiseEnter;
        }

        private void RaiseValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null) ValueChanged(sender, e);
        }
        private void RaiseEnter(object sender, EventArgs e)
        {
            if (Enter != null) Enter(sender, e);
        }
        public DateTime Value
        {
            get { return _dtp.Value; }
            set { _dtp.Value = value; }
        }

        public DateTime? ValueOrNull
        {
            get { return _dtp.Value; }
            set
            {
                if (value == null) value = DateTime.MinValue;
                _dtp.Value = value.GetValueOrDefault();
            }
        }

        public string CustomFormat
        {
            get { return _dtp.CustomFormat; }
            set { _dtp.CustomFormat = value; }
        }

        public Habanero.Faces.Base.DateTimePickerFormat Format
        {
            get { return (Habanero.Faces.Base.DateTimePickerFormat) _dtp.Format; }
            set { _dtp.Format = (System.Windows.Forms.DateTimePickerFormat) value; }
        }

        public bool ShowUpDown
        {
            get { return _dtp.ShowUpDown; }
            set { _dtp.ShowUpDown = value; }
        }

        public bool ShowCheckBox
        {
            get { return _dtp.ShowCheckBox; }
            set { _dtp.ShowCheckBox = value; }
        }

        public bool Checked
        {
            get { return _dtp.Checked; }
            set { _dtp.Checked = value; }
        }

        public string NullDisplayValue { get; set; }

        public event EventHandler Enter;
        public event EventHandler ValueChanged;
    }
}