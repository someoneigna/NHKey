using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey
{
    interface IHotkeyFactory
    {

        /// <summary>
        /// Create a Hotkey with the current window handle.
        /// </summary>
        /// <param name="virtualKey">Keyboard virtual key.</param>
        /// <param name="virtualModifier">Keyboard virtual modifier key combination.</param>
        /// <returns>A Hotkey with current window handle passed at constructor.</returns>
        Hotkey Create(int virtualKey, int virtualModifier);

        /// <summary>
        /// Make a new Hotkey from the passed instance.
        /// </summary>
        /// <param name="hotkey">A valid hotkey instance.</param>
        /// <returns>A copy of the passed hotkey.</returns>
        Hotkey CreateWith(Hotkey hotkey);
    }
}
