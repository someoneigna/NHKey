using NHkey.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    /// <summary>
    /// Handles persistance of <see cref="HotkeyAssociation"/>s
    /// through a <see cref="JSONHotkeyContext"/> or <see cref="XMLHotkeyContext"/>
    /// </summary>
    public class HotkeyRepository : IRepository<Model.HotkeyAssociation>, IDisposable
    {
        private readonly IContext<HotkeyAssociation> dataSource;

        /// <summary>
        /// Creates a HotkeyRepository with
        /// <paramref name="dataContext"/> as data
        /// source for the hotkeys.
        /// </summary>
        /// <param name="ncontext"></param>
        public HotkeyRepository(IContext<HotkeyAssociation> dataContext)
        {
            dataSource = dataContext;
            Load();
        }

        public void Add(Model.HotkeyAssociation element)
        {
            if (!dataSource.Collection.Contains(element))
            {
                dataSource.Add(element);
            }
            else
            {
                dataSource.Update(element);
            }
        }

        public void Remove(Model.HotkeyAssociation element)
        {
            dataSource.Remove(element);
        }

        public void Load()
        {
            dataSource.Load();
        }

        public void Save()
        {
            dataSource.Save();
        }

        public List<Model.HotkeyAssociation> GetAll()
        {
            return dataSource.GetAll().ToList<Model.HotkeyAssociation>();
        }

        public void Dispose()
        {
            dataSource.Save();
        }

        public void Update(Model.HotkeyAssociation element)
        {
            dataSource.Update(element);
        }

        /// <summary>
        /// Get <see cref="=HotkeyAssociation"s from another repository./>
        /// </summary>
        /// <param name="repository">A <see cref="=HotkeyRepository"/> to copy hotkeys from.</param>
        internal void CopyFrom(HotkeyRepository repository)
        {
            dataSource.Add(repository.GetAll());
        }

        internal void ImportFrom(List<Model.HotkeyAssociation> readHotkeys)
        {
            var allHotkeys = dataSource.GetAll() as List<HotkeyAssociation>;

            // If we had no hotkeys import all
            if (allHotkeys.Count == 0 && readHotkeys.Count > 0)
            {
                dataSource.Add(readHotkeys);
            }
            else
            {
                UpdateImportedHotkeys(readHotkeys, allHotkeys);
            }
        }

        /// <summary>
        /// Compares current hotkeys in the respository with the ones imported from a xml/json file.
        /// They are differenced by name.
        /// </summary>
        /// <param name="readHotkeys">The <see cref="HotkeyAssociation"/>s read from a file.</param>
        /// <param name="currentHotkeys">The <see cref="HotkeyAssociation"/>s currently in the repository.</param>
        private void UpdateImportedHotkeys(List<HotkeyAssociation> readHotkeys, List<HotkeyAssociation> currentHotkeys)
        {

            for (int i = 0; i < readHotkeys.Count; i++)
            {
                HotkeyAssociation updated = currentHotkeys.Find((hk) => hk.GetHashCode() == readHotkeys[i].GetHashCode());

                // If a hotkey with same key found
                if (updated != null)
                {
                    // If has different values (filepath, parameters or name)
                    if (!updated.Equals(readHotkeys[i]))
                    {
                        // Update the old hotkey with the new values
                        updated.Swap(readHotkeys[i]);
                        dataSource.Update(updated);
                    }
                }
                // If no key matches by key plus values neither by only key, then it's a new one. Add it to repository.
                else
                {
                    dataSource.Add(readHotkeys[i]);
                }

            }
        }

        /// <summary>
        /// Returns if the keybind exists for another hotkey.
        /// </summary>
        /// <param name="element">A <see cref="HotkeyAssociation"/></param>
        /// <returns>If the key combination of the hotkey already is used.</returns>
        public bool Exists(HotkeyAssociation element)
        {
            return dataSource.Collection.Any(hk => hk.Hotkey.Equals(element.Hotkey) && hk != element);
        }
    }
}
