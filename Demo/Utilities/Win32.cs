using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Demo.Utilities
{
    public class Win32
    {

        #region image preview api

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref W32Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref W32MonitorInfo lpmi);

        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromPoint(W32Point pt, uint dwFlags);
        #endregion

        #region window api
        internal const uint GW_HWNDNEXT = 2;

        [DllImport("User32")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("User32")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);
        [DllImport("user32.dll", SetLastError=true)]
        static extern bool GetWindowRect(IntPtr hwnd, out W32Rect lpRect);
        [DllImport("shcore.dll",SetLastError = true)]
        private static extern uint SetProcessDpiAwareness(int awareness);
        #endregion

        /// <summary>
        /// Gets the window rect in device units.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Rect.</returns>
        public static Rect GetWindowRect(Window window) {
            var hwnd = new WindowInteropHelper(window).Handle;
            GetWindowRect(hwnd, out var r);
            return new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        private const int PROCESS_PER_MONITOR_DPI_AWARE = 2;
        /// <summary>
        /// Sets the dpi awareness for the current process to: per monitor dpi aware.
        /// </summary>
        public static void SetProcessDpiAwarenessPerMonitor() 
        {
            var result = SetProcessDpiAwareness(PROCESS_PER_MONITOR_DPI_AWARE);
            switch (result) {
                case 0: return;
                case 0x80070005: return; // The DPI awareness is already set, either by calling this API previously or through the application (.exe) manifest.
                default: Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); return;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32MonitorInfo
    {
        public int Size;
        public W32Rect Monitor;
        public W32Rect WorkArea;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
