using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHkey.Model
{
    [DataContract(Name = "Hotkey")]
    public class Hotkey : IDisposable, INotifyPropertyChanged
    {

        #region Fields
        private string name, filepath;
        private BitmapSource icon;
        private bool registered;

        #endregion

        #region Properties
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        [DataMember]
        public VirtualKeyBinding Bind { get; set; }

        [DataMember]
        public string FilePath
        {

            get { return filepath; }
            set
            {
                filepath = value;
                OnPropertyChanged("FilePath");
            }
        }


        private string parameters;

        [DataMember]
        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; OnPropertyChanged("Parameters"); }
        }

        public string HotkeyText
        {
            get { return this.ToString(); }
        }

        public BitmapSource Icon
        {
            get { return icon; }
            set { icon = value; OnPropertyChanged("Icon"); }
        }

        public IntPtr Handle { get; protected set; }

        public int Id { get; protected set; }

        #endregion

        #region Constructors
        public Hotkey(int vkey, int vmod)
            : this(vkey, vmod, IntPtr.Zero)
        {
        }

        public Hotkey(int vkey, int vmod, IntPtr wHandle)
        {
            Handle = wHandle;
            registered = false;
            Bind = new VirtualKeyBinding(vkey, vmod);
            Id = GetHashCode();
            OnPropertyChanged("HotkeyText");
        }
        
     
        public Hotkey() : this(0,0, IntPtr.Zero)
        {
        }

        public Hotkey(Hotkey other) : this(other.Bind.Key, other.Bind.Mod, other.Handle)
        {
            FilePath = other.FilePath;
            Name = other.Name;
            Parameters = other.Parameters;
        }

        #endregion

        #region Private Methods
       
        #endregion

        #region Public Methods

        public void SwitchBind(System.Windows.Input.KeyBinding tempBind)
        {
            Bind = new VirtualKeyBinding(tempBind);
            OnPropertyChanged("HotkeyText");
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

            Id = GetHashCode();

            registered = Register();

            return registered;
        }

        public bool Register()
        {
            return RegisterHotKey(Handle, Id, Bind.Mod, Bind.Key);
        }

        public bool Unregister()
        {
            return UnregisterHotKey(Handle, Id);
        }
        

        public override string ToString()
        {
            return Bind.ToString();
        }


        public override int GetHashCode()
        {
            return Bind.Mod ^ Bind.Key ^ Handle.ToInt32();
        }

        #endregion

        #region DllImports
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region Static Methods
        
        #endregion

        public void Dispose()
        {
            registered = Unregister();
            GC.SuppressFinalize(this);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
