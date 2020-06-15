using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using BookStoreTeam31.Models;

namespace BookStoreTeam31.Controllers
{
    [Authorize/*(Roles = "Admin")*/]
    public class SACHesController : Controller
    {
        private CsK24_BookStoreEntities db = new CsK24_BookStoreEntities();

        // GET: Products
        public ActionResult Index()
        {
            var model = db.SACHes.ToList();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Search(string keyword)
        {
            var model = db.SACHes.ToList();
            model = model.Where(p => p.TENSACH.ToLower().Contains(keyword.ToLower())).ToList();
            ViewBag.Keyword = keyword;
            return View("Index2", model);
        }

        // for customer to view products
        [AllowAnonymous]
        public ActionResult Index2()
        {
            var model = db.SACHes.ToList();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Details(int masach)
        {
            var model = db.SACHes.Find(masach);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Picture(int masach)
        {
            var path = Server.MapPath(PICTURE_PATH);
            return File(path + masach, "images");
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SACH model, HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        db.SACHes.Add(model);
                        db.SaveChanges();

                        // store picture
                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MASACH);

                        scope.Complete();
                        return RedirectToAction("Index");
                    }
                }
                else ModelState.AddModelError("", "Picture not found!");
            }

            return View(model);
        }

        private const string PICTURE_PATH = "~/Upload/Sach/";

        private void ValidateProduct(SACH sach)
        {
            if (sach.GIASACH < 0)
                ModelState.AddModelError("Price", "Price is less than Zero");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int masach)
        {
            var model = db.SACHes.Find(masach);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SACH model, HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    if (picture != null)
                    {
                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MASACH);
                    }

                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int masach)
        {
            var model = db.SACHes.Find(masach);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int masach)
        {
            using (var scope = new TransactionScope())
            {
                var model = db.SACHes.Find(masach);
                db.SACHes.Remove(model);
                db.SaveChanges();

                var path = Server.MapPath(PICTURE_PATH);
                System.IO.File.Delete(path + model.MASACH);

                scope.Complete();
                return RedirectToAction("Index");
            }
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
