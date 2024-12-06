namespace _8Git
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            notifyIcon = new NotifyIcon(components);
            contextMenuStripNotify = new ContextMenuStrip(components);
            showToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStripNotify.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = contextMenuStripNotify;
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "notifyIcon1";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += showToolStripMenuItem_Click;
            // 
            // contextMenuStripNotify
            // 
            contextMenuStripNotify.Items.AddRange(new ToolStripItem[] { showToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStripNotify.Name = "contextMenuStripNotify";
            contextMenuStripNotify.Size = new Size(112, 52);
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(111, 24);
            showToolStripMenuItem.Text = "Show";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(111, 24);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(100, 100);
            ContextMenuStrip = contextMenuStripNotify;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Opacity = 0D;
            Text = "8Git";
            Load += FormMain_Load;
            contextMenuStripNotify.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStripNotify;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem showToolStripMenuItem;
    }
}