using System;
using System.Runtime.InteropServices;

namespace ConwaysGameOfLifeApp
{
    internal class UniverseConsoleUi
    {
        private int m_width;
        private int m_height;
        private int m_distanceToNextRow;
        private int m_distanceToNextColumn;
        private bool[,] m_previousDrawing;
        private const char c_livingCell = '█';
        private const char c_deadCell = 'x';

        public UniverseConsoleUi()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            ConsoleHelper.SetConsoleFont();
        }

        public void Draw(bool[,] liveCells, int generation)
        {
            SetDrawingCharacteristics(liveCells.GetLength(0), liveCells.GetLength(1));

            //Console.Clear();
            Console.Title = $"Game of Life (generation {generation})";

            //double buffering: first collect changed cells
            bool?[,] changes = CalculateChangedCells(liveCells);

            //double buffering: now paint only the changed cells
            DrawChangedCells(liveCells, changes);

            m_previousDrawing = liveCells; //double buffering: prepare for next cycle
        }
        
        private void SetDrawingCharacteristics(int height, int width)
        {
            m_distanceToNextRow = 1;
            m_distanceToNextColumn = 1;
            m_height = height;
            m_width = width;
            if (m_height >= Console.LargestWindowHeight)
            {
                m_distanceToNextRow = m_height / Console.LargestWindowHeight;
                m_height = Console.LargestWindowHeight;
            }

            if (m_width >= Console.BufferWidth)
            {
                m_distanceToNextColumn = m_width / Console.BufferWidth;
                m_width = Console.BufferWidth;
            }
        }

        private void DrawChangedCells(bool[,] liveCells, bool?[,] changes)
        {
            int row = 0;
            for (int i = 0; MaxRows(i, m_height, liveCells.GetLength(0)); i += m_distanceToNextRow)
            {
                int col = 0;
                for (int j = 0; MaxColumns(j, m_width, liveCells.GetLength(1)); j += m_distanceToNextColumn)
                {
                    try
                    {
                        if (changes[i, j] != null)
                        {
                            if (changes[i, j].Value)
                            {
                                Console.SetCursorPosition(col, row);
                                Console.Write(c_livingCell);
                            }
                            else
                            {
                                Console.SetCursorPosition(col, row);
                                Console.Write(c_deadCell);
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return;
                    }

                    col++;
                }

                row++;
            }
        }

        private bool MaxColumns(int j, int maxWidth, int universeWidth)
        {
            return j < universeWidth && j < m_width && j < maxWidth;
        }

        private bool MaxRows(int i, int maxHeight, int universeHeight)
        {
            return i < universeHeight && i < m_height && i < maxHeight;
        }

        private bool?[,] CalculateChangedCells(bool[,] liveCells)
        {
            var changes = new bool?[liveCells.GetLength(0), liveCells.GetLength(1)];

            for (int i = 0; MaxRows(i, m_height, liveCells.GetLength(0)); i += m_distanceToNextRow)
            {
                for (int j = 0; MaxColumns(j, m_width, liveCells.GetLength(1)); j += m_distanceToNextColumn)
                {
                    if (m_previousDrawing != null)
                    {
                        if (liveCells[i, j] != m_previousDrawing[i, j])
                        {
                            changes[i, j] = liveCells[i, j];
                        }
                    }
                    else
                    {
                        changes[i, j] = liveCells[i, j];
                    }
                }
            }

            return changes;
        }
    }

    internal static class ConsoleHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal uint nFont;
            internal COORD dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static void SetConsoleFont(string fontName = "Courier New"/*"Lucida Console"*/)
        {
            unsafe
            {
                IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
                if (hnd != INVALID_HANDLE_VALUE)
                {
                    CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
                    info.cbSize = (uint)Marshal.SizeOf(info);

                    // Set console font to Lucida Console.
                    var newInfo = new CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = TMPF_TRUETYPE;
                    IntPtr ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);

                    // Get some settings from current font.
                    newInfo.dwFontSize = new COORD(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.FontWeight = info.FontWeight;
                    SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                }
            }
        }

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        enum WindowLongFlags
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        [Flags]
        enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW =
                WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,

            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring

            //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000

            //#endif /* WIN32WINNT >= 0x0500 */
        }

        public static void EnableResize()
        {
            IntPtr consoleWindow = GetConsoleWindow();
            SetWindowLong(consoleWindow, (int)WindowLongFlags.GWL_STYLE, s_originalConsoleStyle);
        }

        private static uint s_originalConsoleStyle;
        private static uint s_nonResizeableConsole;
        public static void DisableResize()
        {
            IntPtr consoleWindow = GetConsoleWindow();
            if (s_originalConsoleStyle == 0)
            {
                s_originalConsoleStyle = (uint)GetWindowLong(consoleWindow, (int)WindowLongFlags.GWL_STYLE);
                s_nonResizeableConsole = (uint)((WindowStyles)s_originalConsoleStyle & ~WindowStyles.WS_THICKFRAME);
            }

            SetWindowLong(consoleWindow, (int)WindowLongFlags.GWL_STYLE, s_nonResizeableConsole);
        }
    }
}