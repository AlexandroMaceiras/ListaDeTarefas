using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ListaDeTarefas.Models
{
    public class ListaViewModel2
    {
        public string Introducao { get; set; }
        public List<Lista> Listas { get; set; }
    }
}