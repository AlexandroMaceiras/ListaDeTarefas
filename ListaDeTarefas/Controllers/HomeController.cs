using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ListaDeTarefas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Criando um MVC 5 com Entity Framework 6.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "alexandro@maceiras.com.br";

            return View();
        }
    }
}