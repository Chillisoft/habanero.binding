using System;
using System.ComponentModel;
using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Naked;

namespace Habanero.Binding
{
    /// <summary>
    /// An interface for building a View. The view consists of a collection of 
    /// Property Descriptors. <see cref="PropertyDescriptorCollection"/>.
    /// The collection of descriptors can be used to AutoGenerate the columns in a grid.
    /// The collection can also be used to provide the information for binding to a grid, list etc.
    /// 
    /// </summary>
    public interface IViewBuilder
    {
        /// <summary>
        /// Returns the Collection of Property Descriptors that will be used for the View.
        /// </summary>
        /// <returns></returns>
        PropertyDescriptorCollection GetPropertyDescriptors();
    }
    /// <summary>
    /// An <see cref="IViewBuilder"/> based on the Property Descriptors from the <see cref="TypeDescriptor"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultViewBuilder<T> : IViewBuilder
    {
        /// <summary>
        /// Constructs a <see cref="PropertyDescriptorCollection"/> based on the <see cref="TypeDescriptor"/>
        /// for this class.
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptorCollection GetPropertyDescriptors()
        {
            var origDescriptors = TypeDescriptor.GetProperties(typeof(T));
            var propDescriptorsToBind = origDescriptors.Cast<PropertyDescriptor>().Where(HasIgnoreAttribute());
            return new PropertyDescriptorCollection(propDescriptorsToBind.ToArray());
        }

        private static Func<PropertyDescriptor, bool> HasIgnoreAttribute()
        {
            return propDescriptor => !propDescriptor.HasAttribute<TypeDescriptorIgnoreAttribute>();
        }
    }
/*    public static class TypeExtensions
    {
        public static PropertyDescriptorCollection GetFilteredProperties(this Type type)
        {
            var propertyInfos = type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(TypeDescriptorIgnoreAttribute), true).Length == 0);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyDescriptor = new PropertyDescriptor(propertyInfo);  
            }
            return propertyInfos.ToArray();
        }
    }*/
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIDefViewBuilder<T> : IViewBuilder where T : class, IBusinessObject
    {
        private readonly string _uiName;
        /// <summary>
        /// Constructs an <see cref="IViewBuilder"/> based on a uiName.
        /// </summary>
        /// <param name="uiName"></param>
        public UIDefViewBuilder(string uiName)
        {
            _uiName = uiName;
        }
        /// <summary>
        /// Constructs an <see cref="IViewBuilder"/> with a default uiName
        /// </summary>
        public UIDefViewBuilder()
        {
            _uiName = "default";
        }
        /// <summary>
        /// Constructs a <see cref="PropertyDescriptorCollection"/> based on the <see cref="UIDef"/>
        /// defined for this class. Will also create a <see cref="UIDef"/> based on <see cref="Habanero.Naked"/>.
        /// </summary>
        /// <returns></returns>
        public virtual PropertyDescriptorCollection GetPropertyDescriptors()
        {
            var classDef = ClassDef.Get<T>();
            var uiDef = classDef.UIDefCol.Contains(_uiName)
                               ? classDef.UIDefCol[_uiName]
                               : new UIViewCreator().GetDefaultUIDef(classDef);
            var uiGrid = uiDef.UIGrid;
            var propertyDescriptors = uiGrid.Select(GetPropertyDescriptor).ToList();
            propertyDescriptors.Add(new PropertyDescriptorID());
            return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
        }
        //This code could be moved into a factory for creation at a later date.
        //Also with changes made to Habanero could now have one property descriptor using BOProperty Mapper or
        //ReflectionPropertyMapper as appropriate.
        protected static PropertyDescriptor GetPropertyDescriptor(IUIGridColumn column)
        {
            PropertyDescriptor propertyDescriptorPropDef;
            if (column.HasPropDef)
            {
                propertyDescriptorPropDef = new PropertyDescriptorPropDef(column);
            }else
            {
                propertyDescriptorPropDef = new PropertyDescriptorReflectiveProp(column);
            }
            return propertyDescriptorPropDef;
        }
    }
}
