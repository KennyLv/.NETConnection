#region Using NameSpace
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
#endregion

namespace RenbarLib.Data
{
    /// <summary>
    /// Data object base interface.
    /// </summary>
    interface IBase : IDisposable
    {
        #region IBase成員屬性 IBase Member Properties
        /// <summary>
        /// Database connection string property.
        /// </summary>
        string DataConnectionString
        {
            get;
            set;
        }
        #endregion

        #region IBase成員方法 IBase Member Methods
        /// <summary>
        /// 獲取資料庫模型 Data structure schema method.
        /// </summary>
        /// <returns>System.Data.DataSet</returns>
        DataSet GetDataBaseSchema();

        /// <summary>
        /// 獲取資料庫鏈接Data object content method.
        /// </summary>
        /// <param name="DataBaseObject">fill data object.</param>
        void GetDataBaseContent(ref DataSet DataBaseObject);  ///？？？？？？？？？引用修改的是哪里的地址？？？？？？？？？？
        #endregion
    }

    class MysqlBase : IBase
    {
        #region Declare Global Variable Section
        // create connection object using dotnet framework provider ..
        private MySqlConnection DataConnection = new MySqlConnection();
        // declare memory dataset object ..
        private DataSet BaseObject = null;
        #endregion

        #region Clean Up Resource Method Procedure

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
       
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // clear dataset object instance ..
                if (BaseObject != null)
                    this.BaseObject.Dispose();

                // release and clear data connection object instance ..
                if (this.DataConnection.State != ConnectionState.Closed)
                    this.DataConnection.Close();

                this.DataConnection.Dispose();
            }
        }

        #endregion

        #region IBase 成員

        public string DataConnectionString
        {
            get
            {
                return this.DataConnection.ConnectionString;
            }
            set
            {
                this.DataConnection.ConnectionString = value;
            }
        }

        public DataSet GetDataBaseSchema()
        {
            BaseObject = new DataSet();

            // open database connection ..
            this.DataConnection.Open();

            // use system information structure schema command ..
            string
                sysText = " select  table_name   from information_schema.tables  ";
            sysText += " where table_type='base table' and table_schema='bt_test' ";

            MySqlCommand sys_cmd = new MySqlCommand(sysText, this.DataConnection);
            MySqlDataReader sys_dr = sys_cmd.ExecuteReader();
            while (sys_dr.Read())
                BaseObject.Tables.Add(new DataTable(sys_dr[0].ToString(), "DataStructure"));

            // close data reader object ..
            sys_dr.Close();

            // set data structure and primary keys ..
            for (int i = 0; i < BaseObject.Tables.Count; i++)
            {
                MySqlDataAdapter sys_adapter = new MySqlDataAdapter(" Select * From " + BaseObject.Tables[i].TableName, this.DataConnection);
                sys_adapter.FillSchema(BaseObject.Tables[i], SchemaType.Source);
            }

            // use system information structure schema command ..
            string
                sysText2 = " Select KC.Constraint_Name As CN, KC.Table_Name As TableName,  ";
            sysText2 += " KC.Column_Name As ColName,Referenced_Column_Name As Ref_Name ";
            sysText2 += " From INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC ";
            sysText2 += " Join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ";
            sysText2 += " On KC.Constraint_Name = RC.Constraint_Name ";

            MySqlCommand sys_cmd2 = new MySqlCommand(sysText2, this.DataConnection);
            MySqlDataReader sys_dr2 = sys_cmd2.ExecuteReader();
            while (sys_dr2.Read())
            {
                // define data foreign key ..
                string
                    relationName = sys_dr2[0].ToString().Trim(),
                    //parTable = sys_dr2[3].ToString().Trim().Substring(3, (sys_dr2[3].ToString().Length - 3)),
                    parTable = sys_dr2[3].ToString().Trim().Substring(0, (sys_dr2[3].ToString().Length - 3)),
                    chiTable = sys_dr2[1].ToString().Trim(),
                    mappingColumn = sys_dr2[2].ToString().Trim(),
                    refcolumn = parTable + "_Id";

                DataColumn
                    parColumn = BaseObject.Tables[parTable].Columns[refcolumn],
                    chiColumn = BaseObject.Tables[chiTable].Columns[mappingColumn];

                // add to relation collection ..
                BaseObject.Relations.Add(relationName, parColumn, chiColumn);
            }

            // close database connection ..
            this.DataConnection.Close();

            return BaseObject;
        }

        public void GetDataBaseContent(ref DataSet DataBaseObject)
        {
            // open database connection ..
            this.DataConnection.Open();

            // pause the database object of constraints ..
            DataBaseObject.EnforceConstraints = false;

            // clear ..
            if (DataBaseObject.Tables.Count > 0)
                DataBaseObject.Clear();

            foreach (DataTable Table in DataBaseObject.Tables)
            {
                string sysText = string.Format("Select * From {0}", Table.TableName);
                MySqlDataAdapter sys_adapter = new MySqlDataAdapter(sysText, this.DataConnection);

                // write data to memory data table ..
                sys_adapter.Fill(DataBaseObject.Tables[Table.TableName]);
            }

            // resume the database object of constraints ..
            //取得或設定EnforceConstraints值，指出在嘗試任何更新作業時，是否遵循條件約束 (Constraint) 規則。
            DataBaseObject.EnforceConstraints = true;

            // close database connection ..
            this.DataConnection.Close();
        }

        #endregion

       

    }
}