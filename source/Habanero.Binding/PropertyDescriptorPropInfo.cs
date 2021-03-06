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
        protected PropertyInfo PropInfo { get; private set; }

        /// <summary>
        /// Constructs the PropertyDescriptor with the specified <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyDescriptorPropInfo(PropertyInfo propertyInfo)

            : this(propertyInfo, new Attribute[0])
        {
        }

        /// <summary>
        /// Constructs the PropertyDescriptor with the specified <see cref="PropertyInfo"/> and 
        /// its Array of attributes.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attribs"></param>
        public PropertyDescriptorPropInfo(PropertyInfo propertyInfo, Attribute[] attribs)

            : base(propertyInfo.Name, attribs)
        {

            PropInfo = propertyInfo;
        }
 
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
        /// 1) Does the reflective PropInfo have a Setter.
        /// 2) Is a readOnlyAtribute (true) set on PropInfo (i.e. on the Property declared in the class.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return !this.PropInfo.HasSetMethod() || this.HasReadOnlyAttribute;
            }
        }


        private bool HasReadOnlyAttribute
        {
            get
            {
                var readOnlyAttribute = this.PropInfo.GetAttribute<ReadOnlyAttribute>();
                if (readOnlyAttribute == null) return false;
                return readOnlyAttribute.IsReadOnly;
            }
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


        // ReSharper disable RedundantCast
        //Using this redundant cast to prevent reference check warning.
        private void CheckComponentType(object component)
        {
            if (component == null) throw new ArgumentNullException("component");
            if (this.ComponentType != component.GetType())
            {
                throw new InvalidCastException("You cannot GetValue since the component is not of type " +
                                               this.ComponentType.Name);
            }
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
            get { return this.PropInfo.ReflectedType; }
        }

        /// <summary>
        /// The property type of the <see cref="PropertyInfo"/> wrapped by this descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the property.
        /// </returns>
        public override Type PropertyType
        {
            get { return this.PropInfo.PropertyType; }
        }

        //TODO brett 21 Jan 2011: Should use DisplayNameAttribute if it has one
        /// <summary>
        /// Gets the name that can be displayed in a window, such as a Properties window.
        /// </summary>
        /// <returns>
        /// The name to display for the member.
        /// </returns>
        public override string DisplayName
        {
            get { return StringUtilities.DelimitPascalCase(this.Name, " "); }
        }
    }
    
    /// <summary>
    /// Provides extension methods for PropertyDescriptor.
    /// </summary>
    public static class AttributeExt
    {
        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyDescriptor prop) where T : Attribute
        {
            foreach (Attribute att in prop.Attributes)
            {
                var tAtt = att as T;
                if (tAtt != null) return tAtt;
            }
            return null;
        }
        /// <summary>
        /// Does the wrapped <see cref="PropertyInfo"/> have an attribute of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyDescriptor prop) where T : Attribute
        {
            T attribute = prop.GetAttribute<T>();
            return attribute != null;
        }
        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyInfo prop) where T : Attribute
        {
            foreach (Attribute att in prop.GetCustomAttributes(typeof(T), true))
            {
                var tAtt = att as T;
                if (tAtt != null) return tAtt;
            }
            return null;
        }
        /// <summary>
        /// Does the wrapped <see cref="PropertyInfo"/> have an attribute of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo prop) where T : Attribute
        {
            T attribute = prop.GetAttribute<T>();
            return attribute != null;
        }
        /// <summary>
        /// Returns true if the property has a public Set Method and false otherwise
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool HasSetMethod(this PropertyInfo prop)
        {
            return prop.GetSetMethod(false) != null;
        }
    }
}