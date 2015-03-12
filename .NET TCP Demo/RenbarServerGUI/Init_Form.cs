#region Using NameSpace
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace RenbarServerGUI
{
    public partial class Init_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // create windows forms effect class object instance ..
        global::RenbarLib.Environment.Forms.Effect FormEffect
            = new global::RenbarLib.Environment.Forms.Effect();
        /***********************************************************************************************/

        // declare renbar server base class object ..
        private ServerBase __Base = null;

        // declare initialize error flag ..
        private bool __HasError = false;
        #endregion

        #region Form Constructor And Destructor Procedure
        /// <summary>
        /// Primary initialize methods form constructor procedure.
        /// </summary>
        public Init_Form()
        {
            // initialize component standard procedure ..
            InitializeComponent();
        }

        /// <summary>
        /// Primary destructor procedure.
        /// </summary>
        ~Init_Form()
        {
            this.Dispose(true);
        }
        #endregion

        #region Form Load Event Procedure
        /// <summary>
        /// Form load trigger event procedure.
        /// </summary>
        private void Init_Form_Load(object sender, EventArgs e)
        {
            // setting text ..
            this.Label_Product.Text
                = global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo;

            string[] ary = global::RenbarLib.Environment.AssemblyInfoClass.CopyrightInfo.Split('.');
            this.Label_Copyright.Text = string.Format("{0}.\n{1}.", ary[0], ary[1]);

            this.Label_Version.Text
                = string.Format("Version {0}", global::RenbarLib.Environment.AssemblyInfoClass.VersionInfo);

            this.Label_Status.Text = "Initializing base component ..";

            // setting product title style ..
            this.Label_Product.Font = new Font(new FontFamily("Arial Black"), 22, FontStyle.Bold, GraphicsUnit.Pixel);

            // show load event effect ..
            //this.FormEffect.ShowEffect(this);

            // start initialize thread ..
            this.Initialize();
        }

        /// <summary>
        /// Object initialize procedure.
        /// </summary>
        private void Initialize()
        {
            try
            {
                // init server base instance ..
                this.__Base = new ServerBase();

                // get rows count ..
                uint count = __Base.DataRowsCount;

                for (int i = 0; i < count; i++)
                {
                    this.ProgressBar_Status.Value = Convert.ToInt32(Convert.ToDouble((100 / (double)count) * (double)i));

                    // refresh progress bar status ..
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                __HasError = true;

                MessageBox.Show(this, ex.Message, global::RenbarLib.Environment.AssemblyInfoClass.ProductInfo,
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                this.Close();
            }
        }
        #endregion

        #region From Close Event Procedure
        /// <summary>
        /// Form closing trigger event procedure.
        /// </summary>
        private void Init_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // show close event effect ..
            this.FormEffect.CloseEffect(this);

            if (!this.__HasError)
                // set dislog result ..
                this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Server Base Object Instance Property
        /// <summary>
        /// Get server base class instance.
        /// </summary>
        internal ServerBase Base
        {
            get
            {
                return this.__Base;
            }
        }
        #endregion
    }
}