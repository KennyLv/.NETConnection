#region Using NameSpace
using System;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
#endregion

namespace RenbarGUI.Forms
{
    public partial class ChangeUser_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar client environment class object ..
        private Customization EnvCust = new Customization();
        /***********************************************************************************************/

        // declare comstomize message ..
        private string SuccessMessage = string.Empty;
        private string FailMessage = string.Empty;
        #endregion

        #region Form Constructor Procedure
        /// <summary>
        /// Primary renbar client changeuser form constructor procedure.
        /// </summary>
        /// <param name="Setting">current environment setting information.</param>
        public ChangeUser_Form(Settings Setting)
        {
            // initialize form control component ..
            InitializeComponent();

            // initializing user interface behavior ..
            Language(Setting.Lang);
        }

        /// <summary>
        /// Setting language resource.
        /// </summary>
        private void Language(Customization.Language Lang)
        {
            switch (Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:
                    // ChangeUser Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // User Group Box ..
                    this.GroupBox_User.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_User.Name, Customization.Language.En_Us);

                    // Id Label ..
                    this.Label_Id.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Id.Name, Customization.Language.En_Us);

                    // Password Label ..
                    this.Label_Pwd.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Pwd.Name, Customization.Language.En_Us);
                    
                    // OK Button ..
                    this.Button_OK.Text = this.EnvCust.GetLocalization(this.Name, this.Button_OK.Name, Customization.Language.En_Us);

                    // Success Changed String ..
                    SuccessMessage = this.EnvCust.GetLocalization(this.Name, "Success_String", Customization.Language.En_Us);

                    // Fail Changed String ..
                    FailMessage = this.EnvCust.GetLocalization(this.Name, "Fail_String", Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // ChangeUser Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // User Group Box ..
                    this.GroupBox_User.Text = this.EnvCust.GetLocalization(this.Name, this.GroupBox_User.Name, Customization.Language.Zh_Tw);

                    // Id Label ..
                    this.Label_Id.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Id.Name, Customization.Language.Zh_Tw);

                    // Password Label ..
                    this.Label_Pwd.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Pwd.Name, Customization.Language.Zh_Tw);

                    // OK Button ..
                    this.Button_OK.Text = this.EnvCust.GetLocalization(this.Name, this.Button_OK.Name, Customization.Language.Zh_Tw);

                    // Success Changed String ..
                    SuccessMessage = this.EnvCust.GetLocalization(this.Name, "Success_String", Customization.Language.Zh_Tw);

                    // Fail Changed String ..
                    FailMessage = this.EnvCust.GetLocalization(this.Name, "Fail_String", Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }
        #endregion

        #region Form Closing Event Procedure
        private void ChangeUser_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Form Button Click Event Procedure
        /// <summary>
        /// Confirm validation.
        /// </summary>
        private void Button_OK_Click(object sender, EventArgs e)
        {
            #region Check Input Infomation
            if (this.TextBox_Id.Text.Length <= 0)
            {
                this.TextBox_Id.Focus();
                return;
            }

            if (this.TextBox_Pwd.Text.Length <= 0)
            {
                this.TextBox_Pwd.Focus();
                return;
            }
            #endregion

            // disable controls ..
            this.GroupBox_User.Enabled = false;
            this.Button_OK.Enabled = false;
            this.Update();

            //驗證身份
            if (this.EnvCust.ADvalid(this.TextBox_Id.Text, this.TextBox_Pwd.Text))
            {
                // add changed user code ..
                global::System.Globalization.TextInfo txtInfo = global::System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;

                // change current user ..
                Customization.User = txtInfo.ToTitleCase(this.TextBox_Id.Text.Trim());
                MessageBox.Show(this, this.SuccessMessage, AssemblyInfoClass.ProductInfo, 
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                this.Close();
            }
            else
            {
                MessageBox.Show(this, this.FailMessage,AssemblyInfoClass.ProductInfo, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

            // enable controls ..
            this.GroupBox_User.Enabled = true;
            this.Button_OK.Enabled = true;
            this.Update();
        }
        #endregion
    }
}