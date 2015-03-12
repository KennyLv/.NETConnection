namespace RenderServerGUI
{
    partial class Net_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Net_Form));
            this.Label_Server = new System.Windows.Forms.Label();
            this.Label_Port = new System.Windows.Forms.Label();
            this.NumericUpDown_Port = new System.Windows.Forms.NumericUpDown();
            this.Label_Server_Help = new System.Windows.Forms.Label();
            this.Label_Port_Help = new System.Windows.Forms.Label();
            this.Button_AU = new System.Windows.Forms.Button();
            this.Label_Connect = new System.Windows.Forms.Label();
            this.PictureBox_Connect = new System.Windows.Forms.PictureBox();
            this.comboBox_Server = new System.Windows.Forms.ComboBox();
            this.checkBox_IsMaster = new System.Windows.Forms.CheckBox();
            this.Button_Delete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown_Port)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Connect)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_Server
            // 
            this.Label_Server.AutoSize = true;
            this.Label_Server.Location = new System.Drawing.Point(10, 20);
            this.Label_Server.Name = "Label_Server";
            this.Label_Server.Size = new System.Drawing.Size(78, 13);
            this.Label_Server.TabIndex = 4;
            this.Label_Server.Text = "[Label_Server]";
            // 
            // Label_Port
            // 
            this.Label_Port.AutoSize = true;
            this.Label_Port.Location = new System.Drawing.Point(10, 93);
            this.Label_Port.Name = "Label_Port";
            this.Label_Port.Size = new System.Drawing.Size(66, 13);
            this.Label_Port.TabIndex = 5;
            this.Label_Port.Text = "[Label_Port]";
            // 
            // NumericUpDown_Port
            // 
            this.NumericUpDown_Port.Location = new System.Drawing.Point(94, 89);
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
            this.NumericUpDown_Port.Size = new System.Drawing.Size(93, 21);
            this.NumericUpDown_Port.TabIndex = 3;
            this.NumericUpDown_Port.Value = new decimal(new int[] {
            6600,
            0,
            0,
            0});
            // 
            // Label_Server_Help
            // 
            this.Label_Server_Help.AutoSize = true;
            this.Label_Server_Help.Location = new System.Drawing.Point(94, 44);
            this.Label_Server_Help.Name = "Label_Server_Help";
            this.Label_Server_Help.Size = new System.Drawing.Size(105, 13);
            this.Label_Server_Help.TabIndex = 6;
            this.Label_Server_Help.Text = "[Label_Server_Help]";
            // 
            // Label_Port_Help
            // 
            this.Label_Port_Help.AutoSize = true;
            this.Label_Port_Help.Location = new System.Drawing.Point(94, 122);
            this.Label_Port_Help.Name = "Label_Port_Help";
            this.Label_Port_Help.Size = new System.Drawing.Size(93, 13);
            this.Label_Port_Help.TabIndex = 7;
            this.Label_Port_Help.Text = "[Label_Port_Help]";
            // 
            // Button_AU
            // 
            this.Button_AU.Location = new System.Drawing.Point(240, 161);
            this.Button_AU.Name = "Button_AU";
            this.Button_AU.Size = new System.Drawing.Size(99, 30);
            this.Button_AU.TabIndex = 5;
            this.Button_AU.Text = "添加或修改";
            this.Button_AU.UseVisualStyleBackColor = true;
            this.Button_AU.Click += new System.EventHandler(this.Button_AU_Click);
            // 
            // Label_Connect
            // 
            this.Label_Connect.AutoSize = true;
            this.Label_Connect.Location = new System.Drawing.Point(91, 166);
            this.Label_Connect.Name = "Label_Connect";
            this.Label_Connect.Size = new System.Drawing.Size(86, 13);
            this.Label_Connect.TabIndex = 8;
            this.Label_Connect.Text = "[Label_Connect]";
            this.Label_Connect.Visible = false;
            // 
            // PictureBox_Connect
            // 
            this.PictureBox_Connect.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox_Connect.Image")));
            this.PictureBox_Connect.Location = new System.Drawing.Point(45, 166);
            this.PictureBox_Connect.Name = "PictureBox_Connect";
            this.PictureBox_Connect.Size = new System.Drawing.Size(16, 16);
            this.PictureBox_Connect.TabIndex = 9;
            this.PictureBox_Connect.TabStop = false;
            this.PictureBox_Connect.Visible = false;
            // 
            // comboBox_Server
            // 
            this.comboBox_Server.FormattingEnabled = true;
            this.comboBox_Server.Location = new System.Drawing.Point(94, 16);
            this.comboBox_Server.Name = "comboBox_Server";
            this.comboBox_Server.Size = new System.Drawing.Size(126, 21);
            this.comboBox_Server.TabIndex = 1;
            this.comboBox_Server.SelectedIndexChanged += new System.EventHandler(this.comboBox_Server_SelectedIndexChanged);
            // 
            // checkBox_IsMaster
            // 
            this.checkBox_IsMaster.AutoSize = true;
            this.checkBox_IsMaster.Location = new System.Drawing.Point(240, 20);
            this.checkBox_IsMaster.Name = "checkBox_IsMaster";
            this.checkBox_IsMaster.Size = new System.Drawing.Size(71, 17);
            this.checkBox_IsMaster.TabIndex = 2;
            this.checkBox_IsMaster.Text = "Is Master";
            this.checkBox_IsMaster.UseVisualStyleBackColor = true;
            // 
            // Button_Delete
            // 
            this.Button_Delete.Location = new System.Drawing.Point(240, 105);
            this.Button_Delete.Name = "Button_Delete";
            this.Button_Delete.Size = new System.Drawing.Size(99, 30);
            this.Button_Delete.TabIndex = 4;
            this.Button_Delete.Text = "刪除";
            this.Button_Delete.UseVisualStyleBackColor = true;
            this.Button_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
            // 
            // Net_Form
            // 
            this.AcceptButton = this.Button_AU;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 203);
            this.Controls.Add(this.Button_Delete);
            this.Controls.Add(this.checkBox_IsMaster);
            this.Controls.Add(this.comboBox_Server);
            this.Controls.Add(this.PictureBox_Connect);
            this.Controls.Add(this.Label_Connect);
            this.Controls.Add(this.Button_AU);
            this.Controls.Add(this.Label_Port_Help);
            this.Controls.Add(this.Label_Server_Help);
            this.Controls.Add(this.NumericUpDown_Port);
            this.Controls.Add(this.Label_Port);
            this.Controls.Add(this.Label_Server);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Net_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[Net_Form]";
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
        private System.Windows.Forms.Label Label_Server_Help;
        private System.Windows.Forms.Label Label_Port_Help;
        private System.Windows.Forms.Button Button_AU;
        private System.Windows.Forms.Label Label_Connect;
        private System.Windows.Forms.PictureBox PictureBox_Connect;
        private System.Windows.Forms.ComboBox comboBox_Server;
        private System.Windows.Forms.CheckBox checkBox_IsMaster;
        private System.Windows.Forms.Button Button_Delete;
    }
}