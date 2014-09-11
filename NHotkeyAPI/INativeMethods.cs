using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHotkeyAPI
{
    public interface INativeMethods
    {
        bool Register(IntPtr hWnd, int id, int fsModifiers, int vk);

        bool Unregister(IntPtr hWnd, int id);
    }
}
