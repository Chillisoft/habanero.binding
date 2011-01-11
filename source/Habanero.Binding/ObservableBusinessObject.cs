using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Binding
{
    ///<summary>
    /// This is simply a <see cref="BusinessObject{T}"/> that implments INotifyPropertyChanged and IDataErrorInfo.
    /// This Class can therefore be used as a base class for your BusinessObjects instead of <see cref="BusinessObject"/>
    /// or <see cref="BusinessObject{T}"/>.
    /// This class does all the mappings between BusinessObject as an EventSource and the required Interfaces
    /// for Binding in WPF and WinForms.
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public abstract class ObservableBusinessObject<T> : BusinessObject<T>, INotifyPropertyChanged, IDataErrorInfo
    {
        ///<summary>
        /// The PropertyChanged Event from <see cref="INotifyPropertyChanged"/>.<see cref="INotifyPropertyChanged.PropertyChanged"/>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected ObservableBusinessObject(IClassDef def) : base(def)
        {
            RegisterForPropertyUpdatedEvent();
        }

        protected ObservableBusinessObject()
        {
            RegisterForPropertyUpdatedEvent();
        }

        protected ObservableBusinessObject(ConstructForFakes constructForFakes) : base(constructForFakes)
        {
            //This is used by Rhino Mocks or a similar Mocking library and should therefore do nothing.
        }

        private void RegisterForPropertyUpdatedEvent()
        {
            this.PropertyUpdated += (sender, args) => NotifyPropertyChanged(args.Prop.PropertyName);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("").
        /// </returns>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        public string this[string columnName]
        {
            get
            {
                var boProp = GetBoProp(columnName);
                if (boProp == null) return "";
                boProp.Validate();
                return boProp.InvalidReason;
            }
        }

        private IBOProp GetBoProp(string columnName)
        {
            return base._boPropCol.Contains(columnName) ? base._boPropCol[columnName] : null;
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error
        {
            get { return this.Status.IsValidMessage; }
        }
    }
}