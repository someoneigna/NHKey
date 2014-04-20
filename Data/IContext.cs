using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    public interface IContext<T>
    {
        void Add(T entity);
        
        void Remove(int ID);
        void Remove(T entity);
        void Update(T entity);

        ICollection<T> GetAll();

        void Load();
        void Save();

        IQueryable<T> Collection { get; }
    }
}
