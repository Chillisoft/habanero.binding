using System;
using System.ComponentModel;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;

namespace Habanero.Binding
{

    /// <summary>
    /// This is a PropertyDescriptor that wraps the <see cref="IUIGridColumn"/> for cases
    /// where the GridColumn references a <see cref="IPropDef"/>.
    /// </summary>
    public class PropertyDescriptorPropDef : PropertyDescriptor
    {
        private IUIGridColumn GridColumn { get; set; }

        public PropertyDescriptorPropDef(IUIGridColumn gridColumn)
            : base(gridColumn.PropertyName, null)
        {
            if (gridColumn == null) throw new ArgumentNullException("gridColumn");
            if(gridColumn.ClassDef == null) throw new HabaneroArgumentException("gridColumn.ClassDef");
            GridColumn = gridColumn;
        }

        public override string Description
        {
            get { return PropertyName; }
        }

        public override string Name
        {
            get { return PropertyName; }
        }
        
        public override bool CanResetValue(object component)
        {
            return IsComponentDirty(component);
        }

        private bool IsComponentDirty(object component)
        {
            IBusinessObject bo = GetBusinessObject(component);
            var boProp = GetBOProp(bo);
            string message;
            return (!this.IsReadOnly && bo.IsEditable(out message) & boProp.IsDirty);
        }

        public override object GetValue(object component)
        {
            IBusinessObject bo = GetBusinessObject(component);
            BOMapper boMapper = new BOMapper(bo);
            return boMapper.GetPropertyValueToDisplay(PropertyName);
        }

        private string PropertyName
        {
            get { return this.GridColumn.PropertyName; }
        }

        public override void ResetValue(object component)
        {
            IBusinessObject bo = GetBusinessObject(component);
            var boProp = GetBOProp(bo);
            boProp.RestorePropValue();
        }

        private IBOProp GetBOProp(IBusinessObject bo)
        {
            return bo.Props[this.GridColumn.PropertyName];
        }

        public override void SetValue(object component, object value)
        {
            IBusinessObject bo = GetBusinessObject(component);
            BOMapper boMapper = new BOMapper(bo);
            boMapper.SetDisplayPropertyValue(PropertyName, value);
        }

        private IBusinessObject GetBusinessObject(object component)
        {
            CheckComponentType(component);
            return (IBusinessObject)component;
        }
        // ReSharper disable RedundantCast
        //Using this redundant cast to prevent reference check warning.
        private void CheckComponentType(object component)
        {
            if (component == null) throw new ArgumentNullException("component");
            if ((Type) this.ComponentType != component.GetType())

                throw new InvalidCastException("You cannot GetValue since the component is not of type " +
                                               this.GridColumn.ClassDef.ClassName);
        }
        // ReSharper restore RedundantCast

        private object DefaultValue
        {
            get { return this.PropDef != null ? this.PropDef.DefaultValue : null; }
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        /// <param name="component">The component with the property to be examined for persistence. </param>
        public override bool ShouldSerializeValue(object component)
        {
            //From Microsoft Documentation.
            //Notes to Inheritors:  When overridden in a derived class, 
            //this method returns true if the current value of the property is different from 
            //its default value. It looks for a default value by first looking for a DefaultValueAttribute. 
            //If the method finds this attribute, it compares the value of the attribute with the property's current value. 
            //If this method cannot find a DefaultValueAttribute, it looks for a "ShouldSerializeMyProperty" method 
            //that you need to implement. If it is found, ShouldSerializeValue  invokes it. 
            //If this method cannot find a DefaultValueAttribute or a "ShouldSerializeMyProperty" method, 
            //it cannot create optimizations and it returns true. 

            //You should also only serialise if dirty
            //The Habanero framework has a lot more intelligence than comparing to the Default value
            //I think this line should be used but for safety am sticking with IsCurrentValueNonDefault for now.
            //return IsComponentDirty(component);
            return IsCurrentValueNonDefault(component);
        }

        private bool IsCurrentValueNonDefault(object component)
        {
            return this.DefaultValue != null && !this.DefaultValue.Equals(this.GetValue(component));
        }
        private IPropDef PropDef
        {
            get { return this.GridColumn.PropDef; }
        }

        public override Type ComponentType
        {
            get { return this.GridColumn.ClassDef == null ? null: this.GridColumn.ClassDef.ClassType; }
        }

        public override bool IsReadOnly
        {
            get { return !this.GridColumn.Editable; }
        }

        public override Type PropertyType
        {
            get { return this.GridColumn.GetPropertyType(); }
        }

        public override string DisplayName
        {
            get { return GridColumn.GetHeading(); }
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

/*
        private static Type GetPropertyType(IClassDef classDef, string propertyName)
        {
            IPropDef def = classDef.GetPropDef(propertyName, false);
            if (def == null) return classDef.GetPropertyType(propertyName);
            var lookupList = def.LookupList;
            Type propertyType = def.PropertyType;
            if (lookupList != null && !(lookupList is NullLookupList))
            {
                propertyType = typeof(object);
            }
            return propertyType;
        }*/
    }
}