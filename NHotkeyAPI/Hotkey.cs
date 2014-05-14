using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHkey.NHotkeyAPI
{
    public class Hotkey : IDisposable, IEquatable<Hotkey>, IEquatable<Tuple<int,int>>
    {
        #region Properties

        public bool Invalid { get { return (Key == 0 && Modifier == 0); } }

        public int Key { get; protected set; }

        public int Modifier { get; protected set; }

        public bool Registered { get; protected set; }

        public string HotkeyText
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// The Window handle used to register the hotkey.
        /// </summary>
        public IntPtr Handle { get; protected set; }

        public int Id { get { return GetHashCode(); } }

        #endregion

        #region Constructors
        public Hotkey(int vkey, int vmod)
            : this(vkey, vmod, IntPtr.Zero)
        {
        }

        public Hotkey(Tuple<int, int> bind, IntPtr wHandle) : this(bind.Item1, bind.Item2, wHandle)
        {
        }

        public Hotkey(int vkey, int vmod, IntPtr wHandle)
        {
            Handle = wHandle;
            Key = vkey;
            Modifier = vmod;
        }

        public Hotkey()
            : this(0, 0, IntPtr.Zero)
        {
        }

        public Hotkey(Hotkey other)
            : this(other.Key, other.Modifier, other.Handle)
        {
        }

        #endregion

        #region Public Methods

        public virtual void SwitchBind(Tuple<int, int> bind)
        {
            SwitchBind(bind.Item1, bind.Item2);
        }

        public virtual void SwitchBind(int key, int mod)
        {
            Key = key;
            Modifier = mod;
            Reload(Handle);
        }

        /// <summary>
        /// Changes the hotkey window handle and registers the hotkey.
        /// </summary>
        /// <param name="newHandle">A new window handle.</param>
        /// <returns>True if hotkey registered correctly, false otherwise.</returns>
        public bool Reload(IntPtr newHandle)
        {
            Unregister();

            Handle = newHandle;

            Registered = Register();

            return Registered;
        }

        public bool Register()
        {
            Registered = RegisterHotKey(Handle, Id, Modifier, Key);
            if (!Registered)
            {
                var error = new Win32Exception(Marshal.GetLastWin32Error());
            }

            return Registered;
        }

        public bool Unregister()
        {
            Registered = UnregisterHotKey(Handle, Id);
            return Registered;
        }


        public override string ToString()
        {
            return Key + " " + Modifier;
        }


        public override int GetHashCode()
        {
            return Modifier ^ Key ^ Handle.ToInt32();
        }

        #endregion

        #region DllImports
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        public void Dispose()
        {
            Registered = Unregister();
            GC.SuppressFinalize(this);
        }

        public bool Equals(Hotkey other)
        {
            return ( Key == other.Key &&
                     Modifier == other.Modifier);
        }

        public bool Equals(Tuple<int, int> bind)
        {
            return ( Key == bind.Item1 &&
                     Modifier == bind.Item2 );
        }

        public void SetHandle(IntPtr WindowHandle)
        {
            if (Registered)
            {
                Unregister();
                Handle = WindowHandle;
                Register();
            }
            else
            {
                Handle = WindowHandle;
            }
            
        }
    }
}
