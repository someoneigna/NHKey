using System;
using System.Windows.Input;

namespace NHkey.Model
{
    interface IVirtualKeyBinding
    {
        KeyBinding GetInputBinding();
        int Key { get; set; }
        int Mod { get; set; }
    }
}
