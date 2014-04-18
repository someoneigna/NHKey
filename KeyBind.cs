using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace NHkey
{
    [DataContract]
    public class KeyBind
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

        public KeyBind(int vkey, int vmod)
        {
            Key = vkey;
            Mod = vmod;
        }

        public KeyBind(KeyBind other) : this(other.Key, other.Mod)
        {
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
