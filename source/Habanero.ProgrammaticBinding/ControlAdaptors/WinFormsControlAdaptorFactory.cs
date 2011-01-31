using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace Habanero.ProgramaticBinding.ControlAdaptors
{
    public interface IControlAdaptorFactory
    {
        IControlHabanero GetHabaneroControl<TControlType>(TControlType control) where TControlType : Control;
        IControlHabanero GetHabaneroControl(Type controlType, Control control);
    }

    /// <summary>
    /// The Adaptor Factory is a Factory Class for
    /// Creating the Appropriate <see cref="IWinFormsControlAdapter"/>
    /// for the given environment e.g. A WinForms.ComboBox will be adapted 
    /// with the relevant <see cref="IWinFormsComboBoxAdapter"/>
    /// </summary>
    public class WinFormsControlAdaptorFactory : IControlAdaptorFactory
    {
        //TODO brett 16 Dec 2010: This adapter registry needs to be moved to a binding context or something.
        private Dictionary<Type, Type> _adapaterRegistry = new Dictionary<Type, Type>();


        public WinFormsControlAdaptorFactory()
        {
            _adapaterRegistry.Add(typeof(ListBox), typeof(WinFormsListBoxAdapter));
            _adapaterRegistry.Add(typeof(CheckBox), typeof(WinFormsCheckBoxAdapter));
            _adapaterRegistry.Add(typeof(TextBox), typeof(WinFormsTextBoxAdapter));
            _adapaterRegistry.Add(typeof(DateTimePicker), typeof(WinFormsDateTimePickerAdapter));
            _adapaterRegistry.Add(typeof(ComboBox), typeof(WinFormsComboBoxAdapter));
            _adapaterRegistry.Add(typeof(NumericUpDown), typeof(WinFormsNumericUpDownAdapter));
            _adapaterRegistry.Add(typeof(DataGridView), typeof(WinFormsDataGridViewAdapter));
        }

        public IControlHabanero GetHabaneroControl<TControlType>(TControlType control) where TControlType : Control
        {
            return GetHabaneroControl(typeof(TControlType), control);
        }

        public IControlHabanero GetHabaneroControl(Type controlType, Control control)
        {
            //TODO brett 16 Dec 2010: Should raise error if control type not registered
            if (control == null) throw new ArgumentNullException("control");
            IControlHabanero habaneroControl = null;
            if (typeof(IControlHabanero).IsInstanceOfType(control))
            {
                habaneroControl = control as IControlHabanero;
            }
            else
            {
                if (_adapaterRegistry.ContainsKey(controlType))
                {
                    var adapterType = _adapaterRegistry[controlType];
                    habaneroControl = Activator.CreateInstance(adapterType, control) as IControlHabanero;
                }
            }
            return habaneroControl;
        }
    }
}