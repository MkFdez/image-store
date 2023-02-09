using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Validation
{
    public class FromSignUp : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            //base.OnAuthorization(filterContext);
            if(filterContext.HttpContext != null)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Home", action = "Index" }));
                    return;
                }
                string a = String.Join("",filterContext.HttpContext.Request.UrlReferrer.ToString().Take(38));
                if (a != "https://localhost:44307/Account/Signup")
                {
                    filterContext.Result = new RedirectToRouteResult(new
           RouteValueDictionary(new { controller = "Home", action = "Index" }));
                }
            }
        }
    }
}
