using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ListaDeTarefas.Models
{
    public class TarefasInicializador : DropCreateDatabaseIfModelChanges<TarefaContexto>
    {
        protected override void Seed(TarefaContexto contexto)
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Email = "EmailInicializador@EmailInicializador.com.br", Ativo = true, Senha = "xxx"}
            };

            usuarios.ForEach(x => contexto.Usuarios.Add(x));
            //contexto.SaveChanges();

            var tarefas = new List<Tarefa>
            {
                new Tarefa { Nome = "Tarefa 1 do Inicializador", Ativa = true, Concluida = false },
                new Tarefa { Nome = "Tarefa 2 do Inicializador", Ativa = true, Concluida = false }
            };

            tarefas.ForEach(x => contexto.Tarefas.Add(x));
            //contexto.SaveChanges();

            //base.Seed(contexto);
            var listas = new List<Lista>
            {
                new Lista { Nome = "Lista 1 de Tarefas do Inicializador", Ativa = true}
            };

            listas.ForEach(x => contexto.Listas.Add(x));
            contexto.SaveChanges();
        }
    }
}