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

        private string _programFile;
        /// <summary>
        /// Fullpath to the program.
        /// </summary>
        /// <value>_programFile</value>
        public string ProgramFile
        {
            get
            {
                return Model.FilePath;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ProgramFile", "Cant be assigned an null path.");
                }

                _programFile = value;
                Model.FilePath = _programFile;

                if (!File.Exists(value))
                {
                    Model.Orphaned = true;
                }
                else
                {
                    OnPropertyChanged("Image");
                }
                OnPropertyChanged("Program");
            }
        }

        /// <summary>
        /// Formatted program name.
        /// </summary>
        public string Program
        {
            get
            {
                if (ProgramFile != null)
                {
                    int startIndex = ProgramFile.LastIndexOf("\\") + 1;
                    return ProgramFile.Substring(startIndex);
                }
                return null;
            }
        }

        public HotkeyConfigViewModel(HotkeyAssociation association = null)
        {
            Model = (association != null) ? association : new HotkeyAssociation();
            Bind = Model.GetKeyBinding();
        }
    }
}
