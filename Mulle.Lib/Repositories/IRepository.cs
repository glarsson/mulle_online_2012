using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mulle.Lib.Entities;

namespace Mulle.Lib.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        void Add(TEntity entity);
        void Remove(TEntity entity);
        void Save();
        TEntity Get(int id);
        IQueryable<TEntity> Items { get; }
    }
}
