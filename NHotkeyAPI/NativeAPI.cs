using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHotkeyAPI
{
    public class NativeAPI : INativeMethods
    {
        public bool Register(IntPtr hWnd, int id, int fsModifiers, int vk)
        {
            return NativeMethods.RegisterHotKey(hWnd, id, fsModifiers, vk);
        }

        public bool Unregister(IntPtr hWnd, int id)
        {
            return NativeMethods.UnregisterHotKey(hWnd, id);
        }
    }
}
