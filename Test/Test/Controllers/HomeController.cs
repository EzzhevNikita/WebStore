using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using NLog;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static SalesPerson sl = new SalesPerson();
        static UserMethods um = new UserMethods();
        static DbContext db = new DbContext();
        static Auth admin = new Auth();

        [CustomAuthorizeAttribute(allowAnonymus: true)]
        //главная страница отображения товаров
        public ActionResult Index()
        {
            var products = um.GetItems().ToList();
            if (products == null)
            {
                ModelState.AddModelError("Error", "Products not found");
                return View(products);
            }
            else return View(products);
        }

        [CustomAuthorizeAttribute(Roles = "User")]
        [HttpPost]
        //добавление товара в корзину
        public ActionResult Index(int id)
        {
            um.AddToCart(id);
            return Redirect("~/Home/Busket");
        }


        [CustomAuthorizeAttribute(Roles = "User")]
        //просмотр товаров в корзине
        public ActionResult Busket()
        {
            var busket =  um.ViewBasket().ToList();
            if (busket == null)
            {
                ModelState.AddModelError("Error", "Cart not found");
                return View(busket);
            }
            return View(busket);
        }

        [CustomAuthorizeAttribute(Roles = "User")]
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            um.DeleteProduct(id);
            return Redirect("~/Home/Busket");
        }

        [CustomAuthorizeAttribute(Roles = "User")]
        //оформление заказа
        public ActionResult MakeOrder()
        {
            um.MakeAnOder();
            return View();
        }


        [CustomAuthorizeAttribute(Roles = "User")]
        public ActionResult History()
        {
            var history = um.ViewHistory();
            if (history == null)
            {
                ModelState.AddModelError("Error", "History is not found");
                return View(history);
            }
            return View(history);
        }

        [CustomAuthorizeAttribute(Roles = "User")]
        public ActionResult Orders()
        {
            var orders = um.ViewOrder();
            if (orders == null)
            {
                ModelState.AddModelError("Error", "Orders not found");
                return View(orders);
            }
            return View(orders);
        }

        [CustomAuthorizeAttribute(Roles = "User")]
        public ActionResult ChangeUserData()
        {
            User user = (from u in db.User where u.id == admin.curUser select u).First();
            if (user == null)
            {
                ModelState.AddModelError("Error", "User for change not found");
                return View(user);
            }
            return View(user);
        }

        
        [HttpPost]
        public ActionResult ChangeUserData(User user)
        {
            um.ChangeUserData(user);
            return View(user);
        }


    }
}