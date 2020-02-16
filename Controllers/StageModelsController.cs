using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Test_redips_js.Models;

namespace Test_redips_js.Controllers
{
    public class StageModelsController : Controller
    {
        private Test_redips_jsContext db = new Test_redips_jsContext();

        // GET: StageModels
        public ActionResult Index()
        {
            return View(db.StageModels.ToList());
        }

        // GET: StageModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StageModel stageModel = db.StageModels.Find(id);
            if (stageModel == null)
            {
                return HttpNotFound();
            }
            return View(stageModel);
        }

        // GET: StageModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StageModels/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "rowid,stg")] StageModel stageModel)
        {
            if (ModelState.IsValid)
            {
                db.StageModels.Add(stageModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stageModel);
        }

        // GET: StageModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StageModel stageModel = db.StageModels.Find(id);
            if (stageModel == null)
            {
                return HttpNotFound();
            }
            return View(stageModel);
        }

        // POST: StageModels/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "rowid,stg")] StageModel stageModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stageModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stageModel);
        }

        // GET: StageModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StageModel stageModel = db.StageModels.Find(id);
            if (stageModel == null)
            {
                return HttpNotFound();
            }
            return View(stageModel);
        }

        // POST: StageModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StageModel stageModel = db.StageModels.Find(id);
            db.StageModels.Remove(stageModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
