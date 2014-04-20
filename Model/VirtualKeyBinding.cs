using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace NHkey.Model
{
    [DataContract]
    public class VirtualKeyBinding : IVirtualKeyBinding
    {
        private int key;
        private int mod;

        #region Properties
        [DataMember]
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        [DataMember]
        public int Mod
        {
            get { return mod; }
            set { mod = value; }
        }
        #endregion

        public VirtualKeyBinding(int vkey, int vmod)
        {
            Key = vkey;
            Mod = vmod;
        }

        public VirtualKeyBinding(VirtualKeyBinding other) : this(other.Key, other.Mod)
        {
        }
        
        public VirtualKeyBinding(KeyBinding other)
        {
            Key = KeyInterop.VirtualKeyFromKey(other.Key);
            Mod = (int)other.Modifiers;
        }


        public KeyBinding GetInputBinding()
        {
            KeyBinding keybind = new KeyBinding();
            keybind.Key = KeyInterop.KeyFromVirtualKey(Key);
            keybind.Modifiers = ModifierKeys.None + Mod;

            return keybind;
        }

        public override string ToString()
        {
            KeyBinding keybind = GetInputBinding();
            return keybind.Key + " + " + keybind.Modifiers;
        }

    }
}
