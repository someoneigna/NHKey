using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.Model
{
    public class HotkeyAssociationFactory
    {
        public static HotkeyAssociation MakeHotkeyAssociation(string name, string filepath, int key, int mod, IntPtr handle, string parameters = null)
        {
            var hotkey = new HotkeyAssociation(NHotkeyAPI.HotkeyFactory.Create(key, mod, handle), name, filepath, parameters);
            return hotkey;
        }
    }
}
