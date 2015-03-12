namespace RenbarGUI.Forms
{
    partial class About_Form
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
            this.pb_logo = new System.Windows.Forms.PictureBox();
            this.Label_ProductName = new System.Windows.Forms.Label();
            this.Label_Version = new System.Windows.Forms.Label();
            this.Label_Copyright = new System.Windows.Forms.Label();
            this.LinkLabel_Help = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pb_logo)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_logo
            // 
            this.pb_logo.Image = global::RenbarGUI.Properties.Resources.sofastudio;
            this.pb_logo.Location = new System.Drawing.Point(1, 1);
            this.pb_logo.Name = "pb_logo";
            this.pb_logo.Size = new System.Drawing.Size(190, 88);
            this.pb_logo.TabIndex = 25;
            this.pb_logo.TabStop = false;
            // 
            // Label_ProductName
            // 
            this.Label_ProductName.AutoSize = true;
            this.Label_ProductName.Location = new System.Drawing.Point(193, 9);
            this.Label_ProductName.Name = "Label_ProductName";
            this.Label_ProductName.Size = new System.Drawing.Size(110, 13);
            this.Label_ProductName.TabIndex = 26;
            this.Label_ProductName.Text = "[Label_ProductName]";
            // 
            // Label_Version
            // 
            this.Label_Version.AutoSize = true;
            this.Label_Version.Location = new System.Drawing.Point(195, 34);
            this.Label_Version.Name = "Label_Version";
            this.Label_Version.Size = new System.Drawing.Size(81, 13);
            this.Label_Version.TabIndex = 27;
            this.Label_Version.Text = "[Label_Version]";
            // 
            // Label_Copyright
            // 
            this.Label_Copyright.AutoSize = true;
            this.Label_Copyright.Location = new System.Drawing.Point(190, 59);
            this.Label_Copyright.Name = "Label_Copyright";
            this.Label_Copyright.Size = new System.Drawing.Size(93, 13);
            this.Label_Copyright.TabIndex = 28;
            this.Label_Copyright.Text = "[Label_Copyright]";
            // 
            // LinkLabel_Help
            // 
            this.LinkLabel_Help.AutoSize = true;
            this.LinkLabel_Help.Location = new System.Drawing.Point(12, 116);
            this.LinkLabel_Help.Name = "LinkLabel_Help";
            this.LinkLabel_Help.Size = new System.Drawing.Size(85, 13);
            this.LinkLabel_Help.TabIndex = 29;
            this.LinkLabel_Help.TabStop = true;
            this.LinkLabel_Help.Text = "[LinkLabel_Help]";
            this.LinkLabel_Help.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ilbl_help_LinkClicked);
            // 
            // About_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 140);
            this.Controls.Add(this.LinkLabel_Help);
            this.Controls.Add(this.Label_Copyright);
            this.Controls.Add(this.Label_Version);
            this.Controls.Add(this.Label_ProductName);
            this.Controls.Add(this.pb_logo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About_Form";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[About_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.About_Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pb_logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_logo;
        private System.Windows.Forms.Label Label_ProductName;
        private System.Windows.Forms.Label Label_Version;
        private System.Windows.Forms.Label Label_Copyright;
        private System.Windows.Forms.LinkLabel LinkLabel_Help;
    }
}
