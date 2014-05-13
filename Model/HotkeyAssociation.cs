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
    [DataContract]
    public class HotkeyAssociation : INotifyPropertyChanged
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
            set { 
                name = value;
                OnPropertyChanged("Name"); 
            } 
        }

        private string path;
        public string FilePath
        {
            get { return path; }
            set { 
                path = value;
                OnPropertyChanged("FilePath");
            }
        }

        private string parameters;
        public string Parameters
        {
            get { return parameters; }
            set { 
                parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        private BitmapSource icon;
        public BitmapSource Icon
        {
            get 
            { 
                if (icon == null && !string.IsNullOrEmpty(FilePath))
                {
                    Icon = Helpers.BitmapHelper.GetIcon(FilePath);
                }
                return icon; 
            }
            set {
                icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HotkeyAssociation(Hotkey hotkey, string name, string path, string arguments = null)
        {
            Hotkey = hotkey;
            Name = name;
            FilePath = path;
            Parameters = arguments;
        }

        public HotkeyAssociation(HotkeyAssociation editHotkey)
        {
            Hotkey = (editHotkey.Hotkey != null) ? new Hotkey(editHotkey.Hotkey) : null;
            FilePath = (editHotkey.FilePath != null) ? string.Copy(editHotkey.FilePath) : null;
            Parameters = (editHotkey.Parameters != null) ? string.Copy(editHotkey.Parameters) : null;
            Icon = Icon;
            Name = (editHotkey.Name != null) ? string.Copy(editHotkey.Name) : null;
        }

        public HotkeyAssociation()
        {
            Hotkey = new Hotkey();
        }

        

        internal void SetBind(KeyBinding tempBind)
        {
            int vkey = KeyInterop.VirtualKeyFromKey(tempBind.Key);
            int vmod = (int)tempBind.Modifiers;

            if (Hotkey != null)
                Hotkey.SwitchBind(vkey, vmod);
            else
                Hotkey = new Hotkey(vkey, vmod);
        }

        public override string ToString()
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
        /// Swap the values for HotkeyAssociation skipping the Hotkey.
        /// </summary>
        /// <param name="dialogResultHotkey">Another <see cref="HotkeAssociation"/>.</param>
        internal void Swap(HotkeyAssociation dialogResultHotkey)
        {
            Name = dialogResultHotkey.Name;
            FilePath = dialogResultHotkey.FilePath;
            Parameters = dialogResultHotkey.Parameters;
            Icon = dialogResultHotkey.Icon;
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
