// ------------------------------------------------------------------------------
// This class was auto-generated for use with the Habanero Enterprise Framework.
// NB Custom code should be placed in the provided stub class.
// Please do not modify this class directly!
// ------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using Habanero.BO;

namespace BindingListViewSample.BO
{
    public partial class Employee : BusinessObject
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual System.Guid? EmployeeID
        {
            get
            {
                return ((System.Guid?)(base.GetPropertyValue("EmployeeID")));
            }
            set
            {
                base.SetPropertyValue("EmployeeID", value);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.String FirstName
        {
            get
            {
                return ((System.String)(base.GetPropertyValue("FirstName")));
            }
            set
            {
                base.SetPropertyValue("FirstName", value);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.String LastName
        {
            get
            {
                return ((System.String)(base.GetPropertyValue("LastName")));
            }
            set
            {
                base.SetPropertyValue("LastName", value);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.Decimal Salary
        {
            get
            {
                return ((System.Decimal)(base.GetPropertyValue("Salary")));
            }
            set
            {
                base.SetPropertyValue("Salary", value);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual System.DateTime BirthDate
        {
            get
            {
                return ((System.DateTime)(base.GetPropertyValue("BirthDate")));
            }
            set
            {
                base.SetPropertyValue("BirthDate", value);
            }
        }
        
        #region Relationships
        
        #endregion
        
    }
}
