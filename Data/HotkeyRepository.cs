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
    }
}
