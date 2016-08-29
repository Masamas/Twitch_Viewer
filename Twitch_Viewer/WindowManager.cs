using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Collections.ObjectModel;

namespace Twitch_Viewer
{
    public static class VlcWindowManager
    {
        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static List<IntPtr> GetVlcHandles()
        {
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                int size = GetWindowTextLength(wnd);
                if (size > 0)
                {
                    var builder = new StringBuilder(size + 1);
                    GetWindowText(wnd, builder, builder.Capacity);
                    var title = builder.ToString();
                    if (title == "VLC (Direct3D output)")
                        windows.Add(wnd);
                }

                return true;
            }, IntPtr.Zero);

            return windows;
        }

        public static async Task TryMoveNewVlcWindows(int oldCount)
        {
            for (int i = 0; i < 200; i++)
            {
                await Task.Delay(100);

                var vlcWindows = GetVlcHandles();
                if (vlcWindows.Count > oldCount)
                {
                    MoveVlcWindows(vlcWindows);
                    break;
                }
            }
        }
        
        private static void MoveVlcWindows(List<IntPtr> windows)
        {
            // values if Taskbar is visible
            //var workArea = SystemParameters.WorkArea;
            //int width = (int)workArea.Width / 3;
            //int height = (int)(width * 0.5625);

            int width = (int)SystemParameters.PrimaryScreenWidth / 3;
            int height = (int)(width * 0.5625);
            int xPos = (int)SystemParameters.PrimaryScreenWidth - width;
            int yPos = (int)SystemParameters.PrimaryScreenHeight - height;

            if (windows.Count < 1)
                return;

            foreach (IntPtr hWnd in windows)
                MoveWindow(hWnd, xPos, yPos, width, height + 1, true);
        }
    }
}
