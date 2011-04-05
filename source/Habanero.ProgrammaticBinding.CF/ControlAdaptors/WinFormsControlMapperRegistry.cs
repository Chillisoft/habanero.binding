using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.CF;

namespace Habanero.ProgrammaticBinding.CF.ControlAdaptors
{
    public interface IControlMapperRegistry
    {
        Type GetMapperType<T>(Control control) where T : class, IBusinessObject;
    }

    /// <summary>
    /// The Adaptor Factory is a Factory Class for
    /// Creating the Appropriate <see cref="IWinFormsComboBoxAdapter"/>
    /// for the given environment e.g. A WinForms.ComboBox will be adapted 
    /// with the relevant <see cref="IWinFormsControlAdapter"/>
    /// </summary>
    public class WinFormsControlMapperRegistry : IControlMapperRegistry
    {
        //TODO brett 16 Dec 2010: This adapter registry needs to be moved to a binding context or something.
        private readonly Dictionary<Type, Type> _mapperRegistry = new Dictionary<Type, Type>();


        private IControlNamingConvention ControlNamingConvention { get; set; }

        public WinFormsControlMapperRegistry()
            : this(new DefaultControlNamingConvention())
        {
        }

        public WinFormsControlMapperRegistry(IControlNamingConvention namingConvention)
        {
            if (namingConvention == null) throw new ArgumentNullException("namingConvention");
            ControlNamingConvention = namingConvention;
//            _controlNamingConvention = new DefaultControlNamingConvention();
            _mapperRegistry.Add(typeof(CheckBox), typeof(CheckBoxMapper));
            _mapperRegistry.Add(typeof(TextBox), typeof(TextBoxMapper));
            _mapperRegistry.Add(typeof(DateTimePicker), typeof(DateTimePickerMapper));
            _mapperRegistry.Add(typeof(NumericUpDown), typeof(NumericUpDownMapper));
        }

        public Type GetMapperType<TBo>(Control control) where TBo : class, IBusinessObject
        {
            if (control == null) throw new ArgumentNullException("control");
            var controlType = control.GetType();
            if (controlType == typeof(ComboBox)) return GetComboBoxMapperType<TBo>(control);
            if (controlType == typeof(NumericUpDown)) return GetNumericUpDownMapperType<TBo>(control);
            if (_mapperRegistry.ContainsKey(controlType)) return  _mapperRegistry[controlType];
            return null;
        }

        private Type GetNumericUpDownMapperType<TBo>(Control control) where TBo : class, IBusinessObject
        {
            string propName = ControlNamingConvention.GetPropName(control);
            var classDef = ClassDef.Get<TBo>();
            var propDef = classDef.GetPropDef(propName, false);
            if (propDef == null) return null;
            return IsInteger(propDef) ? typeof (NumericUpDownIntegerMapper) : typeof (NumericUpDownCurrencyMapper);
        }

        private static bool IsInteger(ISingleValueDef propDef)
        {
            return typeof(int) == propDef.PropertyType || typeof(long) == propDef.PropertyType;
        }

        private Type GetComboBoxMapperType<T>(Control control) where T: class, IBusinessObject
        {
            //Note_ the Naming convention is only needed in the registry to deal with
            // ComboBoxes since they could be enum, Relationship or lookups so have to 
            // get the property name to resolve.
            string propName = ControlNamingConvention.GetPropName(control);
            var classDef = ClassDef.Get<T>();
            var propDef = classDef.GetPropDef(propName, false);
            if (propDef == null)
            {
                var relationshipDef = classDef.GetRelationship(propName) as ISingleRelationshipDef;
                if (relationshipDef != null)
                {
                    return typeof (AutoLoadingRelationshipComboBoxMapper);
                }
                else
                {
                    //ToDo: some sort of reflective stuff since this is a reflective prop
                }
                return null;
            }
          //  if (propDef.PropertyType.ToTypeWrapper().IsEnumType()) return typeof (EnumComboBoxMapper);
            if(propDef.HasLookupList()) return typeof (LookupComboBoxMapper);

            return null;
        }
    }
    /// <summary>
    /// An interface used in HabaneroBinding by Convention allowing the developer to 
    /// specify any naming convention required.
    /// </summary>
    public interface IControlNamingConvention
    {
        /// <summary>
        /// Returns the Prop Name based on the <see cref="IControlNamingConvention"/>
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        string GetPropName(Control control);
        /// <summary>
        /// Is the Control valid i.e. is the control named in a way that the name could 
        /// be used to derive a valid PropertyName
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        Result IsValidControl(Control control);
    }
    /// <summary>
    /// The Default Naming Convention is a three letter prefix and then
    /// the Property or Relationship Name.
    /// </summary>
    public class DefaultControlNamingConvention : IControlNamingConvention
    {
        /// <summary>
        /// Returns the Prop Name based on the <see cref="DefaultControlNamingConvention"/>
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public string GetPropName(Control control)
        {
            CheckControlValid(control);
            var name = control.Name;
            return name.Substring(3);
        }


        /// <summary>
        /// Is the Control valid i.e. is the control named in a way that the name could 
        /// be used to derive a valid PropertyName
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public Result IsValidControl(Control control)
        {
            if (control == null) return new Result(false, "control cannot be null");
            if (string.IsNullOrEmpty(control.Name)) return new Result(false, "control.Name cannot be empty");
            if (control.Name.Length <= 3) return new Result(false, "The control.Name must be greater than 3 characters");
            return new Result(true);
        }

        private void CheckControlValid(Control control)
        {
            var isValidControl = this.IsValidControl(control);
            if (!isValidControl.Successful) throw new HabaneroArgumentException("control",isValidControl.Message);
        }
    }
}