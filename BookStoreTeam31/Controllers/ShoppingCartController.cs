using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStoreTeam31.Models;

namespace BookStoreTeam31.Controllers
{
    public class ShoppingCartController : Controller
    {
        private CsK24_BookStoreEntities db = new CsK24_BookStoreEntities();

        private List<CHITIETHOADON> ShoppingCart = null;

        private void GetShoppingCart()
        {
            if (Session["ShoppingCart"] != null)
                ShoppingCart = Session["ShoppingCart"] as List<CHITIETHOADON>;
            else
            {
                ShoppingCart = new List<CHITIETHOADON>();
                Session["ShoppingCart"] = ShoppingCart;
            }
        }

        // GET: ShoppingCart
        public ActionResult Index()
        {
            GetShoppingCart();
            var hashtable = new Hashtable();
            foreach (var chitiethoadon in ShoppingCart)
            {
                if (hashtable[chitiethoadon.SACH.MASACH] != null)
                {
                    (hashtable[chitiethoadon.SACH.MASACH] as CHITIETHOADON).SOLUONG += chitiethoadon.SOLUONG;
                }
                else hashtable[chitiethoadon.SACH.MASACH] = chitiethoadon;
            }

            ShoppingCart.Clear();
            foreach (CHITIETHOADON chitiethoadon in hashtable.Values)
                ShoppingCart.Add(chitiethoadon);
            return View(ShoppingCart);
        }

        // GET: ShoppingCart/Create
        [HttpPost]
        public ActionResult Create(int Masach, int soluong)
        {
            GetShoppingCart();
            var sach = db.SACHes.Find(Masach);
            ShoppingCart.Add(new CHITIETHOADON
            {
                SACH = sach,
                SOLUONG = soluong
            });

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Edit(int[] ma_sach, int[] soluong)
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            if (ma_sach != null)
                for (int i = 0; i < ma_sach.Length; i++)
                    if (soluong[i] > 0)
                    {
                        var sach = db.SACHes.Find(ma_sach[i]);
                        ShoppingCart.Add(new CHITIETHOADON
                        {
                            SACH = sach,
                            SOLUONG = soluong[i]
                        });
                    }
            Session["ShoppingCart"] = ShoppingCart;

            return RedirectToAction("Index");
        }

        // GET: ShoppingCart/Delete/5
        public ActionResult Delete()
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            Session["ShoppingCart"] = ShoppingCart;
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
