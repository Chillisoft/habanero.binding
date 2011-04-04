using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.ProgrammaticBinding.CF.ControlAdaptors;
using Habanero.ProgrammaticBinding.ControlAdaptors;

namespace Habanero.ProgrammaticBinding.CF
{
    public interface IHabaneroSelectorControlBinder<TBo> where TBo : class, IBusinessObject, new()
    {
        /// <summary>
        /// Event Occurs when a business object is selected
        /// </summary>
        event EventHandler<BOEventArgs<TBo>> BusinessObjectSelected;

        /// <summary>
        /// Gets or Sets the business object, that is currently 
        /// selected in the <see cref="ListControl"/>(e.g. ListBox/ComboBox) else null if no item is selected
        /// </summary>
        TBo SelectedBusinessObject { get; set; }
        /// <summary>
        /// The Selector control being bound to.
        /// </summary>
        IListControl ListControl { get; }
        /// <summary>
        /// You can bind a Selector Control to one or more ReadUpdate Control Binders.
        /// This allows you to easily create the standard Selector Editor type forms.
        /// Binding a <see cref="HabaneroControlBinder{T}"/> enables the behaviour where a 
        /// Business Object is selected using the Selector Control <see cref="ListControl"/>
        /// and the ReadUpdate Controls managed by the <see cref="HabaneroControlBinder{T}"/> are automatically
        /// bound to the selected BusinessObject.
        /// </summary>
        /// <param name="controlBinder"></param>
        /// <returns></returns>
        HabaneroControlBinder<TBo> BindEditorControlBinder(HabaneroControlBinder<TBo> controlBinder);
        /// <summary>
        /// Sets the Busienss Object Collection.
        /// </summary>
        /// <param name="businessObjectCollection"></param>
        void SetBusinessObjectCollection(BusinessObjectCollection<TBo> businessObjectCollection);
    }

    /// <summary>
    /// This is a convenience class that is used to programmatically bind a Business Object
    /// to a WinForms Selector Controls. 
    /// A Selector control is any control that can be used to select a business Object from a list
    /// of business objects.
    /// E.g. a CheckedListBox,
    /// ComboBox,
    /// ListBox,
    /// ListView,
    /// ReadOnlyGrid
    /// EditableGrid ?
    /// </summary>
    /// <typeparam name="TBo">The Type of the Business Object</typeparam>
    /// <typeparam name="TControlType">The Type of the Control</typeparam>
    public class HabaneroSelectorControlBinder<TBo, TControlType> : IHabaneroSelectorControlBinder<TBo> where TBo : class, IBusinessObject, new() 
        where TControlType : ListControl
    {
        private readonly IBOColSelector _selectorManager;
        private readonly IListControl _lstControl;
        private readonly IControlAdaptorFactory _controlAdaptorFactory;

        /// <summary>
        /// Event Occurs when a business object is selected
        /// </summary>
        public event EventHandler<BOEventArgs<TBo>> BusinessObjectSelected;

        /// <summary>
        /// Construcst the SelectorControlBinder with the Selector Control.
        /// </summary>
        /// <param name="listControl"></param>
        public HabaneroSelectorControlBinder(TControlType listControl)
        {
            _controlAdaptorFactory = new WinFormsControlAdaptorFactory();
            ControlBinders = new List<HabaneroControlBinder<TBo>>();

            _lstControl = _controlAdaptorFactory.GetHabaneroControl(listControl) as IListControl;
            _selectorManager = CreateBoColSelector(ListControl);
            _selectorManager.BusinessObjectSelected += OnBusinessObjectSelected;
        }

        private static IBOColSelector CreateBoColSelector(IControlHabanero ctl)
        {

                if (ctl is IComboBox) return new ComboBoxCollectionSelector(ctl as IComboBox, GlobalUIRegistry.ControlFactory);
                if (ctl is IListBox) return new ListBoxCollectionManager(ctl as IListBox);
                return null; //TODO brett 16 Dec 2010: Need to improve this maybe with similar error as below.
/*
                    throw new InvalidXmlDefinitionException
                        (String.Format
                             ("No suitable 'mapperType' has been provided in the class "
                              + "definitions for the form control '{0}'.  Either add the "
                              + "'mapperType' attribute or check that spelling and "
                              + "capitalisation are correct.",
                              ctl.Name));*/

        }

        /// <summary>
        /// Returns the business object, in object form, that is currently 
        /// selected in the ListBox list, or null if none is selected
        /// </summary>
        public TBo SelectedBusinessObject
        {
            get { return _selectorManager.SelectedBusinessObject as TBo; }
            set
            {
                _selectorManager.SelectedBusinessObject = value;
                SetBusinessObjectOnControlBinders(value);
            }
        }

        private void SetBusinessObjectOnControlBinders(TBo value)
        {
            foreach (var habaneroControlBinder in ControlBinders)
            {
                habaneroControlBinder.BusinessObject = value;
            }
        }

        /// <summary>
        /// The Selector control being bound to.
        /// </summary>
        public IListControl ListControl
        {
            get { return _lstControl; }
        }

        /// <summary>
        /// Sets the Busienss Object Collection.
        /// </summary>
        /// <param name="businessObjectCollection"></param>
        public void SetBusinessObjectCollection(BusinessObjectCollection<TBo> businessObjectCollection)
        {
            _selectorManager.BusinessObjectCollection = businessObjectCollection;
        }

        private void OnBusinessObjectSelected(object sender, BOEventArgs boEventArgs)
        {
            var selectedBO = boEventArgs.BusinessObject as TBo;
            SetBusinessObjectOnControlBinders(selectedBO);
            if (this.BusinessObjectSelected != null)
            {
                this.BusinessObjectSelected(this, new BOEventArgs<TBo>(selectedBO));
            }
        }

        private IList<HabaneroControlBinder<TBo>> ControlBinders { get; set; }

        /// <summary>
        /// You can bind a Selector Control to one or more ReadUpdate Control Binders.
        /// This allows you to easily create the standard Selector Editor type forms.
        /// Binding a <see cref="HabaneroControlBinder{T}"/> enables the behaviour where a 
        /// Business Object is selected using the Selector Control <see cref="IHabaneroSelectorControlBinder{TBo}.ListControl"/>
        /// and the ReadUpdate Controls managed by the <see cref="HabaneroControlBinder{T}"/> are automatically
        /// bound to the selected BusinessObject.
        /// </summary>
        /// <param name="controlBinder"></param>
        /// <returns></returns>
        public HabaneroControlBinder<TBo> BindEditorControlBinder(HabaneroControlBinder<TBo> controlBinder)
        {
            ControlBinders.Add(controlBinder);
            return controlBinder;
        }
    }
}