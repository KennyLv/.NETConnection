using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace RenbarLib.Environment
{
    /// <summary>
    /// Renbar file system class.
    /// </summary>
    public class FileSystem : Log
    {
        #region Render File Enumeration
        /// <summary>
        /// Render文件格式枚舉 Render file format type enumeration.
        /// </summary>
        public enum RenderFileType
        {
            /// <summary>
            /// Smedge Saved Render File Format.
            /// </summary>
            Smr,
            /// <summary>
            /// Renbar Saved Render File Format.
            /// </summary>
            Rbr
        }

        /// <summary>
        /// Render使用方法枚舉 Render usage method enumeration.
        /// </summary>
        public enum RenderMethod
        {
            /// <summary>
            /// 用于圖形用戶界面 Usage for graphics user interface.
            /// </summary>
            Gui,
            /// <summary>
            /// 用于文字命令 Usage for text command.
            /// </summary>
            Console
        }
        #endregion

        #region 轉換Render文件 Convert Render File？？？？？？？？？？？？？
        /// <summary>
        /// 定義空數據結構 Define empty data structure.
        /// </summary>
        /// <returns>System.Collection.Generic.IDictionary</returns>
        public void DataStructure(ref IDictionary<string, object> Dictionary)
        {
            Dictionary.Add("name", null);
            Dictionary.Add("waitFor", null);
            Dictionary.Add("isUpdateOnly", null);
            Dictionary.Add("type", null);
            Dictionary.Add("ABProjectName", null);
            Dictionary.Add("ABNode", null);
            Dictionary.Add("ProjectName", null);
            Dictionary.Add("Command", null);
            Dictionary.Add("FirstPool", null);
            Dictionary.Add("SecondPool", null);
            Dictionary.Add("Priority", null);
            Dictionary.Add("StartFrame", null);
            Dictionary.Add("EndFrame", null);
            Dictionary.Add("PacketSize", null);
            Dictionary.Add("Note", null);
        }

        /// <summary>
        /// 轉換已保存的Render文件 Convert saved render file.
        /// </summary>
        /// <param name="Type">file format type.</param>
        /// <param name="Method">usage render method.</param>
        /// <param name="FullFilePath">full file path.</param>
        /// <param name="Dictionary">data object dictionary.</param>
        /// <returns>System.Boolean</returns>
        public bool Convert(RenderFileType Type, RenderMethod Method, string FullFilePath, ref IDictionary<string, object> Dictionary)
        {
            bool result = true;

            if (!File.Exists(FullFilePath))
            {
                string ExceptionMsg = "Render file does not exist.";

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                return false;
            }

            try
            {
                switch (Type)
                {
                    #region Renbar File Type
                    case RenderFileType.Rbr:
                        #region RenderMethod: Gui
                        if (Method == RenderMethod.Gui)
                        {
                            // define data structure ..
                            if (Dictionary.Count == 0)
                                this.DataStructure(ref Dictionary);

                            // create xmlreader object instance ..
                            using (XmlReader xdr = XmlTextReader.Create(FullFilePath))
                            {
                                while (xdr.Read())
                                {
                                    if (xdr.NodeType == XmlNodeType.Element)
                                    {
                                        // the xml file has attribute mark ..
                                        if (xdr.HasAttributes)
                                        {
                                            for (int i = 0; i < xdr.AttributeCount; i++)
                                            {
                                                // read attribute name value ..
                                                xdr.MoveToAttribute(i);

                                                // append attribute data value to dictionary collection ..
                                                if (Dictionary.ContainsKey(xdr.Name))
                                                    Dictionary[xdr.Name] = xdr.Value;
                                            }
                                        }
                                        else
                                        {
                                            if (!xdr.Depth.Equals(0))
                                                // append element data value to dictionary collection ..
                                                if (Dictionary.ContainsKey(xdr.Name))
                                                    Dictionary[xdr.Name] = xdr.ReadString();
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region RenderMethod:Console
                        else
                        {
                            // create xmldocument object instance ..
                            XmlDocument xdoc = new XmlDocument();

                            // load document to xmldocument object ..
                            xdoc.Load(FullFilePath);

                            // get all element content ..
                            foreach (XmlNode _node in xdoc.SelectNodes("/JobDefinition/Job"))
                            {
                                string name = string.Empty;

                                // initialize data structure ..
                                IDictionary<string, object> items = new Dictionary<string, object>();
                                this.DataStructure(ref items);

                                foreach (XmlAttribute _atr in _node.Attributes)
                                {
                                    // assign dictionary name ..
                                    if (_atr.Name.ToLower().Equals("name"))
                                        name = _atr.Value;

                                    if (items.ContainsKey(_atr.Name))
                                        items[_atr.Name] = _atr.Value;
                                }

                                #region Setting Dictionary Values
                                if (items.ContainsKey("ABName"))
                                    items["ABName"] = _node["ABProjectName"].InnerText;
                                if (items.ContainsKey("ABPath"))
                                    items["ABPath"] = _node["ABNode"].InnerText;
                                if (items.ContainsKey("ProjectName"))
                                    items["ProjectName"] = _node["ProjectName"].InnerText;
                                if (items.ContainsKey("Command"))
                                    items["Command"] = _node["Command"].InnerText;
                                if (items.ContainsKey("FirstPool"))
                                    items["FirstPool"] = _node["FirstPool"].InnerText;
                                if (items.ContainsKey("SecondPool"))
                                    items["SecondPool"] = _node["SecondPool"].InnerText;
                                if (items.ContainsKey("Priority"))
                                    items["Priority"] = _node["Priority"].InnerText;
                                if (items.ContainsKey("StartFrame"))
                                    items["StartFrame"] = _node["StartFrame"].InnerText;
                                if (items.ContainsKey("EndFrame"))
                                    items["EndFrame"] = _node["EndFrame"].InnerText;
                                if (items.ContainsKey("PacketSize"))
                                    items["PacketSize"] = _node["PacketSize"].InnerText;
                                if (items.ContainsKey("Note"))
                                    items["Note"] = _node["Note"].InnerText;
                                #endregion

                                // append element data value to dictionary collection ..
                                Dictionary.Add(name, items);
                            }
                        }
                        #endregion
                        break;
                    #endregion

                    #region Smedge File Type
                    case RenderFileType.Smr:
                        using (TextReader tdr = new StreamReader(FullFilePath))
                        {
                            do
                            {
                                string[] kv = tdr.ReadLine().Split('=');

                                // analysis text contents ..
                                if (kv[0].IndexOf("Start") > -1)
                                    Dictionary["StartFrame"] = kv[1].Trim();
                                if (kv[0].IndexOf("End") > -1)
                                    Dictionary["EndFrame"] = kv[1].Trim();
                                if (kv[0].IndexOf("JobName") > -1)
                                    Dictionary["name"] = kv[1].Trim();
                                if (kv[0].IndexOf("Note") > -1)
                                    Dictionary["Note"] = kv[1].Trim();
                                if (kv[0].IndexOf("Packet") > -1)
                                    Dictionary["PacketSize"] = kv[1].Trim();
                                if (kv[0].IndexOf("Pool") > -1)
                                    Dictionary["FirstPool"] = kv[1].Trim();
                                if (kv[0].IndexOf("Priority") > -1)
                                    Dictionary["Priority"] = kv[1].Trim();
                                if (kv[0].IndexOf("SceneName") > -1)
                                    Dictionary["Command"] = kv[1].Trim();

                            } while (tdr.Peek() > -1);
                        }
                        break;
                    #endregion
                }
            }
            catch (IOException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                result = false;
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                result = false;
            }

            return result;
        }
        #endregion

        #region 創建備份文件 Create Backup Render File
        /// <summary>
        /// Generate render replication file.
        /// </summary>
        /// <param name="FullFilePath">full file path (drv:\dir\..\dir\file).</param>
        /// <param name="Method">usage render method.</param>
        /// <param name="Dictionary">attribute dictionary.</param>
        /// <returns>System.Boolean</returns>
        public bool Create(string FullFilePath, RenderMethod Method, IDictionary<string, object> Dictionary)
        {
            bool result = true;
            string backup_name = string.Empty;

            // check the file procedure ..
            if (!Directory.Exists(Path.GetDirectoryName(FullFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FullFilePath));

            FileInfo __info = new FileInfo(FullFilePath);

            if (__info.Exists)
            {
                if (__info.Name.IndexOf("backup") < 0)
                {
                    string[] parameters = new string[] {
                        Path.GetDirectoryName(FullFilePath),
                        Path.GetFileNameWithoutExtension(FullFilePath),
                        Path.GetExtension(FullFilePath)
                    };

                    backup_name = string.Format(@"{0}\{1}(backup){2}", parameters);
                }
                else
                    backup_name = FullFilePath;
            }

            try
            {
                // setting write type and style ..
                XmlWriterSettings xwr_settings = new XmlWriterSettings();
                //設定 XmlWriter 遵循的一致性層級，如果正在讀取或寫入的資料流不符合一致性等級，則會擲回例外狀況
                xwr_settings.ConformanceLevel = ConformanceLevel.Document;
                //設定是否要縮排項目
                xwr_settings.Indent = true;

                using (XmlWriter xwr = XmlTextWriter.Create(backup_name, xwr_settings))
                {
                    // write primary start element ..
                    xwr.WriteStartElement("JobDefinition");

                    if (Method == RenderMethod.Gui)
                    {
                        //for graphic...
                        this.WriteXML(xwr, Dictionary);
                    }
                    else
                    {
                        // for console ..只寫Value
                        foreach (KeyValuePair<string, object> kv in Dictionary)
                        {
                            this.WriteXML(xwr, (IDictionary<string, object>)kv.Value);
                        }
                    }

                    // write primary end element ..
                    xwr.WriteEndElement();

                    // flush xml data stream ..
                    xwr.Flush();
                }
            }
            catch (IOException ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                result = false;
            }
            catch (Exception ex)
            {
                string ExceptionMsg = ex.Message + ex.StackTrace;

                // write to log file ..
                base.Writer(AssemblyInfoClass.ProductInfo, Log.Level.Error, ExceptionMsg, true);

                result = false;
            }

            return result;
        }

        /// <summary>
        /// Write xml element content.
        /// </summary>
        /// <param name="Writer">executable writer object.</param>
        /// <param name="Dictionary">data content.</param>
        private void WriteXML(XmlWriter Writer, IDictionary<string, object> Dictionary)
        {
            // define attributes ..
            string[] attrs = new string[] { "name", "waitfor", "isupdateonly", "type" };

            // write secondary start element ..
            Writer.WriteStartElement("Job");

            foreach (KeyValuePair<string, object> kv in Dictionary)
            {
                string value = string.Empty;

                if (null != kv.Value)
                    value = kv.Value.ToString();

                // find same conditions of attribute value ..
                if (Array.IndexOf<string>(attrs, kv.Key.ToLower()) > -1)
                {
                    if (!string.IsNullOrEmpty(value))
                        // write attribute ..
                        Writer.WriteAttributeString(kv.Key, value);
                }
                else
                    // write element ..
                    Writer.WriteElementString(kv.Key, value);
            }

            // write secondary end element ..
            Writer.WriteEndElement();
        }
        #endregion
    }
}