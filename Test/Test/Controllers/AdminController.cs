using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using Test.Models;
using NLog;


namespace Test.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Admin admin = new Admin();

        public ActionResult AdminPanel()
        {
            var shusers = admin.ReceiveUsers().ToList();
            if (shusers == null)
            {
                ModelState.AddModelError("Error", "Users not found");
                return View(shusers);
            }
            else return View(shusers);
        }

        [HttpPost]
        public ActionResult AdminPanel(int saveid)
        {
            return Redirect(String.Format("Edit/{0}", saveid));
        }

        public ActionResult Edit(int id)
        {
            var userToEdit = admin.ReceiveUserById(id);
            if (userToEdit == null)
            {
                ModelState.AddModelError("Error", "User not found");
                return View(userToEdit);
            }
            else return View(userToEdit);
        }

        [HttpPost]
        public ActionResult Edit(AdminPanelViewModel user)
        {
            admin.SaveChanges(user);
            return Redirect("~/Admin/AdminPanel");
        }

        [HttpPost]
        public ActionResult Freeze(int id)
        {
            var userToDelete = admin.ReceiveUserById(id);
            if (userToDelete == null)
            {
                ModelState.AddModelError("Error", "User not found");
                return Redirect("~/Admin/AdminPanel");
            }
            else
            {
                admin.Freeze(userToDelete);
                return Redirect("~/Admin/AdminPanel");
            }
        }

        [HttpPost]
        public ActionResult Restore(int id)
        {
            var userToRestore = admin.ReceiveUserById(id);
            if (userToRestore == null)
            {
                ModelState.AddModelError("Error", "User not found");
                return Redirect("~/Admin/AdminPanel");
            }
            else
            {
                admin.Restore(userToRestore);
                return Redirect("~/Admin/AdminPanel");
            }
        }

        public ActionResult CreateNewUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewUser(AdminCreateUserViewModel newuser)
        {
            admin.CreateUser(newuser);
            return View();
        }
    }
}