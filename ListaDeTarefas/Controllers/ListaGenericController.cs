using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ListaDeTarefas.Models;
using System.Text;
using ListaDeTarefas.Repositorios;

namespace ListaDeTarefas.Controllers
{
    public class ListaGenericController : Controller
    {
        private readonly TarefaContexto _db = new TarefaContexto();
        private const int QuantPorPag = 3;

        private readonly ListaUmGenericRepository _listaGenericRepository = new ListaUmGenericRepository();
        private readonly TarefaUmGenericRepository _tarefaGenericRepository = new TarefaUmGenericRepository();

        public ListaGenericController()
        {
            AtualizaTarefasAtivas();
        }

        public void AtualizaTarefasAtivas()
        {
            // Este LINQ funcionou perfeitamente porque eu fiz ele sem NEW e assim consegui trazer só os valores do TarefasAtivas e assim incluir ele usando o foreach. 
            using (var transaction = _db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var query = (from s in _listaGenericRepository.GetAll()
                             join T in _tarefaGenericRepository.GetAll()
                             on s.ListaId equals T.ListaId
                             where T.Ativa == true
                             group T by T.ListaId into g
                             select g).ToList();

                // Atualiza o db.Listas com os resultados da query
                foreach (var itemDoLista in _listaGenericRepository.GetAll())
                {
                    var flagNaoAchou = 0;
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
            _listaGenericRepository.GetAll();

            List<Lista> listas;
            List<Lista> proxima;

            //AtualizaTarefasAtivas();

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
                    listas = _listaGenericRepository.GetAll().OrderBy(s => s.Usuario.Email).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderBy(s => s.Usuario.Email).Skip(QuantPorPag).OrderBy(s => s.Usuario.Email).Take(QuantPorPag).ToList();
                    break;
                case "Email_desc":
                    ViewBag.flagEmail = 0;
                    listas  = _listaGenericRepository.GetAll().OrderByDescending(s => s.Usuario.Email).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderByDescending(s => s.Usuario.Email).Skip(QuantPorPag).OrderByDescending(s => s.Usuario.Email).Take(QuantPorPag).ToList();
                    break;
                case "Ativa":
                    ViewBag.flagAtiva = 1;
                    listas  = _listaGenericRepository.GetAll().OrderBy(s => s.Ativa).ThenBy(s => s.ListaId).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderBy(s => s.Ativa).ThenBy(s => s.ListaId).Skip(QuantPorPag).OrderBy(s => s.Ativa).ThenBy(s => s.ListaId).Take(QuantPorPag).ToList();
                    break;
                case "Ativa_desc":
                    ViewBag.flagAtiva = 0;
                    listas  = _listaGenericRepository.GetAll().OrderByDescending(s => s.Ativa).ThenByDescending(s => s.ListaId).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderByDescending(s => s.Ativa).ThenByDescending(s => s.ListaId).Skip(QuantPorPag).OrderByDescending(s => s.Ativa).ThenByDescending(s => s.ListaId).Take(QuantPorPag).ToList();
                    break;
                case "Nome":
                    ViewBag.flagNome = 1;
                    listas  = _listaGenericRepository.GetAll().OrderBy(s => s.Nome).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderBy(s => s.Nome).Skip(QuantPorPag).OrderBy(s => s.Nome).Take(QuantPorPag).ToList();
                    break;
                case "Nome_desc":
                    ViewBag.flagNome = 0;
                    listas  = _listaGenericRepository.GetAll().OrderByDescending(s => s.Nome).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderByDescending(s => s.Nome).Skip(QuantPorPag).OrderByDescending(s => s.Nome).Take(QuantPorPag).ToList();
                    break;
                case "Prazo":
                    ViewBag.flagPrazo = 1;
                    listas  = _listaGenericRepository.GetAll().OrderBy(s => s.Prazo).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderBy(s => s.Prazo).Skip(QuantPorPag).OrderBy(s => s.Prazo).Take(QuantPorPag).ToList();
                    break;
                case "Prazo_desc":
                    ViewBag.flagPrazo = 0;
                    listas  = _listaGenericRepository.GetAll().OrderByDescending(s => s.Prazo).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderByDescending(s => s.Prazo).Skip(QuantPorPag).OrderByDescending(s => s.Prazo).Take(QuantPorPag).ToList();
                    break;
                default:
                    ViewBag.flagEmail = 1;
                    listas = _listaGenericRepository.GetAll().OrderBy(s => s.Usuario.Email).ThenByDescending(x => x.Nome).Take(QuantPorPag).ToList();
                    proxima = _listaGenericRepository.GetAll().OrderBy(s => s.Usuario.Email).Skip(QuantPorPag).OrderBy(s => s.Usuario.Email).Take(QuantPorPag).ToList();
                    break;
            }

            ViewBag.Ordem = ordem;
            ViewBag.Introducao = "Todas as Listas Existentes";
            ViewBag.PaginaAtual = 1;

            ViewBag.Proxima = proxima.Count > 0;

            ViewBag.Anterior = false;

            return View(listas);
        }

        [Authorize, ActionName("Index"), HttpPost]
        public ActionResult Index2(string termo)
        {
            var listas = _listaGenericRepository.GetAll().Where(x => x.Nome.Contains(termo)).ToList();
            ViewBag.Proxima = false;
            ViewBag.Anterior = false;

            if (termo != "")
            {
                ViewBag.Introducao = termo != "" ? "Resultados de Busca" : "Todas as Listas Existentes";

                if (!listas.Any())
                    ViewBag.Erro = termo;

                return View("Index", listas);
            }
            return RedirectToAction("Index");
        }


        [Authorize, ActionName("IndexBootstrap"), HttpPost]
        public ActionResult Index2Bootstrap(string termo)
        {
            var listas = _listaGenericRepository.GetAll().Where(x => x.Nome.Contains(termo)).ToList();
            ViewBag.Proxima = false;
            ViewBag.Anterior = false;

            if (termo != "")
            {
                ViewBag.Introducao = termo != "" ? "Resultados de Busca" : "Todas as Listas Existentes";

                if (!listas.Any())
                    ViewBag.Erro = termo;

                return View("IndexBootstrap", listas);
            }
            return RedirectToAction("IndexBootstrap");
        }

        public ActionResult IndexBootstrap(string ordem)
        {
            //AtualizaTarefasAtivas();
            List<Lista> listas = _listaGenericRepository.GetAll().ToList();

            return View(listas);
        }

        [Authorize]
        public ActionResult IndexPagina(int pagina, string ordem)
        {
            //AtualizaTarefasAtivas();

            ViewBag.UsuarioOrdem    = ordem == "Email"  ? "Email_desc"  : "Email";
            ViewBag.AtivaOrdem      = ordem == "Ativa"  ? "Ativa_desc"  : "Ativa";
            ViewBag.NomeOrdem       = ordem == "Nome"   ? "Nome_desc"   : "Nome";
            ViewBag.PrazoOrdem      = ordem == "Prazo"  ? "Prazo_desc"  : "Prazo";
            ViewBag.TarefasOrdem    = ordem == "Tarefas"? "Tarefas_desc": "Tarefas";

            ViewBag.Ordem = ordem;

            ViewBag.Introducao = "Todas as Listas Existentes";
            var listas = _listaGenericRepository.GetAll().ToList();
            var proxima = _listaGenericRepository.GetAll().ToList();

            if (ordem == "Email" || string.IsNullOrEmpty(ordem))
            {
                ViewBag.flagEmail = 1;
                listas  = _listaGenericRepository.GetAll().OrderBy(x => x.Usuario.Email).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Usuario.Email).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderBy(x => x.Usuario.Email).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Usuario.Email).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Email_desc")
            {
                ViewBag.flagEmail = 0;
                listas  = _listaGenericRepository.GetAll().OrderByDescending(x => x.Usuario.Email).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Usuario.Email).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderByDescending(x => x.Usuario.Email).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Usuario.Email).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Ativa")
            {
                ViewBag.flagAtiva = 1;
                listas  = _listaGenericRepository.GetAll().OrderBy(x => x.Ativa).ThenBy(s => s.ListaId).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Ativa).ThenBy(s => s.ListaId).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderBy(x => x.Ativa).ThenBy(s => s.ListaId).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Ativa).ThenBy(s => s.ListaId).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Ativa_desc")
            {
                ViewBag.flagAtiva = 0;
                listas  = _listaGenericRepository.GetAll().OrderByDescending(x => x.Ativa).ThenByDescending(s => s.ListaId).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Ativa).ThenByDescending(s => s.ListaId).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderByDescending(x => x.Ativa).ThenByDescending(s => s.ListaId).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Ativa).ThenByDescending(s => s.ListaId).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Nome")
            {
                ViewBag.flagNome = 1;
                listas  = _listaGenericRepository.GetAll().OrderBy(x => x.Nome).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Nome).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderBy(x => x.Nome).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Nome).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Nome_desc")
            {
                ViewBag.flagNome = 0;
                listas  = _listaGenericRepository.GetAll().OrderByDescending(x => x.Nome).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Nome).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderByDescending(x => x.Nome).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Nome).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Prazo")
            {
                ViewBag.flagPrazo = 1;
                listas  = _listaGenericRepository.GetAll().OrderBy(x => x.Prazo).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Prazo).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderBy(x => x.Prazo).Skip((pagina - 1) * QuantPorPag).OrderBy(x => x.Prazo).Take(QuantPorPag + 1).ToList();
            }
            if (ordem == "Prazo_desc")
            {
                ViewBag.flagPrazo = 0;
                listas  = _listaGenericRepository.GetAll().OrderByDescending(x => x.Prazo).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Prazo).Take(QuantPorPag).ToList();
                proxima = _listaGenericRepository.GetAll().OrderByDescending(x => x.Prazo).Skip((pagina - 1) * QuantPorPag).OrderByDescending(x => x.Prazo).Take(QuantPorPag + 1).ToList();
            }

            ViewBag.Proxima = proxima.Count > QuantPorPag;

            ViewBag.Anterior = pagina > 1;

            ViewBag.PaginaAtual = pagina;

            return View("Index", listas);
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lista lista = _listaGenericRepository.Find(id);
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
            return Content("Ítens por Página: " + QuantPorPag, "text/html", Encoding.UTF8);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ListaId,Nome,Ativa,UsuarioId,Prazo")] Lista lista)
        {
            if (ModelState.IsValid)
            {
                _listaGenericRepository.Create(lista);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UsuarioId = "";

            //Preenche o combo Usuário (Email) com os emails dos usuários ativos existêntes e mantem o que já havia sido selecionado.
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }


        [Authorize]
        public ActionResult EditBootstrap(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lista lista = _listaGenericRepository.Find(id);
            if (lista == null)
            {
                return HttpNotFound();
            }
            //Preenche o combo Usuário (Email) com os emails dos usuários ativos existêntes e mantem o que já havia sido selecionado.
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBootstrap([Bind(Include = "ListaId,Nome,Ativa,UsuarioId,Prazo")] Lista lista)
        {
            if (ModelState.IsValid)
            {
                //Contornando um problema que ele acha que o cara a ser modificado é um já existente e não pode ser Attacked
                // então faço uma pesquisa pelo id do cara e transfiro pra outro e envio este novo para ele usar. IDIOTA! PAU!
                // Não dava esse pau antes!
                var lista2 = _listaGenericRepository.Find(lista.ListaId);
                lista2.Nome = lista.Nome;
                lista2.Ativa = lista.Ativa;
                lista2.Prazo = lista.Prazo;
                lista2.Tarefas = lista.Tarefas;
                lista2.TarefasAtivas = lista.TarefasAtivas;
                lista2.Usuario = lista.Usuario;
                lista2.UsuarioId = lista.UsuarioId;

                _listaGenericRepository.Atualizar(lista2);
                _listaGenericRepository.SalvarMudanca();
                return RedirectToAction("IndexBootstrap");
            }
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
            Lista lista = _listaGenericRepository.Find(id);
            if (lista == null)
            {
                return HttpNotFound();
            }
            //Preenche o combo Usuário (Email) com os emails dos usuários ativos existêntes e mantem o que já havia sido selecionado.
            ViewBag.UsuarioId = new SelectList(_db.Usuarios.Where(x => x.Ativo == true), "UsuarioId", "Email", lista.UsuarioId);
            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ListaId,Nome,Ativa,UsuarioId,Prazo")] Lista lista)
        {
            if (ModelState.IsValid)
            {
                //Contornando um problema que ele acha que o cara a ser modificado é um já existente e não pode ser Attacked
                // então faço uma pesquisa pelo id do cara e transfiro pra outro e envio este novo para ele usar. IDIOTA! PAU!
                // Não dava esse pau antes!
                var lista2 = _listaGenericRepository.Find(lista.ListaId);
                lista2.Nome = lista.Nome;
                lista2.Ativa = lista.Ativa;
                lista2.Prazo = lista.Prazo;
                lista2.Tarefas = lista.Tarefas;
                lista2.TarefasAtivas = lista.TarefasAtivas;
                lista2.Usuario = lista.Usuario;
                lista2.UsuarioId = lista.UsuarioId;

                _listaGenericRepository.Atualizar(lista2);
                _listaGenericRepository.SalvarMudanca();
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
            
            Lista lista = _listaGenericRepository.Find(id);
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
            Lista lista = _listaGenericRepository.Find(id);
            _listaGenericRepository.Delete(x => x == lista);
            //_ListaGenericRepository.Delete(lista);
            _listaGenericRepository.SalvarMudanca();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
