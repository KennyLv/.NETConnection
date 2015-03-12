namespace RenbarGUI.Forms
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
            this.components = new System.ComponentModel.Container();
            this.Renbar_MenuBar = new System.Windows.Forms.MenuStrip();
            this.Menu_Queue = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Queue_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Queue_Job_New = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Queue_Job_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Job_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_Queue_Job_History = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Queue_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_Queue_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_ChangeUser = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_PoolMgr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_Settings_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_Settings_Record_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_Network = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_Lang = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_Lang_Eng = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Settings_Lang_Cht = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_About = new System.Windows.Forms.ToolStripMenuItem();
            this.Renbar_StatusBar = new System.Windows.Forms.StatusStrip();
            this.Status_Label_User = new System.Windows.Forms.ToolStripStatusLabel();
            this.Renbar_Container1 = new System.Windows.Forms.SplitContainer();
            this.GroupBox_Process = new System.Windows.Forms.GroupBox();
            this.ListView_Process = new System.Windows.Forms.ListView();
            this.Proc_Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Proc_View_Output = new System.Windows.Forms.ToolStripMenuItem();
            this.Renbar_Container2 = new System.Windows.Forms.SplitContainer();
            this.GroupBox_Queue = new System.Windows.Forms.GroupBox();
            this.ListView_Queue = new System.Windows.Forms.ListView();
            this.Queue_Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Queue_Update_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.Queue_Repeat_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.Queue_Pause_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.Queue_Delete_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Queue_SetPriority_Job = new System.Windows.Forms.ToolStripMenuItem();
            this.GroupBox_Host = new System.Windows.Forms.GroupBox();
            this.ListView_Host = new System.Windows.Forms.ListView();
            this.Host_Menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Host_Add_Machine = new System.Windows.Forms.ToolStripMenuItem();
            this.Host_Remove_Machine = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Line = new System.Windows.Forms.ToolStripSeparator();
            this.Host_Setting_Priority = new System.Windows.Forms.ToolStripMenuItem();
            this.Renbar_MenuBar.SuspendLayout();
            this.Renbar_StatusBar.SuspendLayout();
            this.Renbar_Container1.Panel1.SuspendLayout();
            this.Renbar_Container1.Panel2.SuspendLayout();
            this.Renbar_Container1.SuspendLayout();
            this.GroupBox_Process.SuspendLayout();
            this.Proc_Menu.SuspendLayout();
            this.Renbar_Container2.Panel1.SuspendLayout();
            this.Renbar_Container2.Panel2.SuspendLayout();
            this.Renbar_Container2.SuspendLayout();
            this.GroupBox_Queue.SuspendLayout();
            this.Queue_Menu.SuspendLayout();
            this.GroupBox_Host.SuspendLayout();
            this.Host_Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // Renbar_MenuBar
            // 
            this.Renbar_MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Queue,
            this.Menu_Settings,
            this.Menu_About});
            this.Renbar_MenuBar.Location = new System.Drawing.Point(0, 0);
            this.Renbar_MenuBar.Name = "Renbar_MenuBar";
            this.Renbar_MenuBar.Size = new System.Drawing.Size(1016, 24);
            this.Renbar_MenuBar.TabIndex = 0;
            this.Renbar_MenuBar.Text = "menuStrip1";
            // 
            // Menu_Queue
            // 
            this.Menu_Queue.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Queue_Job,
            this.toolStrip_Queue_Separator,
            this.Menu_Queue_Exit});
            this.Menu_Queue.Name = "Menu_Queue";
            this.Menu_Queue.Size = new System.Drawing.Size(88, 20);
            this.Menu_Queue.Text = "[Menu_Queue]";
            // 
            // Menu_Queue_Job
            // 
            this.Menu_Queue_Job.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Queue_Job_New,
            this.Menu_Queue_Job_Load,
            this.toolStrip_Job_Separator,
            this.Menu_Queue_Job_History});
            this.Menu_Queue_Job.Enabled = false;
            this.Menu_Queue_Job.Name = "Menu_Queue_Job";
            this.Menu_Queue_Job.Size = new System.Drawing.Size(203, 22);
            this.Menu_Queue_Job.Text = "[Menu_Queue_Job]";
            // 
            // Menu_Queue_Job_New
            // 
            this.Menu_Queue_Job_New.Name = "Menu_Queue_Job_New";
            this.Menu_Queue_Job_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.Menu_Queue_Job_New.Size = new System.Drawing.Size(240, 22);
            this.Menu_Queue_Job_New.Text = "[Menu_Queue_Job_New]";
            this.Menu_Queue_Job_New.Click += new System.EventHandler(this.Menu_Queue_Job_New_Click);
            // 
            // Menu_Queue_Job_Load
            // 
            this.Menu_Queue_Job_Load.Name = "Menu_Queue_Job_Load";
            this.Menu_Queue_Job_Load.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.Menu_Queue_Job_Load.Size = new System.Drawing.Size(240, 22);
            this.Menu_Queue_Job_Load.Text = "[Menu_Queue_Job_Load]";
            this.Menu_Queue_Job_Load.Click += new System.EventHandler(this.Menu_Queue_Job_Load_Click);
            // 
            // toolStrip_Job_Separator
            // 
            this.toolStrip_Job_Separator.Name = "toolStrip_Job_Separator";
            this.toolStrip_Job_Separator.Size = new System.Drawing.Size(237, 6);
            // 
            // Menu_Queue_Job_History
            // 
            this.Menu_Queue_Job_History.Name = "Menu_Queue_Job_History";
            this.Menu_Queue_Job_History.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.Menu_Queue_Job_History.Size = new System.Drawing.Size(240, 22);
            this.Menu_Queue_Job_History.Text = "[Menu_Queue_Job_History]";
            this.Menu_Queue_Job_History.Click += new System.EventHandler(this.Menu_Queue_Job_History_Click);
            // 
            // toolStrip_Queue_Separator
            // 
            this.toolStrip_Queue_Separator.Name = "toolStrip_Queue_Separator";
            this.toolStrip_Queue_Separator.Size = new System.Drawing.Size(200, 6);
            // 
            // Menu_Queue_Exit
            // 
            this.Menu_Queue_Exit.Name = "Menu_Queue_Exit";
            this.Menu_Queue_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.Menu_Queue_Exit.Size = new System.Drawing.Size(203, 22);
            this.Menu_Queue_Exit.Text = "[Menu_Queue_Exit]";
            this.Menu_Queue_Exit.Click += new System.EventHandler(this.Menu_Queue_Exit_Click);
            // 
            // Menu_Settings
            // 
            this.Menu_Settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Settings_ChangeUser,
            this.Menu_Settings_PoolMgr,
            this.toolStrip_Settings_Separator,
            this.Menu_Settings_Record_Option,
            this.Menu_Settings_Network,
            this.Menu_Settings_Lang});
            this.Menu_Settings.Name = "Menu_Settings";
            this.Menu_Settings.Size = new System.Drawing.Size(94, 20);
            this.Menu_Settings.Text = "[Menu_Settings]";
            // 
            // Menu_Settings_ChangeUser
            // 
            this.Menu_Settings_ChangeUser.Name = "Menu_Settings_ChangeUser";
            this.Menu_Settings_ChangeUser.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.Menu_Settings_ChangeUser.Size = new System.Drawing.Size(258, 22);
            this.Menu_Settings_ChangeUser.Text = "[Menu_Settings_ChangeUser]";
            this.Menu_Settings_ChangeUser.Click += new System.EventHandler(this.Menu_Settings_ChangeUser_Click);
            // 
            // Menu_Settings_PoolMgr
            // 
            this.Menu_Settings_PoolMgr.Enabled = false;
            this.Menu_Settings_PoolMgr.Name = "Menu_Settings_PoolMgr";
            this.Menu_Settings_PoolMgr.ShortcutKeyDisplayString = "";
            this.Menu_Settings_PoolMgr.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
            this.Menu_Settings_PoolMgr.Size = new System.Drawing.Size(258, 22);
            this.Menu_Settings_PoolMgr.Text = "[Menu_Settings_PoolMgr]";
            this.Menu_Settings_PoolMgr.Click += new System.EventHandler(this.Menu_Settings_PoolMgr_Click);
            // 
            // toolStrip_Settings_Separator
            // 
            this.toolStrip_Settings_Separator.Name = "toolStrip_Settings_Separator";
            this.toolStrip_Settings_Separator.Size = new System.Drawing.Size(255, 6);
            // 
            // Menu_Settings_Record_Option
            // 
            this.Menu_Settings_Record_Option.Checked = true;
            this.Menu_Settings_Record_Option.CheckOnClick = true;
            this.Menu_Settings_Record_Option.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_Settings_Record_Option.Name = "Menu_Settings_Record_Option";
            this.Menu_Settings_Record_Option.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.R)));
            this.Menu_Settings_Record_Option.Size = new System.Drawing.Size(258, 22);
            this.Menu_Settings_Record_Option.Text = "[Menu_Settings_Record]";
            this.Menu_Settings_Record_Option.Click += new System.EventHandler(this.Menu_Settings_Record_Option_Click);
            // 
            // Menu_Settings_Network
            // 
            this.Menu_Settings_Network.Name = "Menu_Settings_Network";
            this.Menu_Settings_Network.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.N)));
            this.Menu_Settings_Network.Size = new System.Drawing.Size(258, 22);
            this.Menu_Settings_Network.Text = "[Menu_Settings_Network]";
            this.Menu_Settings_Network.Click += new System.EventHandler(this.Menu_Settings_Network_Click);
            // 
            // Menu_Settings_Lang
            // 
            this.Menu_Settings_Lang.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Settings_Lang_Eng,
            this.Menu_Settings_Lang_Cht});
            this.Menu_Settings_Lang.Name = "Menu_Settings_Lang";
            this.Menu_Settings_Lang.Size = new System.Drawing.Size(258, 22);
            this.Menu_Settings_Lang.Text = "[Menu_Settings_Lang]";
            // 
            // Menu_Settings_Lang_Eng
            // 
            this.Menu_Settings_Lang_Eng.Checked = true;
            this.Menu_Settings_Lang_Eng.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_Settings_Lang_Eng.Name = "Menu_Settings_Lang_Eng";
            this.Menu_Settings_Lang_Eng.Size = new System.Drawing.Size(227, 22);
            this.Menu_Settings_Lang_Eng.Text = "Menu_Settings_Lang_Eng[en-us]";
            this.Menu_Settings_Lang_Eng.Click += new System.EventHandler(this.Menu_Settings_Lang_Eng_Click);
            // 
            // Menu_Settings_Lang_Cht
            // 
            this.Menu_Settings_Lang_Cht.Name = "Menu_Settings_Lang_Cht";
            this.Menu_Settings_Lang_Cht.Size = new System.Drawing.Size(227, 22);
            this.Menu_Settings_Lang_Cht.Text = "Menu_Settings_Lang_Cht[zh-tw]";
            this.Menu_Settings_Lang_Cht.Click += new System.EventHandler(this.Menu_Settings_Lang_Cht_Click);
            // 
            // Menu_About
            // 
            this.Menu_About.Name = "Menu_About";
            this.Menu_About.Size = new System.Drawing.Size(87, 20);
            this.Menu_About.Text = "[Menu_About]";
            this.Menu_About.Click += new System.EventHandler(this.Menu_About_Click);
            // 
            // Renbar_StatusBar
            // 
            this.Renbar_StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status_Label_User});
            this.Renbar_StatusBar.Location = new System.Drawing.Point(0, 719);
            this.Renbar_StatusBar.Name = "Renbar_StatusBar";
            this.Renbar_StatusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.Renbar_StatusBar.Size = new System.Drawing.Size(1016, 22);
            this.Renbar_StatusBar.TabIndex = 1;
            this.Renbar_StatusBar.Text = "statusStrip1";
            // 
            // Status_Label_User
            // 
            this.Status_Label_User.Name = "Status_Label_User";
            this.Status_Label_User.Size = new System.Drawing.Size(99, 17);
            this.Status_Label_User.Text = "[Status_Label_User]";
            // 
            // Renbar_Container1
            // 
            this.Renbar_Container1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Renbar_Container1.Location = new System.Drawing.Point(0, 24);
            this.Renbar_Container1.Name = "Renbar_Container1";
            this.Renbar_Container1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Renbar_Container1.Panel1
            // 
            this.Renbar_Container1.Panel1.Controls.Add(this.GroupBox_Process);
            // 
            // Renbar_Container1.Panel2
            // 
            this.Renbar_Container1.Panel2.Controls.Add(this.Renbar_Container2);
            this.Renbar_Container1.Panel2MinSize = 120;
            this.Renbar_Container1.Size = new System.Drawing.Size(1016, 695);
            this.Renbar_Container1.SplitterDistance = 219;
            this.Renbar_Container1.TabIndex = 2;
            // 
            // GroupBox_Process
            // 
            this.GroupBox_Process.Controls.Add(this.ListView_Process);
            this.GroupBox_Process.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox_Process.Location = new System.Drawing.Point(0, 0);
            this.GroupBox_Process.Name = "GroupBox_Process";
            this.GroupBox_Process.Size = new System.Drawing.Size(1016, 219);
            this.GroupBox_Process.TabIndex = 4;
            this.GroupBox_Process.TabStop = false;
            this.GroupBox_Process.Text = "[GroupBox_Process]";
            // 
            // ListView_Process
            // 
            this.ListView_Process.ContextMenuStrip = this.Proc_Menu;
            this.ListView_Process.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListView_Process.Location = new System.Drawing.Point(3, 17);
            this.ListView_Process.MultiSelect = false;
            this.ListView_Process.Name = "ListView_Process";
            this.ListView_Process.Size = new System.Drawing.Size(1010, 199);
            this.ListView_Process.TabIndex = 0;
            this.ListView_Process.UseCompatibleStateImageBehavior = false;
            this.ListView_Process.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_Process_ColumnClick);
            this.ListView_Process.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView_Process_MouseDown);
            // 
            // Proc_Menu
            // 
            this.Proc_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Proc_View_Output});
            this.Proc_Menu.Name = "Proc_Menu";
            this.Proc_Menu.ShowImageMargin = false;
            this.Proc_Menu.Size = new System.Drawing.Size(143, 26);
            // 
            // Proc_View_Output
            // 
            this.Proc_View_Output.Name = "Proc_View_Output";
            this.Proc_View_Output.ShortcutKeyDisplayString = "";
            this.Proc_View_Output.Size = new System.Drawing.Size(142, 22);
            this.Proc_View_Output.Text = "[Proc_View_Output]";
            this.Proc_View_Output.Visible = false;
            this.Proc_View_Output.Click += new System.EventHandler(this.Proc_View_Output_Click);
            // 
            // Renbar_Container2
            // 
            this.Renbar_Container2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Renbar_Container2.Location = new System.Drawing.Point(0, 0);
            this.Renbar_Container2.Name = "Renbar_Container2";
            this.Renbar_Container2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Renbar_Container2.Panel1
            // 
            this.Renbar_Container2.Panel1.Controls.Add(this.GroupBox_Queue);
            // 
            // Renbar_Container2.Panel2
            // 
            this.Renbar_Container2.Panel2.Controls.Add(this.GroupBox_Host);
            this.Renbar_Container2.Panel2MinSize = 60;
            this.Renbar_Container2.Size = new System.Drawing.Size(1016, 472);
            this.Renbar_Container2.SplitterDistance = 239;
            this.Renbar_Container2.TabIndex = 0;
            // 
            // GroupBox_Queue
            // 
            this.GroupBox_Queue.Controls.Add(this.ListView_Queue);
            this.GroupBox_Queue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox_Queue.Location = new System.Drawing.Point(0, 0);
            this.GroupBox_Queue.Name = "GroupBox_Queue";
            this.GroupBox_Queue.Size = new System.Drawing.Size(1016, 239);
            this.GroupBox_Queue.TabIndex = 5;
            this.GroupBox_Queue.TabStop = false;
            this.GroupBox_Queue.Text = "[GroupBox_Queue]";
            // 
            // ListView_Queue
            // 
            this.ListView_Queue.AllowDrop = true;
            this.ListView_Queue.ContextMenuStrip = this.Queue_Menu;
            this.ListView_Queue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListView_Queue.Location = new System.Drawing.Point(3, 17);
            this.ListView_Queue.Name = "ListView_Queue";
            this.ListView_Queue.Size = new System.Drawing.Size(1010, 219);
            this.ListView_Queue.TabIndex = 0;
            this.ListView_Queue.UseCompatibleStateImageBehavior = false;
            this.ListView_Queue.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_Queue_DragDrop);
            this.ListView_Queue.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_Queue_ColumnClick);
            this.ListView_Queue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView_Queue_MouseDown);
            this.ListView_Queue.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView_Queue_DragEnter);
            // 
            // Queue_Menu
            // 
            this.Queue_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Queue_Update_Job,
            this.Queue_Repeat_Job,
            this.Queue_Pause_Job,
            this.Queue_Delete_Job,
            this.toolStripSeparator1,
            this.Queue_SetPriority_Job});
            this.Queue_Menu.Name = "Queue_Menu";
            this.Queue_Menu.Size = new System.Drawing.Size(187, 120);
            // 
            // Queue_Update_Job
            // 
            this.Queue_Update_Job.Name = "Queue_Update_Job";
            this.Queue_Update_Job.ShortcutKeyDisplayString = "";
            this.Queue_Update_Job.Size = new System.Drawing.Size(186, 22);
            this.Queue_Update_Job.Text = "[Queue_Update_Job]";
            this.Queue_Update_Job.Visible = false;
            this.Queue_Update_Job.Click += new System.EventHandler(this.Queue_Update_Job_Click);
            // 
            // Queue_Repeat_Job
            // 
            this.Queue_Repeat_Job.Name = "Queue_Repeat_Job";
            this.Queue_Repeat_Job.Size = new System.Drawing.Size(186, 22);
            this.Queue_Repeat_Job.Text = "[Queue_Repeat_Job]";
            this.Queue_Repeat_Job.Visible = false;
            this.Queue_Repeat_Job.Click += new System.EventHandler(this.Queue_Repeat_Job_Click);
            // 
            // Queue_Pause_Job
            // 
            this.Queue_Pause_Job.Name = "Queue_Pause_Job";
            this.Queue_Pause_Job.Size = new System.Drawing.Size(186, 22);
            this.Queue_Pause_Job.Text = "[Queue_Pause_Job]";
            this.Queue_Pause_Job.Visible = false;
            this.Queue_Pause_Job.Click += new System.EventHandler(this.Queue_Pause_Job_Click);
            // 
            // Queue_Delete_Job
            // 
            this.Queue_Delete_Job.Name = "Queue_Delete_Job";
            this.Queue_Delete_Job.ShortcutKeyDisplayString = "Del";
            this.Queue_Delete_Job.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.Queue_Delete_Job.Size = new System.Drawing.Size(186, 22);
            this.Queue_Delete_Job.Text = "[Queue_Delete_Job]";
            this.Queue_Delete_Job.Visible = false;
            this.Queue_Delete_Job.Click += new System.EventHandler(this.Queue_Delete_Job_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // Queue_SetPriority_Job
            // 
            this.Queue_SetPriority_Job.Name = "Queue_SetPriority_Job";
            this.Queue_SetPriority_Job.Size = new System.Drawing.Size(186, 22);
            this.Queue_SetPriority_Job.Text = "[Queue_SetPriority_Job]";
            this.Queue_SetPriority_Job.Visible = false;
            this.Queue_SetPriority_Job.Click += new System.EventHandler(this.Queue_SetPriority_Job_Click);
            // 
            // GroupBox_Host
            // 
            this.GroupBox_Host.Controls.Add(this.ListView_Host);
            this.GroupBox_Host.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox_Host.Location = new System.Drawing.Point(0, 0);
            this.GroupBox_Host.Name = "GroupBox_Host";
            this.GroupBox_Host.Size = new System.Drawing.Size(1016, 229);
            this.GroupBox_Host.TabIndex = 6;
            this.GroupBox_Host.TabStop = false;
            this.GroupBox_Host.Text = "[GroupBox_Host]";
            // 
            // ListView_Host
            // 
            this.ListView_Host.ContextMenuStrip = this.Host_Menu;
            this.ListView_Host.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListView_Host.Location = new System.Drawing.Point(3, 17);
            this.ListView_Host.Name = "ListView_Host";
            this.ListView_Host.Size = new System.Drawing.Size(1010, 209);
            this.ListView_Host.TabIndex = 0;
            this.ListView_Host.UseCompatibleStateImageBehavior = false;
            this.ListView_Host.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_Host_ColumnClick);
            this.ListView_Host.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView_Host_MouseDown);
            // 
            // Host_Menu
            // 
            this.Host_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Host_Add_Machine,
            this.Host_Remove_Machine,
            this.MenuItem_Line,
            this.Host_Setting_Priority});
            this.Host_Menu.Name = "Host_Menu";
            this.Host_Menu.Size = new System.Drawing.Size(191, 76);
            // 
            // Host_Add_Machine
            // 
            this.Host_Add_Machine.Name = "Host_Add_Machine";
            this.Host_Add_Machine.Size = new System.Drawing.Size(190, 22);
            this.Host_Add_Machine.Text = "[Host_Add_Machine]";
            this.Host_Add_Machine.Visible = false;
            this.Host_Add_Machine.Click += new System.EventHandler(this.Host_Add_Machine_Click);
            // 
            // Host_Remove_Machine
            // 
            this.Host_Remove_Machine.Name = "Host_Remove_Machine";
            this.Host_Remove_Machine.Size = new System.Drawing.Size(190, 22);
            this.Host_Remove_Machine.Text = "[Host_Remove_Machine]";
            this.Host_Remove_Machine.Visible = false;
            this.Host_Remove_Machine.Click += new System.EventHandler(this.Host_Remove_Machine_Click);
            // 
            // MenuItem_Line
            // 
            this.MenuItem_Line.Name = "MenuItem_Line";
            this.MenuItem_Line.Size = new System.Drawing.Size(187, 6);
            // 
            // Host_Setting_Priority
            // 
            this.Host_Setting_Priority.Name = "Host_Setting_Priority";
            this.Host_Setting_Priority.Size = new System.Drawing.Size(190, 22);
            this.Host_Setting_Priority.Text = "[Host_Setting_Priority]";
            this.Host_Setting_Priority.Click += new System.EventHandler(this.Host_Setting_Priority_Click);
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 741);
            this.Controls.Add(this.Renbar_Container1);
            this.Controls.Add(this.Renbar_StatusBar);
            this.Controls.Add(this.Renbar_MenuBar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.KeyPreview = true;
            this.MainMenuStrip = this.Renbar_MenuBar;
            this.MinimumSize = new System.Drawing.Size(800, 650);
            this.Name = "Main_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "[Main_Form]";
            this.Load += new System.EventHandler(this.Main_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            this.Resize += new System.EventHandler(this.Main_Form_Resize);
            this.Renbar_MenuBar.ResumeLayout(false);
            this.Renbar_MenuBar.PerformLayout();
            this.Renbar_StatusBar.ResumeLayout(false);
            this.Renbar_StatusBar.PerformLayout();
            this.Renbar_Container1.Panel1.ResumeLayout(false);
            this.Renbar_Container1.Panel2.ResumeLayout(false);
            this.Renbar_Container1.ResumeLayout(false);
            this.GroupBox_Process.ResumeLayout(false);
            this.Proc_Menu.ResumeLayout(false);
            this.Renbar_Container2.Panel1.ResumeLayout(false);
            this.Renbar_Container2.Panel2.ResumeLayout(false);
            this.Renbar_Container2.ResumeLayout(false);
            this.GroupBox_Queue.ResumeLayout(false);
            this.Queue_Menu.ResumeLayout(false);
            this.GroupBox_Host.ResumeLayout(false);
            this.Host_Menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Renbar_MenuBar;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_PoolMgr;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue_Job;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue_Job_New;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue_Job_Load;
        private System.Windows.Forms.ToolStripSeparator toolStrip_Job_Separator;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue_Job_History;
        private System.Windows.Forms.ToolStripSeparator toolStrip_Queue_Separator;
        private System.Windows.Forms.ToolStripMenuItem Menu_Queue_Exit;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_ChangeUser;
        private System.Windows.Forms.ToolStripSeparator toolStrip_Settings_Separator;
        private System.Windows.Forms.ToolStripMenuItem Menu_About;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_Lang;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_Lang_Eng;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_Lang_Cht;
        private System.Windows.Forms.StatusStrip Renbar_StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel Status_Label_User;
        private System.Windows.Forms.SplitContainer Renbar_Container1;
        private System.Windows.Forms.SplitContainer Renbar_Container2;
        private System.Windows.Forms.GroupBox GroupBox_Process;
        private System.Windows.Forms.ListView ListView_Process;
        private System.Windows.Forms.GroupBox GroupBox_Queue;
        private System.Windows.Forms.ListView ListView_Queue;
        private System.Windows.Forms.GroupBox GroupBox_Host;
        private System.Windows.Forms.ListView ListView_Host;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_Record_Option;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings_Network;
        private System.Windows.Forms.ContextMenuStrip Proc_Menu;
        private System.Windows.Forms.ToolStripMenuItem Proc_View_Output;
        private System.Windows.Forms.ContextMenuStrip Queue_Menu;
        private System.Windows.Forms.ToolStripMenuItem Queue_Update_Job;
        private System.Windows.Forms.ToolStripMenuItem Queue_Delete_Job;
        private System.Windows.Forms.ContextMenuStrip Host_Menu;
        private System.Windows.Forms.ToolStripMenuItem Host_Add_Machine;
        private System.Windows.Forms.ToolStripMenuItem Host_Remove_Machine;
        private System.Windows.Forms.ToolStripSeparator MenuItem_Line;
        private System.Windows.Forms.ToolStripMenuItem Host_Setting_Priority;
        private System.Windows.Forms.ToolStripMenuItem Queue_SetPriority_Job;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Queue_Pause_Job;
        private System.Windows.Forms.ToolStripMenuItem Queue_Repeat_Job;
    }
}