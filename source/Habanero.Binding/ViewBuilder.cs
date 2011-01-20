using System;
using System.ComponentModel;
using System.Linq;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Naked;

namespace Habanero.Binding
{

    public interface IViewBuilder
    {
        PropertyDescriptorCollection GetGridView();
    }

    public class ViewBuilder<T> : IViewBuilder where T : class, IBusinessObject
    {
        private readonly string _uiName;

        public ViewBuilder(string uiName)
        {
            _uiName = uiName;
        }

        public ViewBuilder()
        {
            _uiName = "default";
        }

        public virtual PropertyDescriptorCollection GetGridView()
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
                propertyDescriptorPropDef =  new PropertyDescriptorPropInfo(column);
            }
            return propertyDescriptorPropDef;
        }
    }
}
