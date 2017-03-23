using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ListaDeTarefas.Models
{
    public class ListaViewModel
    {
        public string Introducao { get; set; }
        public List<Lista> Listas { get; set; }
        public IEnumerable TarefasAtivas { get; set; } 
    }

    public class Lista
    {
        [Key]
        public int ListaId { get; set; }
        [Required]
        public string Nome { get; set; }
        public bool Ativa { get; set; }
        public virtual Usuario Usuario { get; set; } // Sem o Virtual o nome do usuário não aparece no Delete da Lista
        public int UsuarioId { get; set; }
        public virtual ICollection<Tarefa> Tarefas { get; set; } // Sem o virtual a quantia de tarefas não aparece na lista.
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage="Data em formato inválido")]
        public DateTime? Prazo { get; set; }
        public IEnumerable TarefasAtivas { get; set; } 
    }

    public class Tarefa
    {
        [Key]
        public int TarefaId { get; set; }
        public string Nome { get; set; }
        //[Column("Terminada")] Isto pode colocar este nome físico no BD diferente do nome usado no sistema
        public bool Concluida { get; set; }
        public bool Ativa { get; set; }
        public virtual Lista Lista { get; set; }
        public int ListaId { get; set; }
    }

    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }        
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        [Required]
        public bool Ativo { get; set; }
        public virtual ICollection<Lista> Lista { get; set; }
    }
}