using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

#nullable disable

namespace _8Git.Forms
{
    public partial class Bubble : Form
    {
        // Import user32.dll to allow moving the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Bubble()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.LightBlue;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Opacity = 0.7;

            this.Region = new Region(CreateRoundedRectanglePath(new Rectangle(0, 0, this.Width, this.Height), 20));

            this.Resize += (s, e) =>
            {
                this.Region = new Region(CreateRoundedRectanglePath(new Rectangle(0, 0, this.Width, this.Height), 20));
            };

            this.MouseDown += CustomForm_MouseDown;
        }

        private void Bubble_Load(object sender, EventArgs e)
        {

        }

        public long tick = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            tick++;

            if (0 <= tick && tick < 50)
            {
                Opacity = 1;
            }

            if (50 <= tick && tick < 100)
            {
                Opacity -= 0.02;
            }

            if (Opacity-0.02<=0)
            {
                this.Close();
            }
        }

        private void Bubble_Click(object sender, EventArgs e)
        {
            Opacity = 1;
            tick = 0;
        }

        public void ShowBubble(int X, int Y, string text)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Left = X;
            this.Top = Y;
            this.label1.Text = text;
            this.TopMost = true;
            this.timer.Start();
            this.Show();

        }

        private void CustomForm_MouseDown(object sender, MouseEventArgs e)
        {
            Opacity = 1;
            tick = 0;

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90); // Top-left corner
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90); // Top-right corner
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right corner
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left corner
            path.CloseFigure();

            return path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
