using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ImageMaker.CommonView.Helpers
{
    public static class WindowInteropService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint SC_CLOSE = 0xF060;
        private const int WM_SHOWWINDOW = 0x00000018;

        [HandleProcessCorruptedStateExceptions]
        public static void SetWindowCloseStatus(this Window window, bool status)
        {
            try
            {
                var hWnd = new WindowInteropHelper(window);
                var sysMenu = GetSystemMenu(hWnd.Handle, false);
                uint flags = status ? MF_BYCOMMAND | MF_ENABLED : MF_BYCOMMAND | MF_GRAYED;
                EnableMenuItem(sysMenu, SC_CLOSE, flags);
            }
            catch (Exception e)
            {
                Debug.WriteLine("message: {0}; stacktrace: {1}", e.Message, e.StackTrace);
            }
            
        }
    }
}
