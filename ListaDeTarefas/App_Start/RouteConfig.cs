using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ListaDeTarefas
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "ListaGeneric", action = "Index", id = UrlParameter.Optional }
            // Esse namespace é definido para o uso da AREAS como por exemplo o AreaTeste e nele poder ter nomes 
            //de controllers que possam ser iguais aos da area principal.
            , namespaces: new[] { "ListaDeTarefas.Controllers" }
            );
        }
    }
}
