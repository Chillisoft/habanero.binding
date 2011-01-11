using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace HabaneroProgramaticBinding.UI
{
    public class HabaneroControlBinder
    {
        private IBusinessObject _businessObject;

        public HabaneroControlBinder()
        {
            ControlMappers = new List<ControlMapper>();
        }

        public TextBoxMapperWinForms AddTextBoxMapper(TextBox txtBox, string propName)
        {
            var mapper = new TextBoxMapperWinForms(txtBox, propName, false, GlobalUIRegistry.ControlFactory);
            AddMapper(mapper);
            return mapper;
        }

        public DateTimePickerMapperWin AddDateTimePicker(DateTimePicker dtPicker, string propName)
        {
            var mapper = new DateTimePickerMapperWin(dtPicker, propName, false, GlobalUIRegistry.ControlFactory);
            AddMapper(mapper);
            return mapper;
        }

        private ControlMapper AddMapper(ControlMapper mapper)
        {
            this.ControlMappers.Add(mapper);
            return mapper;
        }

        public IList<ControlMapper> ControlMappers { get; private set; }

        public IBusinessObject BusinessObject
        {
            get {
                return _businessObject;
            }
            set {
                _businessObject = value;
                foreach (var controlMapper in ControlMappers)
                {
                    controlMapper.BusinessObject = _businessObject;
                }
            }
        }
    }

}
