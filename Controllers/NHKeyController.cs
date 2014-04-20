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

using NHkey.Data;
using NHkey.Model;

namespace NHkey.Controllers
{

    // This should disolve into ViewModels. I have to keep refactoring it.
    public class NHKeyController : INotifyPropertyChanged
    {
        HotkeyRepository repository;
        public IDictionary<int, Hotkey> Hotkeys { get; protected set; }

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
            
            Hotkeys = new Dictionary<int, Hotkey>();

            hotkeyFactory = new HotkeyFactory(windowHandle);
            
            RecentlyUsed = new List<Hotkey>();
            
            repository = new HotkeyRepository(new JSONHotkeyContext(SaveFilePath));
        }

        public NHKeyController() : this(IntPtr.Zero)
        {
        }

        public void OnLoad()
        {
            options.Load();
            repository.Load();

            foreach (var hotkey in repository.GetAll())
            {
                hotkey.Icon = GetIcon(hotkey.FilePath);
                hotkey.Reload(WindowHandle);
                Hotkeys.Add(hotkey.GetHashCode(), hotkey);
            }

            this.Hidden = options.Hidden;
        }

        public void OnStart()
        {
            
        }

        public void OnClose()
        {
            foreach (var hotkey in Hotkeys.Values)
            {
                hotkey.Dispose();
            }

            repository.Save();
            options.Save();
        }

        public void ActivateHotkey(int id)
        {
            if (Hotkeys.ContainsKey(id))
            {
                RecentlyUsed.Add(Hotkeys[id]);
                var hotkey = Hotkeys[id];
                Execute (hotkey.FilePath, hotkey.Parameters);
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
            
            // Unregister hotkey from global event loop
            success = orig.Unregister();

            replace.Reload(WindowHandle);
            repository.Remove(orig);
            repository.Add(replace);

            Hotkeys.Remove(orig.GetHashCode());
            Hotkeys.Add(replace.GetHashCode(), replace);

            /*
            if (hotkeys.ContainsKey(orig.GetHashCode()))
            {
                orig.Unregister();
                hotkeys.Remove(orig.GetHashCode());
                AddNewHotkey(replace);
                success = true;
            }*/
            return success;
        }

        public bool AddNewHotkey(Hotkey hotkey)
        {
            if (hotkey == null) { throw new ArgumentNullException("hotkey", "Cant add a null hotkey."); }
            bool completed = false;

            // This seems wrong
            completed = hotkey.Reload(WindowHandle);

            repository.Add(hotkey);

            //Add hotkey to ListBox

            /*
            if (!hotkeys.ContainsKey(hotkey.GetHashCode()))
            {
                hotkey.Icon = NHKeyController.GetIcon(hotkey.FilePath);
                hotkeys.Add(hotkey.GetHashCode(), hotkey);
                completed = true;
            }*/
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

        public IDictionary<int, Hotkey> GetHotkeySource()
        {
            if (Hotkeys == null)
                repository.Load();

            return Hotkeys;
        }

        public Options GetOptions()
        {
            return options;
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

        public void Execute(string programPath, string parameters)
        {
            System.Diagnostics.Process.Start(programPath, parameters);
        }
        
    }
}
