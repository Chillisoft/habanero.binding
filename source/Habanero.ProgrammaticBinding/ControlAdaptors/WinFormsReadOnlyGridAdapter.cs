using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using DataGridViewSelectionMode = Habanero.Faces.Base.DataGridViewSelectionMode;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is an interface used specificaly for adapting a any control that inherits from System.Windows.Control 
    /// so that it can be treated as an IControlHabanero and can therefore be used by Faces for Habanero.Binding,
    /// <see cref="PanelBuilder"/>
    /// or any other such required behaviour.
    /// </summary>
    public interface IWinFormsReadOnlyGridAdapter : IReadOnlyGrid, IWinFormsControlAdapter
    {
    }
    /// <summary>
    /// This is a Control Adapter for any <see cref="DataGridView"/> control.
    ///  It wraps the <see cref="DataGridView"/> control behind a standard interface.
    /// This allows Faces to interact with it as if it is a Habanero Control.
    /// </summary>
    public class WinFormsReadOnlyGridAdapter : WinFormsGridBaseAdapter, IWinFormsReadOnlyGridAdapter
    {
        private readonly IHabaneroLogger _logger =
            GlobalRegistry.LoggerFactory.GetLogger(
                "Habanero.ProgrammaticBinding.ControlAdaptors.WinFormsReadOnlyGridAdapter");

        public WinFormsReadOnlyGridAdapter(DataGridView gridView) : base(gridView)
        {
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// Creates a dataset provider that is applicable to this grid. For example, a readonly grid would
        /// return a <see cref="ReadOnlyDataSetProvider"/>, while an editable grid would return an editable one.
        /// </summary>
        /// <param name="col">The collection to create the datasetprovider for</param>
        /// <returns>Returns the data set provider</returns>
        public override IDataSetProvider CreateDataSetProvider(IBusinessObjectCollection col)
        {
            return new ReadOnlyDataSetProvider(col)
            {
                RegisterForBusinessObjectPropertyUpdatedEvents = true
            };
        }
    }
}