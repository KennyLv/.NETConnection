namespace RenderServerGUI
{
    partial class RenderEvents_Form
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
            this.Render_LogBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Render_LogBox
            // 
            this.Render_LogBox.BackColor = System.Drawing.Color.White;
            this.Render_LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Render_LogBox.Location = new System.Drawing.Point(0, 0);
            this.Render_LogBox.Name = "Render_LogBox";
            this.Render_LogBox.ReadOnly = true;
            this.Render_LogBox.Size = new System.Drawing.Size(634, 455);
            this.Render_LogBox.TabIndex = 0;
            this.Render_LogBox.Text = "";
            this.Render_LogBox.WordWrap = false;
            // 
            // Log_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 455);
            this.Controls.Add(this.Render_LogBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Log_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Log_Form";
            this.Load += new System.EventHandler(this.Log_Form_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox Render_LogBox;
    }
}