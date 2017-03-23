using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ListaDeTarefas.Models;

namespace ListaDeTarefas.Controllers
{
    public class TarefaController : Controller
    {
        private TarefaContexto _db = new TarefaContexto();

        [Authorize]
        public ActionResult Index()
        {
            var tarefas = _db.Tarefas.Include(t => t.Lista);
            return View(tarefas.ToList());
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = _db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            return View(tarefa);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.ListaId = new SelectList(_db.Listas.Where(x => x.Ativa == true), "ListaId", "Nome");
            return View();
        }

        // POST: /Tarefa/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TarefaId,Nome,Concluida,Ativa,ListaId")] Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _db.Tarefas.Add(tarefa);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ListaId = new SelectList(_db.Listas.Where(x => x.Ativa == true), "ListaId", "Nome", tarefa.ListaId);
            return View(tarefa);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = _db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListaId = new SelectList(_db.Listas.Where(x => x.Ativa == true), "ListaId", "Nome", tarefa.ListaId);
            return View(tarefa);
        }

        // POST: /Tarefa/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TarefaId,Nome,Concluida,Ativa,ListaId")] Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tarefa).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ListaId = new SelectList(_db.Listas.Where(x => x.Ativa == true), "ListaId", "Nome", tarefa.ListaId);
            return View(tarefa);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = _db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            return View(tarefa);
        }

        // POST: /Tarefa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tarefa tarefa = _db.Tarefas.Find(id);
            _db.Tarefas.Remove(tarefa);
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
