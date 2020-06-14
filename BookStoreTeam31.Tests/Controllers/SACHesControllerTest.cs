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

namespace BookStoreTeam31.Tests.Controllers
{
    [TestClass]
    public class SACHesControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var controller = new SACHesController();

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SACH>;
            Assert.IsNotNull(model);

            var db = new CsK24_BookStoreEntities();
            Assert.AreEqual(db.SACHes.Count(), model.Count);
        }

        [TestMethod]
        public void TestIndex2()
        {
            var controller = new SACHesController();

            var result = controller.Index2() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SACH>;
            Assert.IsNotNull(model);

            var db = new CsK24_BookStoreEntities();
            Assert.AreEqual(db.SACHes.Count(), model.Count);
        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new SACHesController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var sach = new SACH
            {
                TENSACH = rand.NextDouble().ToString(),
                THONGTIN = rand.NextDouble().ToString(),
                GIASACH = -rand.Next()
            };

            var controller = new SACHesController();

            var result0 = controller.Create(sach, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Price is less than Zero", controller.ModelState["Price"].Errors[0].ErrorMessage);

            sach.GIASACH = -sach.GIASACH;
            controller.ModelState.Clear();

            result0 = controller.Create(sach, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Picture not found!", controller.ModelState[""].Errors[0].ErrorMessage);

            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result1 = controller.Create(sach, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);

                var db = new CsK24_BookStoreEntities();
                var entity = db.SACHes.SingleOrDefault(p => p.TENSACH == sach.TENSACH && p.THONGTIN == sach.THONGTIN);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sach.GIASACH, entity.GIASACH);

                Assert.IsTrue(fileName.StartsWith("~/Upload/Sach/"));
                Assert.IsTrue(fileName.EndsWith(entity.MASACH.ToString()));
            }
        }

        [TestMethod]
        public void TestEditG()
        {
            var controller = new SACHesController();
            var result0 = controller.Edit(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.First();
            var result1 = controller.Edit(sach.MASACH) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SACH;
            Assert.IsNotNull(model);
            Assert.AreEqual(sach.TENSACH, model.TENSACH);
            Assert.AreEqual(sach.GIASACH, model.GIASACH);
            Assert.AreEqual(sach.THONGTIN, model.THONGTIN);
        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.AsNoTracking().First();
            sach.TENSACH = rand.NextDouble().ToString();
            sach.THONGTIN = rand.NextDouble().ToString();
            sach.GIASACH = -rand.Next();

            var controller = new SACHesController();

            var result0 = controller.Edit(sach, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Price is less than Zero", controller.ModelState["Price"].Errors[0].ErrorMessage);

            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                sach.GIASACH = -sach.GIASACH;
                controller.ModelState.Clear();
                var result1 = controller.Edit(sach, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);

                var entity = db.SACHes.Find(sach.MASACH);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sach.TENSACH, entity.TENSACH);
                Assert.AreEqual(sach.THONGTIN, entity.THONGTIN);
                Assert.AreEqual(sach.GIASACH, entity.GIASACH);

                Assert.AreEqual("~/Upload/Sach/" + sach.MASACH, fileName);
                //Assert.IsTrue(fileName.StartsWith("~/Upload/Products/"));
                //Assert.IsTrue(fileName.EndsWith(entity.id.ToString()));
            }
        }

        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new  SACHesController();
            var result0 = controller.Delete(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.First();
            var result1 = controller.Delete(sach.MASACH) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SACH;
            Assert.IsNotNull(model);
            Assert.AreEqual(sach.TENSACH, model.TENSACH);
            Assert.AreEqual(sach.GIASACH, model.GIASACH);
            Assert.AreEqual(sach.THONGTIN, model.THONGTIN);
        }

        [TestMethod]
        public void TestDeleteP()
        {
            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.AsNoTracking().First();

            var controller = new SACHesController();

            var context = new Mock<HttpContextBase>();
            var server = new Mock<HttpServerUtilityBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            var tempDir = System.IO.Path.GetTempFileName();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s =>
            {
                filePath = s;
                return tempDir;
            });

            using (var scope = new TransactionScope())
            {
                System.IO.File.Delete(tempDir);
                System.IO.Directory.CreateDirectory(tempDir);
                tempDir = tempDir + "/";
                System.IO.File.Create(tempDir + sach.MASACH).Close();
                Assert.IsTrue(System.IO.File.Exists(tempDir + sach.MASACH));

                var result = controller.DeleteConfirmed(sach.MASACH) as RedirectToRouteResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.SACHes.Find(sach.MASACH);
                Assert.IsNull(entity);

                Assert.AreEqual("~/Upload/Sach/", filePath);
                Assert.IsFalse(System.IO.File.Exists(tempDir + sach.MASACH));
            }
        }

        [TestMethod]
        public void TestPicture()
        {
            var controller = new SACHesController();

            var context = new Mock<HttpContextBase>();
            var server = new Mock<HttpServerUtilityBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => filePath = s);

            var result = controller.Picture(0) as FilePathResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Upload/Sach/0", result.FileName);
            Assert.AreEqual("images", result.ContentType);
        }

        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new SACHesController()) { }
        }

        [TestMethod]
        public void TestDetails()
        {
            var controller = new SACHesController();
            var result0 = controller.Details(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.First();
            var result1 = controller.Details(sach.MASACH) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SACH;
            Assert.IsNotNull(model);
            Assert.AreEqual(sach.TENSACH, model.TENSACH);
            Assert.AreEqual(sach.GIASACH, model.GIASACH);
            Assert.AreEqual(sach.THONGTIN, model.THONGTIN);
        }

        [TestMethod]
        public void TestSearch()
        {
            var db = new CsK24_BookStoreEntities();
            var sach = db.SACHes.ToList();
            var keyword = sach.First().TENSACH.Split().First();
            sach = sach.Where(p => p.TENSACH.ToLower().Contains(keyword.ToLower())).ToList();

            var controller = new SACHesController();
            var result = controller.Search(keyword) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SACH>;
            Assert.IsNotNull(model);

            Assert.AreEqual("Index2", result.ViewName);
            Assert.AreEqual(sach.Count(), model.Count);
            Assert.AreEqual(keyword, result.ViewData["Keyword"]);
        }
    }
}
