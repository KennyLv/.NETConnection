namespace RenbarGUI.Forms
{
    partial class ChangeUser_Form
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
            this.GroupBox_User = new System.Windows.Forms.GroupBox();
            this.TextBox_Pwd = new System.Windows.Forms.TextBox();
            this.Label_Pwd = new System.Windows.Forms.Label();
            this.TextBox_Id = new System.Windows.Forms.TextBox();
            this.Label_Id = new System.Windows.Forms.Label();
            this.Button_OK = new System.Windows.Forms.Button();
            this.GroupBox_User.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_User
            // 
            this.GroupBox_User.Controls.Add(this.TextBox_Pwd);
            this.GroupBox_User.Controls.Add(this.Label_Pwd);
            this.GroupBox_User.Controls.Add(this.TextBox_Id);
            this.GroupBox_User.Controls.Add(this.Label_Id);
            this.GroupBox_User.Location = new System.Drawing.Point(12, 13);
            this.GroupBox_User.Name = "GroupBox_User";
            this.GroupBox_User.Size = new System.Drawing.Size(288, 100);
            this.GroupBox_User.TabIndex = 0;
            this.GroupBox_User.TabStop = false;
            this.GroupBox_User.Text = "[GroupBox_User]";
            // 
            // TextBox_Pwd
            // 
            this.TextBox_Pwd.Location = new System.Drawing.Point(121, 62);
            this.TextBox_Pwd.Name = "TextBox_Pwd";
            this.TextBox_Pwd.PasswordChar = '*';
            this.TextBox_Pwd.Size = new System.Drawing.Size(150, 21);
            this.TextBox_Pwd.TabIndex = 2;
            // 
            // Label_Pwd
            // 
            this.Label_Pwd.AutoSize = true;
            this.Label_Pwd.Location = new System.Drawing.Point(6, 65);
            this.Label_Pwd.Name = "Label_Pwd";
            this.Label_Pwd.Size = new System.Drawing.Size(66, 13);
            this.Label_Pwd.TabIndex = 7;
            this.Label_Pwd.Text = "[Label_Pwd]";
            // 
            // TextBox_Id
            // 
            this.TextBox_Id.Location = new System.Drawing.Point(121, 24);
            this.TextBox_Id.Name = "TextBox_Id";
            this.TextBox_Id.Size = new System.Drawing.Size(150, 21);
            this.TextBox_Id.TabIndex = 1;
            // 
            // Label_Id
            // 
            this.Label_Id.AutoSize = true;
            this.Label_Id.Location = new System.Drawing.Point(6, 27);
            this.Label_Id.Name = "Label_Id";
            this.Label_Id.Size = new System.Drawing.Size(56, 13);
            this.Label_Id.TabIndex = 6;
            this.Label_Id.Text = "[Label_Id]";
            // 
            // Button_OK
            // 
            this.Button_OK.Location = new System.Drawing.Point(225, 134);
            this.Button_OK.Name = "Button_OK";
            this.Button_OK.Size = new System.Drawing.Size(75, 25);
            this.Button_OK.TabIndex = 5;
            this.Button_OK.Text = "[Button_OK]";
            this.Button_OK.UseVisualStyleBackColor = true;
            this.Button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // ChangeUser_Form
            // 
            this.AcceptButton = this.Button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 170);
            this.Controls.Add(this.Button_OK);
            this.Controls.Add(this.GroupBox_User);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeUser_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[ChangeUser_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChangeUser_Form_FormClosing);
            this.GroupBox_User.ResumeLayout(false);
            this.GroupBox_User.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox_User;
        private System.Windows.Forms.Button Button_OK;
        private System.Windows.Forms.TextBox TextBox_Pwd;
        private System.Windows.Forms.Label Label_Pwd;
        private System.Windows.Forms.TextBox TextBox_Id;
        private System.Windows.Forms.Label Label_Id;
    }
}