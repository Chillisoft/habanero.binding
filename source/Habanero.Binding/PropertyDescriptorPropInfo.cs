using System;
using System.ComponentModel;
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.Binding
{
    /// <summary>
    /// This was copied from the article http://www.sql.ru/forum/actualthread.aspx?tid=579972
    /// </summary>
    public class PropertyDescriptorPropInfo : PropertyDescriptor
    {
        //Created and kept as a member variable for performance improvements.
        protected PropertyInfo PropInfo { get; set; }

        public PropertyDescriptorPropInfo(IUIGridColumn gridColumn)
            : base(gridColumn.PropertyName, null)
        {
            if (gridColumn == null) throw new ArgumentNullException("gridColumn");
            if(gridColumn.ClassDef == null) throw new HabaneroArgumentException("gridColumn.ClassDef");
            GridColumn = gridColumn;
            var propertyName = gridColumn.PropertyName.Trim('-');
            PropInfo = ReflectionUtilities.GetPropertyInfo(gridColumn.ClassDef.ClassType, propertyName);
            CheckPropInfo(gridColumn);
        }

        protected virtual void CheckPropInfo(IUIGridColumn gridColumn)
        {
            if(PropInfo.IsNull())
            {
                var errMessage 
                    = string.Format("The GridColumn for reflective property '{0}' " 
                                    + "is invalid since this reflective property does not exist on the class of type '{1}' "
                                    , gridColumn.PropertyName, gridColumn.ClassDef.ClassName);
                throw new HabaneroArgumentException("gridColumn", errMessage);
            }
        }

        private IUIGridColumn GridColumn { get; set; }

/*        public PropertyDescriptorPropInfo(PropertyInfo prop, Attribute[] attribs)

            : base(prop.Name, attribs)
        {

            _propInfo = prop;

        }*/
 
        private object DefaultValue
        {
            get
            {
                if (PropInfo.IsNull()) return null;
                if (PropInfo.IsDefined(typeof(DefaultValueAttribute), false))
                {
                    return ((DefaultValueAttribute)PropInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false)[0]).Value;
                }
                return null;
            }
        }
        /// <summary>
        /// Indicates whether the PropDescripor is ReadOnly.
        /// This takes into account the following.
        /// 1) Has the GridColumn Been set to ReadOnly Explicitely e.g. via ClassDef.xml.
        /// 2) Does the reflective PropInfo have a Setter.
        /// 3) Is a readOnlyAtribute (true) set on PropInfo.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return !this.GridColumn.Editable || HasReadOnlyAttribute;
            }
        }

        private bool HasReadOnlyAttribute
        {
            get { return this.Attributes.Contains(new System.ComponentModel.ReadOnlyAttribute(true)); }
        }

        public override object GetValue(object component)
        {
            
            CheckComponentType(component);
            if (this.ComponentType == component.GetType())
            {
                return PropInfo.IsNull() ? null : PropInfo.GetValue(component, null);
            }

            PropertyInfo[] propInfos = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //This code is copied from http://www.sql.ru/forum/actualthread.aspx?tid=579972 
            // and i have no idea what the following code is trying to achieve.
            //It looks like it might be trying to deal with the prop info being
            // on a customer e.g. when a PropInfo for CustomerName on Customer when the component is Order.
            foreach (PropertyInfo mypropinfo in propInfos)
            {
                object obj = mypropinfo.GetValue(component, null);

                PropertyInfo[] nestedpropsInfo = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo mynestedpropinfo in nestedpropsInfo)
                {
                    if (PropInfo.Name == mynestedpropinfo.Name) return mynestedpropinfo.GetValue(obj, null);
                }
            }
            return null;
        }

        public override string Name
        {
            get { return this.PropertyName; }
        }

        private string PropertyName
        {
            get { return this.GridColumn.PropertyName; }
        }

        // ReSharper disable RedundantCast
        //Using this redundant cast to prevent reference check warning.
        private void CheckComponentType(object component)
        {
            if (component == null) throw new ArgumentNullException("component");
            if ((Type)this.ComponentType != component.GetType())

                throw new InvalidCastException("You cannot GetValue since the component is not of type " +
                                               this.GridColumn.ClassDef.ClassName);
        }
        // ReSharper restore RedundantCast
        public override bool CanResetValue(object component)
        {
            return IsCurrentValueDifferentFromDefault(component);
        }

        public override void ResetValue(object component)
        {
            this.SetValue(component, this.DefaultValue);
        }

        public override void SetValue(object component, object value)
        {
            if (PropInfo.IsNull()) return;
            CheckComponentType(component);
            if (PropInfo.DeclaringType == component.GetType())
            {
                
                PropInfo.SetValue(component, value, null);
            }
            else
            {
                PropertyInfo[] propsInfo = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo mypropinfo in propsInfo)
                {
                    object obj = mypropinfo.GetValue(component, null);
                    PropertyInfo[] nestedpropsInfo = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo mynestedpropinfo in nestedpropsInfo)
                    {
                        if (PropInfo.Name == mynestedpropinfo.Name) mynestedpropinfo.SetValue(obj, value, null);
                    }
                }
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return IsCurrentValueDifferentFromDefault(component);
        }

        private bool IsCurrentValueDifferentFromDefault(object component)
        {
            return !this.IsReadOnly & (this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component)));
        }

        public override Type ComponentType
        {
            get { return this.GridColumn.ClassDef == null ? PropInfo.DeclaringType : this.GridColumn.ClassDef.ClassType; }
        }

        public override Type PropertyType
        {
            get { return this.GridColumn.GetPropertyType(); }
        }

        /// <summary>
        /// If this Property is associated with a LookupList then 
        /// this will return the Lookup list.
        /// </summary>
        public virtual ILookupList LookupList
        {
            get { return this.GridColumn.LookupList; }
        }
        /// <summary>
        /// Returns the width defined on the UIProp.
        /// </summary>
        public int Width
        {
            get { return this.GridColumn.Width; }
        }
        /// <summary>
        /// The alignment from the UIProp
        /// </summary>
        public PropAlignment Alignment
        {
            get { return this.GridColumn.Alignment; }
        }

        public override string DisplayName
        {
            get { return GridColumn.GetHeading(); }
        }
    }

}