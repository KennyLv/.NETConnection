#region Using NameSpace
using System;
using System.Collections.Generic;
using System.Net;

// Import renbar common library namespace ..
using RenbarLib.Network.Protocol;

#endregion

namespace RenbarLib.Environment.Forms.Customizations.Service
{
    /// <summary>
    /// 客户端结构体 Renbar client item structure.
    /// </summary>
    /// <typeparam name="TKey">key</typeparam>
    /// <typeparam name="TValue"></typeparam>
    public struct ItemPair<TKey, TValue>
    {
        #region Declare Global Variable Section
        // define key and value object ..
        private TKey _key;
        private TValue _var;
        #endregion

        #region Item Pair Constructure Procedure
        /// <summary>
        /// Primary item pair constructure procedure.
        /// </summary>
        /// <param name="key">key type.</param>
        /// <param name="value">value type.</param>
        internal ItemPair(TKey key, TValue value)       //internal限制外部存取
        {
            // mapping key and value ..
            this._key = key;
            this._var = value;
        }
        #endregion

        #region Get Key, Value Properties
        /// <summary>
        /// Get this key.
        /// </summary>
        /// <returns>ItemPair.TKey</returns>
        internal TKey Key
        {
            get
            {
                return this._key;
            }
        }

        /// <summary>
        /// Get this value.
        /// </summary>
        /// <returns>ItemPair.TValue</returns>
        internal TValue Value
        {
            get
            {
                return this._var;
            }
        }
        #endregion

        #region Override Base Method Procedure
        /// <summary>
        /// Base "Equals" method.
        /// </summary>
        /// <param name="obj">equals object.</param>
        /// <returns>System.Boolean</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Base "GetHashCode" method.
        /// </summary>
        /// <returns>System.Int32</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Return customize string.
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 任務結構體 Pool Pool2 Waitfor Renbar client job form enumerable structure.
    /// </summary>
    public struct DropList
    {
        #region Drop-Down Properties
        /// <summary>
        /// Get or set first pool list.
        /// </summary>
        public IDictionary<string, string> Pool
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set second pool list.
        /// </summary>
        public IDictionary<string, string> Pool2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set waitfor list.
        /// </summary>
        public IDictionary<string, string> Waitfor
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// Renbar任務的通用属性 Renbar job common attribute structure.
    /// </summary>
    public struct Attributes
    {
        #region 主要属性Primary Attribute Properties
        /// <summary>
        /// Get or set job name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set first pool guid.
        /// </summary>
        public Guid FirstPool
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set second pool guid.
        /// </summary>
        public Guid SecondPool
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job current status.
        /// </summary>
        public JobStatusFlag Status
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job total start time.
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job total finish time.
        /// </summary>
        public DateTime FinishTime
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job note (description).
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job submit machine.
        /// </summary>
        public Guid SubmitMachine
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job submit account.
        /// </summary>
        public string SubmitAcct
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job submit time.
        /// </summary>
        public DateTime SubmitTime
        {
            get;
            set;
        }
        #endregion

        #region 次要属性Slave Attribute Properties
        /// <summary>
        /// Get or set job project name.
        /// </summary>
        public string Project
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job start frame.
        /// </summary>
        public int Start
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job end frame.
        /// </summary>
        public int End
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job packet size.
        /// </summary>
        public ushort PacketSize
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set process render type.
        /// </summary>
        public string ProcType
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job wait for.
        /// </summary>
        public Guid WaitFor
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set job priority.
        /// </summary>
        public ushort Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set alienbrain name.
        /// </summary>
        public string AbName
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set alienbrain path.
        /// </summary>
        public string AbPath
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set alienbrain update only.
        /// </summary>
        public bool AbUpdateOnly
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// Renbar client environment setting structure.
    /// </summary>
    [Serializable]
    public struct Settings
    {
        #region 设置环境Environment Properties
        /// <summary>
        /// 设置用户界面语言Get or set user interface language.
        /// </summary>
        internal Customization.Language Lang
        {
            get;
            set;
        }

        /// <summary>
        /// 设置服务器地址Get or set server ip address.
        /// </summary>
        public IPAddress ServerIpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 设置服务器端口号Get or set server port number.
        /// </summary>
        public ushort ServerPort
        {
            get;
            set;
        }
        #endregion
    }
}