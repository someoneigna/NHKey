using GalaSoft.MvvmLight.Command;
using NHkey.Data;
using NHkey.Helpers;
using NHkey.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private Dictionary<int, HotkeyAssociation> _hotkeys;

        /// <summary>
        /// Contains a Dictionary of keybind codes, and <see cref="HotkeyAssociation"/>s
        /// </summary>
        /// <value>Sets and gets the value of _hotkeys</value>
        public Dictionary<int, HotkeyAssociation> Hotkeys
        { 
            get { return _hotkeys; } 
            protected set { _hotkeys = value; OnPropertyChanged("Hotkeys"); } 
        }

        public static IntPtr WindowHandle;

        private HotkeyRepository Repository = App.Instance.Repository;

        private static string HotkeyDataSaveFilepath = Directory.GetCurrentDirectory() + "\\" + "hotkeys.data";

        public Options CurrentOptions { get; set; }
        public Action Close { get; protected set; }

        public MainWindowViewModel()
        {
            Hotkeys = LoadFromRepository();

            CurrentOptions = new Options();

            // Actions
            Close += new Action(() => SaveAndExit());
        }

        /// <summary>
        /// Gets the <see cref="HotkeyAssociation"/> elements from
        /// the <see cref="HotkeyRespository"/> and returns a
        /// Dictionary with the key combination as the key for each <see cref="Hotkey"/>
        /// </summary>
        private Dictionary<int, HotkeyAssociation> LoadFromRepository()
        {
            var hotkeyDict = App.Instance.Repository.GetAll().ToDictionary<HotkeyAssociation, int>((hk) => hk.Hotkey.Id);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, hotkeyDict.ToList()));

            return hotkeyDict;
        }

        /// <summary>
        /// Saves options and the hotkeys.
        /// </summary>
        private void SaveAndExit()
        {
            saveHotkeys();
            CurrentOptions.Save();
        }

        /// <summary>
        /// For each hotkey key in <see cref="Hotkeys"/> dictionary,
        /// disables the key hook.
        /// </summary>
        public void DisableHotkeys()
        {
            Hotkeys.Values.ToList().ForEach(hk => hk.Disable());
        }

        /// <summary>
        /// For each hotkey key in <see cref="Hotkeys"/> dictionary,
        /// disables the key hook.
        /// </summary>
        public void EnableHotkeys()
        {
            Hotkeys.Values.ToList().ForEach(hk =>
            {
                if (hk.Hotkey.Handle == IntPtr.Zero)
                {
                    hk.Hotkey.SetHandle(WindowHandle);
                }
                hk.Enable();
            });
        }

        /// <summary>
        /// Update the hotkey associations with the current window handle.
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
            App.Instance.Repository.Save();
        }

        /// <summary>
        /// Checks the program for hotkeys that still exist and marks their
        /// name if they dont.
        /// </summary>
        /// <param name="orphanedHotkeyLabel">The string to append to no longer valid hotkeys.</param>
        public void MarkOrphanedHotkeys(string orphanedHotkeyLabel)
        {
            foreach (var hotkey in Hotkeys.Values)
            {
                if (!File.Exists(hotkey.FilePath))
                {
                    // Then it's invalid (orphaned)
                    hotkey.Orphaned = true;
                }
            }
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
            if (newHotkey.Invalid)
            {
                throw new ArgumentException("newHotkey", "The Hotkey for the Association has to be valid.");
            }

            if (oldHotkey != null && oldHotkey.Invalid)
            {
                throw new ArgumentException("oldHotkey", "The Hotkey for the Association has to be valid.");
            }

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

        /// <summary>
        /// Removes a <see cref="HotkeyAssociation"/> from the dictionary.
        /// <para>Disables <paramref name="oldhotkey"/>, removes it from Hotkey dictionary
        /// and removes it from repository.</para>
        /// </summary>
        /// <param name="oldHotkey"></param>
        public void RemoveHotkey(HotkeyAssociation oldHotkey)
        {
            oldHotkey.Disable();
            Hotkeys.Remove(oldHotkey.GetHashCode());
            Repository.Remove(oldHotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldHotkey));
        }

        /// <summary>
        /// Replaces a hotkey on the dictionary and repository.
        /// </summary>
        /// <param name="oldHotkey">The hotkey before modification.</param>
        /// <param name="newHotkey">A copy of the hoktye with the modifications.</param>
        public void UpdateHotkey(HotkeyAssociation oldHotkey, HotkeyAssociation newHotkey)
        {
            // Swap the oldHotkey data with the new one, maintaining the key.
            Hotkeys[oldHotkey.GetHashCode()].Swap(newHotkey);
            Repository.Update(oldHotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newHotkey, oldHotkey));
        }

        /// <summary>
        /// Adds a new hotkey to the dictionary and the repository.
        /// </summary>
        /// <param name="hotkey">The <see cref="HotkeyAssociation"/> to add.</param>
        public void AddHotkey(HotkeyAssociation hotkey)
        {
            hotkey.Hotkey.Reload(WindowHandle);
            Hotkeys.Add(hotkey.GetHashCode(), hotkey);
            Repository.Add(hotkey);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, hotkey));
        }

        /// <summary>
        /// Create a process with the hotkey program and parameters.
        /// </summary>
        /// <param name="hotkey">A <see cref="HotkeyAssociation"/> got from the call to the hotkey bind.</param>
        internal void Execute(HotkeyAssociation hotkey)
        {
            try
            {
                if (hotkey.Enabled)
                {
                    System.Diagnostics.Process.Start(hotkey.FilePath, hotkey.Parameters);
                }
            }
            catch(Win32Exception win32ex)
            {
                App.Instance.Log.Append("MainWindowModel.Execute", "Failed to execute hotkey: " + win32ex.Message);
            }
        }

        /// <summary>
        /// Save the hotkeys to a json/xml backup file.
        /// </summary>
        /// <param name="path">Destiny of the backup xml/json hotkey data file.</param>
        internal void ExportHotkeys(string path)
        {
            using (var newRepository = new HotkeyRepository(new JSONHotkeyContext(path)))
            {
                newRepository.CopyFrom(Repository);
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

                Repository.ImportFrom(hotkeys);
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
    }
}
