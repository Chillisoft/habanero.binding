using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace HabaneroProgramaticBinding.UI
{
    /// <summary>
    /// Provides a user interface for indicating that a control on a form has an error associated with it
    /// </summary>
    public class ErrorProviderManualBindingWin : ErrorProviderWin
    {
        protected override Control GetControl(IControlHabanero objControl)
        {
            Control control = objControl as Control;
            if (control != null) return control;
            var controlWrapperWin = objControl as ControlWrapperWinForms;
            if (controlWrapperWin != null) return controlWrapperWin.WrappedControl;
            return new Control();
        }
    }
}