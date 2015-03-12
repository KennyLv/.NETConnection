#region Using NameSpace
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion

namespace RenbarLib.Environment.Forms.Controls
{
    /// <summary>
    /// Notification component class.
    /// </summary>
    public class Notification : Form
    {
        #region Declare Global Variable Section
        // ******************************* OnTimer behaivor variable *********************************//
        protected Bitmap BackgroundBitmap = null;
        protected Bitmap CloseBitmap = null;
        protected Point CloseBitmapLocation;
        protected Size CloseBitmapSize;
        protected Rectangle RealTitleRectangle;
        protected Rectangle RealContentRectangle;
        protected Rectangle WorkAreaRectangle;
        protected Timer timer = new Timer();
        protected States taskbarState = States.HIDDEN;
        protected string titleText;
        protected string contentText;
        protected Color normalTitleColor = Color.FromArgb(255, 0, 0);
        protected Color hoverTitleColor = Color.FromArgb(255, 0, 0);
        protected Color normalContentColor = Color.FromArgb(0, 0, 0);
        protected Color hoverContentColor = Color.FromArgb(0, 0, 0x66);
        protected Font normalTitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverTitleFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        protected Font normalContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font hoverContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected int nShowEvents;
        protected int nHideEvents;
        protected int nVisibleEvents;
        protected int nIncrementShow;
        protected int nIncrementHide;
        protected bool bIsMouseOverPopup = false;
        protected bool bIsMouseOverClose = false;
        protected bool bIsMouseOverContent = false;
        protected bool bIsMouseOverTitle = false;
        protected bool bIsMouseDown = false;
        protected bool bKeepVisibleOnMouseOver = true;
        protected bool bReShowOnMouseOver = false;
        // *******************************************************************************************//

        // ********************************** Appearance and draw variable ***************************//s
        public Rectangle TitleRectangle;
        public Rectangle ContentRectangle;
        public bool TitleClickable = false;
        public bool ContentClickable = true;
        public bool CloseClickable = true;
        public bool EnableSelectionRectangle = true;
        public event EventHandler CloseClick = null;
        public event EventHandler TitleClick = null;
        public event EventHandler ContentClick = null;
        // *******************************************************************************************//
        #endregion

        #region 枚舉通告欄顯示狀態 Notification Status Enumeration
        /// <summary>
        /// List of the different popup animation status
        /// </summary>
        public enum States
        {
            HIDDEN = 0,
            APPEARING = 1,
            VISIBLE = 2,
            DISAPPEARING = 3
        }
        #endregion

        #region 構造函數 Primary Constructor Procedure
        /// <summary>
        /// Controls constructor procedure.
        /// </summary>
        public Notification()
        {
            // 設定顯示樣式 setting window style ..
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            base.Show();
            base.Hide();

            WindowState = FormWindowState.Normal;
            //不在工作列中顯示
            ShowInTaskbar = false;
            TopMost = true;
            //不顯示最大最小
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            //顯示時間及方式
            timer.Enabled = true;
            timer.Tick += new EventHandler(OnTimer);//？？？？？？？？何時啟動？？？？？？？？？？？？
        }
        #endregion

        #region 屬性 接受顯示樣式設置 Appearance Style Properties
        /// <summary>
        /// Get the current notifier states. (hidden, showing, visible, hiding)
        /// </summary>
        public States CurrentState
        {
            get
            {
                return this.taskbarState;
            }
        }

        /// <summary>
        /// Get or set the popup title text
        /// </summary>
        public string TitleText
        {
            get
            {
                return this.titleText;
            }
            set
            {
                this.titleText = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the popup content text
        /// </summary>
        public string ContentText
        {
            get
            {
                return this.contentText;
            }
            set
            {
                this.contentText = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the normal title color
        /// </summary>
        public Color NormalTitleColor
        {
            get
            {
                return this.normalTitleColor;
            }
            set
            {
                this.normalTitleColor = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the hover title color
        /// </summary>
        public Color HoverTitleColor
        {
            get
            {
                return this.hoverTitleColor;
            }
            set
            {
                this.hoverTitleColor = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the normal content color.
        /// </summary>
        public Color NormalContentColor
        {
            get
            {
                return this.normalContentColor;
            }
            set
            {
                this.normalContentColor = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the hover content color.
        /// </summary>
        public Color HoverContentColor
        {
            get
            {
                return this.hoverContentColor;
            }
            set
            {
                this.hoverContentColor = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the normal title font.
        /// </summary>
        public Font NormalTitleFont
        {
            get
            {
                return this.normalTitleFont;
            }
            set
            {
                this.normalTitleFont = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the hover title font.
        /// </summary>
        public Font HoverTitleFont
        {
            get
            {
                return this.hoverTitleFont;
            }
            set
            {
                this.hoverTitleFont = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the normal content font.
        /// </summary>
        public Font NormalContentFont
        {
            get
            {
                return this.normalContentFont;
            }
            set
            {
                this.normalContentFont = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Get or set the hover content font.
        /// </summary>
        public Font HoverContentFont
        {
            get
            {
                return this.hoverContentFont;
            }
            set
            {
                this.hoverContentFont = value;

                // refresh content window ..
                this.Refresh();
            }
        }

        /// <summary>
        /// Indicates if the popup should remain visible when the mouse pointer is over it.
        /// </summary>
        public bool KeepVisibleOnMousOver
        {
            get
            {
                return this.bKeepVisibleOnMouseOver;
            }
            set
            {
                this.bKeepVisibleOnMouseOver = value;
            }
        }

        /// <summary>
        /// Indicates if the popup should appear again when mouse moves over it while it's disappearing.
        /// </summary>
        public bool ReShowOnMouseOver
        {
            get
            {
                return this.bReShowOnMouseOver;
            }
            set
            {
                this.bReShowOnMouseOver = value;
            }
        }
        #endregion

        #region 方法 控制通告欄顯示及樣式 Notification Behaivor Procedure
        /// <summary>
        /// Displays the popup for a certain amount of time.？？？？？？？？使用Timer控制顯示動畫？？？？？重復地方改進？？？？？？
        /// </summary>
        /// <param name="strTitle">The string which will be shown as the title of the popup.</param>
        /// <param name="strContent">The string which will be shown as the content of the popup.</param>
        /// <param name="nTimeToShow">顯示動畫時間Duration of the showing animation. (in milliseconds)</param>
        /// <param name="nTimeToStay">停留時間Duration of the visible state before collapsing. (in milliseconds)</param>
        /// <param name="nTimeToHide">消失動畫時間Duration of the hiding animation. (in milliseconds)</param>
        public void Show(string strTitle, string strContent, int nTimeToShow, int nTimeToStay, int nTimeToHide)
        {
            // assign the background image area ..
            this.WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);

            // assign title, content and stay time ..
            this.titleText = strTitle;
            this.contentText = strContent;
            this.nVisibleEvents = nTimeToStay;

            // calcuate rectangles ..
            this.CalculateMouseRectangles();

            // 計算顯示動畫的像素時間增量 calculate the pixel increment and the timer value for the showing animation ..
            int nEvents;
            if (nTimeToShow > 10)
            {
                nEvents = Math.Min((nTimeToShow / 10), this.BackgroundBitmap.Height);

                this.nShowEvents = nTimeToShow / nEvents;
                this.nIncrementShow = this.BackgroundBitmap.Height / nEvents;
            }
            else
            {
                this.nShowEvents = 10;
                this.nIncrementShow = this.BackgroundBitmap.Height;
            }

            // 計算隱藏動畫的像素時間增量 calculate the pixel increment and the timer value for the hiding animation ..
            if (nTimeToHide > 10)
            {
                nEvents = Math.Min((nTimeToHide / 10), this.BackgroundBitmap.Height);

                this.nHideEvents = nTimeToHide / nEvents;
                this.nIncrementHide = this.BackgroundBitmap.Height / nEvents;
            }
            else
            {
                this.nHideEvents = 10;
                this.nIncrementHide = this.BackgroundBitmap.Height;
            }


            ///？？？？？？？？？？？？？重復？？？？？？？重復？？？？？？？？？？？
            #region Setting Current Taskbar State Behaivor
            switch (this.taskbarState)
            {
                case States.HIDDEN:
                    // assign currently behaivor states ..
                    this.taskbarState = States.APPEARING;

                    // setting control location and size ..
                    this.SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - 1, BackgroundBitmap.Width, 0);

                    // setting tick time and start the timer ..
                    timer.Interval = nShowEvents;
                    timer.Start();

                    // show the popup without stealing focus ..
                    global::RenbarLib.Win32.PlatformInvoke.ShowWindow(this.Handle, 4);
                    break;

                case States.APPEARING:
                    // refresh window ..
                    this.Refresh();
                    break;

                case States.VISIBLE:
                    // stop timer ..
                    timer.Stop();

                    // reset tick time and start the timer ..
                    timer.Interval = nVisibleEvents;
                    timer.Start();

                    // refresh window ..
                    this.Refresh();
                    break;

                case States.DISAPPEARING:
                    // stop timer ..
                    timer.Stop();

                    // assign currently behaivor states ..
                    this.taskbarState = States.VISIBLE;

                    // setting control location and size ..
                    this.SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - BackgroundBitmap.Height - 1, BackgroundBitmap.Width, BackgroundBitmap.Height);

                    // reset tick time and start the timer ..
                    timer.Interval = nVisibleEvents;
                    timer.Start();

                    // refresh window ..
                    this.Refresh();
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        public new void Hide()
        {
            if (taskbarState != States.HIDDEN)
            {
                // stop timer ..
                timer.Stop();

                // assign currently behaivor states ..
                this.taskbarState = States.HIDDEN;

                // hide the control ..
                base.Hide();
            }
        }

        /// <summary>
        /// Settings the background bitmap and its transparency color.
        /// </summary>
        /// <param name="strFilename">Path of the background bitmap on the disk.</param>
        /// <param name="transparencyColor">color of the bitmap which won't be visible.</param>
        public void SetBackgroundBitmap(string strFilename, Color transparencyColor)
        {
            // create and assign bitmap to background image ..
            this.BackgroundBitmap = new Bitmap(strFilename);

            // settings width, height and region ..
            this.Width = this.BackgroundBitmap.Width;
            this.Height = this.BackgroundBitmap.Height;
            //
            this.Region = this.BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        /// Sets the background bitmap and its transparency color.
        /// </summary>
        /// <param name="image">Image or bitmap object which represents the background bitmap.</param>
        /// <param name="transparencyColor">Color of the bitmap which won't be visible.</param>
        public void SetBackgroundBitmap(Image image, Color transparencyColor)
        {
            // create and assign bitmap to background image ..
            this.BackgroundBitmap = new Bitmap(image);

            // settings width, height and region ..
            this.Width = BackgroundBitmap.Width;
            this.Height = BackgroundBitmap.Height;
            this.Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        /// <summary>
        /// Sets the 3-State close button bitmap, its transparency color and its coordinates.
        /// </summary>
        /// <param name="strFilename">Path of the 3-state close button bitmap on the disk. (width must a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible.</param>
        /// <param name="position">Location of the close button on the popup.</param>
        public void SetCloseBitmap(string strFilename, Color transparencyColor, Point position)
        {
            // create and assign bitmap to background image ..
            this.CloseBitmap = new Bitmap(strFilename);

            // settings transparent effect, size and location ..
            this.CloseBitmap.MakeTransparent(transparencyColor);
            this.CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            this.CloseBitmapLocation = position;
        }

        /// <summary>
        /// Sets the 3-State Close Button bitmap, its transparency color and its coordinates
        /// </summary>
        /// <param name="image">Image/Bitmap object which represents the 3-state Close button Bitmap (width must be a multiple of 3)</param>
        /// <param name="transparencyColor">Color of the Bitmap which won't be visible</param>
        /// /// <param name="position">Location of the close button on the popup</param>
        /// <returns>Nothing</returns>
        public void SetCloseBitmap(Image image, Color transparencyColor, Point position)
        {
            // create and assign bitmap to background image ..
            this.CloseBitmap = new Bitmap(image);

            // settings transparent effect, size and location ..
            this.CloseBitmap.MakeTransparent(transparencyColor);
            this.CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            this.CloseBitmapLocation = position;
        }
        #endregion

        #region 私有方法 繪制窗體 Draw Notify Control Event Procedure
        /// <summary>
        /// Draw close button on background image.
        /// </summary>
        /// <param name="grfx">draw graphics object.</param>
        protected void DrawCloseButton(Graphics grfx)
        {
            if (CloseBitmap != null)
            {
                // create rectangle instance ..
                Rectangle rectDest = new Rectangle(CloseBitmapLocation, CloseBitmapSize);
                Rectangle rectSrc;

                if (bIsMouseOverClose)
                {
                    if (bIsMouseDown)
                        rectSrc = new Rectangle(new Point(this.CloseBitmapSize.Width * 2, 0), this.CloseBitmapSize);
                    else
                        rectSrc = new Rectangle(new Point(this.CloseBitmapSize.Width, 0), this.CloseBitmapSize);
                }
                else
                    rectSrc = new Rectangle(new Point(0, 0), CloseBitmapSize);

                // draw close button image ..
                grfx.DrawImage(CloseBitmap, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Draw text on background image.
        /// </summary>
        /// <param name="grfx">draw graphics object.</param>
        protected void DrawText(Graphics grfx)
        {
            if (titleText != null && titleText.Length != 0)
            {
                // create string layout object instance ..
                StringFormat sf = new StringFormat();

                // settings layout properties ..
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                // decide different string transparent ..
                if (this.bIsMouseOverTitle)
                    grfx.DrawString(titleText, hoverTitleFont, new SolidBrush(hoverTitleColor), TitleRectangle, sf);
                else
                    grfx.DrawString(titleText, normalTitleFont, new SolidBrush(normalTitleColor), TitleRectangle, sf);
            }

            if (contentText != null && contentText.Length != 0)
            {
                // create string layout object instance ..
                StringFormat sf = new StringFormat();

                // settings layout properties ..
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                sf.Trimming = StringTrimming.Word;

                if (bIsMouseOverContent)
                {
                    // draw string transparent ..
                    grfx.DrawString(contentText, hoverContentFont, new SolidBrush(hoverContentColor), ContentRectangle, sf);

                    // draw over content border rectangle ..
                    if (this.EnableSelectionRectangle)
                        ControlPaint.DrawBorder3D(grfx, RealContentRectangle, Border3DStyle.Etched, Border3DSide.Top | Border3DSide.Bottom | Border3DSide.Left | Border3DSide.Right);

                }
                else
                    // draw string transparent ..
                    grfx.DrawString(contentText, normalContentFont, new SolidBrush(normalContentColor), ContentRectangle, sf);
            }
        }

        /// <summary>
        /// Calcuate mouse rectangles.
        /// </summary>
        protected void CalculateMouseRectangles()
        {
            // create draw graphics object instance ..
            Graphics grfx = CreateGraphics();

            // create string layout object instance ..
            StringFormat sf = new StringFormat();

            // settings layout properties ..
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

            // measure title and content string ..
            SizeF sizefTitle = grfx.MeasureString(titleText, hoverTitleFont, TitleRectangle.Width, sf);
            SizeF sizefContent = grfx.MeasureString(contentText, hoverContentFont, ContentRectangle.Width, sf);

            // disponse draw graphics object ..
            grfx.Dispose();

            // should check if the title size really fits inside the pre-defined title rectangle ..
            if (sizefTitle.Height > TitleRectangle.Height)
                this.RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, TitleRectangle.Width, TitleRectangle.Height);
            else
                this.RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, (int)sizefTitle.Width, (int)sizefTitle.Height);

            this.RealTitleRectangle.Inflate(0, 2);

            // should check if the content size really fits inside the pre-defined content rectangle ..
            if (sizefContent.Height > ContentRectangle.Height)
                this.RealContentRectangle = new Rectangle((ContentRectangle.Width - (int)sizefContent.Width) / 2 + ContentRectangle.Left, ContentRectangle.Top, (int)sizefContent.Width, ContentRectangle.Height);
            else
                this.RealContentRectangle = new Rectangle((ContentRectangle.Width - (int)sizefContent.Width) / 2 + ContentRectangle.Left, (ContentRectangle.Height - (int)sizefContent.Height) / 2 + ContentRectangle.Top, (int)sizefContent.Width, (int)sizefContent.Height);

            this.RealContentRectangle.Inflate(0, 2);
        }

        /// <summary>
        /// 繪制位圖區域 Return bitmap region.
        /// </summary>
        /// <param name="bitmap">measure bitmap object.</param>
        /// <param name="transparencyColor">transparency color.</param>
        /// <returns>System.Drawing.Region</returns>
        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int height = bitmap.Height;
            int width = bitmap.Width;
            //逐像素繪制圖形
            GraphicsPath path = new GraphicsPath();
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;
                    int x0 = i;
                    while ((i < width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;
                    path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }
            }
            Region region = new Region(path);
            path.Dispose();
            return region;
        }
        #endregion

        #region 設定通告欄顯示時間及方式 Notification OnTimer Behaivor Events
        protected void OnTimer(Object obj, EventArgs e)
        {
            switch (taskbarState)
            {
                case States.APPEARING:
                    if (this.Height < this.BackgroundBitmap.Height)
                        // setting control location and size ..
                        this.SetBounds(Left, Top - nIncrementShow, Width, Height + nIncrementShow);
                    else
                    {
                        // stop timer ..
                        timer.Stop();

                        // assig background image height ..
                        this.Height = this.BackgroundBitmap.Height;

                        // assign currently behaivor states ..
                        taskbarState = States.VISIBLE;

                        // setting tick time and start the timer ..
                        timer.Interval = nVisibleEvents;
                        timer.Start();
                    }
                    break;

                
                case States.VISIBLE:
                    // stop timer ..
                    timer.Stop();

                    // setting tick time ..
                    timer.Interval = nHideEvents;
                    if ((bKeepVisibleOnMouseOver && !bIsMouseOverPopup) || (!bKeepVisibleOnMouseOver))
                        // assign currently behaivor states ..
                        taskbarState = States.DISAPPEARING;

                    // strat timer ..
                    timer.Start();
                    break;

                case States.DISAPPEARING:
                    if (bReShowOnMouseOver && bIsMouseOverPopup)
                        // assign currently behaivor states ..
                        taskbarState = States.APPEARING;
                    else
                    {
                        if (this.Top < this.WorkAreaRectangle.Bottom)
                            // setting control location and size ..
                            this.SetBounds(Left, Top + nIncrementHide, Width, Height - nIncrementHide);
                        else
                            this.Hide();
                    }

                    break;
            }

        }
        #endregion

        #region 響應鼠標事件 Notification Override Event Procedure
        /// <summary>
        /// Override the form OnMouseEnter method.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            // trigger mouse enter event ..
            base.OnMouseEnter(e);

            this.bIsMouseOverPopup = true;

            // refresh mouse leave window ..
            this.Refresh();
        }

        /// <summary>
        /// Override the form OnMouseLeave method.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            // trigger mouse leave event ..
            base.OnMouseLeave(e);

            this.bIsMouseOverPopup = false;
            this.bIsMouseOverClose = false;
            this.bIsMouseOverTitle = false;
            this.bIsMouseOverContent = false;

            // refresh mouse leave window ..
            this.Refresh();
        }

        /// <summary>
        /// Override the form OnMouseMove method.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // trigger mouse move event ..
            base.OnMouseMove(e);

            bool bContentModified = false;

            if ((e.X > this.CloseBitmapLocation.X) &&
                (e.X < this.CloseBitmapLocation.X + this.CloseBitmapSize.Width) &&
                (e.Y > this.CloseBitmapLocation.Y) &&
                (e.Y < this.CloseBitmapLocation.Y + this.CloseBitmapSize.Height) &&
                this.CloseClickable)
            {
                if (!this.bIsMouseOverClose)
                {
                    this.bIsMouseOverClose = true;
                    this.bIsMouseOverTitle = false;
                    this.bIsMouseOverContent = false;
                    this.Cursor = Cursors.Hand;

                    bContentModified = true;
                }
            }
            else if (this.RealContentRectangle.Contains(new Point(e.X, e.Y)) && this.ContentClickable)
            {
                if (!this.bIsMouseOverContent)
                {
                    this.bIsMouseOverClose = false;
                    this.bIsMouseOverTitle = false;
                    this.bIsMouseOverContent = true;
                    this.Cursor = Cursors.Hand;

                    bContentModified = true;
                }
            }
            else if (this.RealTitleRectangle.Contains(new Point(e.X, e.Y)) && this.TitleClickable)
            {
                if (!bIsMouseOverTitle)
                {
                    this.bIsMouseOverClose = false;
                    this.bIsMouseOverTitle = true;
                    this.bIsMouseOverContent = false;
                    this.Cursor = Cursors.Hand;

                    bContentModified = true;
                }
            }
            else
            {
                if (this.bIsMouseOverClose || this.bIsMouseOverTitle || this.bIsMouseOverContent)
                    bContentModified = true;

                this.bIsMouseOverClose = false;
                this.bIsMouseOverTitle = false;
                this.bIsMouseOverContent = false;
                this.Cursor = Cursors.Default;
            }

            if (bContentModified)
                // refresh mouse move window ..
                this.Refresh();
        }

        /// <summary>
        /// Override the form OnMouseDown method.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // trigger mouse down event ..
            base.OnMouseDown(e);

            this.bIsMouseDown = true;

            if (bIsMouseOverClose)
                // refresh mouse down window ..
                this.Refresh();
        }

        /// <summary>
        /// Override the form OnMouseUp method.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // trigger mouse up event ..
            base.OnMouseUp(e);

            bIsMouseDown = false;

            if (this.bIsMouseOverClose)
            {
                this.Hide();

                if (this.CloseClick != null)
                    this.CloseClick(this, new EventArgs());
            }
            else if (this.bIsMouseOverTitle)
            {
                if (this.TitleClick != null)
                    this.TitleClick(this, new EventArgs());
            }
            else if (this.bIsMouseOverContent)
            {
                if (this.ContentClick != null)
                    this.ContentClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// Override the form OnPaintBackground method.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // assign draw image of graphics object ..
            Graphics grfx = e.Graphics;

            // assign draw graphics unit ..
            grfx.PageUnit = GraphicsUnit.Pixel;

            Graphics offScreenGraphics;
            Bitmap offscreenBitmap;

            offscreenBitmap = new Bitmap(this.BackgroundBitmap.Width, this.BackgroundBitmap.Height);
            offScreenGraphics = Graphics.FromImage(offscreenBitmap);

            if (this.BackgroundBitmap != null)
                offScreenGraphics.DrawImage(this.BackgroundBitmap, 0, 0, this.BackgroundBitmap.Width, this.BackgroundBitmap.Height);

            this.DrawCloseButton(offScreenGraphics);
            this.DrawText(offScreenGraphics);

            grfx.DrawImage(offscreenBitmap, 0, 0);
        }
        #endregion

        private void InitializeComponent()
        {
            //掛起
            this.SuspendLayout();
            // 
            // Notification
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "Notification";
            this.Load += new System.EventHandler(this.Notification_Load);
            //重新顯示
            this.ResumeLayout(false);

        }
        //？？？？？？？？？？？？實現何種操作？？？？？？？？？？？？？？？？？？？？？
        private void Notification_Load(object sender, EventArgs e)
        {

        }
    }
}