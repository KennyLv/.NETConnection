namespace RenderServerGUI
{
    partial class IP_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IP_Form));
            this.Label_Server = new System.Windows.Forms.Label();
            this.Label_Port = new System.Windows.Forms.Label();
            this.NumericUpDown_Port = new System.Windows.Forms.NumericUpDown();
            this.TextBox_Server = new System.Windows.Forms.TextBox();
            this.Label_Server_Help = new System.Windows.Forms.Label();
            this.Label_Port_Help = new System.Windows.Forms.Label();
            this.Button_Connect = new System.Windows.Forms.Button();
            this.Label_Connect = new System.Windows.Forms.Label();
            this.PictureBox_Connect = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Connect)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_Server
            // 
            this.Label_Server.AutoSize = true;
            this.Label_Server.Location = new System.Drawing.Point(10, 19);
            this.Label_Server.Name = "Label_Server";
            this.Label_Server.Size = new System.Drawing.Size(78, 13);
            this.Label_Server.TabIndex = 4;
            this.Label_Server.Text = "[Label_Server]";
            // 
            // Label_Port
            // 
            this.Label_Port.AutoSize = true;
            this.Label_Port.Location = new System.Drawing.Point(10, 99);
            this.Label_Port.Name = "Label_Port";
            this.Label_Port.Size = new System.Drawing.Size(66, 13);
            this.Label_Port.TabIndex = 5;
            this.Label_Port.Text = "[Label_Port]";
            // 
            // NumericUpDown_Port
            // 
            this.NumericUpDown_Port.Location = new System.Drawing.Point(110, 97);
            this.NumericUpDown_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumericUpDown_Port.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumericUpDown_Port.Name = "NumericUpDown_Port";
            this.NumericUpDown_Port.Size = new System.Drawing.Size(60, 21);
            this.NumericUpDown_Port.TabIndex = 1;
            this.NumericUpDown_Port.Value = new decimal(new int[] {
            6600,
            0,
            0,
            0});
            // 
            // TextBox_Server
            // 
            this.TextBox_Server.Location = new System.Drawing.Point(110, 16);
            this.TextBox_Server.Name = "TextBox_Server";
            this.TextBox_Server.Size = new System.Drawing.Size(110, 21);
            this.TextBox_Server.TabIndex = 0;
            // 
            // Label_Server_Help
            // 
            this.Label_Server_Help.AutoSize = true;
            this.Label_Server_Help.Location = new System.Drawing.Point(107, 51);
            this.Label_Server_Help.Name = "Label_Server_Help";
            this.Label_Server_Help.Size = new System.Drawing.Size(105, 13);
            this.Label_Server_Help.TabIndex = 6;
            this.Label_Server_Help.Text = "[Label_Server_Help]";
            // 
            // Label_Port_Help
            // 
            this.Label_Port_Help.AutoSize = true;
            this.Label_Port_Help.Location = new System.Drawing.Point(107, 131);
            this.Label_Port_Help.Name = "Label_Port_Help";
            this.Label_Port_Help.Size = new System.Drawing.Size(93, 13);
            this.Label_Port_Help.TabIndex = 7;
            this.Label_Port_Help.Text = "[Label_Port_Help]";
            // 
            // Button_Connect
            // 
            this.Button_Connect.Location = new System.Drawing.Point(227, 126);
            this.Button_Connect.Name = "Button_Connect";
            this.Button_Connect.Size = new System.Drawing.Size(75, 23);
            this.Button_Connect.TabIndex = 3;
            this.Button_Connect.Text = "連接";
            this.Button_Connect.UseVisualStyleBackColor = true;
            this.Button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // Label_Connect
            // 
            this.Label_Connect.AutoSize = true;
            this.Label_Connect.Location = new System.Drawing.Point(33, 168);
            this.Label_Connect.Name = "Label_Connect";
            this.Label_Connect.Size = new System.Drawing.Size(86, 13);
            this.Label_Connect.TabIndex = 8;
            this.Label_Connect.Text = "[Label_Connect]";
            this.Label_Connect.Visible = false;
            // 
            // PictureBox_Connect
            // 
            this.PictureBox_Connect.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox_Connect.Image")));
            this.PictureBox_Connect.Location = new System.Drawing.Point(12, 167);
            this.PictureBox_Connect.Name = "PictureBox_Connect";
            this.PictureBox_Connect.Size = new System.Drawing.Size(16, 16);
            this.PictureBox_Connect.TabIndex = 9;
            this.PictureBox_Connect.TabStop = false;
            this.PictureBox_Connect.Visible = false;
            // 
            // IP_Form
            // 
            this.AcceptButton = this.Button_Connect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 195);
            this.Controls.Add(this.PictureBox_Connect);
            this.Controls.Add(this.Label_Connect);
            this.Controls.Add(this.Button_Connect);
            this.Controls.Add(this.Label_Port_Help);
            this.Controls.Add(this.Label_Server_Help);
            this.Controls.Add(this.TextBox_Server);
            this.Controls.Add(this.NumericUpDown_Port);
            this.Controls.Add(this.Label_Port);
            this.Controls.Add(this.Label_Server);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IP_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[NetIP_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Net_Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Port)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Connect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Server;
        private System.Windows.Forms.Label Label_Port;
        private System.Windows.Forms.NumericUpDown NumericUpDown_Port;
        private System.Windows.Forms.TextBox TextBox_Server;
        private System.Windows.Forms.Label Label_Server_Help;
        private System.Windows.Forms.Label Label_Port_Help;
        private System.Windows.Forms.Button Button_Connect;
        private System.Windows.Forms.Label Label_Connect;
        private System.Windows.Forms.PictureBox PictureBox_Connect;
    }
}