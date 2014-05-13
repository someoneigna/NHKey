using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace NHkey.Model
{
    /// <summary>
    /// Handles app starting options and language.
    /// </summary>
    public class Options : INotifyPropertyChanged
    {
        public enum Field
        {
            B_INIT_HIDDEN,
            B_START_WITH_WINDOWS,
            S_LANGUAGE,
            MAX_OPTIONS
        };

        private readonly string saveFile = Directory.GetCurrentDirectory() + "\\" + "config.data";

        private bool loaded;
        private const int FIELD_NAME_POS = 0;
        private const int VALUE_POS = 1;
        private object[] value = new object[(int)Field.MAX_OPTIONS];
        private string[] str = new string[(int)Field.MAX_OPTIONS];
        private Dictionary<string, object> map = new Dictionary<string, object>();

        private static readonly string[] availableLanguages = new string[] { "Spanish", "English" };
        public static readonly Dictionary<string, string> languageFile = new Dictionary<string, string>()
        {
            {"Spanish", "es-AR"},
            {"English", "en-US"}
        };

        public string LanguageFile { get { return languageFile[Language]; } }

        public string[] AvailableLanguages
        {
            get { return availableLanguages; }
        }

        public Options()
        {
            // Set default values
            for (int i = 0; i < (int)Field.MAX_OPTIONS; i++)
            {
                switch(i)
                {
                    case 0:
                    case 1:
                        value[i] = false;
                        break;
                    case 2:
                        value[i] = "Spanish";
                        break;
                }

                str[i] = Enum.GetName(typeof(Field), i);
                map.Add(str[i], value[i]);
            }

            if (!loaded)
                Load();
        }

        public Options(Options other) : this()
        {
            // Copy values from other into this
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = other.value[i];
            }
        }


        public string Language
        {
            get
            {
                return (string)this.value[(int)Field.S_LANGUAGE];
            }
            set
            {
                if (availableLanguages.Contains(value))
                {
                    this.value[(int)Field.S_LANGUAGE] = value;
                    //OnPropertyChanged("Language");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Language", "That language is unavailable.");
                }
            }
        }

        public bool Hidden
        {
            get
            {
                return (bool)this.value[(int)Field.B_INIT_HIDDEN];
            }
            set
            {
                this.value[(int)Field.B_INIT_HIDDEN] = value;
                OnPropertyChanged("Hidden");
            }
        }

        public bool WindowsStartup
        {
            get
            {
                return (bool)this.value[(int)Field.B_START_WITH_WINDOWS];
            }
            set
            {
                this.value[(int)Field.B_START_WITH_WINDOWS] = value;
                OnPropertyChanged("WindowsStartup");
            }
        }

        public void Set(Field opt, object value)
        {
            int index = (int)opt;
            this.value[index] = value;
        }

        /// <summary>
        /// Add the application into registry to run at Windows startup.
        /// </summary>
        private void RegisterAppInit()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (this.WindowsStartup)
            {
                key.SetValue("NHKey", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
            }
            else
            {
                key.DeleteValue("NHKey", false);
            }
            
            key.Close();
        }

        private void WriteSaveFile()
        {
            try
            {
                string data = null;
                for (int i = 0; i < (int)Field.MAX_OPTIONS; i++)
                {
                    data += str[i].Remove(0, 2) + "=" + value[i] + "\n";
                }

                File.WriteAllText(saveFile, data);

            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        public void Save()
        {
            RegisterAppInit();
            WriteSaveFile();
        }

        public void Load()
        {
            if (!File.Exists(saveFile)) return;
            try
            {
                string[] data = File.ReadAllLines(saveFile);
                string[] var = null;

                if (data != null)
                {
                    for (int i = 0; i < (int)Field.MAX_OPTIONS && i < data.Length; i++)
                    {
                        var = data[i].Split('=');
                        if (var[FIELD_NAME_POS] == str[i].Remove(0, 2))
                        {
                            if (str[i].Contains("B_"))
                                value[i] = bool.Parse(var[VALUE_POS]);
                            else
                                value[i] = var[VALUE_POS];
                        }
                    }
                    Language = value[(int)Field.S_LANGUAGE] as string;
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally { }
        }

        public string[] GetNames()
        {
            return this.str;
        }

        public object[] GetValues()
        {
            return this.value;
        }

        public object GetValue(string value)
        {
            return map[value];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }
    }
}
