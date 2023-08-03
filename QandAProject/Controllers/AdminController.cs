using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;

namespace QandAProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        
        public ActionResult ViewPublication()
        {
            return View();
        }

        /*
         <description>
            status = 0 = Accepted
            status = 1 = Banned
            status = 2 = permanent Ban
        </description>
            */
        [HttpPost]
        public ActionResult BanPublication(int id)
        {
            using(var context = new Project1DBEntities())
            {
                context.Publications.FirstOrDefault(x => x.PublicationId == id).StatusId = 1;
                context.SaveChanges();
            }
            return null;
        }
        [HttpPost]
        public ActionResult UnBanPublication(int id)
        {
            using (var context = new Project1DBEntities())
            {
                context.Publications.FirstOrDefault(x => x.PublicationId == id).StatusId = 0;
                context.SaveChanges();
            }
            return null;
        }
        public ActionResult BanUser()
        {
            return null;
        }

    }
}