using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using NLog;

namespace Test.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Seller")]
    public class SalesController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        SalesPerson sp = new SalesPerson();
        public ActionResult SellerPanel()
        {
            IEnumerable<SellerPanelViewModel> product = sp.GetItems();
            if (product == null)
            {
                ModelState.AddModelError("Error", "Products not found");
                return View(product);
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult SellerPanel(int saveid)
        {
            return Redirect(String.Format("Edit/{0}", saveid));
        }

        public ActionResult Edit(int id)
        {
            SellerPanelViewModel productToEdit = sp.GetItemById(id);
            if (productToEdit == null)
            {
                ModelState.AddModelError("Error", "Product to edit not found");
                return View(productToEdit);
            }
            return View(productToEdit);
        }

        [HttpPost]
        public ActionResult Edit(SellerPanelViewModel productToEdit)
        {
            sp.SaveChanges(productToEdit);
            return Redirect("~/Sales/SellerPanel");
        }

        public ActionResult AddNewItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewItem(AddItemViewModel newProduct)
        {
            sp.AddItem(newProduct);
            return Redirect("~/Sales/SellerPanel");
        }

        public ActionResult AddNewCategory()
        {
            IEnumerable<Category> categories = sp.ViewCategories();
            if (categories == null)
            {
                ModelState.AddModelError("Error", "Categories not found");
                return View(categories);
            }
            return View(categories);
        }

        [HttpPost]
        public ActionResult AddNewCategory(string Name)
        {
            sp.AddCat(Name);
            return Redirect("~/Sales/AddNewCategory");
        }

        public ActionResult ViewOrders()
        {
            var orders = sp.GetOrders();
            if (orders == null)
            {
                ModelState.AddModelError("Error", "Orders not found");
                return View(orders);
            }
            return View(orders);
        }
        
        [HttpGet]
        public ActionResult ChangeStatus(string Status, int saveid)
        {
            sp.ChangeStatus(saveid, Status);
            return Redirect("~/Sales/ViewOrders");
        }
    }
}