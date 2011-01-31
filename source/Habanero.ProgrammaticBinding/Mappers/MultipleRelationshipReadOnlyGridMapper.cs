using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Faces.Base;

namespace Habanero.ProgrammaticBinding.Mappers
{
    public class MultipleRelationshipReadOnlyGridMapper : ControlMapper
    {
        private IReadOnlyGrid Grid { get; set; }

        public MultipleRelationshipReadOnlyGridMapper(IReadOnlyGrid readOnlyGrid, string relationshipName)
            : this(readOnlyGrid, relationshipName, false, GlobalUIRegistry.ControlFactory)
        {

        }

        public MultipleRelationshipReadOnlyGridMapper(IReadOnlyGrid readOnlyGrid, string relationshipName, bool isReadOnly, IControlFactory factory)
            : base(readOnlyGrid, relationshipName, isReadOnly, factory)
        {
            Grid = readOnlyGrid;
        }

        public override void ApplyChangesToBusinessObject()
        {

        }
        public string RelationshipName { get { return this.PropertyName; } }
        protected override void InternalUpdateControlValueFromBo()
        {
            if (_businessObject != null)
            {
                if (!HasIDColumn)
                {
                    CreateIDColumn();
                }
                var businessObjectCollection = _businessObject.Relationships.GetRelatedCollection(PropertyName);
                Grid.BusinessObjectCollection = businessObjectCollection;
            }
            else
            {
                Grid.DataSource = null;
            }
        }

        private bool HasIDColumn
        {
            get
            {
                return GetIDColumn() != null;
            }
        }

        private IDataGridViewColumn GetIDColumn()
        {
            return this.Grid.Columns[GetGridIDColumnName()];
        }
        //this is exact duplicate code of faces code in the GridManager so ideally figure out some way to reuse
        private void CreateIDColumn()
        {
            var gridIDColumnName = GetGridIDColumnName();
            IDataGridViewColumn col = CreateStandardColumn(gridIDColumnName, gridIDColumnName);
            col.Width = 0;
            col.Visible = false;
            col.ReadOnly = true;
            col.DataPropertyName = gridIDColumnName;
            col.ValueType = typeof(string);
        }

        private string GetGridIDColumnName()
        {
            if (Grid == null)
            {
                const string errorMessage = "There was an attempt to access the ID field for a grid when the grid is not yet initialised";
                throw new HabaneroDeveloperException(errorMessage, errorMessage);
            }
            return Grid.IDColumnName;
        }

        private IDataGridViewColumn CreateStandardColumn(string columnName, string columnHeader)
        {
            int colIndex = this.Grid.Columns.Add(columnName, columnHeader);
            return this.Grid.Columns[colIndex];
        }
    }
}