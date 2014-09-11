using System;
namespace NHotkeyAPI
{
    public interface IHotkey
    {
        void Dispose();
        bool Equals(NHkey.NHotkeyAPI.Hotkey other);
        bool Equals(Tuple<int, int> bind);
        int GetHashCode();
        IntPtr Handle { get; }
        string HotkeyText { get; }
        int Id { get; }
        int Key { get; }
        int Modifier { get; }
        bool Register();
        bool Registered { get; }
        bool Reload(IntPtr newHandle);
        void SetHandle(IntPtr WindowHandle);
        void SwitchBind(int key, int modifier);
        void SwitchBind(Tuple<int, int> bind);
        string ToString();
        bool Unregister();
    }
}
