using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Faces.Base;
using Habanero.Faces.CF;
using Habanero.ProgrammaticBinding.CF.ControlAdaptors;

namespace Habanero.ProgrammaticBinding.CF
{
    /// <summary>
    /// This is a convenience class that is used to programmatically bind Properties of a Business Object
    /// to WinForms Controls. <see cref="AddTextBoxMapper"/> etc.
    /// Note_: There is no validation that the propName is a property of the BusinessObjet.
    /// </summary>
    /// <typeparam name="TBo"></typeparam>
    public class HabaneroControlBinder<TBo> where TBo : class, IBusinessObject
    {
        private readonly IControlAdaptorFactory _controlAdaptorFactory;
        private readonly IControlMapperRegistry _controlMapperRegistry;
        protected readonly IControlNamingConvention _controlNamingConvention;
        private readonly string _compulsoryFieldIndicator;

        private const string DEVELOPER_MESSAGE =
                "The HabaneroControlBinder class is designed to be used for binding Habanero Business Objects. " + 
                "The ClassDef or one of its properties being mapped was not correct so it is likely that your classdefs are not being loaded correctly.";

        public HabaneroControlBinder()
            : this(new DefaultControlNamingConvention())
        {
        } 
        public HabaneroControlBinder(IControlNamingConvention namingConvention)
        {
            //Pull this into an argument in the Constructor
            // to be able to Inject a different Adaptor Factory e.g. for Infragistics.
            _controlAdaptorFactory = new WinFormsControlAdaptorFactory();
            //Pull this into an argument in the Constructor
            // to be able to Inject a different naming convention.          
            _controlNamingConvention = namingConvention;
            //Pull this into an argument in the Constructor
            // to be able to Inject a different Mapper Registry e.g. for Infragistics.
            _controlMapperRegistry = new WinFormsControlMapperRegistry(_controlNamingConvention);
            ControlMappers = new ControlMapperCollection();
            _compulsoryFieldIndicator = "*";
        }

        /// <summary>
        /// The collection of Control Mappers being managed by this <see cref="HabaneroControlBinder{TBo}"/>
        /// </summary>
        public IControlMapperCollection ControlMappers { get; private set; }

        /// <summary>
        /// Gets and Sets the business object to all the control mappers that have been added to this Control Binder.
        /// </summary>
        public TBo BusinessObject
        {
            get { return this.ControlMappers.BusinessObject as TBo; }
            set { this.ControlMappers.BusinessObject = value; }
        }
        /// <summary>
        /// Adds a Text Box mapper to the <paramref name="txtBox"/> for the <see cref="IBusinessObject"/>'s property
        /// identified by propName
        /// </summary>
        /// <param name="txtBox">The TextBox being mapped</param>
        /// <returns></returns>
        public TextBoxMapper AddTextBoxMapper(TextBox txtBox)
        {
            return (TextBoxMapper) AddMapper(txtBox);
        }
        /// <summary>
        /// Adds a Text Box mapper to the <paramref name="txtBox"/> for the <see cref="IBusinessObject"/>'s property
        /// identified by propName
        /// </summary>
        /// <param name="txtBox">The TextBox being mapped</param>
        /// <param name="propName">The <see cref="IBusinessObject"/>'s property it is being mapped to</param>
        /// <returns></returns>
        public TextBoxMapper AddTextBoxMapper(TextBox txtBox, string propName)
        {
            return (TextBoxMapper) AddMapper(txtBox, propName);
        }

        /// <summary>
        /// Adds a <see cref="DateTimePickerMapper"/> to this <see cref="HabaneroControlBinder{TBo}"/>
        /// </summary>
        /// <param name="dtPicker"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public DateTimePickerMapper AddDateTimePicker(DateTimePicker dtPicker, string propName)
        {
            return (DateTimePickerMapper) AddMapper(dtPicker, propName);
        }

        /// <summary>
        /// Adds a <see cref="CheckBoxMapper"/> to this <see cref="HabaneroControlBinder{TBo}"/>
        /// </summary>
        /// <param name="checkBox">CheckBox being mapped to</param>
        /// <param name="propName">property being mapped</param>
        /// <returns></returns>
        public CheckBoxMapper AddCheckBoxMapper(CheckBox checkBox, string propName)
        {
            return (CheckBoxMapper) AddMapper(checkBox, propName);
        }
/*
        /// <summary>
        /// Adds a <see cref="EnumComboBoxMapper"/> to this <see cref="HabaneroControlBinder{TBo}"/>
        /// </summary>
        /// <param name="comboBox">ComboBox being mapped to</param>
        /// <param name="propName">property being mapped</param>
        /// <returns></returns>
        public EnumComboBoxMapper AddEnumComboBoxMapper(ComboBox comboBox, string propName)
        {
            var controlHabanero = GetHabaneroControl(comboBox);
            return AddControlMapper<EnumComboBoxMapper>(controlHabanero, propName);
        }*/

        /// <summary>
        /// Adds a <see cref="LookupComboBoxMapper"/> to this <see cref="HabaneroControlBinder{TBo}"/>
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public LookupComboBoxMapper AddLookupComboBoxMapper(ComboBox comboBox, string propName)
        {
            var controlHabanero = GetHabaneroControl(comboBox);
            return AddControlMapper<LookupComboBoxMapper>(controlHabanero, propName);
        }


        /// <summary>
        /// Adds the Control Mapper for an AutoLoadingComboBoxMapper this is a mapper where
        /// the Lookup items for the ComboBox are automatically loading from the Database.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public IControlMapper AddAutoLoadingRelationshipComboBoxMapper(ComboBox comboBox, string propName)
        {
            var controlHabanero = GetHabaneroControl(comboBox);
            return AddControlMapper<AutoLoadingRelationshipComboBoxMapper>(controlHabanero, propName);
        }

        /// <summary>
        /// Adds Mapper for a ComboBox where the list of ComboBox items is 
        /// provided via a stringList.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="propName"></param>        
        /// <param name="stringList">A pipe (|) seperated string representing 
        /// the list of string options to populate the list e.g Mr|Mrs|Dr </param>
        /// <returns></returns>
        public IControlMapper AddListComboBoxMapper(ComboBox comboBox, string propName, string stringList)
        {
            var controlHabanero = GetHabaneroControl(comboBox);
            var mapper = AddControlMapper<ListComboBoxMapper>(controlHabanero, propName);
            mapper.SetList(stringList);
            return mapper;
        }

        /// <summary>
        /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
        /// for the business object property identified by <paramref name="propName"/>
        /// </summary>
        /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
        /// <typeparam name="TControl">Type of Control</typeparam>
        /// <param name="label"></param>
        /// <param name="control">instance of the Control</param>
        /// <param name="propName">the property name of the property being bound to</param>
        /// <returns></returns>
        public TMapperType Add<TMapperType, TControl>(Label label, TControl control, string propName)
            where TControl : Control
            where TMapperType : IControlMapper
        {
            //ConfigureLabel(label, propName);
            return (TMapperType)Add(typeof(TMapperType), control, propName);
        }
        /* Expression<Func<TBo, object>> not supported by CF

                /// <summary>
                /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
                /// for the business object property identified by <paramref name="propExpression"/>
                /// </summary>
                /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
                /// <typeparam name="TControl">Type of Control</typeparam>
                /// <param name="control">instance of the Control</param>
                /// <param name="propExpression">expression identifying the property being bound to</param>
                /// <returns></returns>
                public TMapperType Add<TMapperType, TControl>(TControl control, Expression<Func<TBo, object>> propExpression)
                    where TControl : Control
                    where TMapperType : IControlMapper
                {
                    var propertyName = GetPropertyName(propExpression);
                    return Add<TMapperType, TControl>(control, propertyName);
                }
        
                /// <summary>
                /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
                /// for the business object property identified by <paramref name="propExpression"/>
                /// </summary>
                /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
                /// <typeparam name="TControl">Type of Control</typeparam>
                /// <param name="control">instance of the Control</param>
                /// <param name="propExpression">expression identifying the property being bound to</param>
                /// <returns></returns>
                public TMapperType Add<TMapperType, TControl>(TControl control, Expression<Func<TBo, object>> propExpression, bool isReadOnly)
                    where TControl : Control
                    where TMapperType : IControlMapper
                {
                    var mapper = Add<TMapperType, TControl>(control, propExpression);
                    mapper.IsReadOnly = isReadOnly;
                    return mapper;
                }

                /// <summary>
                /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
                /// for the business object property identified by <paramref name="propExpression"/>
                /// </summary>
                /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
                /// <typeparam name="TControl">Type of Control</typeparam>
                /// <param name="label">The label associated with this control</param>
                /// <param name="control">instance of the Control</param>
                /// <param name="propExpression">expression identifying the property being bound to</param>
                /// <param name="isReadOnly">Is the property read only in this form. i.e. must <paramref name="control"/> remain disabled 
                /// regardless of the BusinessObject's Property's ReadWrite state.</param>
                /// <returns></returns>
                public TMapperType Add<TMapperType, TControl>(Label label, TControl control, Expression<Func<TBo, object>> propExpression, bool isReadOnly)
                    where TControl : Control
                    where TMapperType : IControlMapper
                {
                    var mapper = Add<TMapperType, TControl>(label, control, propExpression);
                    mapper.IsReadOnly = isReadOnly;
                    return mapper;
                }

                /// <summary>
                /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
                /// for the business object property identified by <paramref name="propExpression"/>
                /// </summary>
                /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
                /// <typeparam name="TControl">Type of Control</typeparam>
                /// <param name="label">The label that the control</param>
                /// <param name="control">instance of the Control</param>
                /// <param name="propExpression">expression identifying the property being bound to</param>
                /// <returns></returns>
                public TMapperType Add<TMapperType, TControl>(Label label, TControl control,
                                                              Expression<Func<TBo, object>> propExpression)
                    where TControl : Control
                    where TMapperType : IControlMapper
                {
                    var propertyName = GetPropertyName(propExpression);
                    return Add<TMapperType, TControl>(label, control, propertyName);
                }
        */

        /// <summary>
        /// Adds the Control mapper of type <typeparamref name="TMapperType"/> to the <paramref name="control"/>
        /// for the business object property identified by <paramref name="propName"/>
        /// </summary>
        /// <typeparam name="TMapperType">Type of Control Mapper</typeparam>
        /// <typeparam name="TControl">Type of Control</typeparam>
        /// <param name="control">instance of the Control</param>
        /// <param name="propName">the property name of the property being bound to</param>
        /// <returns></returns>
        public TMapperType Add<TMapperType, TControl>(TControl control, string propName)
            where TControl : Control
            where TMapperType : IControlMapper
        {
            return (TMapperType)Add(typeof(TMapperType), control, propName);
        }

        /// <summary>
        /// Adds <see cref="IControlMapper"/>s for all relevant controls in the <paramref name="controls"/>
        /// collection. This will not set ToolTips on Labels and will not modify label text to show compulsory fields etc.
        /// If you want to set up labels then see <see cref="AddMappersByConvention(System.Collections.Generic.IEnumerable{System.Windows.Forms.Control},bool)"/>
        /// </summary>
        /// <param name="controls">A collection containing the controls that mappers must be created for</param>
        public void AddMappersByConvention(IEnumerable<Control> controls)
        {
            AddMappersByConvention(controls, false);
        }
        /// <summary>
        /// Tries to automatically map the <paramref name="controls"/> using an appropriate
        /// Mapper. This uses a <see cref="IControlNamingConvention"/> strategy to determine 
        /// the property name based on an appropriate convention.
        /// By default the convention used is <see cref="DefaultControlNamingConvention"/>.
        /// If <paramref name="mapLabels"/> is true then the Labels will be configured
        /// with compulsory indicators and ToolTips where appropriate.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="mapLabels"></param>
        public void AddMappersByConvention(IEnumerable<Control> controls, bool mapLabels)
        {
            if (controls == null) throw new ArgumentNullException("controls");
            foreach (var control in controls)
            {
                if (control.IsLabel() && !IsLabelMapped(control))
                {
                    ConfigureLabelByConvention(mapLabels, control);
                    continue;
                }
                if (!IsControlMapped(control))
                {

                    var mapperType = _controlMapperRegistry.GetMapperType<TBo>(control);
                    if (mapperType == null) continue;
                    this.Add(mapperType, control, _controlNamingConvention.GetPropName(control));
                }
            }
        }

        private bool IsLabelMapped(Control control)
        {
            
            return (control.Text.EndsWith(_compulsoryFieldIndicator));
        }

        private static void ConfigureLabel(Label label, string propName)
        {
            IUIFormField field = new UIFormField(label.Text, propName);// {ClassDef = ClassDef.Get<TBo>()};
           
            var singleValueDef = GetSingleValueDef(propName);
            label.Text = field.GetLabel();
            if (IsCompulsory(singleValueDef)) SetBoldText(label);
        }

        private static void SetBoldText(Label label)
        {
            label.Font = new Font(label.Font.Name, label.Font.Size, FontStyle.Bold);
        }

        private static bool IsCompulsory(ISingleValueDef singleValueDef)
        {
            return singleValueDef != null && singleValueDef.Compulsory;
        }

/* not supported by CF
 * private static IRelationshipDef GetRelationshipDef<TReturn>(Expression<Func<TBo, TReturn>> propertyExpression,
                                                                    bool raiseErrIfNotExists)
        {
            string propertyName = GetPropertyName(propertyExpression);
            return GetRelationshipDef(typeof (TBo), propertyName, raiseErrIfNotExists);
        }

        private static ISingleValueDef GetPropDef<TReturn>(Expression<Func<TBo, TReturn>> propertyExpression,
                                                           bool raiseErrIfNotExists)
        {
            string propertyName = GetPropertyName(propertyExpression);
            return GetPropDef(typeof (TBo), propertyName, raiseErrIfNotExists);
        }*/

/*
        private static ISingleValueDef GetSingleValueDef<TReturn>(Expression<Func<TBo, TReturn>> propertyExpression)
        {
            var singleValueDef = GetPropDef(propertyExpression, false);
            if (singleValueDef == null)
                singleValueDef = GetRelationshipDef(propertyExpression, true) as ISingleValueDef;
            return singleValueDef;
        }
*/

/*        private static string GetPropertyName<TReturn>(Expression<Func<TBo, TReturn>> propExpression)
        {
            return ReflectionUtilities.GetPropertyName(propExpression);
        }*/
                    

        public void ApplyChangesToBusinessObject()
        {
            foreach (var mapper in this.ControlMappers)
            {
                mapper.ApplyChangesToBusinessObject();
            }
        }


        private static ISingleValueDef GetSingleValueDef(string propName)
        {
            return GetPropDef(propName, false) 
                   ?? GetSingleRelationshipDef(propName, false);
        }

        private static ISingleValueDef GetSingleRelationshipDef( string propName, bool raiseErrIfNotExists)
        {
            return GetRelationshipDef(typeof(TBo), propName, raiseErrIfNotExists) as ISingleValueDef;
        }

        private static ISingleValueDef GetPropDef(string propName, bool raiseErrIfNotExists)
        {
            var boType = typeof(TBo);
            ValidateClassDef(boType);
            var classDef = ClassDef.ClassDefs[boType];

            return GetPropDef(classDef, propName, raiseErrIfNotExists);
        }

        private static ISingleValueDef GetPropDef(IClassDef classDef, string propName, bool raiseErrIfNotExists)
        {
            ISingleValueDef def = classDef.GetPropDef(propName, false);
            if (raiseErrIfNotExists) ValidateProp(classDef.ClassType, def, propName);
            return def;
        }

        private static IRelationshipDef GetRelationshipDef(Type type, string relationshipName,
                                                           bool raiseErrIfNotExists)
        {
            ValidateClassDef(type);
            var classDef = ClassDef.ClassDefs[type];
            return GetRelationshipDef(classDef, relationshipName, raiseErrIfNotExists);
        }

        private static IRelationshipDef GetRelationshipDef(IClassDef classDef, string relationshipName, bool raiseErrIfNotExists)
        {
            var relationshipDef = classDef.GetRelationship(relationshipName);
            if (raiseErrIfNotExists) ValidateRelationshipDef(classDef.ClassType, relationshipDef, relationshipName);
            return relationshipDef;
        }

        private static void ValidateClassDef(Type type)
        {
            if (!ClassDef.ClassDefs.Contains(type))
            {
                throw new HabaneroDeveloperException(
                    string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), DEVELOPER_MESSAGE);
            }
        }

        private static void ValidateProp(Type type, ISingleValueDef def, string propName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(
                    string.Format("The property '{0}' for the ClassDef for '{1}' is not defined", propName, type),
                    DEVELOPER_MESSAGE);
            }
        }

        private static void ValidateRelationshipDef(Type type, IRelationshipDef def, string relationshipName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(
                    string.Format("The relationship '{0}' for the ClassDef for '{1}' is not defined", relationshipName,
                                  type), DEVELOPER_MESSAGE);
            }
        }


        private IControlMapper Add<TControl>(Type mapperType, TControl control, string propName)
            where TControl : Control
        {
            var controlHabanero = GetHabaneroControl(control);
            return AddControlMapper(mapperType, controlHabanero, propName);
        }


        private IControlMapper AddMapper<TControl>(TControl control, string propName) where TControl : Control
        {
            var controlType = typeof (TControl);
            return AddMapper(controlType, control, propName);
        }
        private IControlMapper AddMapper<TControl>(TControl control) where TControl : Control
        {
            var controlType = typeof (TControl);
            var propName = _controlNamingConvention.GetPropName(control);
            return AddMapper(controlType, control, propName);
        }

        private TMapperType AddControlMapper<TMapperType>(IControlHabanero controlHabanero, string propName)
            where TMapperType : IControlMapper
        {
            return (TMapperType) AddControlMapper(typeof (TMapperType), controlHabanero, propName);
        }
        private TMapperType AddControlMapper<TMapperType>(IControlHabanero controlHabanero)
            where TMapperType : IControlMapper
        {
            var propName = _controlNamingConvention.GetPropName(controlHabanero.GetControl());
            return (TMapperType) AddControlMapper(typeof (TMapperType), controlHabanero, propName);
        }

        private IControlMapper AddControlMapper(Type controlMapperType, IControlHabanero controlHabanero,
                                                string propName)
        {
            var controlMapper = ControlMapper.Create(controlMapperType, controlHabanero, propName, false,
                                                     GlobalUIRegistry.ControlFactory);
            return AddMapper(controlMapper);
        }

        private IControlMapper AddMapper(Type controlType, Control control, string propName)
        {
            var controlHabanero = _controlAdaptorFactory.GetHabaneroControl(controlType, control);
            if (controlHabanero == null) return null;
            return AddMapper(CreateMapper(controlHabanero, propName));
        }

        private static IControlMapper CreateMapper(IControlHabanero control, string propName)
        {
            return ControlMapper.Create(control, propName);
        }

        private IControlHabanero GetHabaneroControl<TControlType>(TControlType control) where TControlType : Control
        {
            var controlHabanero = _controlAdaptorFactory.GetHabaneroControl(typeof (TControlType), control) 
                        ?? _controlAdaptorFactory.GetHabaneroControl(control.GetType(), control);
            return controlHabanero;
        }

        private IControlMapper AddMapper(IControlMapper mapper)
        {
            return this.ControlMappers.Add(mapper);
        }

        private bool IsControlMapped(Control control)
        {
            var localControl = control;
            var firstOrDefault = this.ControlMappers.FirstOrDefault(mapper => mapper.Control.Equals(localControl));
            return firstOrDefault != null;
        }

        private void ConfigureLabelByConvention(bool mapLabels, Control control)
        {
            if (mapLabels)
            {
                if (control.IsLabel())
                {
                    var propName = _controlNamingConvention.GetPropName(control);
                    var label = control as Label;
                    ConfigureLabel(label, propName);
                    return;
                }
            }
            return;
        }

    }

    internal static class ControlExtensions
    {
        public static bool IsLabel(this Control control)
        {
            return control.GetType() == typeof(Label);
        }
    }
}