using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;

namespace Twitch_Viewer
{
    public class WindowManager
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        public void moveCalc()
        {
            IntPtr hWnd = FindWindow(null, "Rechner");

            MoveWindow(hWnd, 100, 200, 300, 300, true);
        }

        public void GetHwndFromPoint()
        {
            IntPtr hWnd = WindowFromPoint(100, 100);

            MoveWindow(hWnd, 100, 200, 200, 200, true);
        }

        public void GetHwndFromClassName()
        {
            StringBuilder buffer = new StringBuilder(128);

            IntPtr hWnd = FindWindow(null, "VLC (Direct3D output)");

            GetClassName(hWnd, buffer, buffer.Capacity);

            MessageBox.Show($"VLC Class Name: {buffer.ToString()}");
        }

        public void GetHwndFromWindowTitle()
        {
            IntPtr hWnd = FindWindow(null, "VLC (Direct3D output)");

            MoveWindow(hWnd, 100, 200, 200, 200, true);
        }
    }
}
