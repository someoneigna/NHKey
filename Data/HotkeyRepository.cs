using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    public class HotkeyRepository : IRepository<Model.Hotkey>
    {
        private readonly IContext<Model.Hotkey> context;

        public HotkeyRepository(IContext<Model.Hotkey> ncontext)
        {
            context = ncontext;
        }

        public void Add(Model.Hotkey element)
        {
            context.Add(element);
        }

        public void Remove(Model.Hotkey element)
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

        public ICollection<Model.Hotkey> GetAll()
        {
            return context.GetAll();
        }
    }
}
