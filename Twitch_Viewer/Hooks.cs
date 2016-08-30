//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace Hooks
//{
//    public delegate void OnForegroundWindowChangedDelegate(IntPtr hWnd);
//    public delegate void OnWindowMinimizeStartDelegate(IntPtr hWnd);
//    public delegate void OnWindowMinimizeEndDelegate(IntPtr hWnd);
//    public delegate void OnWindowDestroyDelegate(IntPtr hWnd);
//    public delegate void OnWindowCreateDelegate(IntPtr hWnd);

//    public sealed class Hook
//    {
//        #region Windows API

//        private enum SystemEvents : uint
//        {
//            EVENT_SYSTEM_DESTROY = 0x8001,
//            EVENT_SYSTEM_CREATE = 0x0001,
//            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
//            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
//            EVENT_SYSTEM_FOREGROUND = 0x0003
//        }

//        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

//        private delegate void WinEventDelegate(
//            IntPtr hWinEventHook,
//            uint eventType,
//            IntPtr hWnd,
//            int idObject,
//            int idChild,
//            uint dwEventThread,
//            uint dwmsEventTime);

//        [DllImport("User32.dll", SetLastError = true)]
//        private static extern IntPtr SetWinEventHook(
//            uint eventMin,
//            uint eventMax,
//            IntPtr hmodWinEventProc,
//            WinEventDelegate lpfnWinEventProc,
//            uint idProcess,
//            uint idThread,
//            uint dwFlags);

//        [DllImport("user32.dll")]
//        private static extern bool UnhookWinEvent(
//            IntPtr hWinEventHook
//            );

//        #endregion

//        private WinEventDelegate dEvent;
//        private IntPtr pHook;
//        public OnForegroundWindowChangedDelegate OnForegroundWindowChanged;
//        public OnWindowMinimizeStartDelegate OnWindowMinimizeStart;
//        public OnWindowMinimizeEndDelegate OnWindowMinimizeEnd;
//        public OnWindowDestroyDelegate OnWindowDestroy;
//        public OnWindowCreateDelegate OnWindowCreate;

//        public Hook()
//        {
//            dEvent = this.WinEvent;
//            pHook = SetWinEventHook(
//                (uint)SystemEvents.EVENT_SYSTEM_CREATE,
//                (uint)SystemEvents.EVENT_SYSTEM_CREATE,
//                IntPtr.Zero,
//                dEvent,
//                0,
//                0,
//                WINEVENT_OUTOFCONTEXT
//                );

//            if (IntPtr.Zero.Equals(pHook))
//                throw new Win32Exception();

//            GC.KeepAlive(dEvent);
//        }

//        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
//        {
//            Debugger.Log(1, "newEvent", "******Event triggered!******\n");
            
//            switch (eventType)
//            {
//                case (uint)SystemEvents.EVENT_SYSTEM_CREATE:
//                    if (OnWindowCreate != null)
//                        OnWindowCreate(hWnd);
//                    break;
//                case (uint)SystemEvents.EVENT_SYSTEM_DESTROY:
//                    if (OnWindowDestroy != null)
//                        OnWindowDestroy(hWnd);
//                    break;

//                    //extend here when required
//            }
//        }

//        ~Hook()
//        {
//            if (!IntPtr.Zero.Equals(pHook))
//                UnhookWinEvent(pHook);
//            pHook = IntPtr.Zero;
//            dEvent = null;

//            OnWindowCreate = null;
//            OnWindowDestroy = null;
//        }
//    }
//}
