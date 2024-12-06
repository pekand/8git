namespace _8Git
{
    partial class Form8Git
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form8Git));
            treeView = new TreeView();
            contextMenu = new ContextMenuStrip(components);
            timer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // treeView
            // 
            treeView.AllowDrop = true;
            treeView.ContextMenuStrip = contextMenu;
            treeView.Dock = DockStyle.Fill;
            treeView.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeView.ItemHeight = 34;
            treeView.Location = new Point(0, 0);
            treeView.Name = "treeView";
            treeView.Size = new Size(368, 890);
            treeView.TabIndex = 0;
            treeView.AfterLabelEdit += treeView_AfterLabelEdit;
            treeView.BeforeCollapse += treeView_BeforeCollapse;
            treeView.AfterCollapse += TreeView_AfterCollapse;
            treeView.BeforeExpand += treeView_BeforeExpand;
            treeView.AfterExpand += TreeView_AfterExpand;
            treeView.KeyDown += treeView_KeyDown;
            treeView.KeyUp += treeView_KeyUp;
            treeView.MouseDown += treeView_MouseDown;
            treeView.MouseMove += treeView_MouseMove;
            treeView.MouseUp += treeView_MouseUp;
            // 
            // contextMenu
            // 
            contextMenu.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(61, 4);
            contextMenu.Opening += contextMenu_Opening;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            // 
            // Form8Git
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 890);
            Controls.Add(treeView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form8Git";
            Text = "8Git";
            FormClosing += Form8Git_FormClosing;
            FormClosed += Form8Git_FormClosed;
            Load += Form8Git_Load;
            VisibleChanged += Form8Git_VisibleChanged;
            Move += Form8Git_Move;
            Resize += Form8Git_Resize;
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView;
        private ContextMenuStrip contextMenu;
        private System.Windows.Forms.Timer timer;
    }
}
