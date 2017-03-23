using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

namespace ListaDeTarefas.Models
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Find(params object[] key);
        void Create(T entidade);
        void Atualizar(T entidade); //, Expression<Func<X, bool>> predicate) where X : class, IEntityWithKey;
        void Delete(Func<T, bool> predicate);
        void SalvarMudanca();
        void Adicionar(T entidade);
    }
}
