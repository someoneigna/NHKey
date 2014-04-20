using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.Model
{
    /// <summary>
    /// Creates hotkey and handles at disposal windows hotkey unregistration.
    /// </summary>
    public class HotkeyFactory : IHotkeyFactory, IDisposable
    {
        readonly IntPtr currentWindowHandle;
        ICollection<Hotkey> hotkeyList;

        /// <summary>
        /// Construct a Hotkey factory with current window handle.
        /// </summary>
        /// <param name="windowHandle">The handle to the current application window.</param>
        public HotkeyFactory(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                throw new ArgumentException("windowHandle", "The window handle cant be zero.");

            currentWindowHandle = windowHandle;
            hotkeyList = new List<Hotkey>();
        }

       
        public Hotkey Create(int virtualKey, int virtualModifier)
        {
            Hotkey newHotkey = new Hotkey(virtualKey, virtualModifier, currentWindowHandle);
            hotkeyList.Add(newHotkey);
            return newHotkey;
        }

        
        public Hotkey CreateWith(Hotkey hotkey)
        {
            Hotkey newHotkey = new Hotkey(hotkey);
            hotkeyList.Add(newHotkey);

            return newHotkey;
        }

        public void Dispose()
        {
            hotkeyList.Clear();
        }
    }
}
