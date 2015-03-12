namespace RenderServerGUI
{
    partial class Main_Form
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
            this.ListView_Render_Status = new System.Windows.Forms.ListView();
            this.Job_Id = new System.Windows.Forms.ColumnHeader();
            this.Job_Group_Id = new System.Windows.Forms.ColumnHeader();
            this.Process_Id = new System.Windows.Forms.ColumnHeader();
            this.Process_Type = new System.Windows.Forms.ColumnHeader();
            this.Command = new System.Windows.Forms.ColumnHeader();
            this.Args = new System.Windows.Forms.ColumnHeader();
            this.Status = new System.Windows.Forms.ColumnHeader();
            this.Start_Time = new System.Windows.Forms.ColumnHeader();
            this.Finish_Time = new System.Windows.Forms.ColumnHeader();
            this.LinkLabel_ViewLog = new System.Windows.Forms.LinkLabel();
            this.CheckBox_Maintenance = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // ListView_Render_Status
            // 
            this.ListView_Render_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView_Render_Status.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Job_Id,
            this.Job_Group_Id,
            this.Process_Id,
            this.Process_Type,
            this.Command,
            this.Args,
            this.Status,
            this.Start_Time,
            this.Finish_Time});
            this.ListView_Render_Status.FullRowSelect = true;
            this.ListView_Render_Status.GridLines = true;
            this.ListView_Render_Status.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListView_Render_Status.Location = new System.Drawing.Point(0, 0);
            this.ListView_Render_Status.MultiSelect = false;
            this.ListView_Render_Status.Name = "ListView_Render_Status";
            this.ListView_Render_Status.Size = new System.Drawing.Size(932, 428);
            this.ListView_Render_Status.TabIndex = 6;
            this.ListView_Render_Status.UseCompatibleStateImageBehavior = false;
            this.ListView_Render_Status.View = System.Windows.Forms.View.Details;
            // 
            // Job_Id
            // 
            this.Job_Id.Text = "Job Id";
            this.Job_Id.Width = 70;
            // 
            // Job_Group_Id
            // 
            this.Job_Group_Id.Text = "Job Group Id";
            this.Job_Group_Id.Width = 230;
            // 
            // Process_Id
            // 
            this.Process_Id.Text = "Process Id";
            this.Process_Id.Width = 80;
            // 
            // Process_Type
            // 
            this.Process_Type.Text = "Process Type";
            this.Process_Type.Width = 100;
            // 
            // Command
            // 
            this.Command.Text = "Command";
            this.Command.Width = 210;
            // 
            // Args
            // 
            this.Args.Text = "Args";
            this.Args.Width = 100;
            // 
            // Status
            // 
            this.Status.Text = "Status";
            // 
            // Start_Time
            // 
            this.Start_Time.Text = "Start Time";
            this.Start_Time.Width = 140;
            // 
            // Finish_Time
            // 
            this.Finish_Time.Text = "Finish Time";
            this.Finish_Time.Width = 140;
            // 
            // LinkLabel_ViewLog
            // 
            this.LinkLabel_ViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LinkLabel_ViewLog.AutoSize = true;
            this.LinkLabel_ViewLog.Location = new System.Drawing.Point(776, 435);
            this.LinkLabel_ViewLog.Name = "LinkLabel_ViewLog";
            this.LinkLabel_ViewLog.Size = new System.Drawing.Size(144, 13);
            this.LinkLabel_ViewLog.TabIndex = 7;
            this.LinkLabel_ViewLog.TabStop = true;
            this.LinkLabel_ViewLog.Text = "Click Here View Log Message";
            this.LinkLabel_ViewLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_ViewLog_LinkClicked);
            // 
            // CheckBox_Maintenance
            // 
            this.CheckBox_Maintenance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CheckBox_Maintenance.AutoSize = true;
            this.CheckBox_Maintenance.Location = new System.Drawing.Point(12, 434);
            this.CheckBox_Maintenance.Name = "CheckBox_Maintenance";
            this.CheckBox_Maintenance.Size = new System.Drawing.Size(87, 17);
            this.CheckBox_Maintenance.TabIndex = 8;
            this.CheckBox_Maintenance.Text = "Maintenance";
            this.CheckBox_Maintenance.UseVisualStyleBackColor = true;
            this.CheckBox_Maintenance.CheckedChanged += new System.EventHandler(this.CheckBox_Maintenance_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(495, 434);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(112, 13);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click Here IP Message";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 453);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.CheckBox_Maintenance);
            this.Controls.Add(this.LinkLabel_ViewLog);
            this.Controls.Add(this.ListView_Render_Status);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "Main_Form";
            this.Text = "Main_Form";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ListView_Render_Status;
        private System.Windows.Forms.ColumnHeader Job_Group_Id;
        private System.Windows.Forms.ColumnHeader Job_Id;
        private System.Windows.Forms.ColumnHeader Command;
        private System.Windows.Forms.ColumnHeader Args;
        private System.Windows.Forms.ColumnHeader Start_Time;
        private System.Windows.Forms.ColumnHeader Finish_Time;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.ColumnHeader Process_Id;
        private System.Windows.Forms.ColumnHeader Process_Type;
        private System.Windows.Forms.LinkLabel LinkLabel_ViewLog;
        private System.Windows.Forms.CheckBox CheckBox_Maintenance;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}