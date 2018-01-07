using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Test.Models;
using NLog;
using Test.Controllers;



namespace Test
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MailSender mail = new MailSender();
            mail.Start();
            AccountDisable acc = new AccountDisable();
            acc.Start();
        }

        public void Application_Error(Object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var currentController = string.Empty;
            var currentAction = string.Empty;
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (!string.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (!string.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }
            var ex = Server.GetLastError();

            logger.Error(ex.Message);

            var controller = new ErrorController();
            var routeData = new RouteData();

            var action = "Unknown";
            if (ex is HttpException)
            {
                switch (((HttpException)ex).GetHttpCode())
                {
                    case 403:
                        action = "AccessDenied";
                        break;
                    case 404:
                        action = "NotFound";
                        break;
                    default:
                        action = "HttpError";
                        break;
                }
            }
            httpContext.ClearError();
            httpContext.Response.Clear();

            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }
    }
}