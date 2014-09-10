using NHkey.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NHkey.ViewModel
{
    /// <summary>
    /// View model for <see cref="HotkeyConfigDialog"/>
    /// hotkey edition and creation view.
    /// </summary>
    public class HotkeyConfigViewModel : ViewModelBase
    {
        public HotkeyAssociation Model { get; set; }


        private KeyBinding _bind;
        /// <summary>
        /// Temporary KeyBinding to fill <see cref="HotkeyAssociation"/>
        /// inner <see cref="NHotkeyAPI.Hotkey"/>
        /// </summary>
        /// <value>_bind</value>
        public KeyBinding Bind 
        {
            get
            {
                return _bind;
            }
            set
            {
                _bind = value;
                OnPropertyChanged("CombinationText");
                OnPropertyChanged("Bind");
            }
        }

        public HotkeyConfigViewModel(HotkeyAssociation association = null)
        {
            Model = (association != null) ? association : new HotkeyAssociation();
            Bind = Model.GetKeyBinding();
        }

        /// <summary>
        /// Checks if the chosen key combination is
        /// already used.
        /// </summary>
        /// <returns>True if the combination is used, false otherwise</returns>
        public bool HotkeyAvailable()
        {
            return !App.Instance.Repository.Exists(Model);
        }

        /// <summary>
        /// Used for validation of Save button.
        /// </summary>
        public bool Valid()
        {
            return (!string.IsNullOrWhiteSpace(Model.Name) &&
                    !string.IsNullOrWhiteSpace(Model.FilePath) &&
                    HotkeyAvailable());

        }
    }
}
