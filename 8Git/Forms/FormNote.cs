using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

#nullable disable

namespace _8Git.Forms
{
    public partial class FormNote : Form
    {
        long ticks = 0;

        private ElementHost elementHost;
        private TextEditor textEditor;

        public TreeData node;

        public long lastChangeTick = 0;

        public FormNote(TreeData node)
        {
            this.node = node;

            InitializeComponent();

            this.Text = node.name;

            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize ElementHost
            elementHost = new ElementHost
            {
                Dock = DockStyle.Fill
            };


            textEditor = new TextEditor
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 22,
                ShowLineNumbers = true,
                HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible
            };

            elementHost.Child = textEditor;

            this.Controls.Add(elementHost);

            textEditor.Text = node.content;

            textEditor.TextChanged += TextChangedEvent;

        }

        private void TextChangedEvent(object sender, EventArgs e)
        {
            lastChangeTick = ticks;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ticks++;

            if (lastChangeTick > 0 && (ticks-lastChangeTick>5)) {
                node.content = textEditor.Text;
                lastChangeTick = 0;
            }

        }

        private void FormNote_Load(object sender, EventArgs e)
        {

        }


        private void FormNote_FormClosing(object sender, FormClosingEventArgs e)
        {
            node.content = textEditor.Text;
            Program.form8Git.CloseNode(this.node);
        }

    }
}
