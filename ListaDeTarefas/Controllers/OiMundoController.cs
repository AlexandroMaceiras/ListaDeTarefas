using ListaDeTarefas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ListaDeTarefas.Controllers
{
    //Decoração para controlar as variavei de sessao - Não ornigatória!
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class OiMundoController : Controller
    {
        //
        // GET: /OiMundo/
        //public string Index(int id, string nome)
        //{
        //    return "Oi mundo MVC ! <br/> <b> Este é " + nome + " - id->" + id + "<b/>";

        //}
        private TarefaContexto _db = new TarefaContexto();

        //Decoração para poder controlar os erros
        [HandleError]
        public ActionResult Index()
        {
            // Uando variaveis de sessão
            //Session.Add("nome", "Alexandro");
            
            //if(Session["nome"] != null)
            //{
            //    return Content(Session["nome"].ToString());
            //}

            //return RedirectToAction("Index");
            
            return View(_db.Listas.ToList());
        }

        [HandleError(View = "Error", ExceptionType = typeof(InvalidOperationException))]
        public ActionResult Teste()
        {
            //Usando ERROS:
            //Esse throw abaixo repassa o processo para a Exception. E para não parar aqui no throw deve estar em Release. Para isto usar Control + F5 
            //e/ou web.config setado com <compilation debug="false"... e escolher a segunda opção na hora de rodar.
            //throw new InvalidOperationException();
            return View();
        }
        public string BemVindo()
        {
            return "Seja bem vindo";
        }
	}
}