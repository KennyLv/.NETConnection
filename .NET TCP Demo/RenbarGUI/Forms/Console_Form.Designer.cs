namespace RenbarGUI.Forms
{
    partial class Console_Form
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
            this.GroupBox_Output = new System.Windows.Forms.GroupBox();
            this.TextBox_Output = new System.Windows.Forms.TextBox();
            this.Button_Pause = new System.Windows.Forms.Button();
            this.GroupBox_Output.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_Output
            // 
            this.GroupBox_Output.Controls.Add(this.TextBox_Output);
            this.GroupBox_Output.Location = new System.Drawing.Point(12, 12);
            this.GroupBox_Output.Name = "GroupBox_Output";
            this.GroupBox_Output.Size = new System.Drawing.Size(570, 322);
            this.GroupBox_Output.TabIndex = 0;
            this.GroupBox_Output.TabStop = false;
            this.GroupBox_Output.Text = "[GroupBox_Output]";
            // 
            // TextBox_Output
            // 
            this.TextBox_Output.Location = new System.Drawing.Point(6, 20);
            this.TextBox_Output.Multiline = true;
            this.TextBox_Output.Name = "TextBox_Output";
            this.TextBox_Output.ReadOnly = true;
            this.TextBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox_Output.Size = new System.Drawing.Size(558, 296);
            this.TextBox_Output.TabIndex = 0;
            // 
            // Button_Pause
            // 
            this.Button_Pause.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button_Pause.Location = new System.Drawing.Point(507, 340);
            this.Button_Pause.Name = "Button_Pause";
            this.Button_Pause.Size = new System.Drawing.Size(75, 23);
            this.Button_Pause.TabIndex = 1;
            this.Button_Pause.Text = "[Button_Pause]";
            this.Button_Pause.UseVisualStyleBackColor = true;
            this.Button_Pause.Click += new System.EventHandler(this.Button_Pause_Click);
            // 
            // Console_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 375);
            this.Controls.Add(this.Button_Pause);
            this.Controls.Add(this.GroupBox_Output);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "Console_Form";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "[Console_Form]";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Console_Form_FormClosing);
            this.GroupBox_Output.ResumeLayout(false);
            this.GroupBox_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox_Output;
        private System.Windows.Forms.TextBox TextBox_Output;
        private System.Windows.Forms.Button Button_Pause;
    }
}