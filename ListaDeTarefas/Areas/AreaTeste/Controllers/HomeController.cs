using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ListaDeTarefas.Areas.AreaTeste.Controllers
{
    public class HomeController : Controller
    {
        // GET: AreaTeste/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}