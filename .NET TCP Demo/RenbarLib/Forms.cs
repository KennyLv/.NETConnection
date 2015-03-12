using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

// Import win32 api namespace ..
using RenbarLib.Win32;


namespace RenbarLib.Environment.Forms
{
    /// <summary>
    /// Form gradually effect class.
    /// </summary>
    public class Effect
    {
        #region 窗體特效方法 Windows Forms Effects Method
        /// <summary>
        /// Disable form close button.
        /// </summary>
        /// <param name="frm">System.Windows.Forms.Form</param>
        public void DisableCloseButton(Form Form)
        {
            IntPtr hMenu = PlatformInvoke.GetSystemMenu(Form.Handle, false);
            if (hMenu != IntPtr.Zero)
            {
                int n = PlatformInvoke.GetMenuItemCount(hMenu);
                if (n > 0)
                {
                    PlatformInvoke.RemoveMenu(hMenu, (uint)(n - 1),
                        (uint)PlatformInvoke.RemoveMenuFlags.MF_BYPOSITION |
                        (uint)PlatformInvoke.RemoveMenuFlags.MF_REMOVE);

                    PlatformInvoke.RemoveMenu(hMenu, (uint)(n - 2),
                        (uint)PlatformInvoke.RemoveMenuFlags.MF_BYPOSITION |
                        (uint)PlatformInvoke.RemoveMenuFlags.MF_REMOVE);

                    PlatformInvoke.DrawMenuBar(Form.Handle);
                }
            }
        }

        /// <summary>
        /// Windows forms show opacity effect
        /// </summary>
        /// <param name="Form">System.Windows.Forms.Form</param>
        public void ShowEffect(Form Form)
        {
            for (double c = 0; c <= 1 + 0.2; c += 0.2)
            {
                Thread.Sleep(20);
                Application.DoEvents();
                Form.Opacity = c;
                Form.Refresh();
            }
        }

        /// <summary>
        /// Windows forms close opacity effect
        /// </summary>
        /// <param name="Form">System.Windows.Forms.Form</param>
        public void CloseEffect(Form Form)
        {
            for (double d = 1; d >= 0 + 0.2; d -= 0.2)
            {
                Thread.Sleep(20);
                Application.DoEvents();
                Form.Opacity = d;
                Form.Refresh();
            }
        }
        #endregion
    }

    namespace Controls
    {
        namespace ListView.Sort
        {
            /// <summary>
            /// An implementation of the 'IComparer' interface.
            /// </summary>
            public class ListViewColumnSorter : IComparer
            {
                #region Declare Global Variable Section
                /// <summary>
                /// Specifies the column to be sorted.
                /// </summary>
                private int ColumnToSort;
                /// <summary>
                /// Specifies the order in which to sort (i.e. 'Ascending').
                /// </summary>
                private SortOrder OrderOfSort;
                /// <summary>
                /// Case insensitive comparer object.
                /// </summary>
                //private CaseInsensitiveComparer ObjectCompare;
                private NumberCaseInsensitiveComparer ObjectCompare;
                private ImageTextComparer FirstObjectCompare;
                #endregion

                #region 構造列排序工具 Column Sorter Constructor Procedure
                /// <summary>
                /// Class constructor.  Initializes various elements
                /// </summary>
                public ListViewColumnSorter()
                {
                    // Initialize the column to '0'
                    ColumnToSort = 0;
                    // Initialize the sort order to 'none'
                    OrderOfSort = SortOrder.Ascending;

                    // Initialize the CaseInsensitiveComparer object
                    ObjectCompare = new NumberCaseInsensitiveComparer();
                    FirstObjectCompare = new ImageTextComparer();
                }
                #endregion

                #region 實現ICompare接口 Implementation ICompare Interface Procedure
                /// <summary>
                /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
                /// </summary>
                /// <param name="x">First object to be compared.</param>
                /// <param name="y">Second object to be compared.</param>
                /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
                public int Compare(object x, object y)
                {
                    int compareResult;
                    ListViewItem listviewX, listviewY;

                    // Cast the objects to be compared to ListViewItem objects ..
                    listviewX = (ListViewItem)x;
                    listviewY = (ListViewItem)y;

                    if (ColumnToSort == 0)
                        compareResult = FirstObjectCompare.Compare(x, y);
                    else
                        // Compare the two items ..
                        compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

                    // Calculate correct return value based on object comparison ..
                    if (OrderOfSort == SortOrder.Ascending)
                        // Ascending sort is selected, return normal result of compare operation ..
                        return compareResult;
                    else if (OrderOfSort == SortOrder.Descending)
                        // Descending sort is selected, return negative result of compare operation ..
                        return (-compareResult);
                    else
                        // Return '0' to indicate they are equal ..
                        return 0;
                }
                #endregion

                #region 排序參數（SortColumn、Order） Sort Properties
                /// <summary>
                /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
                /// </summary>
                public int SortColumn
                {
                    set
                    {
                        ColumnToSort = value;
                    }
                    get
                    {
                        return ColumnToSort;
                    }
                }

                /// <summary>
                /// Gets or sets the order of sorting to apply (‘None’,'Ascending' or 'Descending').
                /// </summary>
                public SortOrder Order
                {
                    set
                    {
                        OrderOfSort = value;
                    }
                    get
                    {
                        return OrderOfSort;
                    }
                }
                #endregion
       
            }

            /// <summary>
            /// ListView影像索引的排序
            /// </summary>
            public class ImageTextComparer : IComparer
            {
                // declare global variable ..
                private NumberCaseInsensitiveComparer ObjectCompare;

                #region ImageText Constructor Procedure
                public ImageTextComparer()
                {
                    // Initialize the CaseInsensitiveComparer object ..
                    ObjectCompare = new NumberCaseInsensitiveComparer();
                }
                #endregion

                #region Implementation ICompare Interface Procedure
                public int Compare(object x, object y)
                {
                    //int compareResult;
                    int image1, image2;
                    ListViewItem listviewX, listviewY;

                    // Cast the objects to be compared to ListViewItem objects ..
                    listviewX = (ListViewItem)x;
                    image1 = listviewX.ImageIndex;
                    listviewY = (ListViewItem)y;
                    image2 = listviewY.ImageIndex;

                    if (image1 < image2)
                        return -1;
                    else if (image1 == image2)
                        return ObjectCompare.Compare(listviewX.Text, listviewY.Text);
                    else
                        return 1;
                }
                #endregion
            }

            public class NumberCaseInsensitiveComparer : CaseInsensitiveComparer
            {
                #region 比較大小 Case Compare Procedure
                public new int Compare(object x, object y)
                {
                    //如果是數字
                    if ((x is System.String) && IsWholeNumber((string)x) && (y is System.String) && IsWholeNumber((string)y))
                    {
                        int a, b;
                        int.TryParse(x.ToString(), out a);
                        int.TryParse(y.ToString(), out b);

                        return base.Compare(a, b);
                    }
                    //如果是其他類型
                    else
                        return base.Compare(x, y);
                }
                #endregion

                #region 是否全數字 Find Number Pattern
                private bool IsWholeNumber(string strNumber)
                {
                    Regex objNotWholePattern = new Regex("[^0-9]");
                    return !objNotWholePattern.IsMatch(strNumber);
                }
                #endregion
            }
        }
    }
}