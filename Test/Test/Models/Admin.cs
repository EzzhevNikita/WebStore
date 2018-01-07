using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace Test.Models
{
    public class Admin
    {
        DbContext db = new DbContext();
        Auth auth = new Auth();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IEnumerable<AdminPanelViewModel> ReceiveUsers()
        {
            try
            {
                List<AdminPanelViewModel> adminusers = new List<AdminPanelViewModel>();
                IEnumerable<User> users = (from u in db.User where u.id != auth.curUser select u);
                var userinrole = (from uir in db.UserInRole where uir.User_id != auth.curUser select uir).ToList();
                foreach (User u in users)
                {
                    var adminuser = new AdminPanelViewModel()
                    {
                        id = u.id,
                        First_Name = u.First_Name,
                        Last_Name = u.Last_Name,
                        Email = u.Email,
                        Reg_Date = u.Reg_Date,
                        Last_Date = u.Last_Date,
                        Delete_Date = u.Delete_Date,
                        Status = u.Status,
                        Role = (from r in db.Role where r.id == (from d in userinrole where u.id == d.User_id select d.Role_id).First() select r.Role_Name).First().ToString()
                    };
                    adminusers.Add(adminuser);
                }
                return adminusers;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + auth.curUser.ToString() );
                return null;
            }
        }

        public AdminPanelViewModel ReceiveUserById(int id)
        {
            try
            {
                User user = (from u in db.User where u.id == id select u).First();
                int userinrole = (from uir in db.UserInRole where user.id == uir.User_id select uir.Role_id).First();
                var adminuser = new AdminPanelViewModel()
                {
                    id = user.id,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Email = user.Email,
                    Reg_Date = user.Reg_Date,
                    Last_Date = user.Last_Date,
                    Delete_Date = user.Delete_Date,
                    Status = user.Status,
                    Role = (from r in db.Role where r.id == userinrole select r.Role_Name).First().ToString()
                };
                return adminuser;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + auth.curUser.ToString());
                return null;
            }
        }

        public User RecieveUserByEmail(string email)
        {
            try
            {
                User user = (from u in db.User where u.Email == email select u).First();
                return user;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + email);
                return null;
            }
        }

        public void CreateUser(User user, int role_id)
        {
            try
            {
                db.User.InsertOnSubmit(user);
                db.SubmitChanges();
                User_in_Role usinrole = new User_in_Role() { User_id = user.id, Role_id = role_id };
                db.UserInRole.InsertOnSubmit(usinrole);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void CreateUser(AdminCreateUserViewModel user)
        {
            try
            {
                User newuser = new User()
                {
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Email = user.Email,
                    Password = user.Password,
                    Reg_Date = DateTime.Now,
                    Last_Date = DateTime.Now,
                    Delete_Date = DateTime.MaxValue,
                    Status = 0,
                };
                db.User.InsertOnSubmit(newuser);
                db.SubmitChanges();

                User_in_Role usinrole = new User_in_Role()
                {
                    User_id = newuser.id,
                    Role_id = (from r in db.Role where r.Role_Name == user.Role select r.id).First()
                };
                db.UserInRole.InsertOnSubmit(usinrole);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void Freeze(AdminPanelViewModel userToDel)
        {
            try
            {
                User user = (from u in db.User where u.id == userToDel.id select u).First();
                user.Status = 1; // устанавливаем статус аккаунта на 1 (удалён), для последубщей возможности его восстановления пользователем
                user.Delete_Date = DateTime.Now;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + userToDel.id.ToString());
            }
        }

        public void Restore(AdminPanelViewModel userToReStore)
        {
            try
            {
                User user = (from u in db.User where u.id == userToReStore.id select u).First();
                user.Status = 0; // устанавливаем статус аккаунта на 0 (активен)
                user.Delete_Date = DateTime.MaxValue;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + userToReStore.id.ToString());
            }
        }


        public void SaveChanges(AdminPanelViewModel user)
        {
            try
            {
                User us = (from u in db.User where user.id == u.id select u).First();
                us.First_Name = user.First_Name;
                us.Last_Name = user.Last_Name;
                us.Email = user.Email;
                db.SubmitChanges();

                User_in_Role uir = (from i in db.UserInRole where user.id == i.User_id select i).First();
                uir.Role_id = (from r in db.Role where r.Role_Name == user.Role select r.id).First();
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + user.id);
            }
        }

        public void ClearBusket()
        {
            try
            {
                List<Basket> cart = (from b in db.Basket where auth.curUser == b.User_id select b).ToList();
                db.Basket.DeleteAllOnSubmit(cart);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + auth.curUser.ToString());
            }
        }

        public void Restore(RestoreViewModel user)
        {
            try
            {
                User userToChange = (from u in db.User where u.Email == user.email select u).First();
                userToChange.Status = 0;
                userToChange.Password = user.password;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " for user:" + user.email);
            }
        }
    }
}