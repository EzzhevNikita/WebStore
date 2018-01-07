using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using NLog;

namespace Test.Models
{

    public interface IAuth
    {
        void Login(string Name, string Pass);
        void Logoff();
        int curUser { get; }
    }


    public class Auth : IAuth
    {
        DbContext db = new DbContext();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();        
        private const string cookieName = "AuthCookie";
        

        public void Login(string Name, string Pass)
        {
            try
            {//проверяем наличие пользователя в базе
                IEnumerable<User> isinBase = (from u in db.User
                                              where (u.Email == Name && u.Password == Pass)
                                              select u);
                //создание тикета, если пользователь в базе 
                if (isinBase.Any())
                {
                    isinBase.First().Last_Date = DateTime.Now;
                    db.SubmitChanges();
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    isinBase.First().id.ToString(),
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    (from u in db.UserInRole where u.id == isinBase.First().id select u.Role_id).ToString()
                    );
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    SetCookie(cookieName, encryptedTicket, DateTime.Now.AddMinutes(30));
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public void Logoff()
        {
            try
            {
                User user = (from u in db.User where u.id == curUser select u).First();
                user.Last_Date = DateTime.Now;
                db.SubmitChanges();
                SetCookie(cookieName, null, DateTime.Now.AddMilliseconds(-1));
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public int curUser
        {
            get
            {
                try
                {
                    object cookie = HttpContext.Current.Request.Cookies["AuthCookie"] != null ?
                         HttpContext.Current.Request.Cookies["AuthCookie"].Value : null;
                    if (cookie != null & !string.IsNullOrEmpty(cookie.ToString()))
                    {
                        var ticket = FormsAuthentication.Decrypt(cookie.ToString());

                        return Convert.ToInt32(ticket.Name, 16);
                    }
                }
                catch(Exception ex)
                {
                    //logger.Info(ex.Message);
                }
                return -1;
            }
        }

        public static void SetCookie(string Name, string cookieData, DateTime livetime)
        {
            try
            {
                HttpCookie cookie = HttpContext.Current.Response.Cookies[Name];
                if (cookie != null)
                {
                    cookie = new HttpCookie(Name);

                }
                cookie.Value = cookieData;
                cookie.Expires = livetime;

                HttpContext.Current.Response.SetCookie(cookie);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}