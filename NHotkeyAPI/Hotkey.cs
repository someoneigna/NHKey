using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NHotkeyAPI;

namespace NHkey.NHotkeyAPI
{
    /// <summary>
    /// Contains a key bind of virtual keys to
    /// use with WinAPI RegisterHotkey()
    /// </summary>
    public class Hotkey : IDisposable, IEquatable<Hotkey>, IEquatable<Tuple<int, int>>, IHotkey
    {
        #region Properties

        /// <summary>
        /// Virtual Key code
        /// </summary>
        public int Key { get; protected set; }

        /// <summary>
        /// Virtual Modifier key code
        /// </summary>
        public int Modifier { get; protected set; }

        /// <summary>
        /// True when the <see cref="Register"/> has been called and the
        /// keybind is hooked.
        /// </summary>
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

        private INativeMethods NativeAPI = new NativeAPI();

        public Hotkey(INativeMethods nativeApi,
            int key, int modifiers, IntPtr handle) : this(key, modifiers, handle)
        {
            NativeAPI = nativeApi;
        }

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

        /// <summary>
        /// Swaps the key and modifier of the keybind.
        /// </summary>
        /// <param name="bind">Tuple containing virtual key code, and virtual modifier code.</param>
        public virtual void SwitchBind(Tuple<int, int> bind)
        {
            SwitchBind(bind.Item1, bind.Item2);
        }

        /// <summary>
        /// Swaps the key and modifier of the keybind.
        /// </summary>
        /// <param name="key">A virtual key code.</param>
        /// <param name="modifier">A virtual modifier key code.</param>
        public virtual void SwitchBind(int key, int modifier)
        {
            Key = key;
            Modifier = modifier;
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

        /// <summary>
        /// Calls Win32API RegisterHotkey(), registers hotkey hooking it globally.
        /// </summary>
        /// <returns>True if </returns>
        public bool Register()
        {
            bool success = NativeAPI.Register(Handle, Id, Modifier, Key);
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Registered = true;

            return Registered;
        }

        // <summary>
        /// Calls Win32API UnregisterHotkey(), unhooking the keybind off
        /// globally. 
        /// </summary>
        /// <returns>True if unregistered correctly, false otherwise.</returns>
        /// <exception cref="Win32Exception">If failed to unregister keybind properly.</exception>
        public bool Unregister()
        {
            bool success = NativeAPI.Unregister(Handle, Id);
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
