#region Using NameSpace
using NXN.Alienbrain.SDK;
#endregion

namespace RenbarLib.Extension.Alienbrain
{
    /// <summary>
    /// Alienbrain version server api class.
    /// </summary>
    public class AbLib
    {
        #region Declare Global Variable
        private static AbLib _instance;
        private Namespace _ns = null;
        private Command _cmd = null;
        private Item _item = null;
        private Item _ws = null;
        private string _User = null;
        private string _SvrName = null;
        #endregion

        #region SignOff Flag Enumreation
        public enum SignStatus : uint
        {
            /// <summary>
            /// Not in Workflow (no state icon, full access rights).
            /// </summary>
            NOT_IN_WORKFLOW = 0x1,
            /// <summary>
            /// Work in Progress (icon with a white bar, full access rights).
            /// </summary>
            WORK_IN_PROGRESS = 0x2,
            /// <summary>
            /// Awaiting Modification (icon with a red bar, full access rights).
            /// </summary>
            AWAITING_MODIFICATION = 0x3,
            /// <summary>
            /// Awaiting Sign Off (icon with a yellow bar, full access rights).
            /// </summary>
            AWAITING_SIGNOFF = 0x4,
            /// <summary>
            /// Signed Off (icon with a light green bar, full access rights).
            /// </summary>
            SIGNED_OFF = 0x5,
            /// <summary>
            /// Signed Off and Locked (icon with a dark green bar, files cannot be changed anymore).
            /// </summary>
            SIGNED_OFF_AND_LOCKED = 0x6
        }
        #endregion

        #region Dependency Store Reference Type Enumeration
        public enum DependencyStoreUsage
        {
            /// <summary>
            /// Automatic Reference Type.
            /// </summary>
            Scene,
            /// <summary>
            /// Manual Reference Type.
            /// </summary>
            Manual
        }
        #endregion

        #region Initialize Entry Point And Workspace Directory
        /// <summary>
        /// Alienbrain library constructor procedure.
        /// </summary>
        private AbLib()
        {
            // get alienbrain data object instance ..
            _ns = Namespace.GetInstance();
            _ws = _ns.GetItem(@"\Workspace");
        }

        /// <summary>
        /// Return alienbrain object instance procedure.
        /// </summary>
        public static AbLib Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AbLib();

                return _instance;
            }
        }
        #endregion

        #region Convert Alienbrain Workspace Path //////////////Revise(string Path)
        /// <summary>
        /// Convert alienbrain workspace path.
        /// </summary>
        /// <param name="Path">server path (ex: ab/cd/ef/..).</param>
        /// <returns>System.String</returns>
        private string Revise(string Path)
        {
            string __path = _item.Path + @"\" + Path;
            __path = __path.Replace('/', '\\');
            __path = __path.Replace(@"\\", @"\");

            return __path;
        }
        #endregion

        #region Change SignOff Status Procedure  //////////////ChangeStatus(string path, string comment, SignStatus Status)
        /// <summary>
        /// Corresponds to the Context Menu commands for changing the status of files.
        /// </summary>
        /// <param name="path">alienbrain namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="comment">remark text.</param>
        /// <param name="Status">signoff types.</param>
        /// <returns>System.Boolean</returns>
        public bool ChangeStatus(string path, string comment, SignStatus Status)
        {
            Command cmd = new Command("SignOff", 0);

            cmd.SetIn("StatusId", System.Convert.ToInt32(Status));
            cmd.SetIn("Comment", comment);
            cmd.SetIn("ShowDialog", false);
            _ns.RunCommand(Revise(path), cmd);

            if (cmd.WasSuccessful())
                return true;
            else
                return false;
        }
        #endregion

        #region Check Alienbrain WorkSpace Path Procedure  //////////////CheckWorkSpacePath(string SceneName)
        /// <summary>
        /// Check the file has in alienbrain server.
        /// </summary>
        /// <param name="SceneName">alienbrain full file path.</param>
        /// <returns>System.Boolean</returns>
        public bool CheckWorkSpacePath(string SceneName)
        {
            // get alienbrain item data object ..
            Item SubItems = _ns.GetItem(Revise(SceneName));

            // decide result ..
            if (SubItems == null)
                return false;
            else
                return true;
        }
        #endregion

        #region Dependency Event Procedure
        /// <summary>
        /// Dependency get reference files (use auto-reference type).
        /// </summary>
        /// <param name="SceneName">workspace namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="abNameSpace">use alienbrain workspace namespace.</param>
        /// <returns>System.String[]</returns>
        public string[] DependencyGet(string SceneName, bool abNameSpace)
        {
            string[] result = null;

            // declare alienbrain reference api ..
            global::NXN.Alienbrain.SDK.Commands.DependencyGet dependencyGet
                = new global::NXN.Alienbrain.SDK.Commands.DependencyGet();

            // setting parameters ..
            dependencyGet.DependencyMaxResults = 999;
            dependencyGet.DependencySourceBranchHandle = int.Parse(BranchGet_ActiveBranches);
            dependencyGet.DependencyType = NXN.Alienbrain.SDK.Commands.DependencyGet.DependencyTypes.Forward;

            Item subItems = null;

            // get target items ..
            if (abNameSpace)
                subItems = _ns.GetItem(Revise(SceneName));
            else
                subItems = _ns.GetItem(SceneName);

            // running command ..
            dependencyGet.RunCommand(subItems.ToTarget());

            // decide result ..
            if (dependencyGet.WasSuccessful())
            {
                int count = dependencyGet.DependencyNumberResults;
                result = new string[count];

                for (int i = 0; i < count; i++)
                    result[i] = dependencyGet.GetDependencyTargetNamespacePath(i);
            }

            return result;
        }

        /// <summary>
        /// Dependency store reference files.
        /// </summary>
        /// <param name="SourceFile">assign file, workspace namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="ReferenceFiles">reference files, workspace namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="Types">reference type.</param>
        /// <param name="ErrorFiles">reference fail files list.</param>
        /// <returns>System.Boolean</returns>
        public bool DependencyStore(string SourceFile, string[] ReferenceFiles, DependencyStoreUsage Types, ref global::System.Collections.Generic.IList<string> ErrorFiles)
        {
            // return type ..
            bool result = true;

            // declare alienbrain reference api ..
            global::NXN.Alienbrain.SDK.Commands.DependencyStore dependencyStore
                = new global::NXN.Alienbrain.SDK.Commands.DependencyStore();

            // get source file item and handle ..
            global::NXN.Alienbrain.SDK.Item SourceItem = _ns.GetItem(Revise(SourceFile));
            int __sHandle = int.Parse(BranchGet_ActiveBranches);

            for (int i = 0; i < ReferenceFiles.Length; i++)
            {
                try
                {
                    // get target item and handle ..
                    global::NXN.Alienbrain.SDK.Item SubItem = _ns.GetItem(Revise(ReferenceFiles[i]));
                    int __tHandle = int.Parse(BranchGet_ActiveBranches);

                    // setting parameters ..
                    dependencyStore.DependencySourceBranchHandle = __sHandle;
                    dependencyStore.DependencyTargetNamespacePath = SubItem.Path;
                    dependencyStore.DependencyTargetBranchHandle = __tHandle;
                    dependencyStore.DependencyType = NXN.Alienbrain.SDK.Commands.DependencyStore.DependencyTypes.Forward;
                    dependencyStore.DependencyUsage = global::System.Convert.ToString(Types);

                    // running command ..
                    dependencyStore.RunCommand(SourceItem.ToTarget());
                }
                catch (global::System.Exception)
                {
                    // add error reference file ..
                    ErrorFiles.Add(ReferenceFiles[i].ToString());

                    // continue loop ..
                    continue;
                }

                // decide alienbrain result ..
                if (!ErrorFiles.Contains(ReferenceFiles[i].ToString())
                    && !dependencyStore.WasSuccessful())
                {
                    // if alienbrain command result error, but exit loop and exit method ..
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Get active branch handle property.
        /// </summary>
        private string BranchGet_ActiveBranches
        {
            get
            {
                // declare alienbrain reference api ..
                global::NXN.Alienbrain.SDK.Commands.BranchGetActiveBranches bga
                    = new global::NXN.Alienbrain.SDK.Commands.BranchGetActiveBranches();

                // running command ..
                bga.RunCommand(_item.ToTarget());

                // decide result ..
                if (bga.WasSuccessful())
                    return bga.Handles;
                else
                    return "0";
            }
        }
        #endregion

        #region Login, Logout Alienbrain Server Procedure
        /// <summary>
        /// Login alienbrain version server.
        /// </summary>
        /// <param name="VersionSvr">server host name (ex: emosvr.emo.lan).</param>
        /// <param name="Project">load project.</param>
        /// <param name="User">login user.</param>
        /// <param name="Password">login password.</param>
        /// <returns>System.Boolean</returns>
        public bool Login(string VersionSvr, string Project, string User, string Password)
        {
            //¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H¡H
        reload:
            // declare alienbrain reference api ..
            _cmd = new Command("ProjectLoadEx", 0);

            // setting parameters ..
            _cmd.SetIn("Name", Project);
            _cmd.SetIn("Username", User);
            _cmd.SetIn("Password", Password);
            _cmd.SetIn("Hostname", VersionSvr);
            _cmd.SetIn("ShowDialog", false);//ANDY F7300029
            _cmd.SetIn("TimeSyncPolicy", "Always");
            _cmd.SetIn("LogonType", "1");

            try
            {
                // running command ..
                _ws.RunCommand(_cmd);
            }
            catch (global::System.Exception)
            {
                if (ProjectInsert(VersionSvr, Project, User, Password))
                    // reset execute ProjectLoadEx command ..
                    goto reload;
                else
                    return false;
            }

            // decide result ..
            if (_cmd.WasSuccessful())
            {
                _item = _ns.GetItem(@"\Workspace\" + Project);

                if (_item != null)
                {
                    _User = _item.GetProperty("UserName");
                    _SvrName = _item.GetProperty("ServerName");

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Inserts an existing project into a workspace
        /// (without quering the user for the project to insert).
        /// </summary>
        /// <param name="VersionSvr">server host name (ex: emosvr.emo.lan).</param>
        /// <param name="Project">load project.</param>
        /// <param name="User">login user.</param>
        /// <param name="Password">login password.</param>
        /// <returns>System.Boolean</returns>
        private bool ProjectInsert(string VersionSvr, string Project, string User, string Password)
        {
            // declare alienbrain reference api ..
            _cmd = new global::NXN.Alienbrain.SDK.Command("ProjectInsertEx", 0);

            // setting parameters ..
            _cmd.SetIn("Name", Project);
            _cmd.SetIn("Username", User);
            _cmd.SetIn("Password", Password);
            _cmd.SetIn("Hostname", VersionSvr);
            _cmd.SetIn("ShowDialog", "0");
            _cmd.SetIn("TimeSyncPolicy", "Always");
            _cmd.SetIn("LogonType", "1");

            // running command ..
            _ws.RunCommand(_cmd);

            if (_cmd.WasSuccessful())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Logout alienbrain version server.
        /// </summary>
        /// <param name="Project">unload current project.</param>
        /// <returns>System.Boolean</returns>
        public bool Logout(string Project)
        {
            // decide the item instance ..
            if (_item == null)
                return true;

            // declare alienbrain reference api ..
            _cmd = new Command("ProjectUnloadEx", 0);

            // setting parameters ..
            _cmd.SetIn("Name", Project);

            // running command ..
            _ws.RunCommand(_cmd);

            // decide result ..
            if (_cmd.WasSuccessful())
                return true;
            else
                return false;
        }
        #endregion

        #region Import, GetLatest Event Procedure
        /// <summary>
        /// Import local file to alienbrain version server.
        /// </summary>
        /// <param name="Path">alienbrain namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="LocalPath">upload file path.</param>
        /// <param name="Comment">upload alienbrain server file comment.</param>
        /// <returns>System.Boolean</returns>
        public bool Import(string Path, string LocalPath, string Comment)
        {
            // declare alienbrain reference api ..
            _cmd = new Command("Import", 0);

            // setting parameters ..
            _cmd.SetIn("Comment", Comment);
            _cmd.SetIn("LocalPath", LocalPath);
            _cmd.SetIn("CreateVersion", 2);
            _cmd.SetIn("ShowDialog", false);
            _cmd.SetVerboseLevel(Command.VerboseLevels.Error);

            // running command ..
            _ns.RunCommand(Revise("/" + Path), _cmd);

            // decide result ..
            if (_cmd.WasSuccessful())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get latest version from alienbrain version server.
        /// </summary>
        /// <param name="Path">alienbrain namespace path (ex: ab\cd\ef\..).</param>
        /// <param name="OverwriteWritable">Overwrite already get latest file.</param>
        /// <param name="abNameSpace">use alienbrain workspace namespace.</param>
        /// <returns>System.Boolean</returns>
        public bool GetLatest(string Path, bool OverwriteWritable, bool abNameSpace)
        {
            // declare alienbrain reference api ..
            _cmd = new Command("GetLatest", 0);

            // setting parameters ..
            _cmd.SetIn("OverwriteWritable", OverwriteWritable);
            _cmd.SetIn("SmartGet", true);
            _cmd.SetIn("ShowDialog", false);

            // decide convert alienbrain workspace path ..
            if (abNameSpace)
                _ns.RunCommand(Revise(Path), _cmd);
            else
                _ns.RunCommand(Path, _cmd);

            // decide result ..
            if (_cmd.WasSuccessful())
                return true;
            else
                return false;
        }
        #endregion

        #region Set Working Path Event Procedure
        /// <summary>
        /// Set working path.
        /// </summary>
        /// <param name="Path">setting working path.</param>
        /// <returns>System.Boolean</returns>
        public bool SetWorkingPath(string Path)
        {
            // declare alienbrain reference api ..
            _cmd = new Command("SetWorkingPath", 0);

            // setting parameters ..
            _cmd.SetIn("Path", Path);
            _cmd.SetIn("ShowDialog", false);

            // running command ..
            _ns.RunCommand(_item.Path, _cmd);

            // decide result ..
            if (_cmd.WasSuccessful())
                return true;
            else
                return false;
        }
        #endregion
    }
}