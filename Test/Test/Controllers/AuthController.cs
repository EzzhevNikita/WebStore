using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using NLog;

namespace Test.Controllers
{
    [CustomAuthorizeAttribute(allowAnonymus: true)]
    public class AuthController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Auth user = new Auth();
        Admin admin = new Admin();

        public ActionResult Login()
        {
            if (user.curUser == -1)
            {
                return View();
            }
            else
            {
                admin.ClearBusket();
                return Redirect("~/Home/Index");
            }
        }

        public ActionResult RestorePassword()
        {
            if (user.curUser != -1)
            {
                AdminPanelViewModel userToChange = admin.ReceiveUserById(user.curUser);
                RestoreViewModel rvm = new RestoreViewModel()
                {
                    email = userToChange.Email
                };
                return View(rvm);
            }
            else
            {
                return View();
            }
        }


        public void ChangePassword()
        {
            AdminPanelViewModel userToChange = admin.ReceiveUserById(user.curUser);
            RestoreViewModel rvm = new RestoreViewModel()
            {
                email = userToChange.Email
            };
            RestorePassword(rvm);
        }

        [HttpPost]
        public ActionResult RestorePassword(RestoreViewModel rvm)
        {
            if (admin.RecieveUserByEmail(rvm.email) != null)
            {
                if (rvm.password == rvm.repetedpassword)
                {
                    admin.Restore(rvm);
                    return Login();
                }
                else
                {
                    ModelState.AddModelError("", "Passwords are different");
                    return View(rvm);
                }
            }
            else
            {
                ModelState.AddModelError("", "There are no a user wit such email in db");
                return View(rvm);
            }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Incorrect data");
                return View(login);
            }
            else
            {
                user.Login(login.email, login.password);



                if (user.curUser == -1)
                {
                    ModelState.AddModelError("", "Check entered data, user with such data wasn't found");
                    return View(login);
                }
                var luser = admin.ReceiveUserById(user.curUser);
                if (luser.Status == 1)
                {
                    return Redirect(String.Format("~/Auth/AccountRestoring/{0}", user.curUser));
                }
                else
                {
                    return Redirect("~/Home/Index");
                }
            }
        }

        public ActionResult AccountRestoring(int id)
        {
            AdminPanelViewModel apvm = admin.ReceiveUserById(id);
            RestoreViewModel rvm = new RestoreViewModel()
            {
                email = apvm.Email
            };
            return View(rvm);
        }

        [HttpPost]
        public ActionResult AccountRestoring(RestoreViewModel model)
        {
            admin.Restore(model);
            return Redirect("~/Home/Index");
        }

        public ActionResult Logout()
        {

            if (user.curUser != -1)
            {
                admin.ClearBusket();
                user.Logoff();
            }
            return Redirect("~/Home/Index");
        }


        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegViewModel reg)
        {
            if (ModelState.IsValid)
            {
                Models.User user = new Models.User();
                user.First_Name = reg.First_Name;
                user.Last_Name = reg.Last_Name;
                user.Email = reg.Email;
                user.Password = reg.Password;
                user.Reg_Date = DateTime.Now;
                user.Last_Date = DateTime.Now;
                user.Delete_Date = DateTime.MaxValue;
                user.Status = 0;
                admin.CreateUser(user, 2);
                LoginViewModel login = new LoginViewModel()
                {
                    email = reg.Email,
                    password = reg.Password
                };
                Login(login);
            }
            else { return View(reg); }
            return Redirect("~/Home/Index");
        }
    }
}