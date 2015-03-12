#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

// import renbar server library namespace ..
using RenbarLib.Data;
using RenbarLib.Environment;
using RenbarLib.Network.Protocol;
using RenbarServerGUI.Properties;

using MySql.Data.MySqlClient;
#endregion

namespace RenbarServerGUI
{
    /// <summary>
    /// Cleanup obsolete data class.
    /// </summary>
    public class Cleanup : IDisposable
    {
        #region Declare Global Variable Section
        // declare renbar class object ..
        private DataStructure EnvData = null;
        private Log EnvLog = null;

       
        private ulong _JobMaxTime = 1800000;
        private ulong _MachineMaxTime = 604800000;

        // stop running thread flag ..
        private volatile bool requestStop = false;
        #endregion

        #region Cleanup Constructor
        /// <summary>
        /// Cleanup class constructor.
        /// </summary>
        /// <param name="Data">renbar memory database object instance.</param>
        /// <param name="Log">renbar log record object instance.</param>
        public Cleanup(DataStructure Data, Log LogObj)
        {

            this.ViewChangeSettings();


            // assign relationship object ..
            this.EnvData = Data;
            this.EnvLog = LogObj;

            Thread CleanupThread = new Thread(new ThreadStart(this.CleanupTick));
            CleanupThread.Priority = ThreadPriority.Lowest;
            CleanupThread.IsBackground = true;
            CleanupThread.Start();
        }
        #endregion
        
        #region View Currently Environment Settings
        /// <summary>
        /// View and change server environment settings.
        /// </summary>
        private void ViewChangeSettings()
        {
            // checking  ..
            if (this._JobMaxTime != Settings.Default.JobMaxTime)
                this._JobMaxTime = Settings.Default.JobMaxTime;
            if (this._MachineMaxTime != Settings.Default.MachineMaxTime)
                this._MachineMaxTime = Settings.Default.MachineMaxTime;
        }
        #endregion

        #region 釋放資源 Clean Up Resource Method Procedure
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            // clean up base resource ..
            this.Dispose(true);

            // this object will be cleaned up by the dispose method ..
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if server base object should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // stop running threads ..
                this.requestStop = true;
                this.EnvData.Dispose();
            }
        }
        #endregion

        #region 清理 Clean Up Tick Event Procedure
        /// <summary>
        /// Cleanup old data trigger procedure.
        /// </summary>
        private void CleanupTick()
        {
            do
            {
                // delay 10 min to next confirm ..
                Thread.Sleep(600000);

                // confirm data status ..
                if (DisplayBase.IsLock)
                    continue;

                // define tables collection ..
                DataSet refreshData = new DataSet();

                // create text command dictionary ..
                IDictionary<string, MySqlCommand> TextCommands = new Dictionary<string, MySqlCommand>();

                #region 檢驗任務 Check Job Partial
                // read original job data ..
                DataTable
                    Job_Group = this.EnvData.ReadData("Job_Group"),
                    Job_Attr = this.EnvData.ReadData("Job_Attr"),
                    Job = this.EnvData.ReadData("Job");

                // declare job key list ..
                IList<string> JKeys = new List<string>();

                // cleanup job partial ..
                for (int i = 0; i < Job_Group.Rows.Count; i++)
                {
                    // confirm completed status ..
                    if (Convert.ToUInt16(Job_Group.Rows[i]["Status"])
                        == Convert.ToUInt16(JobStatusFlag.COMPLETED))
                    {
                        DateTime canDeleted = Convert.ToDateTime(Job_Group.Rows[i]["Finish_Time"]);
                        TimeSpan lockspan = new TimeSpan(DateTime.Now.Ticks);

                        // check whether the machine had been locked more than 30 minutes ..
                        if (lockspan.Subtract(new TimeSpan(canDeleted.Ticks)).Duration().TotalMilliseconds >= _JobMaxTime)
                        {
                            string key = Job_Group.Rows[i]["Job_Group_Id"].ToString();

                            // add to list ..
                            JKeys.Add(key);

                            // get relation data ..
                            DataRow[]
                                row_attr = Job_Attr.Select(string.Format("Job_Group_Id = '{0}'", key)),
                                row_jobs = Job.Select(string.Format("Job_Group_Id = '{0}'", key));

                            if (row_attr.Length > 0 && row_jobs.Length > 0)
                            {
                                // delete relation data row ..
                                for (int j = 0; j < row_jobs.Length; j++)
                                    row_jobs[j].Delete();

                                row_attr[0].Delete();
                                Job_Group.Rows[i].Delete();
                            }
                        }
                    }
                }
                #endregion

                #region 確認任務更新狀況 Confirm Job Update Data
                if (JKeys.Count > 0)
                {
                    // bind job database text command ..
                    string index = null;

                    foreach (string s in JKeys)
                        index += "'" + s + "', ";

                    string JobGroupId = index.Substring(0, (index.Length - 2));

                    string cmd = string.Empty;
                    cmd = " Select count(*) From Job ";
                    cmd += " Inner Join Job_Group On Job.Job_Group_Id = Job_Group.Job_Group_Id ";
                    cmd += " Inner Join Job_Attr On Job_Attr.Job_Group_Id = Job_Group.Job_Group_Id ";
                    cmd += " Where Job_Group.Job_Group_Id In ("+JobGroupId+") ";
                    MySqlCommand command = new MySqlCommand(cmd);
                    //command.Parameters.AddWithValue("?Job_Group_Id", JobGroupId);
                    if (this.EnvData.IsExistJobMachineId(command) > 0)
                    {
                        cmd = " Delete From Job Where Job_Group_Id In (" + JobGroupId + "); ";
                        cmd += " Delete From Job_Attr Where Job_Group_Id In (" + JobGroupId + "); ";
                        cmd += " Delete From Job_Group Where Job_Group_Id In (" + JobGroupId + "); ";
                    }
                    else
                    {
                        cmd = string.Empty;
                    }

                    // add to refresh dataset ..
                    refreshData.Tables.Add(Job_Group);
                    refreshData.Tables.Add(Job_Attr);
                    refreshData.Tables.Add(Job);
                    command = new MySqlCommand(cmd);
                    //command.Parameters.AddWithValue("?Job_Group_Id", index.Substring(0, (index.Length - 2)));
                    TextCommands.Add(Job.TableName, command);
                    TextCommands.Add(Job_Group.TableName, command);
                    TextCommands.Add(Job_Attr.TableName, command);
                }
                #endregion

                #region 檢驗Machine狀態 Check Machine Partial
                // read original job data ..
                DataTable
                    Machine = this.EnvData.ReadData("Machine"),
                    Machine_Pool = this.EnvData.ReadData("Machine_Pool");

                // declare job key list ..
                IList<string> MKeys = new List<string>();

                // cleanup machine partial ..
                for (int i = 0; i < Machine.Rows.Count; i++)
                {
                    DateTime canDeleted = Convert.ToDateTime(Machine.Rows[i]["Last_Online_Time"]);
                    TimeSpan lockspan = new TimeSpan(DateTime.Now.Ticks);

                    // check whether the machine had been locked more than 7 days ..
                    if (lockspan.Subtract(new TimeSpan(canDeleted.Ticks)).Duration().TotalMilliseconds >= _MachineMaxTime)
                    {
                        string key = Machine.Rows[i]["Machine_Id"].ToString();

                        // add to list ..
                        MKeys.Add(key);

                        DataRow[] row_pool = Machine_Pool.Select(string.Format("Machine_Id = '{0}'", key));

                        // confirm the machine has in pool ..
                        if (row_pool.Length > 0)
                            // delete relation data row ..
                            row_pool[0].Delete();

                        // delete primary data ..
                        Machine.Rows[i].Delete();
                    }
                }
                #endregion

                #region 確認Machine更新數據 Confirm Machine Update Data
                if (MKeys.Count > 0)
                {
                    // bind machine database text command ..
                    string index = null;

                    foreach (string s in MKeys)
                        index += "'" + s + "', ";
                    string MachineId = index.Substring(0, (index.Length - 2));

                    string cmd = string.Empty;
                    cmd += " Select count(*) From Machine_Pool ";
                    cmd += " Inner Join Machine On Machine_Pool.Machine_Id = Machine.Machine_Id ";
                    cmd += " Where Machine.Machine_Id In (" + MachineId + ") ";
                    MySqlCommand command = new MySqlCommand(cmd);
                    //command.Parameters.AddWithValue("?Machine_Id", MachineId);
                    if (this.EnvData.IsExistJobMachineId(command) > 0)
                    {
                        cmd = " Delete From Machine_Pool Where Machine_Id In  (" + MachineId + "); ";
                        cmd += " Delete From Machine Where Machine_Id In  (" + MachineId + "); ";
                    }
                    else
                    {
                        cmd = " Delete From Machine Where Machine_Id In  (" + MachineId + "); ";
                    }


                    // add to refresh dataset ..
                    refreshData.Tables.Add(Machine);
                    refreshData.Tables.Add(Machine_Pool);
                    command = new MySqlCommand(cmd);
                    // add commands ..
                    //command.Parameters.AddWithValue("?Machine_Id", MachineId);
                    TextCommands.Add(Machine.TableName, command);
                    TextCommands.Add(Machine_Pool.TableName, command);
                }
                #endregion

                if (JKeys.Count > 0 || MKeys.Count > 0)
                {
                    // execute database changes ..
                    this.EnvData.WriteData(refreshData, TextCommands);

                    // wait for write data to finish ..
                    Thread.Sleep(1000);

                    // refresh user interface status ..
                    DisplayBase.CanJobUpdate = true;
                    DisplayBase.CanMachineUpdate = true;

                    // cleanup key list ..
                    JKeys.Clear();
                    MKeys.Clear();
                }

            } while (!requestStop);
        }
        #endregion
    }
}