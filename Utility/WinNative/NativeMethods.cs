using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.WinNative
{
#pragma warning disable 1591  //better find expiation on the web. It is much more in detail

    /// <summary>
    /// Native methods, to be usable from all all components
    /// </summary>
    public static partial class NativeMethods
    {
        #region File...

        // Constants used in DLL methods 

        // desired access
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;

        // share mode.
        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const uint FILE_SHARE_DELETE = 0x00000004;

        // CreateFile Flags.
        internal const uint OPEN_EXISTING = 3;

        internal const uint FSCTL_LOCK_VOLUME = 0x00090018;
        internal const uint FSCTL_DISMOUNT_VOLUME = 0x00090020;

        // device types.
        internal const uint FILE_DEVICE_MASS_STORAGE = 0x0000002d;
        internal const uint IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;

        // access.
        internal const uint FILE_ANY_ACCESS = 0;
        internal const uint FILE_READ_ACCESS = 1;
        // method
        internal const uint METHOD_BUFFERED = 0;

        internal const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        internal const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        internal const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        internal const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        internal const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
        internal const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        //#define CTL_CODE( DeviceType, Function, GetTasksList, Access ) ( ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (GetTasksList) )
        internal const uint IOCTL_STORAGE_MEDIA_REMOVAL = ((IOCTL_STORAGE_BASE << 16) | (FILE_READ_ACCESS << 14) | (0x0201 << 2) | METHOD_BUFFERED);
        internal const uint IOCTL_STORAGE_EJECT_MEDIA = ((IOCTL_STORAGE_BASE << 16) | (FILE_READ_ACCESS << 14) | (0x0202 << 2) | METHOD_BUFFERED);
        internal const uint IOCTL_STORAGE_LOAD_MEDIA = ((IOCTL_STORAGE_BASE << 16) | (FILE_READ_ACCESS << 14) | (0x0203 << 2) | METHOD_BUFFERED);
        internal const uint IOCTL_STORAGE_LOAD_MEDIA2 = ((IOCTL_STORAGE_BASE << 16) | (FILE_ANY_ACCESS << 14) | (0x0203 << 2) | METHOD_BUFFERED);

        internal static IntPtr INVALID_HANDLE = new IntPtr(-1);

        // Get a File Handle 
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName,
            uint DesiredAccess,
            uint ShareMode,
            IntPtr lpSecurityAttributes,
            uint CreationDisposition,
            uint FlagsAndAttributes,
            IntPtr TemplateFile);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return value is of no use")]
        [DllImport("kernel32", SetLastError = true)]
        public static extern void CloseHandle(IntPtr driveHandle);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(IntPtr driveHandle,
                uint dwIoControlCode,
                byte[] lpInBuffer,
                uint nInBufferSize,
                byte[] lpOutBuffer,
                uint nOutBufferSize,
                out uint lpBytesReturned,
                IntPtr lpOverlapped);
        #endregion

        #region ShowWindowStuff

        /*
        * ShowWindow() Commands
        */
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_MAX = 11;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool FlashWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)]bool bInvert);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll ", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        #endregion

        #region WM_... window messages

        public const int WM_ENABLE = 0x000A;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_INITMENUPOPUP = 0x0117;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MOVING = 0x0216;
        public const int WM_DRAWCLIPBOARD = 0x0308;
        public const int WM_CHANGECBCHAIN = 0x030D;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_USER = 0x0400;

        public const int WS_VISIBLE = 0x10000000;
        public const int WS_CHILD = 0x40000000;
        public const int WM_SIZE = 0x0005;
        public const int WM_CLOSE = 0x0010;
        public const int WM_PAINT = 15;
        public const int WM_MOVE = 3;

        #endregion

        #region SetWindowPosFunctionality
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_NOZORDER = 0x0004;
        public const UInt32 SWP_NOREDRAW = 0x0008;
        public const UInt32 SWP_NOACTIVATE = 0x0010;
        public const UInt32 SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        public const UInt32 SWP_SHOWWINDOW = 0x0040;
        public const UInt32 SWP_HIDEWINDOW = 0x0080;
        public const UInt32 SWP_NOCOPYBITS = 0x0100;
        public const UInt32 SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        public const UInt32 SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        #endregion

        #region Flags for WM_NCHITTEST
        public const Int32 HTERROR = -2;
        public const Int32 HTTRANSPARENT = -1;
        public const Int32 HTNOWHERE = 0;
        public const Int32 HTCLIENT = 1;
        public const Int32 HTCAPTION = 2;
        public const Int32 HTSYSMENU = 3;
        public const Int32 HTGROWBOX = 4;
        public const Int32 HTSIZE = HTGROWBOX;
        public const Int32 HTMENU = 5;
        public const Int32 HTHSCROLL = 6;
        public const Int32 HTVSCROLL = 7;
        public const Int32 HTMINBUTTON = 8;
        public const Int32 HTMAXBUTTON = 9;
        public const Int32 HTLEFT = 10;
        public const Int32 HTRIGHT = 11;
        public const Int32 HTTOP = 12;
        public const Int32 HTTOPLEFT = 13;
        public const Int32 HTTOPRIGHT = 14;
        public const Int32 HTBOTTOM = 15;
        public const Int32 HTBOTTOMLEFT = 16;
        public const Int32 HTBOTTOMRIGHT = 17;
        public const Int32 HTBORDER = 18;
        public const Int32 HTREDUCE = HTMINBUTTON;
        public const Int32 HTZOOM = HTMAXBUTTON;
        public const Int32 HTSIZEFIRST = HTLEFT;
        public const Int32 HTSIZELAST = HTBOTTOMRIGHT;
        public const Int32 HTOBJECT = 19;
        public const Int32 HTCLOSE = 20;
        public const Int32 HTHELP = 21;
        #endregion

        internal const UInt32 SC_RESTORE = 0xF120;

        #region power management

        public const uint SC_SCREENSAVE = 0xF140;
        public const uint SC_MONITORPOWER = 0xF170;
        public const uint WM_POWERBROADCAST = 0x218;
        public const uint PBT_APMSUSPEND = 0x4;
        public const uint PBT_APMPOWERSTATUSCHANGE = 0x000A;

        /// <summary>
        /// Enables applications to inform the system that it is in use.
        /// It prevents the system to enter in sleeping state or turning off the display.
        /// </summary>
        /// <param name="esFlags">new execution state</param>
        /// <returns>previous execution state</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [Flags]
        public enum ExecutionState : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_CONTINUOUS = 0x80000000
        }

        [DllImport("kernel32", EntryPoint = "GetSystemPowerStatus")]
        public static extern bool GetSystemPowerStatus(ref SystemPowerStatus systemPowerStatus);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemPowerStatus
        {
            public byte ACLineStatus;
            public byte batteryFlag;
            public byte batteryLifePercent;
            public byte reserved1;
            public int batteryLifeTime;
            public int batteryFullLifeTime;
        }


        #endregion


        /// <summary>
        /// GetWindowLongPtr values, GWL_*
        /// </summary>
        public enum GWL
        {
            WNDPROC = (-4),
            HINSTANCE = (-6),
            HWNDPARENT = (-8),
            STYLE = (-16),
            EXSTYLE = (-20),
            USERDATA = (-21),
            ID = (-12)
        }
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int InvalidateRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bErase);

        [DllImport("user32.dll")]
        public static extern int UpdateWindow(IntPtr hWnd);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return value is of no use.")]
        [DllImport("user32.dll")]
        public static extern void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd,
           [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 1;

        /// <summary>
        /// Contains information about the placement of a window on the screen.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            /// <summary>
            /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
            /// <para>
            /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
            /// </para>
            /// </summary>
            public int Length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The current show state of the window.
            /// </summary>
            public uint ShowCmd;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public POINT MinPosition;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public POINT MaxPosition;

            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public RECT NormalPosition;

            /// <summary>
            /// Gets the default (empty) value.
            /// </summary>
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }


        public enum CombineRgnStyles : int
        {
            RGN_AND = 1,
            RGN_OR = 2,
            RGN_XOR = 3,
            RGN_DIFF = 4,
            RGN_COPY = 5,
            RGN_MIN = RGN_AND,
            RGN_MAX = RGN_COPY
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return value is of no use.")]
        [DllImport("gdi32.dll")]
        public static extern void CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, CombineRgnStyles CombineMode);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int x3, int y3);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return value is of no use.")]
        [DllImport("user32.dll")]
        public static extern void SetWindowRgn(IntPtr hWnd, IntPtr hRgn, int bRedraw);

        public struct NativePoint
        {
            public int X;
            public int Y;
        }

        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

        private static int _dpi;
        public static int DPI
        {

            get
            {
                if (_dpi == 0)
                {
                    HandleRef desktopHwnd = new HandleRef(null, IntPtr.Zero);
                    HandleRef desktopDC = new HandleRef(null, GetDC(desktopHwnd));
                    _dpi = GetDeviceCaps(desktopDC, 88 /*LOGPIXELSX*/);
                    ReleaseDC(desktopHwnd, desktopDC);
                }
                return _dpi;
            }
        }

        public static int ConvertDIUToPixel(double diu)
        {
            return (int)(diu * NativeMethods.DPI / 96.0);
        }

        public static double ConvertPixelToDIU(int pixels)
        {
            return (double)pixels * 96.0 / NativeMethods.DPI;
        }

        public static Point ConvertPixelToDIU(Point pixel)
        {
            return new Point(ConvertPixelToDIU((int)pixel.X), ConvertPixelToDIU((int)pixel.Y));
        }

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport("User32.dll")]
        public static extern void ReleaseDC(HandleRef hWnd, HandleRef hDC);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out NativePoint lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, out NativePoint lpPoint);

        /// <summary>
        /// Returns current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        public static Point GetCursorPos()
        {
            NativeMethods.NativePoint mouse;
            NativeMethods.GetCursorPos(out mouse);
            return new Point(mouse.X, mouse.Y);
        }

        #region RECT
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public static readonly RECT Empty = new RECT();

            public int Width
            {
                get { return Math.Abs(Right - Left); }  // Abs needed for BIDI OS
            }

            public int Height
            {
                get { return Bottom - Top; }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + Left + " / top : " + Top + " / right : " + Right + " / bottom : " + Bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return Left.GetHashCode() + Top.GetHashCode() + Right.GetHashCode() + Bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Right == rect2.Right && rect1.Bottom == rect2.Bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }
        #endregion

        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int GW_ENABLEDPOPUP = 6;

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool FlashWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)]bool bInvert);


        #region Window Style
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        //internal const int WS_BORDER =    0x00800000;
        #endregion Window Style

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        public enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static Rect GetWindowRect(IntPtr hWnd)
        {
            RECT rect;
            GetWindowRect(hWnd, out rect);
            return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

        public static void MoveWindow(IntPtr hWnd, Rect rect)
        {
            MoveWindow(hWnd, (int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height, true);
        }

        [DllImport("user32.dll")]
        public static extern int GetDoubleClickTime();

        public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags;
        }

        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #region SHGetFileInfo

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        /// <summary>
        /// Flags that specify the file information to retrieve with SHGetFileInfo
        /// </summary>
        [Flags]
        internal enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }



        #endregion

        #region special folders
        /// <summary>
        /// maximum length of path retrieved by SHGetFolderPath()
        /// </summary>
        public const int MAX_PATH = 260;

        /// <summary>
        /// Get special folder path from CSIDL.
        /// </summary>
        /// <param name="owner">reserved</param>
        /// <param name="folder">folder CSIDL</param>
        /// <param name="token">access token</param>
        /// <param name="flags">flags</param>
        /// <param name="path">path buffer</param>
        /// <returns>0 if successful or an error value otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Does not support all cases.")]
        [DllImport("shfolder.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetFolderPath(IntPtr owner, SpecialFolder folder, IntPtr token, int flags, System.Text.StringBuilder path);

        #endregion

        [DllImport("dwmapi.dll")]
        public static extern void DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool enabled);


        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        internal static extern IntPtr GetDesktopWindow();

        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        public const int WM_SHOWWINDOW = 0x0018;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_GETMINMAXINFO = 0x0024;

        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessage(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("USER32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);

        internal const int SM_CXDOUBLECLK = 36;
        internal const int SM_CYDOUBLECLK = 37;

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("kernel32.dll")]
        public static extern int GetUserGeoID(GeoClass geoClass);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ShowCursor([MarshalAs(UnmanagedType.Bool)] bool bShow);

        private static bool isCursorHidden;

        public static void ShowCursor()
        {
            if (isCursorHidden)
            {
                System.Diagnostics.Trace.Assert(Application.Current.Dispatcher.CheckAccess(), "Consider to show the cursor in the UI thread.");
                int i = 10;
                int count = 0;
                do
                {
                    count = ShowCursor(true);
                    i--;
                } while (count < 0 && i > 0);

                isCursorHidden = count < 0;
            }
        }

        public static void HideCursor()
        {
            if (!isCursorHidden)
            {
                System.Diagnostics.Trace.Assert(Application.Current.Dispatcher.CheckAccess(), "Consider to hide the cursor in the UI thread.");
                int i = 10;
                int count = 0;
                do
                {
                    count = ShowCursor(false);
                    i--;
                } while (count >= 0 && i > 0);
                isCursorHidden = count < 0;
            }
        }

        #region QueryCancelAutoPlay

        [DllImport("ole32.dll")]
        public static extern int CreateClassMoniker([In] ref Guid rclsid, out IMoniker ppmk);

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        public const int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;
        public const int ROTFLAGS_ALLOWANYCLIENT = 2;

        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi,
            BestFitMapping = false, ThrowOnUnmappableChar = true, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr lib);

        #region System Parameter Info

        public const uint SPI_GETSCREENSAVERRUNNING = 0x0072;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, [MarshalAs(UnmanagedType.Bool)] ref bool pvParam, uint fWinIni);

        #endregion

        #region misc native methods
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateStreamOnFile(string pszFile, uint grfMode, out IStream ppstm);

        public const int STGM_READ = 0x00000000;
        public const int STGM_WRITE = 0x00000001;
        public const int STGM_READWRITE = 0x00000002;
        public const int STGM_CREATE = 0x00001000;

        [DllImport("kernel32.dll ", CharSet = CharSet.Unicode)]
        public static extern int GetShortPathName(string path, StringBuilder shortPath, int shortPathLength);

        [DllImport("kernel32.dll ", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(string path);

        [DllImport("kernel32.dll ", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveDirectory(string path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindFirstFile(string fileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return value is of no use")]
        [DllImport("kernel32.dll", EntryPoint = "FindClose")]
        public static extern void FindClose(IntPtr hFindFile);


        [DllImport("user32.dll")]
        public static extern IntPtr LockWorkStation();
        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(int uFlags, int dwReason);


        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);

        #endregion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            internal int dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;
        }

        //the file name maybe longer than MAX_PATH, so use API and short file name 
        public static StringBuilder GetShortPathName(string path)
        {
            StringBuilder shortName = new StringBuilder();
            int length = NativeMethods.GetShortPathName("\\\\?\\" + path, shortName, 0);
            if (length > 0)
            {
                shortName.Capacity = length;
                length = NativeMethods.GetShortPathName("\\\\?\\" + path, shortName, length);
                if (length <= 0)
                {
                    return shortName;
                }
            }
            return shortName;
        }

        public static void FlashWindow(IntPtr hwnd)
        {
            NativeMethods.FLASHWINFO fwi = new NativeMethods.FLASHWINFO();
            fwi.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.FLASHWINFO));
            fwi.dwFlags = NativeMethods.FLASHW_FLAGS.FLASHW_TRAY;
            fwi.hwnd = hwnd;
            fwi.uCount = 1;
            NativeMethods.FlashWindowEx(ref fwi);
        }

        public static bool SwitchToAlreadyRunningInstance()
        {
            var proc = Process.GetProcessesByName("NeroMediaPlayer").FirstOrDefault(o => o.Id != Process.GetCurrentProcess().Id);

            if (proc != null)
            {
                IntPtr hWnd = proc.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    // Restore window if minimised. Do not restore if already in
                    // normal or maximised window state, since we don't want to
                    // change the current state of the window.
                    if (IsIconic(hWnd))
                        ShowWindow(hWnd, SW_RESTORE);

                    // Set foreground window.
                    SetForegroundWindow(hWnd);

                    // flash the window
                    FlashWindow(hWnd);

                    return true;
                }
            }

            return false;
        }


        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004 //RW note: and we dont need presents for the user !
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum FLASHW_FLAGS : uint
        {
            FLASHW_ALL = 0x00000003,
            FLASHW_CAPTION = 0x00000001,
            FLASHW_STOP = 0,
            FLASHW_TIMER = 0x00000004,
            FLASHW_TIMERNOFG = 0x0000000C,
            FLASHW_TRAY = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public FLASHW_FLAGS dwFlags;
            public uint uCount;
            public uint dwTimeout;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

        [DllImport("shell32.dll")]
        public static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        public static class KnownFolder
        {
            public static readonly Guid AddNewPrograms = new Guid("de61d971-5ebc-4f02-a3a9-6c82895e5c04");
            public static readonly Guid AdminTools = new Guid("724EF170-A42D-4FEF-9F26-B60E846FBA4F");
            public static readonly Guid AppUpdates = new Guid("a305ce99-f527-492b-8b1a-7e76fa98d6e4");
            public static readonly Guid CDBurning = new Guid("9E52AB10-F80D-49DF-ACB8-4330F5687855");
            public static readonly Guid ChangeRemovePrograms = new Guid("df7266ac-9274-4867-8d55-3bd661de872d");
            public static readonly Guid CommonAdminTools = new Guid("D0384E7D-BAC3-4797-8F14-CBA229B392B5");
            public static readonly Guid CommonOEMLinks = new Guid("C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D");
            public static readonly Guid CommonPrograms = new Guid("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8");
            public static readonly Guid CommonStartMenu = new Guid("A4115719-D62E-491D-AA7C-E74B8BE3B067");
            public static readonly Guid CommonStartup = new Guid("82A5EA35-D9CD-47C5-9629-E15D2F714E6E");
            public static readonly Guid CommonTemplates = new Guid("B94237E7-57AC-4347-9151-B08C6C32D1F7");
            public static readonly Guid ComputerFolder = new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7");
            public static readonly Guid ConflictFolder = new Guid("4bfefb45-347d-4006-a5be-ac0cb0567192");
            public static readonly Guid ConnectionsFolder = new Guid("6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD");
            public static readonly Guid Contacts = new Guid("56784854-C6CB-462b-8169-88E350ACB882");
            public static readonly Guid ControlPanelFolder = new Guid("82A74AEB-AEB4-465C-A014-D097EE346D63");
            public static readonly Guid Cookies = new Guid("2B0F765D-C0E9-4171-908E-08A611B84FF6");
            public static readonly Guid Desktop = new Guid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");
            public static readonly Guid Documents = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
            public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
            public static readonly Guid Favorites = new Guid("1777F761-68AD-4D8A-87BD-30B759FA33DD");
            public static readonly Guid Fonts = new Guid("FD228CB7-AE11-4AE3-864C-16F3910AB8FE");
            public static readonly Guid Games = new Guid("CAC52C1A-B53D-4edc-92D7-6B2E8AC19434");
            public static readonly Guid GameTasks = new Guid("054FAE61-4DD8-4787-80B6-090220C4B700");
            public static readonly Guid History = new Guid("D9DC8A3B-B784-432E-A781-5A1130A75963");
            public static readonly Guid InternetCache = new Guid("352481E8-33BE-4251-BA85-6007CAEDCF9D");
            public static readonly Guid InternetFolder = new Guid("4D9F7874-4E0C-4904-967B-40B0D20C3E4B");
            public static readonly Guid Links = new Guid("bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968");
            public static readonly Guid LocalAppData = new Guid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
            public static readonly Guid LocalAppDataLow = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");
            public static readonly Guid LocalizedResourcesDir = new Guid("2A00375E-224C-49DE-B8D1-440DF7EF3DDC");
            public static readonly Guid Music = new Guid("4BD8D571-6D19-48D3-BE97-422220080E43");
            public static readonly Guid NetHood = new Guid("C5ABBF53-E17F-4121-8900-86626FC2C973");
            public static readonly Guid NetworkFolder = new Guid("D20BEEC4-5CA8-4905-AE3B-BF251EA09B53");
            public static readonly Guid OriginalImages = new Guid("2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39");
            public static readonly Guid PhotoAlbums = new Guid("69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C");
            public static readonly Guid Pictures = new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB");
            public static readonly Guid Playlists = new Guid("DE92C1C7-837F-4F69-A3BB-86E631204A23");
            public static readonly Guid PrintersFolder = new Guid("76FC4E2D-D6AD-4519-A663-37BD56068185");
            public static readonly Guid PrintHood = new Guid("9274BD8D-CFD1-41C3-B35E-B13F55A758F4");
            public static readonly Guid Profile = new Guid("5E6C858F-0E22-4760-9AFE-EA3317B67173");
            public static readonly Guid ProgramData = new Guid("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97");
            public static readonly Guid ProgramFiles = new Guid("905e63b6-c1bf-494e-b29c-65b732d3d21a");
            public static readonly Guid ProgramFilesX64 = new Guid("6D809377-6AF0-444b-8957-A3773F02200E");
            public static readonly Guid ProgramFilesX86 = new Guid("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E");
            public static readonly Guid ProgramFilesCommon = new Guid("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066");
            public static readonly Guid ProgramFilesCommonX64 = new Guid("6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D");
            public static readonly Guid ProgramFilesCommonX86 = new Guid("DE974D24-D9C6-4D3E-BF91-F4455120B917");
            public static readonly Guid Programs = new Guid("A77F5D77-2E2B-44C3-A6A2-ABA601054A51");
            public static readonly Guid Public = new Guid("DFDF76A2-C82A-4D63-906A-5644AC457385");
            public static readonly Guid PublicDesktop = new Guid("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25");
            public static readonly Guid PublicDocuments = new Guid("ED4824AF-DCE4-45A8-81E2-FC7965083634");
            public static readonly Guid PublicDownloads = new Guid("3D644C9B-1FB8-4f30-9B45-F670235F79C0");
            public static readonly Guid PublicGameTasks = new Guid("DEBF2536-E1A8-4c59-B6A2-414586476AEA");
            public static readonly Guid PublicMusic = new Guid("3214FAB5-9757-4298-BB61-92A9DEAA44FF");
            public static readonly Guid PublicPictures = new Guid("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5");
            public static readonly Guid PublicVideos = new Guid("2400183A-6185-49FB-A2D8-4A392A602BA3");
            public static readonly Guid QuickLaunch = new Guid("52a4f021-7b75-48a9-9f6b-4b87a210bc8f");
            public static readonly Guid Recent = new Guid("AE50C081-EBD2-438A-8655-8A092E34987A");
            public static readonly Guid RecycleBinFolder = new Guid("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC");
            public static readonly Guid ResourceDir = new Guid("8AD10C31-2ADB-4296-A8F7-E4701232C972");
            public static readonly Guid RoamingAppData = new Guid("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D");
            public static readonly Guid SampleMusic = new Guid("B250C668-F57D-4EE1-A63C-290EE7D1AA1F");
            public static readonly Guid SamplePictures = new Guid("C4900540-2379-4C75-844B-64E6FAF8716B");
            public static readonly Guid SamplePlaylists = new Guid("15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5");
            public static readonly Guid SampleVideos = new Guid("859EAD94-2E85-48AD-A71A-0969CB56A6CD");
            public static readonly Guid SavedGames = new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");
            public static readonly Guid SavedSearches = new Guid("7d1d3a04-debb-4115-95cf-2f29da2920da");
            public static readonly Guid SEARCH_CSC = new Guid("ee32e446-31ca-4aba-814f-a5ebd2fd6d5e");
            public static readonly Guid SEARCH_MAPI = new Guid("98ec0e18-2098-4d44-8644-66979315a281");
            public static readonly Guid SearchHome = new Guid("190337d1-b8ca-4121-a639-6d472d16972a");
            public static readonly Guid SendTo = new Guid("8983036C-27C0-404B-8F08-102D10DCFD74");
            public static readonly Guid SidebarDefaultParts = new Guid("7B396E54-9EC5-4300-BE0A-2482EBAE1A26");
            public static readonly Guid SidebarParts = new Guid("A75D362E-50FC-4fb7-AC2C-A8BEAA314493");
            public static readonly Guid StartMenu = new Guid("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19");
            public static readonly Guid Startup = new Guid("B97D20BB-F46A-4C97-BA10-5E3608430854");
            public static readonly Guid SyncManagerFolder = new Guid("43668BF8-C14E-49B2-97C9-747784D784B7");
            public static readonly Guid SyncResultsFolder = new Guid("289a9a43-be44-4057-a41b-587a76d7e7f9");
            public static readonly Guid SyncSetupFolder = new Guid("0F214138-B1D3-4a90-BBA9-27CBC0C5389A");
            public static readonly Guid System = new Guid("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7");
            public static readonly Guid SystemX86 = new Guid("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27");
            public static readonly Guid Templates = new Guid("A63293E8-664E-48DB-A079-DF759E0509F7");
            public static readonly Guid TreeProperties = new Guid("5b3749ad-b49f-49c1-83eb-15370fbd4882");
            public static readonly Guid UserProfiles = new Guid("0762D272-C50A-4BB0-A382-697DCD729B80");
            public static readonly Guid UsersFiles = new Guid("f3ce0f7c-4901-4acc-8648-d5d44b04ef8f");
            public static readonly Guid Videos = new Guid("18989B1D-99B5-455B-841C-AB7C74E4DDFC");
            public static readonly Guid Windows = new Guid("F38BF404-1D43-42F2-9305-67DE0B28FC23");
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("shell32.dll")]
        public static extern uint SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out()] out IntPtr pidl, uint sfgaoIn, [Out()] out uint psfgaoOut);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint SHGetNameFromIDList(IntPtr pidl, SIGDN sigdnName, [Out] out String ppszName);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateItemFromParsingName(
            [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);

        public enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

        public enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        public enum SIGDN : uint
        {
            NORMALDISPLAY = 0,
            PARENTRELATIVEPARSING = 0x80018001,
            PARENTRELATIVEFORADDRESSBAR = 0x8001c001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000
        }

        [Flags]
        public enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszSpec;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        public interface IShellItem
        {
            void BindToHandler(IntPtr pbc,
                [MarshalAs(UnmanagedType.LPStruct)] Guid bhid,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IntPtr ppv);

            void GetParent(out IShellItem ppsi);

            void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

            void Compare(IShellItem psi, uint hint, out int piOrder);
        };

        public enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 1,
            SIATTRIBFLAGS_APPCOMPAT = 3,
            SIATTRIBFLAGS_OR = 2
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        public enum HRESULT : long
        {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E
        }

        [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItemArray
        {
            // Not supported: IBindCtx
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid,
                    [In] ref Guid riid, out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetAttributes([In] NativeMethods.SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCount(out uint pdwNumItems);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
        }

        [ComImport, Guid("b4db1657-70d7-485e-8e3e-6fcb5a5c1802"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IModalWindow
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            int Show([In] IntPtr parent);
        }

        [ComImport, Guid("973510DB-7D7F-452B-8975-74A85828D354"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialogEvents
        {
            // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
            // the dialog should not proceed (e.g. with closing, changing folder); to 
            // support this, we need to use the PreserveSig attribute to enable us to return
            // the proper HRESULT
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            HRESULT OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            HRESULT OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                         [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                      [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
                      out FDE_SHAREVIOLATION_RESPONSE pResponse);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd,
                     [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
                     out FDE_OVERWRITE_RESPONSE pResponse);
        }

        public enum FDAP
        {
            FDAP_BOTTOM = 0,
            FDAP_TOP = 1
        };

        [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialog : IModalWindow
        {
            // Defined on IModalWindow - repeated here due to requirements of COM interop layer
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            int Show([In] IntPtr parent);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // IFileDialog-Specific interface members
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileTypes([In] uint cFileTypes,
                      [In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileTypeIndex([In] uint iFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFileTypeIndex(out uint piFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Unadvise([In] uint dwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetOptions([In] FOS fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetOptions(out FOS pfos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FDAP fdap);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void Close([MarshalAs(UnmanagedType.Error)] int hr);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetClientGuid([In] ref Guid guid);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void ClearClientData();

            // Not supported:  IShellItemFilter is not defined, converting to IntPtr
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
        }

        [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileOpenDialog : IFileDialog
        {
            // Defined on IModalWindow - repeated here due to requirements of COM interop layer
            // --------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            int Show([In] IntPtr parent);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Defined on IFileDialog - repeated here due to requirements of COM interop layer
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileTypes([In] uint cFileTypes, [In] COMDLG_FILTERSPEC[] rgFilterSpec);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileTypeIndex([In] uint iFileType);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFileTypeIndex(out uint piFileType);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Unadvise([In] uint dwCookie);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetOptions([In] FOS fos);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetOptions(out FOS pfos);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void Close([MarshalAs(UnmanagedType.Error)] int hr);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetClientGuid([In] ref Guid guid);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void ClearClientData();
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Not supported:  IShellItemFilter is not defined, converting to IntPtr
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

            // Defined by IFileOpenDialog
            // ---------------------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
        }

        public static class Guids
        {
            public static readonly Guid ShellItem = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
            public static readonly Guid FileOpenDialog = new Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");
        }

        // Simulate mouse events
        [DllImport("user32.dll")]
        internal static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, IntPtr dwExtraInfo);

        public enum MouseEventType : int
        {
            Move = 0x01,
            LeftDown = 0x02,
            LeftUp = 0x04,
            RightDown = 0x08,
            RightUp = 0x10,
            Wheel = 0x0800,
            Absolute = 0x8000
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreatePopupMenu();

        [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("000214e4-0000-0000-c000-000000000046")]
        public interface IContextMenu
        {
            [PreserveSig()]
            Int32 QueryContextMenu(
                IntPtr hmenu,
                uint iMenu,
                uint idCmdFirst,
                uint idCmdLast,
                CMF uFlags);

            [PreserveSig()]
            Int32 InvokeCommand(
                ref CMINVOKECOMMANDINFOEX info);

            [PreserveSig()]
            void GetCommandString(
                int idcmd,
                GetCommandStringInformations uflags,
                int reserved,
                StringBuilder commandstring,
                int cch);
        }
        [Flags]
        public enum CMF : uint
        {
            NORMAL = 0x00000000,
            DEFAULTONLY = 0x00000001,
            VERBSONLY = 0x00000002,
            EXPLORE = 0x00000004,
            NOVERBS = 0x00000008,
            CANRENAME = 0x00000010,
            NODEFAULT = 0x00000020,
            INCLUDESTATIC = 0x00000040,
            EXTENDEDVERBS = 0x00000100,
            RESERVED = 0xffff0000
        }
        public enum GetCommandStringInformations
        {
            VERB = 0x00000004,
            HELPTEXT = 0x00000005,
            VALIDATE = 0x00000006,
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CMINVOKECOMMANDINFOEX
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            public IntPtr lpVerb;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDirectory;
            public int nShow;
            public int dwHotKey;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpTitle;
            public IntPtr lpVerbW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParametersW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectoryW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpTitleW;
            public POINT ptInvoke;
        }

        public static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEV_BROADCAST_HANDLE
        {
            public uint dbch_size;
            public uint dbch_devicetype;
            public uint dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public int dbch_nameoffset;
            public IntPtr dbch_data;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public uint dbch_size;
            public uint dbch_devicetype;
            public uint dbch_reserved;
            public Guid dbch_eventguid;
            public byte dbch_data;
            public byte dbch_data1;
        }
        public const int DBT_DEVTYP_HANDLE = 0x00000006;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        internal const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        internal static Guid WPD_DEVINTERFACE
        {
            get
            {
                return new Guid("{6ac27878-a6fa-4155-ba85-f98f491d4f33}");
            }
        }
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SHGetSpecialFolderLocation(
            IntPtr hWnd,
            SpecialFolder nFolder,
            out IntPtr Pidl);
        public enum SpecialFolder
        {
            /// <summary>
            /// desktop folder
            /// </summary>
            CSIDL_DESKTOP = 0x0000,

            /// <summary>
            /// Internet Explorer (icon on desktop)
            /// </summary>
            CSIDL_INTERNET = 0x0001,

            /// <summary>
            /// Start Menu\Programs
            /// </summary>
            CSIDL_PROGRAMS = 0x0002,

            /// <summary>
            /// My Computer\Control Panel
            /// </summary>
            CSIDL_CONTROLS = 0x0003,

            /// <summary>
            /// My Computer\Printers
            /// </summary>
            CSIDL_PRINTERS = 0x0004,

            /// <summary>
            /// My Documents
            /// </summary>
            CSIDL_PERSONAL = 0x0005,

            /// <summary>
            /// [user name]\Favorites
            /// </summary>
            CSIDL_FAVORITES = 0x0006,

            /// <summary>
            /// Start Menu\Programs\Startup
            /// </summary>
            CSIDL_STARTUP = 0x0007,

            /// <summary>
            /// [user name]\Recent
            /// </summary>
            CSIDL_RECENT = 0x0008,

            /// <summary>
            /// [user name]\SendTo
            /// </summary>
            CSIDL_SENDTO = 0x0009,

            /// <summary>
            /// [desktop]\Recycle Bin
            /// </summary>
            CSIDL_BITBUCKET = 0x000a,

            /// <summary>
            /// [user name]\Start Menu
            /// </summary>
            CSIDL_STARTMENU = 0x000b,

            /// <summary>
            /// Personal was just a silly name for My Documents
            /// </summary>
            CSIDL_MYDOCUMENTS = CSIDL_PERSONAL,

            /// <summary>
            /// "My Music" folder
            /// </summary>
            CSIDL_MYMUSIC = 0x000d,

            /// <summary>
            /// "My Videos" folder
            /// </summary>
            CSIDL_MYVIDEO = 0x000e,

            /// <summary>
            /// [user name]\Desktop
            /// </summary>
            CSIDL_DESKTOPDIRECTORY = 0x0010,

            /// <summary>
            /// My Computer
            /// </summary>
            CSIDL_DRIVES = 0x0011,

            /// <summary>
            /// Network Neighborhood (My Network Places)
            /// </summary>
            CSIDL_NETWORK = 0x0012,

            /// <summary>
            /// [user name]\nethood
            /// </summary>
            /// 
            CSIDL_NETHOOD = 0x0013,

            /// <summary>
            /// windows\fonts
            /// </summary>
            CSIDL_FONTS = 0x0014,

            /// <summary>
            /// document templates
            /// </summary>
            CSIDL_TEMPLATES = 0x0015,

            /// <summary>
            ///  All Users\Start Menu
            /// </summary>
            CSIDL_COMMON_STARTMENU = 0x0016,

            /// <summary>
            /// All Users\Start Menu\Programs
            /// </summary>
            CSIDL_COMMON_PROGRAMS = 0X0017,

            /// <summary>
            /// All Users\Startup
            /// </summary>
            CSIDL_COMMON_STARTUP = 0x0018,

            /// <summary>
            /// All Users\Desktop
            /// </summary>
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,

            /// <summary>
            /// [user name]\Application Data
            /// </summary>
            CSIDL_APPDATA = 0x001a,

            /// <summary>
            /// [user name]\PrintHood
            /// </summary>
            CSIDL_PRINTHOOD = 0x001b,

            /// <summary>
            /// [user name]\Local Settings\Applicaiton Data (non roaming)
            /// </summary>
            CSIDL_LOCAL_APPDATA = 0x001c,

            /// <summary>
            /// Public documents folder
            /// </summary>
            CSIDL_COMMON_DOCUMENTS = 0x002e,

            /// <summary>
            /// Public music folder
            /// </summary>
            CSIDL_COMMON_MUSIC = 0x0035,

            /// <summary>
            /// Public pictures folder
            /// </summary>
            CSIDL_COMMON_PICTURES = 0x0036,

            /// <summary>
            /// Public videos folder
            /// </summary>
            CSIDL_COMMON_VIDEO = 0x0037
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SHChangeNotifyEntry
        {
            public IntPtr pidl;
            [MarshalAs(UnmanagedType.Bool)]
            public Boolean Recursively;
        }
        [DllImport("shell32.dll", EntryPoint = "#2", CharSet = CharSet.Auto)]
        internal static extern uint SHChangeNotifyRegister(
            IntPtr hWnd,
            SHCNF fSources,
            SHCNE fEvents,
            uint wMsg,
            int cEntries,
            ref SHChangeNotifyEntry pFsne);
        internal enum SHCNF
        {
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_DWORD = 0x0003,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_TYPE = 0x00FF,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000
        }
        internal enum SHCNE : uint
        {
            SHCNE_RENAMEITEM = 0x00000001,
            SHCNE_CREATE = 0x00000002,
            SHCNE_DELETE = 0x00000004,
            SHCNE_MKDIR = 0x00000008,
            SHCNE_RMDIR = 0x00000010,
            SHCNE_MEDIAINSERTED = 0x00000020,
            SHCNE_MEDIAREMOVED = 0x00000040,
            SHCNE_DRIVEREMOVED = 0x00000080,
            SHCNE_DRIVEADD = 0x00000100,
            SHCNE_NETSHARE = 0x00000200,
            SHCNE_NETUNSHARE = 0x00000400,
            SHCNE_ATTRIBUTES = 0x00000800,
            SHCNE_UPDATEDIR = 0x00001000,
            SHCNE_UPDATEITEM = 0x00002000,
            SHCNE_SERVERDISCONNECT = 0x00004000,
            SHCNE_UPDATEIMAGE = 0x00008000,
            SHCNE_DRIVEADDGUI = 0x00010000,
            SHCNE_RENAMEFOLDER = 0x00020000,
            SHCNE_FREESPACE = 0x00040000,
            SHCNE_EXTENDED_EVENT = 0x04000000,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_DISKEVENTS = 0x0002381F,
            SHCNE_GLOBALEVENTS = 0x0C0581E0,
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_INTERRUPT = 0x80000000,
        }
        internal const uint WM_DESTROY = 0x0002;
        internal const uint WM_SHNOTIFY = 0x0401;
        [DllImport("shell32.dll", EntryPoint = "#4", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean SHChangeNotifyUnregister(
           UInt32 hNotify);

        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001; // Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002; // A request to remove a device or piece of media has been canceled.
        public const int DBT_DEVICEREMOVEPENDING = 0x8003; // A device or piece of media is about to be removed. Cannot be denied.
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // A device or piece of media has been removed.
        public const int DBT_CUSTOMEVENT = 0x8006; // A custom event has occurred.
        public const int DBT_DEVTYP_VOLUME = 0x00000002;
        public const int BROADCAST_QUERY_DENY = 0x424D5144;  // Return this value to deny a query.
                                                             // Struct for parameters of the WM_DEVICECHANGE message
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }
        internal static Guid IO_MEDIA_EJECT_REQUEST
        {
            get
            {
                return new Guid("{d07433d1-a98e-11d2-917a-00a0c9068ff3}");
            }
        }
        public static bool IsVolume(this DEV_BROADCAST_HDR self)
        {
            return self.dbch_devicetype == NativeMethods.DBT_DEVTYP_VOLUME;
        }
        // Struct for parameters of the WM_DEVICECHANGE message
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
            public int dbcv_flags;
        }
        /// <summary>
        /// Gets the drive name from a DEV_BROADCAST_VOLUME stucture
        /// </summary>
        /// <returns>The drive name, e.g. "C:"</returns>
        public static IEnumerable<string> DriveNames(this DEV_BROADCAST_VOLUME self)
        {
            int unitmask = self.dbcv_unitmask;
            for (char driveLetter = 'A'; driveLetter <= 'Z'; driveLetter++, unitmask >>= 1)
            {
                if ((unitmask & 1) != 0)
                {
                    yield return driveLetter.ToString() + Path.VolumeSeparatorChar;
                }
            }
        }
        public static bool IsInterface(this DEV_BROADCAST_HDR self)
        {
            return self.dbch_devicetype == NativeMethods.DBT_DEVTYP_DEVICEINTERFACE;
        }

        internal struct NotifyInfos
        {
            public NotifyInfos(SHCNE notification) { Notification = notification; Item1 = ""; Item2 = ""; }
            public SHCNE Notification;
            public string Item1;
            public string Item2;
        };
        [StructLayout(LayoutKind.Sequential)]
        internal struct SHNOTIFYSTRUCT
        {
            public IntPtr dwItem1;
            public IntPtr dwItem2;
        };

        [DllImport("Shell32.Dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean SHGetPathFromIDList(
    [In] IntPtr pidl,
    [In, Out, MarshalAs(UnmanagedType.LPWStr)] String pszPath);
    }

    public static class DpiHelpers
    {
        private static int dpi;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DPI", Justification = "It is valid acronym.")]
        public static int DPI
        {

            get
            {
                if (dpi == 0)
                {
                    HandleRef desktopHwnd = new HandleRef(null, IntPtr.Zero);
                    HandleRef desktopDC = new HandleRef(null, NativeMethods.GetDC(desktopHwnd));
                    dpi = NativeMethods.GetDeviceCaps(desktopDC, 88 /*LOGPIXELSX*/);
                    NativeMethods.ReleaseDC(desktopHwnd, desktopDC);
                }
                return dpi;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DIU", Justification = "It is valid acronym.")]
        public static int ConvertDIUToPixel(double diu)
        {
            return (int)(diu * DPI / 96.0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DIU", Justification = "It is valid acronym.")]
        public static double ConvertPixelToDIU(int pixels)
        {
            return (double)pixels * 96.0 / DPI;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DIU", Justification = "It is valid acronym.")]
        public static Point ConvertPixelToDIU(Point pixel)
        {
            return new Point(ConvertPixelToDIU((int)pixel.X), ConvertPixelToDIU((int)pixel.Y));
        }

        private static int normalDPI = 96;

        public static bool IsNormalDPI
        {
            get
            {
                return normalDPI == DPI;
            }
        }
    }

    public enum GeoClass : int
    {
        None = 0,
        Nation = 16,
        Region = 14
    }

    /// <summary>
    /// Dynamic called api 
    /// </summary>
    public class DynamicDllLoader : IDisposable
    {
        private IntPtr hLib;
        private bool _isDisposed = false;

        public DynamicDllLoader(string dllPath)
        {
            if (dllPath == null)
                throw new ArgumentNullException("dllPath");

            hLib = NativeMethods.LoadLibrary(dllPath);
        }

        public Delegate GetDelegate(string apiName, Type type)
        {
            if (apiName == null)
                throw new ArgumentNullException("apiName");

            IntPtr api = NativeMethods.GetProcAddress(hLib, apiName);
            if (api == IntPtr.Zero) return null;

            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, type);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool releaseManagedResources)
        {
            if (!_isDisposed)
            {
                NativeMethods.FreeLibrary(hLib);
                hLib = IntPtr.Zero;
                _isDisposed = true;
            }
        }

        ~DynamicDllLoader()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// CSIDL values for various special folders.
    /// </summary>
    public enum SpecialFolder
    {
        /// <summary>
        /// desktop folder
        /// </summary>
        CSIDL_DESKTOP = 0x0000,

        /// <summary>
        /// Internet Explorer (icon on desktop)
        /// </summary>
        CSIDL_INTERNET = 0x0001,

        /// <summary>
        /// Start Menu\Programs
        /// </summary>
        CSIDL_PROGRAMS = 0x0002,

        /// <summary>
        /// My Computer\Control Panel
        /// </summary>
        CSIDL_CONTROLS = 0x0003,

        /// <summary>
        /// My Computer\Printers
        /// </summary>
        CSIDL_PRINTERS = 0x0004,

        /// <summary>
        /// My Documents
        /// </summary>
        CSIDL_PERSONAL = 0x0005,

        /// <summary>
        /// [user name]\Favorites
        /// </summary>
        CSIDL_FAVORITES = 0x0006,

        /// <summary>
        /// Start Menu\Programs\Startup
        /// </summary>
        CSIDL_STARTUP = 0x0007,

        /// <summary>
        /// [user name]\Recent
        /// </summary>
        CSIDL_RECENT = 0x0008,

        /// <summary>
        /// [user name]\SendTo
        /// </summary>
        CSIDL_SENDTO = 0x0009,

        /// <summary>
        /// [desktop]\Recycle Bin
        /// </summary>
        CSIDL_BITBUCKET = 0x000a,

        /// <summary>
        /// [user name]\Start Menu
        /// </summary>
        CSIDL_STARTMENU = 0x000b,

        /// <summary>
        /// Personal was just a silly name for My Documents
        /// </summary>
        CSIDL_MYDOCUMENTS = CSIDL_PERSONAL,

        /// <summary>
        /// "My Music" folder
        /// </summary>
        CSIDL_MYMUSIC = 0x000d,

        /// <summary>
        /// "My Videos" folder
        /// </summary>
        CSIDL_MYVIDEO = 0x000e,

        /// <summary>
        /// [user name]\Desktop
        /// </summary>
        CSIDL_DESKTOPDIRECTORY = 0x0010,

        /// <summary>
        /// My Computer
        /// </summary>
        CSIDL_DRIVES = 0x0011,

        /// <summary>
        /// Network Neighborhood (My Network Places)
        /// </summary>
        CSIDL_NETWORK = 0x0012,

        /// <summary>
        /// [user name]\nethood
        /// </summary>
        /// 
        CSIDL_NETHOOD = 0x0013,

        /// <summary>
        /// windows\fonts
        /// </summary>
        CSIDL_FONTS = 0x0014,

        /// <summary>
        /// document templates
        /// </summary>
        CSIDL_TEMPLATES = 0x0015,

        /// <summary>
        ///  All Users\Start Menu
        /// </summary>
        CSIDL_COMMON_STARTMENU = 0x0016,

        /// <summary>
        /// All Users\Start Menu\Programs
        /// </summary>
        CSIDL_COMMON_PROGRAMS = 0X0017,

        /// <summary>
        /// All Users\Startup
        /// </summary>
        CSIDL_COMMON_STARTUP = 0x0018,

        /// <summary>
        /// All Users\Desktop
        /// </summary>
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,

        /// <summary>
        /// [user name]\Application Data
        /// </summary>
        CSIDL_APPDATA = 0x001a,

        /// <summary>
        /// [user name]\PrintHood
        /// </summary>
        CSIDL_PRINTHOOD = 0x001b,

        /// <summary>
        /// [user name]\Local Settings\Applicaiton Data (non roaming)
        /// </summary>
        CSIDL_LOCAL_APPDATA = 0x001c,

        /// <summary>
        /// Public documents folder
        /// </summary>
        CSIDL_COMMON_DOCUMENTS = 0x002e,

        /// <summary>
        /// Public music folder
        /// </summary>
        CSIDL_COMMON_MUSIC = 0x0035,

        /// <summary>
        /// Public pictures folder
        /// </summary>
        CSIDL_COMMON_PICTURES = 0x0036,

        /// <summary>
        /// Public videos folder
        /// </summary>
        CSIDL_COMMON_VIDEO = 0x0037
    }
#pragma warning restore 1591  //better find expiation on the web. It is much more in detail
}
