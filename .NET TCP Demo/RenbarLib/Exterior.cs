using System;
using System.Runtime.InteropServices;

namespace RenbarLib.Win32
{
    /// <summary>
    /// Extern platform invoke class.
    /// </summary>
    public static class PlatformInvoke
    {
        #region Show Window Style Enumeration
        internal enum ShowWindowStyles : uint
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window.
            /// If the window is minimized or maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window for the first time.
            /// </summary>
            ShowNormal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            ShowMaximized = 3,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3,
            /// <summary>
            /// Displays a window in its most recent size and position.
            /// This value is similar to SW_SHOWNORMAL, except the window is not actived.
            /// </summary>
            ShowNormalNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window.
            /// This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            /// </summary>
            ShowMinNoActivate = 7,
            /// <summary>
            /// Displays the window in its current size and position.
            /// This value is similar to SW_SHOW, except the window is not activated.
            /// </summary>
            ShowNoActivate = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized,
            /// the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO
            /// structure passed to the CreateProcess function by the program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            /// Windows 2000/XP: Minimizes a window, even if the thread that owns the window is not responding.
            /// This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimized = 11
        }
        #endregion

        #region Remove Menu Bar Enumeration
        internal enum RemoveMenuFlags :uint
        {
            /// <summary>
            /// Indicates that uPosition gives the zero-based relative position of the menu item.
            /// </summary>
            MF_BYPOSITION = 0x0400,
            /// <summary>
            /// Disables the menu item so it cannot be selected.
            /// </summary>
            MF_DISABLED = 0x0002,
            /// <summary>
            /// Remove the menu item button.
            /// </summary>
            MF_REMOVE = 0x1000
        }
        #endregion

        #region Snapshot Flags Enumeration
        /// <summary>
        /// Snapshot flags enumeration for get parent process method used.
        /// </summary>
        [Flags]
        internal enum SnapshotFlags : uint
        {
            /// <summary>
            /// Includes all heaps of the process specified in th32ProcessID in the snapshot.
            /// To enumerate the heaps, see Heap32ListFirst.
            /// </summary>
            HeapList = 0x00000001,
            /// <summary>
            /// Includes all processes in the system in the snapshot.
            /// To enumerate the processes, see Process32First.
            /// </summary>
            Process = 0x00000002,
            /// <summary>
            /// Includes all threads in the system in the snapshot.
            /// To enumerate the threads, see Thread32First.
            /// </summary>
            Thread = 0x00000004,
            /// <summary>
            /// Includes all modules of the process specified in th32ProcessID in the snapshot.
            /// To enumerate the modules, see Module32First.
            /// </summary>
            Module = 0x00000008,
            /// <summary>
            /// Includes all 32-bit modules of the process specified in th32ProcessID
            /// in the snapshot when called from a 64-bit process.
            /// This flag can be combined with TH32CS_SNAPMODULE or TH32CS_SNAPALL.
            /// </summary>
            Module32 = 0x00000010,
            /// <summary>
            /// Indicates that the snapshot handle is to be inheritable.
            /// </summary>
            Inherit = 0x80000000,
            /// <summary>
            /// Includes all processes and threads in the system,
            /// plus the heaps and modules of the process specified in th32ProcessID.
            /// Equivalent to specifying the TH32CS_SNAPHEAPLIST, TH32CS_SNAPMODULE,
            /// TH32CS_SNAPPROCESS, and TH32CS_SNAPTHREAD values combined using an OR operation ('|').
            /// </summary>
            All = 0x0000001F
        }
        #endregion

        #region Get Parent Process Enumeration
        /// <summary>
        /// Describes an entry from a list of the processes
        /// residing in the system address space when a snapshot was taken.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            
            /// <summary>
            /// The size of the structure, in bytes.
            /// Before calling the Process32First function,
            /// set this member to sizeof(PROCESSENTRY32).
            /// If you do not initialize dwSize, Process32First fails.
            /// </summary>
            internal UInt32 dwSize;
            /// <summary>
            /// This member is no longer used and is always set to zero.
            /// </summary>
            internal UInt32 cntUsage;
            /// <summary>
            /// The process identifier.
            /// </summary>
            internal UInt32 th32ProcessID;
            /// <summary>
            /// This member is no longer used and is always set to zero.
            /// </summary>
            internal IntPtr th32DefaultHeapID;
            /// <summary>
            /// This member is no longer used and is always set to zero.
            /// </summary>
            internal UInt32 th32ModuleID;
            /// <summary>
            /// The number of execution threads started by the process.
            /// </summary>
            internal UInt32 cntThreads;
            /// <summary>
            /// The identifier of the process that created this process (its parent process).
            /// </summary>
            internal UInt32 th32ParentProcessID;
            /// <summary>
            /// The base priority of any threads created by this process.
            /// </summary>
            internal Int32 pcPriClassBase;
            /// <summary>
            /// This member is no longer used, and is always set to zero.
            /// </summary>
            internal UInt32 dwFlags;
            /// <summary>
            /// The name of the executable file for the process.
            /// To retrieve the full path to the executable file,
            /// call the Module32First function and check the szExePath
            /// member of the MODULEENTRY32 structure that is returned.
            /// However, if the calling process is a 32-bit process,
            /// you must call the QueryFullProcessImageName function to retrieve
            /// the full path of the executable file for a 64-bit process.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }
        #endregion

        /// <summary>
        /// Takes a snapshot of the specified processes,
        /// as well as the heaps, modules, and threads used by these processes.
        /// </summary>
        /// <param name="dwFlags">The portions of the system to be included in the snapshot.
        /// This parameter can be one or more of the following values.</param>
        /// <param name="th32ProcessID">The process identifier of the process to be included in the snapshot.
        /// This parameter can be zero to indicate the current process.
        /// This parameter is used when the TH32CS_SNAPHEAPLIST, TH32CS_SNAPMODULE,
        /// TH32CS_SNAPMODULE32, or TH32CS_SNAPALL value is specified. Otherwise,
        /// it is ignored and all processes are included in the snapshot.</param>
        /// <returns>System.IntPtr</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        /// <summary>
        /// Redraws the menu bar of the specified window.
        /// If the menu bar changes after the system has created the window,
        /// this function must be called to draw the changed menu bar.
        /// </summary>
        /// <param name="hwnd">handle to the window whose menu bar needs redrawing.</param>
        /// <returns>System.Int32</returns>
        [DllImport("User32.dll", EntryPoint = "DrawMenuBar", SetLastError = true)]
        internal static extern int DrawMenuBar(IntPtr hwnd);

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings.
        /// 
        /// </summary>
        /// <param name="lpClassName">
        /// Pointer to a null-terminated string
        /// that specifies the class name or a class atom created
        /// by a previous call to the RegisterClass or RegisterClassEx function.
        /// </param>
        /// <param name="lpWindowName">
        /// Pointer to a null-terminated string
        /// that specifies the window name (the window's title).
        /// If this parameter is NULL, all window names match.
        /// </param>
        /// <returns>System.Intptr</returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Determines the number of items in the specified menu.
        /// </summary>
        /// <param name="hmenu">handle to the menu to be examined.</param>
        /// <returns>System.Int32</returns>
        [DllImport("User32.dll", EntryPoint = "GetMenuItemCount", SetLastError = true)]
        internal static extern int GetMenuItemCount(IntPtr hmenu);

        /// <summary>
        /// Allows the application to access the window menu
        /// (also known as the system menu or the control menu) for copying and modifying.
        /// </summary>
        /// <param name="hwnd">handle to the window that will own a copy of the window menu.</param>
        /// <param name="revert">specifies the action to be taken.
        /// If this parameter is FALSE, GetSystemMenu returns a handle to the copy of the window menu currently in use.
        /// The copy is initially identical to the window menu, but it can be modified.
        /// If this parameter is TRUE, GetSystemMenu resets the window menu back to the default state.
        /// The previous window menu, if any, is destroyed.</param>
        /// <returns>System.Intptr</returns>
        [DllImport("User32.dll", EntryPoint = "GetSystemMenu", SetLastError = true)]
        internal static extern IntPtr GetSystemMenu(IntPtr hwnd, bool revert);

        /// <summary>
        /// Retrieves information about the first process encountered in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot returned from a previous call
        /// to the CreateToolhelp32Snapshot function.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.
        /// It contains process information such as the name of the executable file,
        /// the process identifier, and the process identifier of the parent process.</param>
        /// <returns>System.Boolean</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// Retrieves information about the next process recorded in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot returned from a previous
        /// call to the CreateToolhelp32Snapshot function.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>System.Boolean</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        internal static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// A menu item or detaches a submenu from the specified menu.
        /// If the menu item opens a drop-down menu or submenu,
        /// RemoveMenu does not destroy the menu or its handle,
        /// allowing the menu to be reused. Before this function is called,
        /// the GetSubMenu function should retrieve a handle to the drop-down menu or submenu.
        /// </summary>
        /// <param name="hmenu">handle to the menu to be changed.</param>
        /// <param name="npos">specifies the menu item to be deleted,
        /// as determined by the uFlags parameter.</param>
        /// <param name="wflags">Specifies how the uPosition parameter is interpreted.</param>
        /// <returns>System.Int32</returns>
        [DllImport("User32.dll", EntryPoint = "RemoveMenu", SetLastError = true)]
        internal static extern int RemoveMenu(IntPtr hmenu, uint npos, uint wflags);

        /// <summary>
        /// Puts the thread that created the specified window into
        /// the foreground and activates the window.
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，
        /// 并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <param name="hWnd">Handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>System.Boolean</returns>
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Sets the specified window's show state.
        /// 该函数设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hwnd">handle to the window.</param>
        /// <param name="cmndShow">specifies how the window is to be shown.</param>
        /// <returns>System.Int32</returns>
        [DllImport("User32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        internal static extern int ShowWindow(IntPtr hwnd, int cmndShow);

        /// <summary>
        /// Sets the show state of a window created by a different thread.
        /// </summary>
        /// <param name="hWnd">handle to the window.</param>
        /// <param name="cmdShow">specifies how the window is to be shown.</param>
        /// <returns>System.Boolean</returns>
        [DllImport("User32.dll", EntryPoint = "ShowWindowAsync", SetLastError = true)]
        internal static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);



    }
}