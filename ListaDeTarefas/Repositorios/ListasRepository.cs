using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ListaDeTarefas.Models
{
    public class ListasRepository : IListasRepository
    {
        //Assim é tendo um construtor.
        private TarefaContexto _db;

        //Assim é sem precisar de um contrutor.
        //private TarefaContexto _db = new TarefaContexto();
        
        private bool disposed = false;

        public ListasRepository(TarefaContexto db)
        {
            _db = db;
        }

        public void Create(Lista entidade)
        {
            if(entidade != null)
            {
                _db.Listas.Add(entidade);
                _db.SaveChanges();
            }
        }

        public void Delete(Lista entidade)
        {
            if (entidade != null)
            {
                var id = Details(entidade.ListaId);
                if (id != null)
                {
                    _db.Listas.Remove(id);
                    _db.SaveChanges();
                }
            }
        }

        public Lista Details(int? id)
        {
            var lista = _db.Listas.Find(id);
            return lista;
        }


        public void Edit(Lista entidade)
        {
            if(entidade != null)
            {
                _db.Entry(entidade).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public IEnumerable<Lista> Listas()
        {
            var listas = _db.Listas.AsParallel().ToList();
            return listas;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_db != null)
                    {
                        _db.Dispose();
                        _db = null;
                    }
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Destrutor. O destrutor é chamado quando a CLR verifica que não existem mais referências para o objeto, e então vai chamá-lo.
        /// </summary>
        ~ListasRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
