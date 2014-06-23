using NHkey.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    public class HotkeyRepository : IRepository<Model.HotkeyAssociation>, IDisposable
    {
        private readonly IContext<Model.HotkeyAssociation> context;

        public HotkeyRepository(IContext<Model.HotkeyAssociation> ncontext)
        {
            context = ncontext;
            Load();
        }

        public void Add(Model.HotkeyAssociation element)
        {
            if (!context.Collection.Contains(element))
            {
                context.Add(element);
            }
            else
            {
                context.Update(element);
            }
        }

        public void Remove(Model.HotkeyAssociation element)
        {
            context.Remove(element);
        }

        public void Load()
        {
            context.Load();
        }

        public void Save()
        {
            context.Save();
        }

        public List<Model.HotkeyAssociation> GetAll()
        {
            return context.GetAll().ToList<Model.HotkeyAssociation>();
        }

        public void Dispose()
        {
            context.Save();
        }

        public void Update(Model.HotkeyAssociation element)
        {
            context.Update(element);
        }

        /// <summary>
        /// Get <see cref="=HotkeyAssociation"s from another repository./>
        /// </summary>
        /// <param name="repository">A <see cref="=HotkeyRepository"/> to copy hotkeys from.</param>
        internal void CopyFrom(HotkeyRepository repository)
        {
            context.Add(repository.GetAll());
        }

        internal void ImportFrom(List<Model.HotkeyAssociation> readHotkeys)
        {
            var allHotkeys = context.GetAll() as List<HotkeyAssociation>;

            // If we had no hotkeys import all
            if (allHotkeys.Count == 0 && readHotkeys.Count > 0)
            {
                context.Add(readHotkeys);
            }
            else
            {
                UpdateImportedHotkeys(readHotkeys, allHotkeys);
            }
        }

        /// <summary>
        /// Compares current hotkeys in the respository with the ones imported from a xml/json file.
        ///
        /// They are differenced by name. ( Using the hash code would do things unnecessarily complex
        /// (ej: changes in handles affecting the hash) )
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
                        context.Update(updated);
                    }
                }
                // If no key matches by key plus values neither by only key, then it's a new one. Add it to repository.
                else
                {
                    context.Add(readHotkeys[i]);
                }

            }
        }
    }
}
