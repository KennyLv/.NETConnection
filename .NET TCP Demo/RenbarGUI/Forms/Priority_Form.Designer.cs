namespace RenbarGUI.Forms
{
    partial class Priority_Form
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
            this.Label_Priority_Value = new System.Windows.Forms.Label();
            this.Numeric_Priority_Value = new System.Windows.Forms.NumericUpDown();
            this.Button_Ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_Priority_Value)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_Priority_Value
            // 
            this.Label_Priority_Value.AutoSize = true;
            this.Label_Priority_Value.Location = new System.Drawing.Point(12, 10);
            this.Label_Priority_Value.Name = "Label_Priority_Value";
            this.Label_Priority_Value.Size = new System.Drawing.Size(112, 13);
            this.Label_Priority_Value.TabIndex = 0;
            this.Label_Priority_Value.Text = "[Label_Priority_Value]";
            // 
            // Numeric_Priority_Value
            // 
            this.Numeric_Priority_Value.Location = new System.Drawing.Point(82, 8);
            this.Numeric_Priority_Value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Numeric_Priority_Value.Name = "Numeric_Priority_Value";
            this.Numeric_Priority_Value.Size = new System.Drawing.Size(120, 21);
            this.Numeric_Priority_Value.TabIndex = 1;
            this.Numeric_Priority_Value.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // Button_Ok
            // 
            this.Button_Ok.Location = new System.Drawing.Point(127, 35);
            this.Button_Ok.Name = "Button_Ok";
            this.Button_Ok.Size = new System.Drawing.Size(75, 23);
            this.Button_Ok.TabIndex = 2;
            this.Button_Ok.Text = "[Button_Ok]";
            this.Button_Ok.UseVisualStyleBackColor = true;
            this.Button_Ok.Click += new System.EventHandler(this.Button_Ok_Click);
            // 
            // Priority_Form
            // 
            this.AcceptButton = this.Button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 65);
            this.Controls.Add(this.Button_Ok);
            this.Controls.Add(this.Numeric_Priority_Value);
            this.Controls.Add(this.Label_Priority_Value);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Priority_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[Priority_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Priority_Form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_Priority_Value)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Priority_Value;
        private System.Windows.Forms.NumericUpDown Numeric_Priority_Value;
        private System.Windows.Forms.Button Button_Ok;

    }
}