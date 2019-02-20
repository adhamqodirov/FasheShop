using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FasheShop.Models;
using System.IO;

namespace FasheShop.Controllers
{
    public class NewsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: News
        public ActionResult Index()
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname"); return View();
        }

        // POST: News/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,Excerpt,Date,Picture")] News news, HttpPostedFileBase file)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                path = Path.Combine(Server.MapPath("~/Content/images/"), Path.GetFileName(file.FileName));
                file.SaveAs(path);

                news.Picture = "/Content/images/" + file.FileName;


                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,Excerpt,Date,Picture")] News news)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname");
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            ViewBag.cname = db.CustomizeSettings.FirstOrDefault(x => x.code == "cname"); if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
