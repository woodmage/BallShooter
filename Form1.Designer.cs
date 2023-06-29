namespace BallShooter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            statusline = new Label();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1024, 1000);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseMove += DoMouse;
            // 
            // statusline
            // 
            statusline.BorderStyle = BorderStyle.Fixed3D;
            statusline.Font = new Font("Segoe UI Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            statusline.Location = new Point(0, 1000);
            statusline.Name = "statusline";
            statusline.Size = new Size(1024, 40);
            statusline.TabIndex = 1;
            statusline.Text = "label1";
            statusline.UseMnemonic = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.FromArgb(0, 0, 15);
            pictureBox2.Location = new Point(1024, 0);
            pictureBox2.Margin = new Padding(0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(760, 1040);
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1784, 1041);
            Controls.Add(pictureBox2);
            Controls.Add(statusline);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Ball Shooter";
            Load += OnLoad;
            KeyUp += HandleKeys;
            Resize += OnResize;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Label statusline;
        private PictureBox pictureBox2;
    }
}