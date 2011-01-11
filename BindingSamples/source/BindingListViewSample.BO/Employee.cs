// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// ------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Binding;
using Habanero.BO;

namespace BindingListViewSample.BO
{
    public partial class Employee : INotifyPropertyChanged, IDataErrorInfo
    {

        public Employee()
        {
            this.PropertyUpdated += (sender, args) => NotifyPropertyChanged(args.Prop.PropertyName);
        }

        protected Employee(ConstructForFakes constructForFakes)
            : base(constructForFakes)
        {
        }

        //--------------The INotifyPropertyChanged only has this method --------------
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        //---------------The IDataErrorInfo has the this and the Error Method
        public string this[string columnName]
        {
            get
            {
                var boProp = GetBoProp(columnName);
                return boProp != null ? boProp.InvalidReason : "";
            }
        }

        public string Error
        {
            get { return this.Status.IsValidMessage; }
        }

        private IBOProp GetBoProp(string columnName)
        {
            return base._boPropCol.Contains(columnName) ? base._boPropCol[columnName] : null;
        }


        //----------------If you do not specifically bind a list e.g. ComboBox to a property then it will
        // use the ToString of the object.
        public override string ToString()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }
    }
}