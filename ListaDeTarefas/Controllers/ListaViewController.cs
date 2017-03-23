using ListaDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ListaDeTarefas.Controllers
{
    public class ListaViewController : Controller
    {
        private TarefaContexto _db = new TarefaContexto();

        public ActionResult Index()
        {
            // Este ActionResult é chamado por uma view sem vinculo com EF pois o modelo dele não tem
            //vinculo com o EF devido a ele ter sido criado como uma classe e não como uma entidade.
            //Este modelo não faz parte do contexto mesmo estando no mesmo arquivo dos outros modelos que tem vinculos
            // com o EF no contexto.

            var query = from T in _db.Tarefas 
                        where T.Ativa == true 
                        group T by T.ListaId into g
                        select (g.Select(m => m.Ativa)).Count();

                                                

            ListaViewModel listaViewModel = new ListaViewModel();

            listaViewModel.TarefasAtivas = query.ToList();

            listaViewModel.Introducao = "Bem Vindo";
            listaViewModel.Listas = _db.Listas.ToList();
            
            return View(listaViewModel);
        }

        public ActionResult OutroListaView()
        {
            // Este ActionResult é chamado por uma view sem vinculo com EF pois o modelo dele não tem
            //vinculo com o EF devido a ele ter sido criado como uma classe e não como uma entidade.
            //Este modelo não faz parte do contexto e nem está no mesmo arquivo dos outros modelos 
            //provando que isto não é necessário.
            ListaViewModel2 listaViewModel2 = new ListaViewModel2();
            listaViewModel2.Introducao = "Todas as Listas Existentes";
            listaViewModel2.Listas = _db.Listas.ToList();

            return View(listaViewModel2);
        }

        public ActionResult BuscarLista(string termo)
        {
            ListaViewModel2 listaViewModel2 = new ListaViewModel2();
            if (termo != "")
            {
                listaViewModel2.Introducao = "Resultados de Busca";
                listaViewModel2.Listas = _db.Listas.Where(x => x.Nome.Contains(termo)).ToList();
            }
            else
            {
                listaViewModel2.Introducao = "Todas as Listas Existentes";
                listaViewModel2.Listas = _db.Listas.ToList();
            }
            return View("OutroListaView", listaViewModel2);
        }
	}
}