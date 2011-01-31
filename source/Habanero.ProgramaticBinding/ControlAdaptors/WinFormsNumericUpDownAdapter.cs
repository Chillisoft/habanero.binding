using System;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using HorizontalAlignment = Habanero.Faces.Base.HorizontalAlignment;

namespace Habanero.ProgramaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsNumericUpDownAdapter : WinFormsControlAdapter, IWinFormsNumericUpDownAdapter
    {
        private readonly NumericUpDown _numericUpDown;
        public WinFormsNumericUpDownAdapter(NumericUpDown control)
            : base(control)
        {
            _numericUpDown = control;
        }

        public event EventHandler Enter;
        public event EventHandler ValueChanged;

        public int DecimalPlaces
        {
            get { return _numericUpDown.DecimalPlaces; }
            set { _numericUpDown.DecimalPlaces = value; }
        }

        public decimal Minimum
        {
            get { return _numericUpDown.Minimum; }
            set { _numericUpDown.Minimum = value; }
        }

        public decimal Maximum
        {
            get { return _numericUpDown.Maximum; }
            set { _numericUpDown.Maximum = value; }
        }

        public decimal Value
        {
            get { return _numericUpDown.Value; }
            set { _numericUpDown.Value = value; }
        }

        public void Select(int i, int length)
        {
            _numericUpDown.Select(i, length);
        }

        public HorizontalAlignment TextAlign
        {
            get { return (HorizontalAlignment)_numericUpDown.TextAlign; }
            set { _numericUpDown.TextAlign = (System.Windows.Forms.HorizontalAlignment)value; }
        }
    }
}