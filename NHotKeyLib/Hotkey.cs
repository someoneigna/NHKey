using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using NHotkeyAPI;

namespace NHkey.NHotkeyAPI
{
    public class Hotkey : IDisposable, IEquatable<Hotkey>, IEquatable<Tuple<int, int>>
    {
        #region Properties

        /// <summary>
        /// Virtual Key code
        /// </summary>
        public int Key { get; protected set; }

        public int Modifier { get; protected set; }

        public bool Registered { get; protected set; }

        /// <summary>
        /// Returns the text representation of the hotkey values.
        /// </summary>
        public string HotkeyText
        {
            get { return this.ToString(); }
        }

        /// <summary>
        /// The Window handle used to register the hotkey.
        /// </summary>
        public IntPtr Handle { get; protected set; }

        /// <summary>
        /// Returns the hashcode used to distinguish and activate the hotkeys.
        /// </summary>
        public int Id { get { return GetHashCode(); } }

        #endregion

        #region Constructors
        public Hotkey(int vkey, int vmod)
            : this(vkey, vmod, IntPtr.Zero)
        {
        }

        public Hotkey(Tuple<int, int> bind, IntPtr wHandle)
            : this(bind.Item1, bind.Item2, wHandle)
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
        }

        /// <summary>
        /// Changes the hotkey window handle and registers the hotkey.
        /// </summary>
        /// <param name="newHandle">A new window handle.</param>
        /// <returns>True if hotkey registered correctly, false otherwise.</returns>
        public bool Reload(IntPtr newHandle)
        {
            if (Registered) Unregister();

            Handle = newHandle;

            Register();

            return Registered;
        }

        public bool Register()
        {
            bool success = NativeMethods.RegisterHotKey(Handle, Id, Modifier, Key);
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Registered = true;

            return Registered;
        }

        public bool Unregister()
        {
            bool success = NativeMethods.UnregisterHotKey(Handle, Id);
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Registered = false;

            return Registered;
        }

        public override string ToString()
        {
            return Key + " " + Modifier;
        }

        public override int GetHashCode()
        {
            return Modifier ^ Key; // If you want to allow the same hotkey in another window: ^ Handle.ToInt32();
        }

        #endregion

        public void Dispose()
        {
            Unregister();
            GC.SuppressFinalize(this);
        }

        public bool Equals(Hotkey other)
        {
            return ( Key == other.Key &&
                     Modifier == other.Modifier );
        }

        public bool Equals(Tuple<int, int> bind)
        {
            return ( Key == bind.Item1 &&
                     Modifier == bind.Item2 );
        }

        /// <summary>
        /// Changes the window handle for the hotkey.
        /// If already registered unregisters and then swaps the handle.
        /// </summary>
        /// <param name="WindowHandle">A handle to a window (Form or WPF Window)</param>
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
