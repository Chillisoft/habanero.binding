using System;
using System.Drawing;
using System.Windows.Forms;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using AnchorStyles = Habanero.Faces.Base.AnchorStyles;
using DockStyle = Habanero.Faces.Base.DockStyle;

namespace Habanero.ProgramaticBinding.ControlAdaptors
{
    /// <summary>
    /// This is a ControlWraper for Any Control that Inherits from System.Windows.Forms.Control
    /// It wraps this Control behind a standard interface that allows any Control in a Windows Environment 
    /// to take advantage of the Habanero ControlMappers <see cref="IControlMapper"/>
    /// </summary>
    public class WinFormsControlAdapter : IWinFormsControlAdapter
    {
        public Control WrappedControl { get; private set; }

        public WinFormsControlAdapter(Control gridView)
        {
            if (gridView == null) throw new ArgumentNullException("gridView");
            WrappedControl = gridView;
        }

        public bool Focus()
        {
            return WrappedControl.Focus();
        }

        public void Select()
        {
            WrappedControl.Select();
        }

        public void SuspendLayout()
        {
            WrappedControl.SuspendLayout();
        }

        public void ResumeLayout(bool performLayout)
        {
            WrappedControl.ResumeLayout(performLayout);
        }

        public void Invalidate()
        {
            WrappedControl.Invalidate();
        }

        public void Dispose()
        {
            WrappedControl.Dispose();
        }

        public AnchorStyles Anchor
        {
            get { return (AnchorStyles) WrappedControl.Anchor; }
            set { WrappedControl.Anchor = (System.Windows.Forms.AnchorStyles) value; }
        }

        public int Width
        {
            get { return WrappedControl.Width; }
            set { WrappedControl.Width = value; }
        }

        public IControlCollection Controls
        {
            get { return new ControlCollectionWin(WrappedControl.Controls); }
        }

        public bool Visible
        {
            get { return WrappedControl.Visible; }
            set { WrappedControl.Visible = value; }
        }

        public int TabIndex
        {
            get { return WrappedControl.TabIndex; }
            set { WrappedControl.TabIndex = value; }
        }

        public bool Focused
        {
            get { return WrappedControl.Focused; }
        }

        public int Height
        {
            get { return WrappedControl.Height; }
            set { WrappedControl.Height = value; }
        }

        public int Top
        {
            get { return WrappedControl.Top; }
            set { WrappedControl.Top = value; }
        }

        public int Bottom
        {
            get { return WrappedControl.Bottom; }
        }

        public int Left
        {
            get { return WrappedControl.Top; }
            set { WrappedControl.Top = value; }
        }

        public int Right
        {
            get { return WrappedControl.Top; }
        }

        public string Text
        {
            get { return WrappedControl.Text; }
            set { WrappedControl.Text = value; }
        }

        public string Name
        {
            get { return WrappedControl.Name; }
            set { WrappedControl.Name = value; }
        }

        public bool Enabled
        {
            get { return WrappedControl.Enabled; }
            set { WrappedControl.Enabled = value; }
        }

        public Color ForeColor
        {
            get { return WrappedControl.ForeColor; }
            set { WrappedControl.ForeColor = value; }
        }

        public Color BackColor
        {
            get { return WrappedControl.BackColor; }
            set { WrappedControl.BackColor = value; }
        }

        public bool TabStop
        {
            get { return WrappedControl.TabStop; }
            set { WrappedControl.TabStop = value; }
        }

        public Size Size
        {
            get { return WrappedControl.Size; }
            set { WrappedControl.Size = value; }
        }

        public Size ClientSize
        {
            get { return WrappedControl.ClientSize; }
            set { WrappedControl.ClientSize = value; }
        }

        public bool HasChildren
        {
            get { return WrappedControl.HasChildren; }
        }

        public Size MaximumSize
        {
            get { return WrappedControl.MaximumSize; }
            set { WrappedControl.MaximumSize = value; }
        }

        public Size MinimumSize
        {
            get { return WrappedControl.MinimumSize; }
            set { WrappedControl.MinimumSize = value; }
        }

        public Font Font
        {
            get { return WrappedControl.Font; }
            set { WrappedControl.Font = value; }
        }

        public Point Location
        {
            get { return WrappedControl.Location; }
            set { WrappedControl.Location = value; }
        }

        public DockStyle Dock
        {
            get { return DockStyleWin.GetDockStyle(WrappedControl.Dock); }
            set { WrappedControl.Dock = DockStyleWin.GetDockStyle(value); }
        }


        public event EventHandler Click;
        public event EventHandler DoubleClick;
        public event EventHandler Resize;
        public event EventHandler VisibleChanged;
        public event EventHandler TextChanged;

        public bool Equals(Control other)
        {
            return ReferenceEquals(this.WrappedControl, other);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is IWinFormsControlAdapter) return base.Equals(obj);
            var control = obj as Control;
            return control != null && Equals(control);
        }

        public override int GetHashCode()
        {
            return this.WrappedControl != null ? this.WrappedControl.GetHashCode() : base.GetHashCode();
        }
    }
}