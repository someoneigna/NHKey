using NHotkeyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.NHotkeyAPI
{
    /// <summary>
    /// Creates hotkey and handles at disposal windows hotkey unregistration.
    /// </summary>
    public static class HotkeyFactory
    {
        /// <summary>
        /// Construct a Hotkey with current window handle and key data.
        /// </summary>
        /// <param name="virtualKey">The virtual key value.</param>
        /// <param name="virtualModifier">The virtual modifier value.</param>
        /// <param name="windowHandle">The handle to the current application window.</param>
        public static Hotkey Create(int virtualKey, int virtualModifier, IntPtr handle)
        {
            return new Hotkey(virtualKey, virtualModifier, handle);
        }

        /// <summary>
        /// Construct a <see cref="Hotkey"/> with the passed hotkey and window handle.
        /// </summary>
        /// <param name="hotkey">A valid<see cref="Hotkey"/>.</param>
        /// <param name="handle">The current window handle.</param>
        /// <returns>A copy constructed hotkey.</returns>
        public static Hotkey CreateWith(Hotkey hotkey, IntPtr handle)
        {
            return new Hotkey(hotkey.Key, hotkey.Modifier, handle);
        }
    }
}
