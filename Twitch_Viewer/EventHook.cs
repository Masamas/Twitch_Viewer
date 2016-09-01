using System;
using System.Runtime.InteropServices;

namespace Twitch_Viewer
{
    public class EventHook
    {
        public delegate void WinEventDelegate(
              IntPtr hWinEventHook, uint eventType,
              IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(
              uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
              uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        public const uint WINEVENT_OUTOFCONTEXT = 0;
        public const uint EVENT_OBJECT_CREATE = 0x8000;

        readonly WinEventDelegate _procDelegate;
        readonly IntPtr _hWinEventHook;

        public EventHook(WinEventDelegate handler, uint eventMin, uint eventMax)
        {
            _procDelegate = handler;
            _hWinEventHook = SetWinEventHook(eventMin, eventMax, IntPtr.Zero, handler, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public EventHook(WinEventDelegate handler, uint eventMin)
              : this(handler, eventMin, eventMin)
        {
        }

        public void Stop()
        {
            UnhookWinEvent(_hWinEventHook);
        }
    }
}