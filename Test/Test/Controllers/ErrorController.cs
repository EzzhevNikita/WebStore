using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Unknown()
        {
            return View();
        }

        public ViewResult AccessDenied()
        {
            return View();
        }

        public ViewResult HttpError()
        {
            return View();
        }
        public ViewResult NotFound()
        {
            return View();
        }
    }
}