using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        Auth userdata = new Auth();
        DbContext db = new DbContext();
        
        private bool AllowAnonymus { get; set; }

        public CustomAuthorizeAttribute() { }

        public CustomAuthorizeAttribute(bool allowAnonymus)
        {
            AllowAnonymus = allowAnonymus;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            int cur = userdata.curUser;
            if (AllowAnonymus)
                return true;
            var user = (from u in db.User
                        where Object.Equals(u.id, userdata.curUser)
                        select u).ToList();
            if (user.Any())
            {
                if (base.Roles == null)
                    return true;
                else
                    return Test.Models.Roles.IsInRole(user.First().id, base.Roles);
            }
            return false;
        }
        
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            filterContext.Result = new System.Web.Mvc.RedirectResult("~/Auth/Login", false);
        }

    }
}