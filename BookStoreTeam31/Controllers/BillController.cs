using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using BookStoreTeam31.Models;

namespace BookStoreTeam31.Controllers
{
    public class BillController : Controller
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

        [Authorize/*(Roles = "Admin")*/]
        public ActionResult Index()
        {
            var model = db.HOADONs.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HOADON model)
        {
            ValidateBill(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    model.NGAY = DateTime.Now;
                    db.HOADONs.Add(model);
                    db.SaveChanges();

                    foreach (var item in ShoppingCart)
                    {
                        db.CHITIETHOADONs.Add(new CHITIETHOADON
                        {
                            MAHOADON = model.MAHOADON,
                            MASACH = item.SACH.MASACH,
                            GIASACH = item.SACH.GIASACH,
                            SOLUONG = item.SOLUONG
                        });
                    }
                    db.SaveChanges();

                    scope.Complete();
                    Session["ShoppingCart"] = null;
                    return RedirectToAction("Index2", "SACHes");
                }
            }
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View(model);
        }

        private void ValidateBill(HOADON model)
        {
            var regex = new Regex("[0-9]{3}");
            GetShoppingCart();
            if (ShoppingCart.Count == 0)
                ModelState.AddModelError("", "Không có sách trong giỏ hàng!");
            if (!regex.IsMatch(model.SDT))
                ModelState.AddModelError("SĐT", "Sai SĐT");
        }
    }
}