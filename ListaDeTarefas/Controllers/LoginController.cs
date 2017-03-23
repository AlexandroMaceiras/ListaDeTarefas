using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ListaDeTarefas.Models;
using System.Web.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ListaDeTarefas.Controllers
{
    public class LoginController : Controller
    {
        private TarefaContexto _db = new TarefaContexto();

        private HashCode _hc = new HashCode();

        //Permite a chamada de 2 botões de submit diferentes para o formulario
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultipleButtonAttribute : ActionNameSelectorAttribute
        {
            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                var isValidName = false;
                var keyValue = string.Format("{0}:{1}", Name, Argument);
                var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                if (value != null)
                {
                    controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                    isValidName = true;
                }

                return isValidName;
            }
        }

        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Login")]
        public ActionResult Index(string email, string senha)
        {
            senha = _hc.Codificar(senha);
            if (_db.Usuarios.Any(x => x.Email == email && x.Senha == senha))
            {
                FormsAuthentication.SetAuthCookie(email, false);
                return RedirectToAction("Index", "ListaGeneric");
            }
            else
            {
                if (!_db.Usuarios.Any(x => x.Email == email))
                {
                    ModelState.AddModelError("Email", "Usuário (Email) inválido.");
                }
                if (!_db.Usuarios.Any(x => x.Email == email && x.Senha == senha))
                {
                    ModelState.AddModelError("Senha", "Senha inválida.");
                }
            }
            return View("Index"); //RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Login");
        }

        // GET: /Login/Details/5
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

        // GET: /Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Login/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UsuarioId,Email,Senha,Ativo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _db.Usuarios.Add(usuario);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: /Login/Edit/5
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

        // POST: /Login/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UsuarioId,Email,Senha,Ativo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(usuario).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: /Login/Delete/5
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

        // POST: /Login/Delete/5
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
