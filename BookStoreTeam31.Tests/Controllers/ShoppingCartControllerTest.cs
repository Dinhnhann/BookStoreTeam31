using Moq;
using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookStoreTeam31.Controllers;
using BookStoreTeam31.Models;
using System.Transactions;
using System.Collections;
using System.Web.Routing;

namespace BookStoreTeam31.Tests.Controllers
{
    public class MockHttpSession : HttpSessionStateBase
    {
        public Hashtable Buffer = new Hashtable();

        public override object this[string key]
        {
            get
            {
                return Buffer[key];
            }
            set
            {
                Buffer[key] = value;
            }
        }
    }

    [TestClass]
    public class ShoppingCartControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var session = new MockHttpSession();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);

            var controller = new ShoppingCartController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            session["ShoppingCart"] = null;
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<CHITIETHOADON>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);

            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.First();
            var shoppingCart = new List<CHITIETHOADON>();

            shoppingCart.Add(new CHITIETHOADON
            {
                SACH = sach,
                SOLUONG = 1
            });

            var chitiethoadon = new CHITIETHOADON();
            chitiethoadon.SACH = sach;
            chitiethoadon.SOLUONG = 2;
            shoppingCart.Add(chitiethoadon);

            session["ShoppingCart"] = shoppingCart;
            result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            model = result.Model as List<CHITIETHOADON>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(sach.MASACH, model.First().SACH.MASACH);
            Assert.AreEqual(3, model.First().SOLUONG);
        }

        [TestMethod]
        public void TestCreate()
        {
            var session = new MockHttpSession();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);

            var controller = new ShoppingCartController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.First();
            var result = controller.Create(sach.MASACH, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);

            var shoppingCart = session["ShoppingCart"] as List<CHITIETHOADON>;
            Assert.IsNotNull(shoppingCart);
            Assert.AreEqual(1, shoppingCart.Count);
            Assert.AreEqual(sach.MASACH, shoppingCart.First().SACH.MASACH);
            Assert.AreEqual(2, shoppingCart.First().SOLUONG);
        }
    }
}
