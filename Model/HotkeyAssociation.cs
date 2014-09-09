using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHkey.NHotkeyAPI;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace NHkey.Model
{
    /// <summary>
    /// Contains a <see cref="NHotkeyAPI.Hotkey"/>,
    /// manages hotkey status and associates it with a program.
    /// </summary>
    [DataContract]
    public class HotkeyAssociation : INotifyPropertyChanged, IEquatable<HotkeyAssociation>
    {
        private NHotkeyAPI.Hotkey hotkey;
        public NHotkeyAPI.Hotkey Hotkey
        {
            get { return hotkey; }
            set
            {
                hotkey = value;
                OnPropertyChanged("Hotkey");
            }
        }

        private string name;
        public string Name 
        { 
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name"); 
            } 
        }

        private string path;
        public string FilePath
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged("FilePath");
                OnPropertyChanged("Icon");
            }
        }

        private string parameters;
        public string Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        private BitmapSource icon;
        public BitmapSource Icon
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    icon = Helpers.BitmapHelper.GetIcon(FilePath);
                }
                return icon; 
            }
            set
            {
                icon = value;
                if (icon != null)
                {
                    OnPropertyChanged("Icon");
                }
            }
        }

        /// <summary>
        /// Indicates if the file pointed at by <see cref="FilePath"/>
        /// no longer exists.
        /// </summary>
        private bool orphaned;

        public bool Orphaned
        {
            get { return orphaned; }
            set
            {
                orphaned = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Orphaned");
            }
        }
         

        /// <summary>
        /// True when no key has been assigned to the keybind, or
        /// no modifier set.
        /// </summary>
        /// <value><see cref="NHotkeyAPI.Hotkey.Key"/></value>
        /// <value><see cref="NHotkeyAPI.Hotkey.Modifier"/></value>
        public bool Invalid
        { 
            get 
            { 
                return (hotkey.Key == 0 || hotkey.Modifier == (int)ModifierKeys.None); 
            } 
        }

        public HotkeyAssociation(Hotkey hotkey, string name, string path, string arguments = null)
        {
            Hotkey = hotkey;
            Name = name;
            FilePath = path;
            Parameters = arguments;
        }

        /// <summary>
        /// Makes a copy of <paramref name="editHotkey"/>
        /// </summary>
        /// <param name="editHotkey">A instace of <see cref="HotkeyAssociation"/></param>
        public HotkeyAssociation(HotkeyAssociation editHotkey)
        {
            if (editHotkey == null)
            {
                throw new ArgumentNullException("editHotkey", "Cant copy a null instance.");
            }

            Hotkey = (editHotkey.Hotkey != null) ? new Hotkey(editHotkey.Hotkey) : null;
            FilePath = (editHotkey.FilePath != null) ? string.Copy(editHotkey.FilePath) : null;
            Parameters = (editHotkey.Parameters != null) ? string.Copy(editHotkey.Parameters) : null;
            Icon = Icon; // Force the Icon to load from FilePath
            Name = (editHotkey.Name != null) ? string.Copy(editHotkey.Name) : null;
        }

        public HotkeyAssociation()
        {
            Hotkey = new Hotkey();
        }

        /// <summary>
        /// Changes the contained <see cref="NHotkeyAPI.Hotkey"/> keybind.
        /// </summary>
        /// <param name="tempBind">A <see cref="KeyBinding"/></param>
        internal void SetBind(KeyBinding tempBind)
        {
            int vkey = KeyInterop.VirtualKeyFromKey(tempBind.Key);
            int vmod = (int)tempBind.Modifiers;

            if (Hotkey != null)
            {
                Hotkey.SwitchBind(vkey, vmod);
            }
            else
            {
                Hotkey = new Hotkey(vkey, vmod);
            }
        }

        public override string ToString()
        {
            var bind = GetKeyBinding();
            return bind.Key + " + " + bind.Modifiers;
        }

        /// <summary>
        /// Used to position the hotkeys in a dictionary.
        /// </summary>
        /// <returns>The <see cref="Hotkey.GetHashCode"/>.</returns>
        public override int GetHashCode()
        {
            return Hotkey.GetHashCode();
        }

        /// <summary>
        /// Registers the contained <see cref="NHotkeyAPI.Hotkey"/>
        /// only if it's not already registered.
        /// </summary>
        public void Enable()
        {
            if (!Hotkey.Registered)
            {
                Hotkey.Register();
            }
        }


        /// <summary>
        /// Unregisters the contained <see cref="NHotkeyAPI.Hotkey"/>
        /// only if it's registered.
        /// </summary>
        public void Disable()
        {
            if (Hotkey.Registered)
            {
                Hotkey.Unregister();
            }
        }

        /// <summary>
        /// Swap the values for HotkeyAssociation skipping the Hotkey.
        /// </summary>
        /// <param name="other">Another <see cref="HotkeyAssociation"/>.</param>
        internal void Swap(HotkeyAssociation other)
        {
            Name = other.Name;
            FilePath = other.FilePath;
            Parameters = other.Parameters;
            Icon = other.Icon;
            Orphaned = other.Orphaned;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public bool Equals(HotkeyAssociation other)
        {
            return ( FilePath == other.FilePath &&
                     Name == other.Name &&
                     Parameters == other.Parameters );
        }

        /// <summary>
        /// True when the filepath is filled and hotkey is registered.
        /// </summary>
        public bool Enabled {
            get {
                return (FilePath != null && hotkey.Registered);
            }
        }

        /// <summary>
        /// Generates a <see cref="KeyBinding"/> from 
        /// <see cref="Hotkey"/> contained values.
        /// </summary>
        /// <returns>A KeyBinding with Hotkey values.</returns>
        public KeyBinding GetKeyBinding()
        {
            KeyBinding bind = new KeyBinding();
            if (Hotkey != null)
            {
                bind.Key = KeyInterop.KeyFromVirtualKey(Hotkey.Key);
                bind.Modifiers = ModifierKeys.None + Hotkey.Modifier;
            }
            else
            {
                bind.Key = Key.None;
                bind.Modifiers = ModifierKeys.None;
            }
            return bind;
        }
    }
}
