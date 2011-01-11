using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace HabaneroProgramaticBinding.UI
{
    public class ControlFactoryManualBindingWin: ControlFactoryWin
    {
        public override IErrorProvider CreateErrorProvider()
        {
            return new ErrorProviderManualBindingWin();
        }
    }
}
