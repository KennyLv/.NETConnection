namespace RenbarGUI.Forms
{
    partial class PoolMgr_Form
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
            this.GroupBox_Pool_Groups = new System.Windows.Forms.GroupBox();
            this.CheckBox_Sharable = new System.Windows.Forms.CheckBox();
            this.Label_Member_List = new System.Windows.Forms.Label();
            this.Label_Pool_Name = new System.Windows.Forms.Label();
            this.Label_Machine_List = new System.Windows.Forms.Label();
            this.ComboBox_Pool_Name = new System.Windows.Forms.ComboBox();
            this.Button_Del_Member = new System.Windows.Forms.Button();
            this.Button_Add_Member = new System.Windows.Forms.Button();
            this.ListBox_Member = new System.Windows.Forms.ListBox();
            this.ListBox_Machine = new System.Windows.Forms.ListBox();
            this.Button_Delete_Pool = new System.Windows.Forms.Button();
            this.Button_Update_Pool = new System.Windows.Forms.Button();
            this.Button_Add_Pool = new System.Windows.Forms.Button();
            this.GroupBox_Pool_Controls = new System.Windows.Forms.GroupBox();
            this.GroupBox_Pool_Groups.SuspendLayout();
            this.GroupBox_Pool_Controls.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_Pool_Groups
            // 
            this.GroupBox_Pool_Groups.Controls.Add(this.CheckBox_Sharable);
            this.GroupBox_Pool_Groups.Controls.Add(this.Label_Member_List);
            this.GroupBox_Pool_Groups.Controls.Add(this.Label_Pool_Name);
            this.GroupBox_Pool_Groups.Controls.Add(this.Label_Machine_List);
            this.GroupBox_Pool_Groups.Controls.Add(this.ComboBox_Pool_Name);
            this.GroupBox_Pool_Groups.Controls.Add(this.Button_Del_Member);
            this.GroupBox_Pool_Groups.Controls.Add(this.Button_Add_Member);
            this.GroupBox_Pool_Groups.Controls.Add(this.ListBox_Member);
            this.GroupBox_Pool_Groups.Controls.Add(this.ListBox_Machine);
            this.GroupBox_Pool_Groups.Location = new System.Drawing.Point(12, 12);
            this.GroupBox_Pool_Groups.Name = "GroupBox_Pool_Groups";
            this.GroupBox_Pool_Groups.Size = new System.Drawing.Size(488, 431);
            this.GroupBox_Pool_Groups.TabIndex = 0;
            this.GroupBox_Pool_Groups.TabStop = false;
            this.GroupBox_Pool_Groups.Text = "[GroupBox_Pool_Groups]";
            // 
            // CheckBox_Sharable
            // 
            this.CheckBox_Sharable.AutoSize = true;
            this.CheckBox_Sharable.Location = new System.Drawing.Point(302, 69);
            this.CheckBox_Sharable.Name = "CheckBox_Sharable";
            this.CheckBox_Sharable.Size = new System.Drawing.Size(129, 17);
            this.CheckBox_Sharable.TabIndex = 2;
            this.CheckBox_Sharable.Text = "[CheckBox_Sharable]";
            this.CheckBox_Sharable.UseVisualStyleBackColor = true;
            // 
            // Label_Member_List
            // 
            this.Label_Member_List.AutoSize = true;
            this.Label_Member_List.Location = new System.Drawing.Point(299, 91);
            this.Label_Member_List.Name = "Label_Member_List";
            this.Label_Member_List.Size = new System.Drawing.Size(106, 13);
            this.Label_Member_List.TabIndex = 7;
            this.Label_Member_List.Text = "[Label_Member_List]";
            // 
            // Label_Pool_Name
            // 
            this.Label_Pool_Name.AutoSize = true;
            this.Label_Pool_Name.Location = new System.Drawing.Point(299, 26);
            this.Label_Pool_Name.Name = "Label_Pool_Name";
            this.Label_Pool_Name.Size = new System.Drawing.Size(99, 13);
            this.Label_Pool_Name.TabIndex = 6;
            this.Label_Pool_Name.Text = "[Label_Pool_Name]";
            // 
            // Label_Machine_List
            // 
            this.Label_Machine_List.AutoSize = true;
            this.Label_Machine_List.Location = new System.Drawing.Point(7, 27);
            this.Label_Machine_List.Name = "Label_Machine_List";
            this.Label_Machine_List.Size = new System.Drawing.Size(107, 13);
            this.Label_Machine_List.TabIndex = 5;
            this.Label_Machine_List.Text = "[Label_Machine_List]";
            // 
            // ComboBox_Pool_Name
            // 
            this.ComboBox_Pool_Name.FormattingEnabled = true;
            this.ComboBox_Pool_Name.Location = new System.Drawing.Point(302, 42);
            this.ComboBox_Pool_Name.Name = "ComboBox_Pool_Name";
            this.ComboBox_Pool_Name.Size = new System.Drawing.Size(180, 21);
            this.ComboBox_Pool_Name.TabIndex = 1;
            this.ComboBox_Pool_Name.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Pool_Name_SelectedIndexChanged);
            // 
            // Button_Del_Member
            // 
            this.Button_Del_Member.Location = new System.Drawing.Point(196, 285);
            this.Button_Del_Member.Name = "Button_Del_Member";
            this.Button_Del_Member.Size = new System.Drawing.Size(100, 23);
            this.Button_Del_Member.TabIndex = 5;
            this.Button_Del_Member.Text = "[Button_Del_Member]";
            this.Button_Del_Member.UseVisualStyleBackColor = true;
            this.Button_Del_Member.Click += new System.EventHandler(this.Button_Del_Member_Click);
            // 
            // Button_Add_Member
            // 
            this.Button_Add_Member.Location = new System.Drawing.Point(196, 123);
            this.Button_Add_Member.Name = "Button_Add_Member";
            this.Button_Add_Member.Size = new System.Drawing.Size(100, 23);
            this.Button_Add_Member.TabIndex = 4;
            this.Button_Add_Member.Text = "[Button_Add_Member]";
            this.Button_Add_Member.UseVisualStyleBackColor = true;
            this.Button_Add_Member.Click += new System.EventHandler(this.Button_Add_Member_Click);
            // 
            // ListBox_Member
            // 
            this.ListBox_Member.FormattingEnabled = true;
            this.ListBox_Member.Location = new System.Drawing.Point(302, 107);
            this.ListBox_Member.Name = "ListBox_Member";
            this.ListBox_Member.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ListBox_Member.Size = new System.Drawing.Size(180, 316);
            this.ListBox_Member.TabIndex = 6;
            // 
            // ListBox_Machine
            // 
            this.ListBox_Machine.FormattingEnabled = true;
            this.ListBox_Machine.Location = new System.Drawing.Point(10, 42);
            this.ListBox_Machine.Name = "ListBox_Machine";
            this.ListBox_Machine.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ListBox_Machine.Size = new System.Drawing.Size(180, 381);
            this.ListBox_Machine.TabIndex = 3;
            // 
            // Button_Delete_Pool
            // 
            this.Button_Delete_Pool.Location = new System.Drawing.Point(13, 78);
            this.Button_Delete_Pool.Name = "Button_Delete_Pool";
            this.Button_Delete_Pool.Size = new System.Drawing.Size(90, 23);
            this.Button_Delete_Pool.TabIndex = 9;
            this.Button_Delete_Pool.Text = "[Button_Delete_Pool]";
            this.Button_Delete_Pool.UseVisualStyleBackColor = true;
            this.Button_Delete_Pool.Click += new System.EventHandler(this.Button_Delete_Pool_Click);
            // 
            // Button_Update_Pool
            // 
            this.Button_Update_Pool.Location = new System.Drawing.Point(13, 49);
            this.Button_Update_Pool.Name = "Button_Update_Pool";
            this.Button_Update_Pool.Size = new System.Drawing.Size(90, 23);
            this.Button_Update_Pool.TabIndex = 8;
            this.Button_Update_Pool.Text = "[Button_Update_Pool]";
            this.Button_Update_Pool.UseVisualStyleBackColor = true;
            this.Button_Update_Pool.Click += new System.EventHandler(this.Button_Update_Pool_Click);
            // 
            // Button_Add_Pool
            // 
            this.Button_Add_Pool.Location = new System.Drawing.Point(13, 20);
            this.Button_Add_Pool.Name = "Button_Add_Pool";
            this.Button_Add_Pool.Size = new System.Drawing.Size(90, 23);
            this.Button_Add_Pool.TabIndex = 7;
            this.Button_Add_Pool.Text = "[Button_Add_Pool]";
            this.Button_Add_Pool.UseVisualStyleBackColor = true;
            this.Button_Add_Pool.Click += new System.EventHandler(this.Button_Add_Pool_Click);
            // 
            // GroupBox_Pool_Controls
            // 
            this.GroupBox_Pool_Controls.Controls.Add(this.Button_Add_Pool);
            this.GroupBox_Pool_Controls.Controls.Add(this.Button_Delete_Pool);
            this.GroupBox_Pool_Controls.Controls.Add(this.Button_Update_Pool);
            this.GroupBox_Pool_Controls.Location = new System.Drawing.Point(506, 12);
            this.GroupBox_Pool_Controls.Name = "GroupBox_Pool_Controls";
            this.GroupBox_Pool_Controls.Size = new System.Drawing.Size(116, 115);
            this.GroupBox_Pool_Controls.TabIndex = 10;
            this.GroupBox_Pool_Controls.TabStop = false;
            this.GroupBox_Pool_Controls.Text = "[GroupBox_Pool_Controls]";
            // 
            // PoolMgr_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 455);
            this.Controls.Add(this.GroupBox_Pool_Controls);
            this.Controls.Add(this.GroupBox_Pool_Groups);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PoolMgr_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[PoolMgr_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PoolMgr_Form_FormClosing);
            this.GroupBox_Pool_Groups.ResumeLayout(false);
            this.GroupBox_Pool_Groups.PerformLayout();
            this.GroupBox_Pool_Controls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox_Pool_Groups;
        private System.Windows.Forms.ListBox ListBox_Member;
        private System.Windows.Forms.ListBox ListBox_Machine;
        private System.Windows.Forms.CheckBox CheckBox_Sharable;
        private System.Windows.Forms.Label Label_Member_List;
        private System.Windows.Forms.Label Label_Pool_Name;
        private System.Windows.Forms.Label Label_Machine_List;
        private System.Windows.Forms.ComboBox ComboBox_Pool_Name;
        private System.Windows.Forms.Button Button_Del_Member;
        private System.Windows.Forms.Button Button_Add_Member;
        private System.Windows.Forms.Button Button_Delete_Pool;
        private System.Windows.Forms.Button Button_Update_Pool;
        private System.Windows.Forms.Button Button_Add_Pool;
        private System.Windows.Forms.GroupBox GroupBox_Pool_Controls;
    }
}