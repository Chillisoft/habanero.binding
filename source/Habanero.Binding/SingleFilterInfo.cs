using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Habanero.Binding
{
    public struct SingleFilterInfo
    {
        internal string PropName;
        internal PropertyDescriptor PropDesc;
        internal Object CompareValue;
        internal FilterOperator OperatorValue;
    }
}
