namespace RenbarServerGUI
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
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.GroupBox_Jobs = new System.Windows.Forms.GroupBox();
            this.ListView_Job_Status = new System.Windows.Forms.ListView();
            this.Job_Group_Id = new System.Windows.Forms.ColumnHeader();
            this.WaitFor = new System.Windows.Forms.ColumnHeader();
            this.Job_Priority = new System.Windows.Forms.ColumnHeader();
            this.Job_Status = new System.Windows.Forms.ColumnHeader();
            this.Start_Time = new System.Windows.Forms.ColumnHeader();
            this.Finish_Time = new System.Windows.Forms.ColumnHeader();
            this.Submit_Acct = new System.Windows.Forms.ColumnHeader();
            this.Submit_Time = new System.Windows.Forms.ColumnHeader();
            this.GroupBox_Machines = new System.Windows.Forms.GroupBox();
            this.ListView_Machine_Status = new System.Windows.Forms.ListView();
            this.Machine_Id = new System.Windows.Forms.ColumnHeader();
            this.HostName = new System.Windows.Forms.ColumnHeader();
            this.Ip = new System.Windows.Forms.ColumnHeader();
            this.IsEnable = new System.Windows.Forms.ColumnHeader();
            this.Last_Online_Time = new System.Windows.Forms.ColumnHeader();
            this.Machine_Status = new System.Windows.Forms.ColumnHeader();
            this.Machine_Priority = new System.Windows.Forms.ColumnHeader();
            this.TCore = new System.Windows.Forms.ColumnHeader();
            this.UCore = new System.Windows.Forms.ColumnHeader();
            this.Note = new System.Windows.Forms.ColumnHeader();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.GroupBox_Jobs.SuspendLayout();
            this.GroupBox_Machines.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MainSplitContainer.Location = new System.Drawing.Point(12, 12);
            this.MainSplitContainer.Name = "MainSplitContainer";
            this.MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.GroupBox_Jobs);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.GroupBox_Machines);
            this.MainSplitContainer.Size = new System.Drawing.Size(893, 474);
            this.MainSplitContainer.SplitterDistance = 236;
            this.MainSplitContainer.TabIndex = 6;
            // 
            // GroupBox_Jobs
            // 
            this.GroupBox_Jobs.Controls.Add(this.ListView_Job_Status);
            this.GroupBox_Jobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox_Jobs.Location = new System.Drawing.Point(0, 0);
            this.GroupBox_Jobs.Name = "GroupBox_Jobs";
            this.GroupBox_Jobs.Size = new System.Drawing.Size(893, 236);
            this.GroupBox_Jobs.TabIndex = 0;
            this.GroupBox_Jobs.TabStop = false;
            this.GroupBox_Jobs.Text = "Jobs";
            // 
            // ListView_Job_Status
            // 
            this.ListView_Job_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView_Job_Status.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Job_Group_Id,
            this.WaitFor,
            this.Job_Priority,
            this.Job_Status,
            this.Start_Time,
            this.Finish_Time,
            this.Submit_Acct,
            this.Submit_Time});
            this.ListView_Job_Status.FullRowSelect = true;
            this.ListView_Job_Status.GridLines = true;
            this.ListView_Job_Status.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListView_Job_Status.Location = new System.Drawing.Point(6, 20);
            this.ListView_Job_Status.MultiSelect = false;
            this.ListView_Job_Status.Name = "ListView_Job_Status";
            this.ListView_Job_Status.Size = new System.Drawing.Size(879, 210);
            this.ListView_Job_Status.TabIndex = 6;
            this.ListView_Job_Status.UseCompatibleStateImageBehavior = false;
            this.ListView_Job_Status.View = System.Windows.Forms.View.Details;
            // 
            // Job_Group_Id
            // 
            this.Job_Group_Id.Text = "Job Group Id";
            this.Job_Group_Id.Width = 230;
            // 
            // WaitFor
            // 
            this.WaitFor.Text = "Wait For";
            this.WaitFor.Width = 230;
            // 
            // Job_Priority
            // 
            this.Job_Priority.Text = "Priority";
            this.Job_Priority.Width = 50;
            // 
            // Job_Status
            // 
            this.Job_Status.Text = "Status";
            this.Job_Status.Width = 100;
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
            // Submit_Acct
            // 
            this.Submit_Acct.Text = "Submit User";
            this.Submit_Acct.Width = 100;
            // 
            // Submit_Time
            // 
            this.Submit_Time.Text = "Submit Time";
            this.Submit_Time.Width = 140;
            // 
            // GroupBox_Machines
            // 
            this.GroupBox_Machines.Controls.Add(this.ListView_Machine_Status);
            this.GroupBox_Machines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox_Machines.Location = new System.Drawing.Point(0, 0);
            this.GroupBox_Machines.Name = "GroupBox_Machines";
            this.GroupBox_Machines.Size = new System.Drawing.Size(893, 234);
            this.GroupBox_Machines.TabIndex = 0;
            this.GroupBox_Machines.TabStop = false;
            this.GroupBox_Machines.Text = "Machines";
            // 
            // ListView_Machine_Status
            // 
            this.ListView_Machine_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ListView_Machine_Status.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Machine_Id,
            this.HostName,
            this.Ip,
            this.IsEnable,
            this.Last_Online_Time,
            this.Machine_Status,
            this.Machine_Priority,
            this.TCore,
            this.UCore,
            this.Note});
            this.ListView_Machine_Status.FullRowSelect = true;
            this.ListView_Machine_Status.GridLines = true;
            this.ListView_Machine_Status.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ListView_Machine_Status.Location = new System.Drawing.Point(6, 20);
            this.ListView_Machine_Status.MultiSelect = false;
            this.ListView_Machine_Status.Name = "ListView_Machine_Status";
            this.ListView_Machine_Status.Size = new System.Drawing.Size(879, 208);
            this.ListView_Machine_Status.TabIndex = 8;
            this.ListView_Machine_Status.UseCompatibleStateImageBehavior = false;
            this.ListView_Machine_Status.View = System.Windows.Forms.View.Details;
            // 
            // Machine_Id
            // 
            this.Machine_Id.Text = "Machine Id";
            this.Machine_Id.Width = 230;
            // 
            // HostName
            // 
            this.HostName.Text = "Host Name";
            this.HostName.Width = 120;
            // 
            // Ip
            // 
            this.Ip.Text = "Ip Address";
            this.Ip.Width = 100;
            // 
            // IsEnable
            // 
            this.IsEnable.Text = "IsEnable";
            // 
            // Last_Online_Time
            // 
            this.Last_Online_Time.Text = "Last Online Time";
            this.Last_Online_Time.Width = 140;
            // 
            // Machine_Status
            // 
            this.Machine_Status.Text = "Status";
            this.Machine_Status.Width = 100;
            // 
            // Machine_Priority
            // 
            this.Machine_Priority.Text = "Priority";
            this.Machine_Priority.Width = 50;
            // 
            // TCore
            // 
            this.TCore.Text = "Processor Core";
            this.TCore.Width = 100;
            // 
            // UCore
            // 
            this.UCore.Text = "Using Core";
            this.UCore.Width = 100;
            // 
            // Note
            // 
            this.Note.Text = "Note";
            this.Note.Width = 100;
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 498);
            this.Controls.Add(this.MainSplitContainer);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MinimumSize = new System.Drawing.Size(925, 525);
            this.Name = "Main_Form";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.FormClosed+=new System.Windows.Forms.FormClosedEventHandler(Main_Form_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            this.Resize += new System.EventHandler(this.Main_Form_Resize);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            this.MainSplitContainer.ResumeLayout(false);
            this.GroupBox_Jobs.ResumeLayout(false);
            this.GroupBox_Machines.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.GroupBox GroupBox_Jobs;
        private System.Windows.Forms.GroupBox GroupBox_Machines;
        private System.Windows.Forms.ListView ListView_Machine_Status;
        private System.Windows.Forms.ColumnHeader Machine_Id;
        private System.Windows.Forms.ColumnHeader HostName;
        private System.Windows.Forms.ColumnHeader Ip;
        private System.Windows.Forms.ColumnHeader IsEnable;
        private System.Windows.Forms.ColumnHeader Last_Online_Time;
        private System.Windows.Forms.ColumnHeader Machine_Status;
        private System.Windows.Forms.ColumnHeader Machine_Priority;
        private System.Windows.Forms.ColumnHeader Note;
        private System.Windows.Forms.ListView ListView_Job_Status;
        private System.Windows.Forms.ColumnHeader Job_Group_Id;
        private System.Windows.Forms.ColumnHeader WaitFor;
        private System.Windows.Forms.ColumnHeader Job_Priority;
        private System.Windows.Forms.ColumnHeader Job_Status;
        private System.Windows.Forms.ColumnHeader Start_Time;
        private System.Windows.Forms.ColumnHeader Finish_Time;
        private System.Windows.Forms.ColumnHeader TCore;
        private System.Windows.Forms.ColumnHeader UCore;
        private System.Windows.Forms.ColumnHeader Submit_Acct;
        private System.Windows.Forms.ColumnHeader Submit_Time;
    }
}