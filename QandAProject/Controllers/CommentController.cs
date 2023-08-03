using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Text.Json;
using QandAProject.Models;
using DataAccess;
using Microsoft.AspNet.Identity;
using Models;

namespace QandAProject.Controllers
    
{
    
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Comment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: Comment/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(string content, int publicationId)
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
          
            using (var context = new Project1DBEntities())
            {
                
                int userid = User.Identity.GetUserId<int>();
                Comment comment = new Comment()
                {
                    UserId = userid,
                    Content = content,
                    DateOfCreated = DateTime.Now,
                    PublicationId = publicationId,
                };
                context.Comments.Add(comment);
                context.SaveChanges();
                return Json(JsonSerializer.Serialize<CommentModel>(new CommentModel()
                {
                    UserName = System.Web.HttpContext.Current.User.Identity.Name,
                    DaysSinceCreated = (DateTime.Now - comment.DateOfCreated).Days,
                    Content = comment.Content,
                })) ;
            }
            
        }
        [HttpPost]
        public ActionResult MoreComments(int actual)
        {
            using (var context = new Project1DBEntities())
            {
                List<CommentModel> comments = new List<CommentModel>();
                
                foreach (var c in context.Comments.OrderBy(x => x.CommentId).Skip(actual).Take(10))
                {
                    comments.Add(new CommentModel()
                    {
                        Content = c.Content,
                        DaysSinceCreated = (DateTime.Now - c.DateOfCreated).Days,
                        ProfilePicture = c.User.ProfilePicture.Image,
                        UserName = c.User.UserName,
                    });
                }
                return Json(JsonSerializer.Serialize<List<CommentModel>>(comments));

            }
        }

        // GET: Comment/Delete/5

    }
}
