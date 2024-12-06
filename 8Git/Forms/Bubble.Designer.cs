namespace _8Git.Forms
{
    partial class Bubble
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bubble));
            timer = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // timer
            // 
            timer.Tick += timer_Tick;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(469, 172);
            label1.TabIndex = 0;
            label1.Text = "Text";
            label1.Click += Bubble_Click;
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(388, 138);
            button1.Name = "button1";
            button1.Size = new Size(93, 40);
            button1.TabIndex = 1;
            button1.Text = "Ok";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Bubble
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(502, 190);
            Controls.Add(button1);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Bubble";
            Text = "Bubble";
            Load += Bubble_Load;
            Click += Bubble_Click;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private Label label1;
        private Button button1;
    }
}