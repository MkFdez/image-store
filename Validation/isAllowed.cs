
using System;
using DataAccess;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
namespace Validation
{
    public class isAllowed : ActionFilterAttribute
    {
        private string[] allowedRoles;
        public isAllowed(string[] roles)
        {
            allowedRoles = roles;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (var context = new Project1DBEntities())
            {
                HttpCookie cookie = filterContext.RequestContext.HttpContext.Request.Cookies["vals"];
                int userId = int.Parse(cookie["userid"]);
                //if (!allowedRoles.ToList().Contains(context.Users.FirstOrDefault(x => x.UserId == userId).AspNetRoles.Description))
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                //}

            }
            base.OnActionExecuting(filterContext);
        }
    }

}



