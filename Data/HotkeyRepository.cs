using NHkey.Model;
using System;
using System.Collections.Generic;
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
        }

        public void Add(Model.HotkeyAssociation element)
        {
            if (!context.Collection.Contains(element))
                context.Add(element);
            else
                context.Update(element);
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

        internal void CopyFrom(HotkeyRepository repository)
        {
            context.Add(repository.GetAll());
        }

        internal void ImportFrom(List<Model.HotkeyAssociation> list)
        {
            var all = context.GetAll();
            
            if (all.Count == 0)
            {
                context.Add(list);
                return;
            }

            var equalComparer = new HotkeyComparer(HotkeyComparer.Mode.Equal);
            var keyComparer = new HotkeyComparer(HotkeyComparer.Mode.DissectByValues);

            // Add new hotkeys
            List<HotkeyAssociation> newOnes = new List<HotkeyAssociation>();
            for (int i = 0; i < list.Count; i++ )
            {
                if (all.Contains(list[i], keyComparer) && ! all.Contains(list[i], equalComparer))
                {
                    HotkeyAssociation update = null;
                    try
                    {
                        update = list.Single((hotkey) => list[i].GetHashCode() == hotkey.GetHashCode() && !list[i].Equals(hotkey));
                    }
                    catch (InvalidOperationException opx)
                    { }

                    if (update != null)
                    {
                        update.Swap(list[i]);
                        context.Update(update);
                    }
                }
                else if (!all.Contains(list[i], equalComparer) && !all.Contains(list[i], keyComparer))
                {
                    context.Add(list[i]);
                }
                list.Remove(list[i]);
            }
        }
    }

    public sealed class HotkeyComparer : IEqualityComparer<HotkeyAssociation>
    {
        public enum Mode 
        { 
            Equal, 
            DissectByValues  // Base on the key hash, avoid comparing values like FileName, Name and Parameters
        }

        public Mode SetMode { get; protected set; }

        public HotkeyComparer(Mode mode)
        {
            SetMode = mode;
        }

        public bool Equals(HotkeyAssociation x, HotkeyAssociation y)
        {
            if (SetMode == Mode.Equal)
                return x.Equals(y);
            else if (SetMode == Mode.DissectByValues)
                return !x.Equals(y) && x.GetHashCode() == y.GetHashCode();
            else
                throw new ArgumentException("Invalid mode.");
        }

        public int GetHashCode(HotkeyAssociation obj)
        {
            return obj.GetHashCode();
        }
    }
}
