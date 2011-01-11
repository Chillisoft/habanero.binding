using System;
using System.Windows.Forms;
using Habanero.Faces.Base;

namespace HabaneroProgramaticBinding.UI
{
    public class DateTimePickerMapperWin : ControlMapper
    {
        /// <summary>
        /// Constructor to initialise a new instance of the class
        /// </summary>
        /// <param name="picker">The DateTimePicker control to which the property is mapped</param>
        /// <param name="propName">The property name</param>
        /// <param name="isReadOnly">Whether this control is read only</param>
        /// <param name="factory">The control factory to be used when creating the controlMapperStrategy</param>
        public DateTimePickerMapperWin(DateTimePicker picker, string propName, bool isReadOnly, IControlFactory factory)
            : base(new ControlWrapperWinForms(picker), propName, isReadOnly, factory)
        {
            DateTimePicker = picker;
            PropertyName = propName;
            DateTimePicker.ValueChanged += (sender, args) => this.ApplyChangesToBusinessObject();
        }

        protected DateTimePicker DateTimePicker { get; private set; }


        /// <summary>
        /// Updates the properties on the represented business object
        /// </summary>
        public override void ApplyChangesToBusinessObject()
        {
            object newValue = GetValueOfDateTimePicker();
            SetPropertyValue(newValue);
        }
        /// <summary>
        /// Returns the value currently held by the picker
        /// </summary>
        /// <returns>Returns the value held</returns>
        private object GetValueOfDateTimePicker()
        {
            return DateTimePicker.Value;
        }

        /// <summary>
        /// Updates the value on the control from the corresponding property
        /// on the represented <see cref="IControlMapper.BusinessObject"/>
        /// </summary>
        protected  override void InternalUpdateControlValueFromBo()
        {
            if (_businessObject == null) return;
            object propertyValue = GetPropertyValue();
            if (propertyValue == null)
            {
                DateTimePicker.Value = DateTime.MinValue;
            } else
            {
                DateTimePicker.Value = Convert.ToDateTime(propertyValue);
            }
        }
    }
}