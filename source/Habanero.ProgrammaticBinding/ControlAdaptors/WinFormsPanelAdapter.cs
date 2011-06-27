using System;
using System.Drawing;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;

namespace Habanero.ProgrammaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsPanelAdapter : WinFormsControlAdapter, IWinFormsPanelAdapter
    {
        public WinFormsPanelAdapter(Panel control)
            : base(control)
        {
        }
    }

    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Label
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsLabelAdapter : WinFormsControlAdapter, IWinFormsLabelAdapter
    {
        private readonly Label _label;

        public WinFormsLabelAdapter(Label control)
            : base(control)
        {
            _label = control;
        }

        #region Implementation of ILabel

        public int PreferredWidth
        {
            get { return _label.PreferredWidth; }
        }

        public bool AutoSize
        {
            get { return _label.AutoSize; }
            set { _label.AutoSize = value; }
        }

        public ContentAlignment TextAlign
        {
            get { return _label.TextAlign; }
            set { _label.TextAlign = value; }
        }

        #endregion
    }

    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Label
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsGroupBoxAdapter : WinFormsControlAdapter, IWinFormsGroupBoxAdapter
    {
        private readonly GroupBox _groupBox;

        public WinFormsGroupBoxAdapter(GroupBox control)
            : base(control)
        {
            _groupBox = control;
        }

    }
}