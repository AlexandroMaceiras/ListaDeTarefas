using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ListaDeTarefas.Models
{
    public class TarefaContexto: DbContext
    {
        public TarefaContexto() : base("TarefasContexto"){
            // Construtor que herda o contrutor base da DBContext e passa o nome da String de Conexão do BD contida no 
            //WEB.CONFIG para o contrutor base DbContext(string de conexão) da DbContext.
        }

        //incluindo as entidades do modelo
        public DbSet<Lista> Listas { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Especificando configurações durante a criação do modelo de bd.
            //Neste caso o comando está Removendo o PluralizingTableNameConvention 
            //(Deixando as tabelas no Singular, sem S no final do nome delas).

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}