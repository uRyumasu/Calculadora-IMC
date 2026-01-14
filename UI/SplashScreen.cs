using System.Runtime.InteropServices;
using Spectre.Console;

namespace CalculadoraIMC.UI;

public class SplashScreen
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;
    private const int SWP_SHOWWINDOW = 0x0040;

    private static CONSOLE_FONT_INFO_EX originalFont;
    private static int originalWidth;
    private static int originalHeight;
    private static int originalBufferWidth;
    private static int originalBufferHeight;
    private static RECT originalWindowRect;
    private static bool stateSaved;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow,
        ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow,
        ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    private static void GoFullscreenWithSmallFont()
    {
        var hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        var consoleWindow = GetConsoleWindow();

        if (!stateSaved)
        {
            GetWindowRect(consoleWindow, out originalWindowRect);
            originalWidth = Console.WindowWidth;
            originalHeight = Console.WindowHeight;
            originalBufferWidth = Console.BufferWidth;
            originalBufferHeight = Console.BufferHeight;

            originalFont = new CONSOLE_FONT_INFO_EX();
            originalFont.cbSize = (uint)Marshal.SizeOf(originalFont);
            GetCurrentConsoleFontEx(hConsole, false, ref originalFont);

            stateSaved = true;
        }

        var newFont = new CONSOLE_FONT_INFO_EX();
        newFont.cbSize = (uint)Marshal.SizeOf(newFont);
        newFont.nFont = 0;
        newFont.dwFontSizeX = 1;
        newFont.dwFontSizeY = 1;
        newFont.FontFamily = originalFont.FontFamily;
        newFont.FontWeight = originalFont.FontWeight;
        newFont.FaceName = originalFont.FaceName;

        SetCurrentConsoleFontEx(hConsole, false, ref newFont);
        Thread.Sleep(100);

        try
        {
            var maxWidth = Console.LargestWindowWidth;
            var maxHeight = Console.LargestWindowHeight;
            Console.SetBufferSize(maxWidth, maxHeight);
            Console.SetWindowSize(maxWidth, maxHeight);
        }
        catch
        {
        }

        Thread.Sleep(100);
        ShowWindow(consoleWindow, SW_MAXIMIZE);
    }

    private static void RestoreEverything()
    {
        if (stateSaved)
        {
            var hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            var consoleWindow = GetConsoleWindow();

            ShowWindow(consoleWindow, SW_RESTORE);
            Thread.Sleep(100);

            try
            {
                Console.SetBufferSize(
                    Math.Max(originalBufferWidth, originalWidth),
                    Math.Max(originalBufferHeight, originalHeight)
                );
            }
            catch
            {
            }

            SetCurrentConsoleFontEx(hConsole, false, ref originalFont);
            Thread.Sleep(200);

            try
            {
                Console.SetWindowSize(originalWidth, originalHeight);
                Console.SetBufferSize(originalBufferWidth, originalBufferHeight);
            }
            catch
            {
            }

            var width = originalWindowRect.Right - originalWindowRect.Left;
            var height = originalWindowRect.Bottom - originalWindowRect.Top;
            SetWindowPos(consoleWindow, IntPtr.Zero,
                originalWindowRect.Left, originalWindowRect.Top,
                width, height, SWP_SHOWWINDOW);
        }
    }

    /// <summary>
    ///     Shows the splash screen with the specified image and duration
    /// </summary>
    /// <param name="imagePath">Path to the splash screen image (should be 1903x336 pixels)</param>
    /// <param name="durationMs">How long to display the splash screen in milliseconds (default: 5000ms)</param>
    public static void Show(string imagePath, int durationMs = 5000)
    {
        GoFullscreenWithSmallFont();

        Console.Clear();
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

        // Display the splash screen image
        var image = new CanvasImage(imagePath)
            .MaxWidth(Console.WindowWidth);

        AnsiConsole.Write(image);

        Thread.Sleep(durationMs);

        RestoreEverything();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CONSOLE_FONT_INFO_EX
    {
        public uint cbSize;
        public uint nFont;
        public short dwFontSizeX;
        public short dwFontSizeY;
        public int FontFamily;
        public int FontWeight;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }
}