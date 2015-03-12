namespace RenbarGUI.Forms
{
    partial class HistoryForm
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
            this.ListView_History = new System.Windows.Forms.ListView();
            this.Button_Refresh = new System.Windows.Forms.Button();
            this.Button_Close = new System.Windows.Forms.Button();
            this.numericUpDown_RecordNum = new System.Windows.Forms.NumericUpDown();
            this.label_MaxRecords = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RecordNum)).BeginInit();
            this.SuspendLayout();
            // 
            // ListView_History
            // 
            this.ListView_History.Location = new System.Drawing.Point(12, 12);
            this.ListView_History.MultiSelect = false;
            this.ListView_History.Name = "ListView_History";
            this.ListView_History.Size = new System.Drawing.Size(403, 379);
            this.ListView_History.TabIndex = 0;
            this.ListView_History.UseCompatibleStateImageBehavior = false;
            this.ListView_History.DoubleClick += new System.EventHandler(this.ListView_History_DoubleClick);
            this.ListView_History.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_History_ColumnClick);
            // 
            // Button_Refresh
            // 
            this.Button_Refresh.Location = new System.Drawing.Point(191, 397);
            this.Button_Refresh.Name = "Button_Refresh";
            this.Button_Refresh.Size = new System.Drawing.Size(95, 32);
            this.Button_Refresh.TabIndex = 1;
            this.Button_Refresh.Text = "button1";
            this.Button_Refresh.UseVisualStyleBackColor = true;
            this.Button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // Button_Close
            // 
            this.Button_Close.Location = new System.Drawing.Point(303, 397);
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.Size = new System.Drawing.Size(95, 32);
            this.Button_Close.TabIndex = 2;
            this.Button_Close.Text = "button2";
            this.Button_Close.UseVisualStyleBackColor = true;
            this.Button_Close.Click += new System.EventHandler(this.Button_Close_Click);
            // 
            // numericUpDown_RecordNum
            // 
            this.numericUpDown_RecordNum.Location = new System.Drawing.Point(100, 405);
            this.numericUpDown_RecordNum.Name = "numericUpDown_RecordNum";
            this.numericUpDown_RecordNum.Size = new System.Drawing.Size(46, 22);
            this.numericUpDown_RecordNum.TabIndex = 5;
            // 
            // label_MaxRecords
            // 
            this.label_MaxRecords.AutoSize = true;
            this.label_MaxRecords.Location = new System.Drawing.Point(12, 407);
            this.label_MaxRecords.Name = "label_MaxRecords";
            this.label_MaxRecords.Size = new System.Drawing.Size(33, 12);
            this.label_MaxRecords.TabIndex = 6;
            this.label_MaxRecords.Text = "label2";
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 439);
            this.Controls.Add(this.label_MaxRecords);
            this.Controls.Add(this.numericUpDown_RecordNum);
            this.Controls.Add(this.Button_Close);
            this.Controls.Add(this.Button_Refresh);
            this.Controls.Add(this.ListView_History);
            this.Name = "HistoryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "HistoryForm";
            this.Load += new System.EventHandler(this.History_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RecordNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView ListView_History;
        private System.Windows.Forms.Button Button_Refresh;
        private System.Windows.Forms.Button Button_Close;
        private System.Windows.Forms.NumericUpDown numericUpDown_RecordNum;
        private System.Windows.Forms.Label label_MaxRecords;
    }
}