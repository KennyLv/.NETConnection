#region Using NameSpace
using System;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
#endregion

namespace RenbarGUI.Forms
{
    partial class About_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create renbar client environment class object ..
        private Customization EnvCust = new Customization();
        /***********************************************************************************************/
        #endregion

        #region Form Constructor Procedure
        /// <summary>
        /// Primary renbar client about form constructor procedure.
        /// </summary>
        /// <param name="Setting">current environment setting information.</param>
        public About_Form(Settings Setting)
        {
            // initialize form control component ..
            InitializeComponent();

            // initializing user interface behavior ..
            Language(Setting.Lang);
        }

        /// <summary>
        /// Setting language resource.
        /// </summary>
        /// <param name="Lang">current language setting.</param>
        private void Language(Customization.Language Lang)
        {
            switch (Lang)
            {
                #region English (United-State)
                case Customization.Language.En_Us:
                    // About Form ..
                    this.Text = string.Format(EnvCust.GetLocalization(this.Name,
                        this.Name, Customization.Language.En_Us) + " {0}", AssemblyInfoClass.ProductInfo);

                    // Product Label ..
                    this.Label_ProductName.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_ProductName.Name,
                        Customization.Language.En_Us) + " {0}", AssemblyInfoClass.ProductInfo);

                    // Version Label ..
                    this.Label_Version.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_Version.Name,
                        Customization.Language.En_Us) + " {0}", AssemblyInfoClass.VersionInfo);

                    // Copyright Label ..
                    this.Label_Copyright.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_Copyright.Name,
                        Customization.Language.En_Us) + " {0}", AssemblyInfoClass.CopyrightInfo);

                    // Link Label ..
                    this.LinkLabel_Help.Text = EnvCust.GetLocalization(this.Name,
                        this.LinkLabel_Help.Name, Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // About Form ..
                    this.Text = string.Format(EnvCust.GetLocalization(this.Name,
                        this.Name, Customization.Language.Zh_Tw) + " {0}", AssemblyInfoClass.ProductInfo);

                    // Product Label ..
                    this.Label_ProductName.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_ProductName.Name,
                        Customization.Language.Zh_Tw) + " {0}", AssemblyInfoClass.ProductInfo);

                    // Version Label ..
                    this.Label_Version.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_Version.Name,
                        Customization.Language.Zh_Tw) + " {0}", AssemblyInfoClass.VersionInfo);

                    // Copyright Label ..
                    this.Label_Copyright.Text = String.Format(
                        EnvCust.GetLocalization(this.Name, this.Label_Copyright.Name,
                        Customization.Language.Zh_Tw) + " {0}", AssemblyInfoClass.CopyrightInfo);

                    // Link Label ..
                    this.LinkLabel_Help.Text = EnvCust.GetLocalization(this.Name,
                        this.LinkLabel_Help.Name, Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }
        #endregion

        #region Form Closing Event Procedure
        private void About_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Link Button Click Event Procedure
        private void ilbl_help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // open release note from wiki ..
            string LinkData = Environment.ExpandEnvironmentVariables("%SystemRoot%" + @"\Explorer.exe");
            //MessageBox.Show(LinkData);
            global::System.Diagnostics.Process.Start(LinkData, "http://wiki.emocm.com");
        }
        #endregion
    }
}