using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.Binding;
using Habanero.Faces.Base;

namespace Habanero.ProgramaticBinding
{
    public class GridBaseManagerBindingList : GridBaseManager
    {
        private static readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger("Habanero.ProgramaticBinding.GridBaseManagerMapper");

        public GridBaseManagerBindingList(IGridBase gridBase) : base(gridBase)
        {
        }

        protected override IBindingListView GetBindingListView(IBusinessObjectCollection businessObjectCollection)
        {
            if (businessObjectCollection == null) throw new ArgumentNullException("businessObjectCollection");
            var classType = GetClassType(businessObjectCollection); 
            if (this.ClassDef == null || this.ClassDef != businessObjectCollection.ClassDef)
            {
                this.ClassDef = businessObjectCollection.ClassDef;
            }
            _logger.Log("Start CreateBindingListView");

             //Needs this code 
            //            var uiDef = ((ClassDef) this.ClassDef).GetUIDef(UiDefName);
//            if (uiDef == null)
//            {
//                throw new ArgumentException
//                    (String.Format
//                         ("You cannot Get the data for the grid {0} since the uiDef {1} cannot be found for the classDef {2}",
//                          this._gridBase.Name, UiDefName, ((ClassDef)this.ClassDef).ClassName));
//            }
            IViewBuilder viewBuilder = null;
            try
            {
                
                Type defaultViewBuilderType = typeof(DefaultViewBuilder<>).MakeGenericType(classType);
                viewBuilder = (IViewBuilder)Activator.CreateInstance(defaultViewBuilderType);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogCategory.Exception);
                Console.WriteLine(e);
            }

            var bindingListType = typeof(BindingListView<>).MakeGenericType(classType);
            return (IBindingListView)Activator.CreateInstance(bindingListType, businessObjectCollection, viewBuilder);
        }

        private static Type GetClassType(IBusinessObjectCollection businessObjectCollection)
        {
            return businessObjectCollection.ClassDef.ClassType;
        }
    }
}
