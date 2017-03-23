using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ListaDeTarefas.Models;
using System.Text;
using ListaDeTarefas.Repositorios;

namespace ListaDeTarefas.Controllers
{
    //[OutputCache(Duration = 60, VaryByParam = "none")]  //Serve pra conservar o cache 60 segundos na memória
    public class ListaController : Controller
    {

        //Aqui estou usando um ListasRepository só para o model listas, se quiser usar pros outros models teria que criar mais
        //um repositorio e uma interface pra cada (Tarefa e Usuario).

        private TarefaContexto _db = new TarefaContexto();
        int _quantPorPag = 3;

        private IListasRepository _listaRepositorio; //Cria objeto do tipo igual a Interface IListasRepository.

        // Assim eu não preciso de um construtor no ListaRepository
        //public ListaController() : this(new ListasRepository()) {} //Construtor que herda o outro contrutor passando uma nova instância ListaRepository para ele.
        //public ListaController(IListasRepository ListaRepositorio) //O outro construtor que recebe a instância ListaRepository como Interface IListaRepository.
        //{
        //    _ListaRepositorio = ListaRepositorio; //Objeto do tipo igual a Interface IListasRepository que vai chamar os métodos e recebe o valor da interface IListaRepository.
        //}

        //Assim eu preciso de um construtor no ListaRepository
        public ListaController()
        {
            _listaRepositorio = new ListasRepository(new TarefaContexto());
        }

        public void AtualizaTarefasAtivas()
        {
            // Este LINQ funcionou perfeitamente porque eu fiz ele sem NEW e assim consegui trazer só os valores do TarefasAtivas e assim incluir ele usando o foreach. 
            using (var transaction = _db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var query = (from s in _listaRepositorio.Listas()
                             join T in _db.Tarefas
                             on s.ListaId equals T.ListaId
                             where T.Ativa == true
                             group T by T.ListaId into g
                             select g).ToList();

                // Atualiza o db.Listas com os resultados da query
                foreach (var itemDoLista in _listaRepositorio.Listas())
                {
                    int flagNaoAchou = 0;
                    foreach (var item in query)
                    {
                        if (itemDoLista.ListaId == item.Key)
                        {
                            itemDoLista.TarefasAtivas = item.Count().ToString();
                            flagNaoAchou = 1;
                        }
                    }

                    if (flagNaoAchou == 0)
                    {
                        itemDoLista.TarefasAtivas = (0).ToString();
                    }
                }
                transaction.Commit();
            }
        }

        [Authorize, ActionName("Index"), HttpGet]
        public ActionResult Index1(string ordem)
        {
            IEnumerable<Lista> lista = _listaRepositorio.Listas();

            List<Lista> listas;
            List<Lista> proxima;

            AtualizaTarefasAtivas();

            ViewBag.UsuarioOrdem    = ordem == "Email" 
                                   || ordem == null     ? "Email_desc"  : "Email";
            ViewBag.AtivaOrdem      = ordem == "Ativa"  ? "Ativa_desc"  : "Ativa";
            ViewBag.NomeOrdem       = ordem == "Nome"   ? "Nome_desc"   : "Nome";
            ViewBag.PrazoOrdem      = ordem == "Prazo"  ? "Prazo_desc"  : "Prazo";
            ViewBag.TarefasOrdem    = ordem == "Tarefas"? "Tarefas_desc": "Tarefas";

            switch (ordem)
            {
                case "Email":
                    ViewBag.flagEmail = 1;
                    listas = _listaRepositorio.Listas().OrderBy(s => s.Usuario.Email).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderBy(s => s.Usuario.Email).Skip(_quantPorPag).OrderBy(s => s.Usuario.Email).Take(_quantPorPag).ToList();
                    break;
                case "Email_desc":
                    ViewBag.flagEmail = 0;
                    listas  = _listaRepositorio.Listas().OrderByDescending(s => s.Usuario.Email).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderByDescending(s => s.Usuario.Email).Skip(_quantPorPag).OrderByDescending(s => s.Usuario.Email).Take(_quantPorPag).ToList();
                    break;
                case "Ativa":
                    ViewBag.flagAtiva = 1;
                    listas  = _listaRepositorio.Listas().OrderBy(s => s.Ativa).OrderBy(s => s.ListaId).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderBy(s => s.Ativa).OrderBy(s => s.ListaId).Skip(_quantPorPag).OrderBy(s => s.Ativa).OrderBy(s => s.ListaId).Take(_quantPorPag).ToList();
                    break;
                case "Ativa_desc":
                    ViewBag.flagAtiva = 0;
                    listas  = _listaRepositorio.Listas().OrderByDescending(s => s.Ativa).OrderByDescending(s => s.ListaId).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderByDescending(s => s.Ativa).OrderByDescending(s => s.ListaId).Skip(_quantPorPag).OrderByDescending(s => s.Ativa).OrderByDescending(s => s.ListaId).Take(_quantPorPag).ToList();
                    break;
                case "Nome":
                    ViewBag.flagNome = 1;
                    listas  = _listaRepositorio.Listas().OrderBy(s => s.Nome).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderBy(s => s.Nome).Skip(_quantPorPag).OrderBy(s => s.Nome).Take(_quantPorPag).ToList();
                    break;
                case "Nome_desc":
                    ViewBag.flagNome = 0;
                    listas  = _listaRepositorio.Listas().OrderByDescending(s => s.Nome).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderByDescending(s => s.Nome).Skip(_quantPorPag).OrderByDescending(s => s.Nome).Take(_quantPorPag).ToList();
                    break;
                case "Prazo":
                    ViewBag.flagPrazo = 1;
                    listas  = _listaRepositorio.Listas().OrderBy(s => s.Prazo).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderBy(s => s.Prazo).Skip(_quantPorPag).OrderBy(s => s.Prazo).Take(_quantPorPag).ToList();
                    break;
                case "Prazo_desc":
                    ViewBag.flagPrazo = 0;
                    listas  = _listaRepositorio.Listas().OrderByDescending(s => s.Prazo).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderByDescending(s => s.Prazo).Skip(_quantPorPag).OrderByDescending(s => s.Prazo).Take(_quantPorPag).ToList();
                    break;
                default:
                    ViewBag.flagEmail = 1;
                    listas = _listaRepositorio.Listas().OrderBy(s => s.Usuario.Email).ThenByDescending(x => x.Nome).Take(_quantPorPag).ToList();
                    proxima = _listaRepositorio.Listas().OrderBy(s => s.Usuario.Email).Skip(_quantPorPag).OrderBy(s => s.Usuario.Email).Take(_quantPorPag).ToList();
                    break;
            }

            ViewBag.Ordem = ordem;
            ViewBag.Introducao = "Todas as Listas Existentes";
            ViewBag.PaginaAtual = 1;

            if (proxima.Count > 0) ViewBag.Proxima = true;
            else ViewBag.Proxima = false;

            ViewBag.Anterior = false;

            return View(listas);
        }

        [Authorize, ActionName("Index"), HttpPost]
        public ActionResult Index2(string termo)
        {
            //ListaViewModel2 listaViewModel2 = new ListaViewModel2();
            //listaViewModel2.Listas = db.Listas.Where(x => x.Nome.Contains(termo)).ToList();
            //return View("../ListaView/OutroListaView", listaViewModel2);

            var listas = _listaRepositorio.Listas().Where(x => x.Nome.Contains(termo)).ToList();
            ViewBag.Proxima = false;
            ViewBag.Anterior = false;

            if (termo != "")
            {
                if (termo != "") ViewBag.Introducao = "Resultados de Busca";
                else ViewBag.Introducao = "Todas as Listas Existentes";

                if (listas.Count() < 1)
                    ViewBag.Erro = termo;

                return View("Index", listas);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult IndexPagina(int pagina, string ordem)
        {
            AtualizaTarefasAtivas();

            ViewBag.UsuarioOrdem    = ordem == "Email"  ? "Email_desc"  : "Email";
            ViewBag.AtivaOrdem      = ordem == "Ativa"  ? "Ativa_desc"  : "Ativa";
            ViewBag.NomeOrdem       = ordem == "Nome"   ? "Nome_desc"   : "Nome";
            ViewBag.PrazoOrdem      = ordem == "Prazo"  ? "Prazo_desc"  : "Prazo";
            ViewBag.TarefasOrdem    = ordem == "Tarefas"? "Tarefas_desc": "Tarefas";

            ViewBag.Ordem = ordem;

            ViewBag.Introducao = "Todas as Listas Existentes";
            var listas = _listaRepositorio.Listas().ToList();
            var proxima = _listaRepositorio.Listas().ToList();

            if (ordem == "Email" || ordem == "" || ordem == null)
            {
                ViewBag.flagEmail = 1;
                listas  = _listaRepositorio.Listas().OrderBy(x => x.Usuario.Email).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Usuario.Email).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderBy(x => x.Usuario.Email).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Usuario.Email).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Email_desc")
            {
                ViewBag.flagEmail = 0;
                listas  = _listaRepositorio.Listas().OrderByDescending(x => x.Usuario.Email).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Usuario.Email).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderByDescending(x => x.Usuario.Email).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Usuario.Email).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Ativa")
            {
                ViewBag.flagAtiva = 1;
                listas  = _listaRepositorio.Listas().OrderBy(x => x.Ativa).OrderBy(s => s.ListaId).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Ativa).OrderBy(s => s.ListaId).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderBy(x => x.Ativa).OrderBy(s => s.ListaId).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Ativa).OrderBy(s => s.ListaId).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Ativa_desc")
            {
                ViewBag.flagAtiva = 0;
                listas  = _listaRepositorio.Listas().OrderByDescending(x => x.Ativa).OrderByDescending(s => s.ListaId).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Ativa).OrderByDescending(s => s.ListaId).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderByDescending(x => x.Ativa).OrderByDescending(s => s.ListaId).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Ativa).OrderByDescending(s => s.ListaId).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Nome")
            {
                ViewBag.flagNome = 1;
                listas  = _listaRepositorio.Listas().OrderBy(x => x.Nome).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Nome).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderBy(x => x.Nome).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Nome).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Nome_desc")
            {
                ViewBag.flagNome = 0;
                listas  = _listaRepositorio.Listas().OrderByDescending(x => x.Nome).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Nome).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderByDescending(x => x.Nome).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Nome).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Prazo")
            {
                ViewBag.flagPrazo = 1;
                listas  = _listaRepositorio.Listas().OrderBy(x => x.Prazo).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Prazo).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderBy(x => x.Prazo).Skip((pagina - 1) * _quantPorPag).OrderBy(x => x.Prazo).Take(_quantPorPag + 1).ToList();
            }
            if (ordem == "Prazo_desc")
            {
                ViewBag.flagPrazo = 0;
                listas  = _listaRepositorio.Listas().OrderByDescending(x => x.Prazo).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Prazo).Take(_quantPorPag).ToList();
                proxima = _listaRepositorio.Listas().OrderByDescending(x => x.Prazo).Skip((pagina - 1) * _quantPorPag).OrderByDescending(x => x.Prazo).Take(_quantPorPag + 1).ToList();
            }

            if (proxima.Count > _quantPorPag) ViewBag.Proxima = true;
            else ViewBag.Proxima = false;

            if (pagina > 1) ViewBag.Anterior = true;
            else ViewBag.Anterior = false;

            ViewBag.PaginaAtual = pagina;

            return View("Index", listas);
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lista lista = _listaRepositorio.Details(id);
            if (lista == null)
            {
                return HttpNotFound();
            }
            return View(lista);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email");
            return View();
        }

        public ActionResult QuantidadePorPagina()
        {
            return Content("Ítens por Página: " + _quantPorPag, "text/html", Encoding.UTF8);
        }

        public ActionResult Descer()
        {
            int x = 2;
            if (x == 1)
            {
                //Deveria servir para abrir um pdf se não tivesse o xxx.pdf ou para baixar um pdf usando o xxx.pdf, mas está dando pau!
                return File(@"C:\Users\Alexandro\Downloads\natural_basico.pdf", "application/pdf", "xxx.pdf");
            }
            else
            {
                //Aqui temos como retornar NADA quando não obtemos o resultado necessário. 
                //Em um método de controlador que tivesse somente um resultado nulo era só declarar ele como Void ao invés de ActionResult.
                return new EmptyResult();
            }
        }

        public JavaScriptResult Jsr()
        {
            return new JavaScriptResult()
            {
                Script = "alert('Rodando um JS através de um método de controlador! Só funciona quando recarrega com novod parâmetros a página.');"
            };
        }

        public ActionResult Redirect()
        {
            return Redirect("http://www.maceiras.com.br");
        }

        public ActionResult RetornaUmJson()
        {
            var modelJson = new List<object>()
            {
                new { CidadeId = 1, Nome = "São Paulo",      Estado = "SP" },
                new { CidadeId = 2, Nome = "Rio de Janeiro", Estado = "RJ" },
                new { CidadeId = 3, Nome = "curitiba",       Estado = "PR" },
                new { CidadeId = 4, Nome = "Salvador",       Estado = "BA" }
            };

            return Json(modelJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ListaId,Nome,Ativa,UsuarioId,Prazo")] Lista lista)
        {
            if (ModelState.IsValid)
            {
                _listaRepositorio.Create(lista);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UsuarioId = "";

            //Preenche o combo Usuário (Email) com os emails dos usuários ativos existêntes e mantem o que já havia sido selecionado.
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lista lista = _listaRepositorio.Details(id);
            if (lista == null)
            {
                return HttpNotFound();
            }
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }

        [ChildActionOnly]  //Restrito para ser chamado de dentro de outra View e não direto pela URL .../Lista/TestePart
        public ActionResult TestePart()
        {
            return PartialView("TestePartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ListaId,Nome,Ativa,UsuarioId,Prazo")] Lista lista)
        {
            if (ModelState.IsValid)
            {
                _listaRepositorio.Edit(lista);

                //Sem repositorio
                //db.Entry(lista).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Mudei o lambda para poder ser feita uma pesquisa do id incluindo o e-mail do usuário. 
            //Porque o usuário vem null. Depois eu mudei o Model colocando o usuário dentro da lista como virtual e não foi
            //mais necessário isto.
            //Lista lista = db.Listas.Include(u => u.Usuario).ToList().Find(l => l.ListaId == id);
            
            Lista lista = _listaRepositorio.Details(id);
            if (lista == null)
            {
                return HttpNotFound();
            }
            return View(lista);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lista lista = _listaRepositorio.Details(id);
            _listaRepositorio.Delete(lista);

            //Sem repositorio
            //Lista lista = db.Listas.Find(id);
            //db.Listas.Remove(lista);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Carrega uma view deste diretório sem a necessidade de exixstir um controller
        /// </summary>
        /// <param name="actionName">É só chamar ela por http://.../List/NomeDaView</param>
        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch(InvalidOperationException)
            {
                //Se não achar a View entra nesse erro procurando a View Erro
                this.View("Erro").ExecuteResult(this.ControllerContext);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private class Cidades
        {
            public int CidadeId { get; set; }
            public string Nome { get; set; }
            public string Estado { get; set; }
        }
    }
}
