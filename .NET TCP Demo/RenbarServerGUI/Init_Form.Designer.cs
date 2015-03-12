namespace RenbarServerGUI
{
    partial class Init_Form
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label_Product = new System.Windows.Forms.Label();
            this.Label_Version = new System.Windows.Forms.Label();
            this.ProgressBar_Status = new System.Windows.Forms.ProgressBar();
            this.Label_Status = new System.Windows.Forms.Label();
            this.Label_Copyright = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RenbarServerGUI.Properties.Resources.sofastudio;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(189, 87);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Label_Product
            // 
            this.Label_Product.AutoSize = true;
            this.Label_Product.Location = new System.Drawing.Point(197, 9);
            this.Label_Product.Name = "Label_Product";
            this.Label_Product.Size = new System.Drawing.Size(83, 13);
            this.Label_Product.TabIndex = 1;
            this.Label_Product.Text = "[Label_Product]";
            // 
            // Label_Version
            // 
            this.Label_Version.AutoSize = true;
            this.Label_Version.Location = new System.Drawing.Point(202, 40);
            this.Label_Version.Name = "Label_Version";
            this.Label_Version.Size = new System.Drawing.Size(81, 13);
            this.Label_Version.TabIndex = 2;
            this.Label_Version.Text = "[Label_Version]";
            // 
            // ProgressBar_Status
            // 
            this.ProgressBar_Status.Location = new System.Drawing.Point(12, 117);
            this.ProgressBar_Status.Name = "ProgressBar_Status";
            this.ProgressBar_Status.Size = new System.Drawing.Size(376, 10);
            this.ProgressBar_Status.TabIndex = 4;
            // 
            // Label_Status
            // 
            this.Label_Status.AutoSize = true;
            this.Label_Status.Location = new System.Drawing.Point(9, 101);
            this.Label_Status.Name = "Label_Status";
            this.Label_Status.Size = new System.Drawing.Size(77, 13);
            this.Label_Status.TabIndex = 5;
            this.Label_Status.Text = "[Label_Status]";
            // 
            // Label_Copyright
            // 
            this.Label_Copyright.AutoSize = true;
            this.Label_Copyright.Location = new System.Drawing.Point(197, 60);
            this.Label_Copyright.Name = "Label_Copyright";
            this.Label_Copyright.Size = new System.Drawing.Size(93, 13);
            this.Label_Copyright.TabIndex = 6;
            this.Label_Copyright.Text = "[Label_Copyright]";
            // 
            // Init_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 135);
            this.Controls.Add(this.Label_Copyright);
            this.Controls.Add(this.Label_Status);
            this.Controls.Add(this.ProgressBar_Status);
            this.Controls.Add(this.Label_Version);
            this.Controls.Add(this.Label_Product);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Init_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Init_Form";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Init_Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Init_Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Label_Product;
        private System.Windows.Forms.Label Label_Version;
        private System.Windows.Forms.ProgressBar ProgressBar_Status;
        private System.Windows.Forms.Label Label_Status;
        private System.Windows.Forms.Label Label_Copyright;
    }
}