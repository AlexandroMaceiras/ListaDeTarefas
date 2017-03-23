using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ListaDeTarefas.Models
{
    public abstract class GenericRepository<T> : IDisposable, IGenericRepository<T> where T : class
    {
        //private bool disposed = false;

        private TarefaContexto _db = new TarefaContexto();

        public void Dispose()
        {
            _db.Dispose();
        }

        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public T Find(params object[] key)
        {
            //Irá aplicar um filtro pela chave primária da classe em si.Quem é a chave primária? Não importa porque 
            //o Find recebe um array de object e efetua a pesquisa.Com isto, se você tiver uma chave composta, o Find 
            //se encarrega de tudo, basta você passar os dados e pronto.
            return _db.Set<T>().Find(key);
        }

        public void Create(T entidade)
        {
            _db.Set<T>().Add(entidade);
            this.SalvarMudanca();
        }

        public void Atualizar(T entidade)
        {
            _db.Entry(entidade).State = EntityState.Modified;
        }

        public void Delete(Func<T, bool> predicate)
        {
            //_db.Set<T>().Remove(entidade);

           // Não funciona reclamando de que deve ser usado com valores primarios tipo Id do objeto, não sei se tem outro jeito.
            _db.Set<T>()                                //Traz o banco de acordo com o tipo.
                .Where(predicate).ToList()              //Lambda procurando o predicate que pode ser vários por isto por em uma lista.
                .ForEach(x => _db.Set<T>().Remove(x));  //Lambda removendo cada um da lista x de dentro do banco.
        }
        public void SalvarMudanca()
        {
            _db.SaveChanges();
        }
        public void Adicionar(T entidade)
        {
            _db.Set<T>().Add(entidade);
        }
    }
}
