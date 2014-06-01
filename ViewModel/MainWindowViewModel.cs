using GalaSoft.MvvmLight.Command;
using NHkey.Data;
using NHkey.Helpers;
using NHkey.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NHkey.ViewModel
{
    public class MainWindowViewModel : ViewModelBase , INotifyCollectionChanged
    {
        private Dictionary<int, HotkeyAssociation> hotkeys;

        public Dictionary<int, HotkeyAssociation> Hotkeys { get { return hotkeys; } protected set { hotkeys = value; OnPropertyChanged("Hotkeys"); } }

        public static IntPtr WindowHandle;

        private HotkeyRepository repository;

        private static string SaveFilePath = Directory.GetCurrentDirectory() + "\\" + "hotkeys.data";

        public Options CurrentOptions { get; set; }
        public Action Close { get; protected set; }

        public MainWindowViewModel()
        {
            repository = new HotkeyRepository(new JSONHotkeyContext(SaveFilePath));

            Hotkeys = LoadFromRepository();

            CurrentOptions = new Options();

            // Actions
            Close += new Action(() => SaveAndExit());
        }

        private Dictionary<int, HotkeyAssociation> LoadFromRepository()
        {
            var hotkeyDict = repository.GetAll().ToDictionary<HotkeyAssociation, int>((hk) => hk.Hotkey.Id);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, hotkeyDict.ToList()));

            return hotkeyDict;
        }

        private void SaveAndExit()
        {
            saveHotkeys();
            CurrentOptions.Save();
        }

        public void DisableHotkeys()
        {
            foreach(var hotkey in Hotkeys.Values.ToList())
            {
                hotkey.Disable();
            }
        }

        public void EnableHotkeys()
        {
            foreach (var hotkey in Hotkeys.Values.ToList())
            {
                if (hotkey.Hotkey.Handle == IntPtr.Zero)
                {
                    hotkey.Hotkey.SetHandle(WindowHandle);
                }
                hotkey.Enable();
            }
        }

        /// <summary>
        /// Update the hotkey associations with the current window handle.
        /// 
        /// Rearranges the dictionary keys and registers the key bindings.
        /// </summary>
        /// <param name="handle">The handle for the current window/form.</param>
        public void SetWindowHandle(IntPtr handle)
        {
            WindowHandle = handle;
            List<HotkeyAssociation> old = Hotkeys.Values.ToList();

            // Register hotkeys with new window handle.
            for(int i=0; i < Hotkeys.Count; i++)
            {
                var hotkey = old[i];

                hotkey.Disable();
                hotkey.Hotkey.SetHandle(handle);
                hotkey.Enable();
            }
        }

        /// <summary>
        /// Saves the hotkeys from the dictionary into the repository. (JSON or XML file)
        /// </summary>
        private void saveHotkeys()
        {
            repository.Save();
        }

        /// <summary>
        /// Updates the dictionary, adds new and deletes old if key changes,
        /// or updates hotkey values if key mantains the same.
        /// </summary>
        /// <param name="newHotkey">A new <see cref="HotkeyAssociation"/> returned from the hotkey creation/editing dialog.</param>
        /// <param name="oldHotkey">The original <see cref="HotkeyAssociation"/> if edited, null if a new one was created.</param>
        /// <returns></returns>
        public bool AddOrUpdateHotkey(HotkeyAssociation newHotkey, HotkeyAssociation oldHotkey)
        {
            if (newHotkey.Hotkey.Invalid)
                throw new ArgumentException("newHotkey", "The Hotkey for the Association has to be valid.");
            if (oldHotkey != null && oldHotkey.Hotkey.Invalid)
                throw new ArgumentException("oldHotkey", "The Hotkey for the Association has to be valid.");

            // If the call was for Edit
            if (oldHotkey != null)
            {
                if (newHotkey.GetHashCode() == oldHotkey.GetHashCode())
                {
                    UpdateHotkey(oldHotkey, newHotkey);
                    return true;
                }
                else
                {
                    if (Hotkeys.ContainsKey(oldHotkey.GetHashCode()))
                    {
                        RemoveHotkey(oldHotkey);
                    }
                    AddHotkey(newHotkey);
                    return true;
                }
            }
            
            // New hotkey made
            if(!Hotkeys.ContainsKey(newHotkey.GetHashCode()))
            {
                AddHotkey(newHotkey);
                return true;
            }
            return false;
        }

        public void RemoveHotkey(HotkeyAssociation oldHotkey)
        {
            oldHotkey.Disable();
            Hotkeys.Remove(oldHotkey.GetHashCode());
            repository.Remove(oldHotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldHotkey));
        }

        public void UpdateHotkey(HotkeyAssociation oldHotkey, HotkeyAssociation newHotkey)
        {
            // Swap the oldHotkey data with the new one, maintaining the key.
            Hotkeys[oldHotkey.GetHashCode()].Swap(newHotkey);
            repository.Update(oldHotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newHotkey, oldHotkey));
        }

        public void AddHotkey(HotkeyAssociation hotkey)
        {
            hotkey.Hotkey.Reload(WindowHandle);
            Hotkeys.Add(hotkey.GetHashCode(), hotkey);
            repository.Add(hotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, hotkey));
        }

        /// <summary>
        /// Create a process with the hotkey program and parameters.
        /// </summary>
        /// <param name="hotkey">A <see cref="HotkeyAssociation"/> got from the call to the hotkey bind.</param>
        internal void Execute(HotkeyAssociation hotkey)
        {
            System.Diagnostics.Process.Start(hotkey.FilePath, hotkey.Parameters);
        }


        /// <summary>
        /// Save the hotkeys to a json/xml backup file.
        /// </summary>
        /// <param name="path">Destiny of the backup xml/json hotkey data file.</param>
        internal void ExportHotkeys(string path)
        {
            using (var newRepository = new HotkeyRepository(new JSONHotkeyContext(path)))
            {
                newRepository.CopyFrom(repository);
                newRepository.Save();
            }
        }

        /// <summary>
        /// Import a json/xml hotkey backup file.
        /// </summary>
        /// <param name="path">Path to the json/xml formated hotkey data file.</param>
        internal void ImportHotkeys(string path)
        {
            using (var newRepository = new HotkeyRepository(new JSONHotkeyContext(path)))
            {
                var hotkeys = newRepository.GetAll();
                DisableHotkeys();

                repository.ImportFrom(hotkeys);
            }
            Hotkeys = LoadFromRepository();
            EnableHotkeys();
        }

        /// <summary>
        /// Change the associated window handle for each hotkey.
        /// </summary>
        /// <param name="hotkeys">A List of <see cref="HotkeyAssociation"/>.</param>
        /// <param name="WindowHandle">The current window handle.</param>
        private void SetHotkeyHandles(List<HotkeyAssociation> hotkeys, IntPtr WindowHandle)
        {
            foreach(var hk in hotkeys)
            {
                hk.Hotkey.SetHandle(WindowHandle);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }


        internal void Load()
        {
            LoadFromRepository();
        }
    }

}
