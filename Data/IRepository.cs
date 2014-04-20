﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    public interface IRepository<T>
    {

        void Add(T element);
        void Remove(T element);
        void Load();
        void Save();

        ICollection<T> GetAll();
    }
}
