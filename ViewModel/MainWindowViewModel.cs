using GalaSoft.MvvmLight.Command;
using NHkey.Data;
using NHkey.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NHkey.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Dictionary<int, HotkeyAssociation> Hotkeys { get; protected set; }
        
        public static IntPtr WindowHandle;

        private HotkeyRepository repository;

        private static string SaveFilePath = Directory.GetCurrentDirectory() + "\\" + "hotkeys.data";

        public Action Close { get; protected set; }

        public MainWindowViewModel()
        {
            repository = new HotkeyRepository(new JSONHotkeyContext(SaveFilePath));
            Hotkeys = repository.GetAll().ToDictionary<HotkeyAssociation, int>((hk) => hk.Hotkey.Id);

            // Actions
            Close += new Action(() => saveHotkeys());
        }

        public void DisableHotkeys()
        {
            foreach(var hotkey in Hotkeys.Values.ToList())
            {
                hotkey.Hotkey.Unregister();
            }
        }

        public void EnableHotkeys()
        {
            foreach (var hotkey in Hotkeys.Values.ToList())
            {
                hotkey.Hotkey.Register();
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

            // Register hotkeys with new window handle and re arrange the dictionary.
            for(int i=0; i < Hotkeys.Count; i++)
            {
                var hotkey = old[i];
                
                // Remove from dictionary the reference to hotkey with old window handle
                Hotkeys.Remove(hotkey.GetHashCode());

                hotkey.Hotkey.Reload(handle); // This will change the hashCode

                // Re add after updating handle for hotkey
                Hotkeys.Add(hotkey.GetHashCode(), hotkey);
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
            Hotkeys[oldHotkey.GetHashCode()].Hotkey.Unregister();
            Hotkeys.Remove(oldHotkey.GetHashCode());
            repository.Remove(oldHotkey);
        }

        public void UpdateHotkey(HotkeyAssociation oldHotkey, HotkeyAssociation newHotkey)
        {
            // Swap the oldHotkey data with the new one, maintaining the key.
            Hotkeys[oldHotkey.GetHashCode()].Swap(newHotkey);
            repository.Update(oldHotkey);
        }

        public void AddHotkey(HotkeyAssociation hotkey)
        {
            hotkey.Hotkey.Reload(WindowHandle);
            Hotkeys.Add(hotkey.GetHashCode(), hotkey);
            repository.Add(hotkey);
        }

        /// <summary>
        /// Create a process with the hotkey program and parameters.
        /// </summary>
        /// <param name="hotkey">A <see cref="HotkeyAssociation"/> got from the call to the hotkey bind.</param>
        internal void Execute(HotkeyAssociation hotkey)
        {
            System.Diagnostics.Process.Start(hotkey.FilePath, hotkey.Parameters);
        }

    }

}
