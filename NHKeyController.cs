using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NHkey
{
    public class NHKeyController : INotifyPropertyChanged
    {
        IDictionary<int, Hotkey> hotkeys;
        public IDictionary<int, Hotkey> Hotkeys { get { return hotkeys; } set { hotkeys = value; OnPropertyChanged("Hotkeys"); } }

        private static string SaveFilePath = Directory.GetCurrentDirectory() + "\\" + "hotkeys.data";
        private static Options options;
        private HotkeyFactory hotkeyFactory;

        public List<Hotkey> RecentlyUsed { get; set; }


        public bool Hidden
        {
            get;
            protected set;
        }


        public NHKeyController(IntPtr windowHandle)
        {
            //Handle to register Hotkeys with WinAPI
            options = new Options();
            WindowHandle = windowHandle;
            hotkeys = new Dictionary<int, Hotkey>();
            hotkeyFactory = new HotkeyFactory(windowHandle);
            RecentlyUsed = new List<Hotkey>();
        }

        public NHKeyController() : this(IntPtr.Zero)
        {
        }

        public void OnLoad()
        {
            options.Load();
            this.Hidden = options.Hidden;
        }

        public void OnStart()
        {
            LoadHotkeys();
        }

        public void OnClose()
        {
            SaveHotkeys();
            options.Save();
        }

        public void ActivateHotkey(int id)
        {
            if (hotkeys.ContainsKey(id))
            {
                RecentlyUsed.Add(hotkeys[id]);
                hotkeys[id].Execute();
            }
        }

        public bool SetOption(Options.Field opt, bool value)
        {
            bool result = false;

            if (Enum.IsDefined(typeof(Options.Field), opt))
            {
                options.Set(opt, value);
                result = true;
            }
            return result;
        }

        public bool ReplaceHotkey(Hotkey orig, Hotkey replace)
        {
            if (replace == null)
            {
                throw new ArgumentNullException("replace", "Cant replace with a null hotkey.");
            }

            bool success = false;
            replace.Reload(WindowHandle);
            if (hotkeys.ContainsKey(orig.GetHashCode()))
            {
                orig.Unregister();
                hotkeys.Remove(orig.GetHashCode());
                AddNewHotkey(replace);
                success = true;
            }
            return success;
        }

        public bool AddNewHotkey(Hotkey hotkey)
        {
            if (hotkey == null) { throw new ArgumentNullException("hotkey", "Cant add a null hotkey."); }
            bool completed = false;

            hotkey.Reload(WindowHandle);
            
            //Add hotkey to ListBox
            if (!hotkeys.ContainsKey(hotkey.GetHashCode()))
            {
                hotkey.Icon = NHKeyController.GetIcon(hotkey.FilePath);
                hotkeys.Add(hotkey.GetHashCode(), hotkey);
                completed = true;
            }
            return completed;
        }

        /// <summary>
        /// Gets the icon bitmap for the specified program.
        /// </summary>
        /// <param name="filename">Full path to the program.</param>
        /// <returns>The icon as a BitmapSource </returns>
        public static BitmapSource GetIcon(string filename)
        {
            if (filename == null) { return null; }
            System.Drawing.Bitmap bitmap = System.Drawing.Icon.ExtractAssociatedIcon(filename).ToBitmap();
            BitmapImage result = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;

                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();

            }
            return result;
        }

        private void AddSavedHotkey(Hotkey hk)
        {
            bool success = false;

            //Add hotkey to ListBox
            if (!hotkeys.ContainsKey(hk.GetHashCode()))
            {

                success = hk.Reload(WindowHandle);
                hk.Icon = NHKeyController.GetIcon(hk.FilePath);
                hotkeys.Add(hk.GetHashCode(), hk);

                if (!success)
                    System.Windows.MessageBox.Show("Lo siento, no se pudo registrar la combinación",
                                                   "Error al registrar combinacion", 
                                                   System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error
                                                   );
            }
        }

        private void SaveHotkeys()
        {
            FileStream saveFile = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer( typeof(Hotkey[]) );

            try
            {
                saveFile = File.Create(SaveFilePath);
                if (saveFile.CanWrite)
                {
                    serializer.WriteObject(saveFile, hotkeys.Values.ToArray());
                }
            }
            catch (IOException except)
            {
                throw except;
            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        private void LoadHotkeys()
        {
            FileStream saveFile = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer( typeof(Hotkey[]) );

            try
            {
                saveFile = File.OpenRead(SaveFilePath);
                if (saveFile.CanRead)
                {
                    Hotkey[] hotkeys = null;
                    try
                    {
                        saveFile.Position = 0;
                        hotkeys = (Hotkey[])serializer.ReadObject(saveFile);
                        for (int i = 0; i < hotkeys.Length; i++)
                        {
                            if (hotkeys[i] != null && hotkeys[i].FilePath != null)
                            {
                                AddSavedHotkey(hotkeys[i]);
                            }
                        }
                    }
                    catch (SerializationException ex)
                    {
                        //System.Windows.MessageBox.Show(ex.Message);
                    }

                }
            }
            catch (IOException except)
            {
                //System.Windows.MessageBox.Show(except.Message);
            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        public IDictionary<int, Hotkey> GetHotkeySource()
        {
            return hotkeys;
        }

        public Options GetOptions()
        {
            return new Options(options);
        }

        public void SetOptions(Options options)
        {
            NHKeyController.options = options;
        }

        public IntPtr WindowHandle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
