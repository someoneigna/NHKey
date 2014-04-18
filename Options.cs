using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace NHkey
{
    public class Options
    {
        public enum Field
        {
            INIT_HIDDEN = 0,
            START_WITH_WINDOWS = 1,
            MAX_OPTIONS = 2
        };

        private const int FIELD_NAME_POS = 0;
        private const int VALUE_POS = 1;
        private bool[] value = new bool[(int)Field.MAX_OPTIONS];
        private string[] str = new string[(int)Field.MAX_OPTIONS];
        private Dictionary<string, bool> map = new Dictionary<string, bool>();

        public Options()
        {

            for (int i = 0; i < (int)Field.MAX_OPTIONS; i++)
            {
                str[i] = Enum.GetName(typeof(Field), i);
                map.Add(str[i], value[i]);
            }
        }

        public Options(Options opt) : this()
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = opt.value[i];
            }
        }

        private string saveFile = Directory.GetCurrentDirectory() + "\\" + "config.data";

        public bool Hidden
        {
            get
            {
                return this.value[(int)Field.INIT_HIDDEN];
            }
            set
            {
                this.value[(int)Field.INIT_HIDDEN] = value;
            }
        }

        public bool WindowsStartup
        {
            get
            {
                return this.value[(int)Field.START_WITH_WINDOWS];
            }
            set
            {
                this.value[(int)Field.START_WITH_WINDOWS] = value;
            }
        }

        public void Set(Field opt, bool value)
        {
            int index = (int)opt;
            this.value[index] = value;
        }

        private void RegisterAppInit()
        {
            if (this.WindowsStartup)
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue("NHKey", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
            }
            else
            {

                RegistryKey del = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                bool found = false;
                try
                {
                    del.DeleteValue("NHKey");
                }
                catch (ArgumentException except)
                {
                }
            }
        }

        private void WriteSaveFile()
        {
            try
            {
                string data = null;
                for (int i = 0; i < (int)Field.MAX_OPTIONS; i++)
                {
                    data += str[i] + "=" + value[i] + "\n";
                }

                File.WriteAllText(saveFile, data);

            }
            catch (IOException ex)
            {

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

            try
            {
                string[] data = File.ReadAllLines(saveFile);
                string[] var = null;

                if (data != null)
                {
                    for (int i = 0; i < (int)Field.MAX_OPTIONS && i < data.Length; i++)
                    {
                        var = data[i].Split('=');
                        if (var[FIELD_NAME_POS] == str[i])
                        {
                            value[i] = bool.Parse(var[VALUE_POS]);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
            }
            finally { }
        }

        public string[] GetNames()
        {
            return this.str;
        }

        public bool[] GetValues()
        {
            return this.value;
        }

        public bool GetValue(string type)
        {
            return map[type];
        }
    }
}
