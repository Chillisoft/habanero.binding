using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Habanero.Base;
using Habanero.Base.Logging;
using Habanero.Binding;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.Util;

namespace Habanero.ProgrammaticBinding
{
    public class MultipleRelationshipDataGridViewMapper : ControlMapper
    {
        private readonly IDataGridView _grid;

        private new static readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger("Habanero.ProgrammaticBinding.MultipleRelationshipDataGridViewMapper");

        private IBindingListView _bindingListView;

        public MultipleRelationshipDataGridViewMapper(IDataGridView grid, string propName)
            : this(grid, propName, false, GlobalUIRegistry.ControlFactory)
        {

        }

        public MultipleRelationshipDataGridViewMapper(IDataGridView grid, string propName, bool isReadOnly, IControlFactory factory)
            : base(grid, propName, isReadOnly, factory)
        {
            _grid = grid;
        }

        public override void ApplyChangesToBusinessObject()
        {

        }

        public IList<IBusinessObject> SelectedBusinessObjects
        {
            get
            {
                IList<IBusinessObject> selectedBOs = new List<IBusinessObject>();
                if (_bindingListView != null)
                {
                    var selectedRows = this._grid.SelectedRows;
                    foreach (IDataGridViewRow selectedRow in selectedRows)
                    {
                        var selectedItem = _bindingListView[selectedRow.Index] as IBusinessObject;
                        selectedBOs.Add(selectedItem);
                    }
                }
                return selectedBOs;
            }
        }

        protected override void InternalUpdateControlValueFromBo()
        {
            if (_businessObject != null)
            {
                var relationship = _businessObject.Relationships[PropertyName];
                var relatedObjectClassDef = relationship.RelatedObjectClassDef;
                var businessObjectCollection = _businessObject.Relationships.GetRelatedCollection(PropertyName);
                _bindingListView = CreateBindingListView(relatedObjectClassDef.ClassType, businessObjectCollection);
                _grid.DataSource = _bindingListView;
            }
            else
            {
                _grid.DataSource = null;
            }
        }

        private IBindingListView CreateBindingListView(Type boType, IBusinessObjectCollection businessObjectCollection)
        {
            _logger.Log("Start CreateBindingListView - Relationship : " + PropertyName, LogCategory.Debug);
            _logger.Log(GetStackTrace(), LogCategory.Debug);
            if (businessObjectCollection == null)
            {
                var businessObjectColType = typeof(BusinessObjectCollection<>).MakeGenericType(boType);
                businessObjectCollection = (IBusinessObjectCollection)Activator.CreateInstance(businessObjectColType);
            }
            IViewBuilder viewBuilder = null;
            try
            {
                var defaultViewBuilderType = typeof(DefaultViewBuilder<>).MakeGenericType(boType);
                viewBuilder = (IViewBuilder) Activator.CreateInstance(defaultViewBuilderType);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, LogCategory.Exception);
                Console.WriteLine(e);
            }

            var bindingListType = typeof(BindingListView<>).MakeGenericType(boType);
            return (IBindingListView)Activator.CreateInstance(bindingListType, businessObjectCollection, viewBuilder);

        }

        private static string GetStackTrace()
        {
                        var stack = new StackTrace();
            return stack.ToString();
   // var frame = stack.GetFrame(1);
        }


    }
}