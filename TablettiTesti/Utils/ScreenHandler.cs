using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TablettiTesti.Utils
{
    class ScreenHandler
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private static int SC_MONITORPOWER = 0xF170;
        private static int WM_SYSCOMMAND = 0x0112;

        public enum MonitorState
        {
            ON = -1,
            OFF = 2,
            STANDBY = 1
        }

        public static void SetMonitorState(MonitorState state)
        {
               SendMessage(Process.GetCurrentProcess().MainWindowHandle, WM_SYSCOMMAND, SC_MONITORPOWER, (int)state);
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;

        public static void Wake()
        {
            mouse_event(MOUSEEVENTF_MOVE, 2000, 0, 0, UIntPtr.Zero);
        }
    }
}
