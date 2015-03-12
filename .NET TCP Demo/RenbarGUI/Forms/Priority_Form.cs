#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Windows.Forms;

// import renbar class library namespace ..
using RenbarLib.Environment;
using RenbarLib.Environment.Forms.Customizations.Service;
using RenbarLib.Network.Protocol;
using System.Threading;
#endregion

namespace RenbarGUI.Forms
{
    public partial class Priority_Form : Form
    {
        #region Declare Global Variable Section
        /************************ reference renbar class library section *******************************/
        // declare renbar client environment class object ..
        private Customization EnvCust = null;

        //
        private Client2Server.CommunicationType CommType = new Client2Server.CommunicationType();

        // declare renbar client communication class object ..
        private Communication EnvComm = null;
        /***********************************************************************************************/

        // declare setting render machines amount ..
        private IList<string> DataList = null;

        // declare setting render machine priority messages ..
        private string SetFailMessage = string.Empty;
        #endregion

        #region 構造函數 Form Constructor Procedure
        /// <summary>
        /// Primary renbar client priority form constructor procedure.
        /// </summary>
        /// <param name="Cust">reference customize class.</param>
        /// <param name="Comm">refrernce communication class.</param>
        /// <param name="Setting">current environment setting information.</param>
        /// <param name="Machines">selected machine items.</param>
        public Priority_Form(ref Customization Cust, ref Communication Comm, Settings Setting, IList<string> ObjectItems,Client2Server.CommunicationType Commtype)
        {
            // initialize form control component ..
            InitializeComponent();

            // assign customize class reference ..
            this.EnvCust = Cust;

            // assign communization class refrernce ..
            this.EnvComm = Comm;

            // assign setting list to machine data list ..
            this.DataList = ObjectItems;

            //
            this.CommType = Commtype;

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
                    // Priority Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.En_Us);

                    // Priority Label ..
                    this.Label_Priority_Value.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Priority_Value.Name, Customization.Language.En_Us);

                    // Ok Button ..
                    this.Button_Ok.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Ok.Name, Customization.Language.En_Us);

                    // Error Message ..
                    this.SetFailMessage = this.EnvCust.GetLocalization(this.Name, "Fail_Msg", Customization.Language.En_Us);
                    break;
                #endregion

                #region Traditional Chinese
                case Customization.Language.Zh_Tw:
                    // Priority Form ..
                    this.Text = this.EnvCust.GetLocalization(this.Name, this.Name, Customization.Language.Zh_Tw);

                    // Priority Label ..
                    this.Label_Priority_Value.Text = this.EnvCust.GetLocalization(this.Name, this.Label_Priority_Value.Name, Customization.Language.Zh_Tw);

                    // Ok Button ..
                    this.Button_Ok.Text = this.EnvCust.GetLocalization(this.Name, this.Button_Ok.Name, Customization.Language.Zh_Tw);

                    // Error Message ..
                    this.SetFailMessage = this.EnvCust.GetLocalization(this.Name, "Fail_Msg", Customization.Language.Zh_Tw);
                    break;
                #endregion
            }
        }
        #endregion

        #region 關閉窗體 Form Closing Event Procedure
        private void Priority_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // return correct dialog result ..
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 完成設定 Button Click Event Procedure
        private void Button_Ok_Click(object sender, EventArgs e)
        {
            bool result = true;
            foreach (string s in this.DataList)//可批量設定？？？？？？？？
            {
                // send request to remote server ..
                KeyValuePair<string, object> responseObject;

                //
                 IDictionary<string, object> Item = new Dictionary<string, object>();

                //
                switch (this.CommType)
                {
                    case Client2Server.CommunicationType.MACHINEPRIORITY:
                        //Item.Add("Machine_Id", s);
                        Item.Add("Machine_Id", s);
                        Item.Add("Priority", this.Numeric_Priority_Value.Value);
                        break;
                    case Client2Server.CommunicationType.JOBPRIORITY:
                        //Item.Add("Job_Group_Id", s);
                        Item.Add("Job_Group_Id", s);
                        Item.Add("Priority", this.Numeric_Priority_Value.Value);
                        break;
                }
                //設定優先級
                do
                {
                    // confirm current request status ..
                    if (EnvComm.CanRequest)
                    {
                        // package sent data ..
                        IList<object> packaged = EnvComm.Package(this.CommType, Item);
                        // wait for result ...
                        responseObject = EnvComm.Request(packaged);
                        break;
                    }
                    Thread.Sleep(500);
                } while (true);

                if (responseObject.Key.Substring(0, 1) == "+")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

           }

            if (!result)
            {
                // display fail message ..
                MessageBox.Show(this, this.SetFailMessage, AssemblyInfoClass.ProductInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                this.Close();
        }
        #endregion
    }
}