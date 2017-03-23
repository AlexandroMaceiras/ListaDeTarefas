using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ListaDeTarefas.Models;

namespace ListaDeTarefas.Controllers
{
    public class UsuarioController : Controller
    {
        private TarefaContexto _db = new TarefaContexto();
        private HashCode _hc = new HashCode();

        [Authorize]
        public ActionResult Index()
        {
            return View(_db.Usuarios.ToList().OrderBy(x => x.Email));
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Usuario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UsuarioId,Email,Senha,Ativo")] Usuario usuario)
        {
            if (_db.Usuarios.Where(x => x.Email == usuario.Email).Count() == 0)
            {
                if (ModelState.IsValid)
                {
                    usuario.Senha = _hc.Codificar(usuario.Senha);
                    _db.Usuarios.Add(usuario);
                    _db.SaveChanges();
                    //return RedirectToAction("Index");

                    //Logando com o usuário criado.
                    FormsAuthentication.SetAuthCookie(usuario.Email, false);
                    return RedirectToAction("Index", "ListaGeneric"); ;
                }
            }
            else { ModelState.AddModelError("Email", "Usuário já cadastrado."); }

            return View(usuario);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: /Usuario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UsuarioId,Email,Senha,Ativo")] Usuario usuario)
        {
            if (_db.Usuarios.Where(x => x.Senha == usuario.Senha
                                    && x.Email == usuario.Email
                                    && x.Ativo == usuario.Ativo).Count() == 0) //Verifica se algo foi alterado
            {
                if (ModelState.IsValid)
                {
                    if (_db.Usuarios.Where(x => x.Senha == usuario.Senha).Count() == 0) //Verifica se a senha foi alterada
                        usuario.Senha = _hc.Codificar(usuario.Senha); //Codifica em SHA1 a senha se ela for uma senha nova
                    _db.Entry(usuario).State = EntityState.Modified; //Muda a estado do EF para alterado - Isto é necessário para o SAVE
                    try
                    {   //Verifica se o usuario é igual a algum já existente a ser alterado, esse teste dá erro se for um usuario novo
                        if (_db.Usuarios.Where(x => x.Email == usuario.Email).ToList().First().UsuarioId == usuario.UsuarioId)
                        {
                            _db.SaveChanges(); //Salva a alteração do nome do usuário já existente
                        }
                        else
                        {   //Se o usuario for um já existente que não seja ele mesmo dá mensagem de erro
                            ModelState.AddModelError("Email", "Este Email já é cadastrado use outro.");
                            return View(usuario);
                        }
                    }
                    catch
                    {
                        _db.SaveChanges(); //Se deu erro no if ele é um usuario novo, então Salva
                    }
                    return RedirectToAction("Index");
                }
            } //Dá mensagem de erro caso nada tenha sido modificado
            ModelState.AddModelError("Email", " Nada foi alterado.");
            ModelState.AddModelError("Senha", " ");
            ModelState.AddModelError("Ativo", " ");
            return View(usuario);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = _db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: /Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = _db.Usuarios.Find(id);
            _db.Usuarios.Remove(usuario);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
