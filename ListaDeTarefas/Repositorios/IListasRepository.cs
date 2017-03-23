using System;
using System.Collections.Generic;

namespace ListaDeTarefas.Models
{
    public interface IListasRepository: IDisposable
    {
        IEnumerable<Lista> Listas();
        Lista Details(int? id);
        void Create(Lista entidade);
        void Edit(Lista entidade);
        void Delete(Lista entidade);
    }
}
