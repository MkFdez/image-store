using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QandAProject.Models;
using DataRepository;
using Aplications;
using Autofac;
using Dependencies;
using Microsoft.AspNet.Identity;

namespace QandAProject.Controllers
{
    public class SellController : Controller
    {
        // GET: Sell
        public IApp _app { get; set; }
        public SellController(IApp app)
        {
            _app = app;
        }
        public ActionResult BuyImage(int pubId)
        {
            BuyImageModel model = new BuyImageModel() { UserId = User.Identity.GetUserId<int>(), PublicationId = pubId };
            return View(model);
        }

        [HttpPost]
        public ActionResult BuyImage(BuyImageModel model)
        {
            if (ModelState.IsValid)
            {
                //buy stuff bla bla bla bla bla bla bla 
                using (var context = new Project1DBEntities())
                {
                    User user = context.Users.FirstOrDefault(x => x.UserId == model.UserId);
                    Publication pub = context.Publications.FirstOrDefault(y => y.PublicationId == model.PublicationId);
                    user.PurPublication.Add(pub);
                    context.SaveChanges();
                    BuyEmailModel info = new BuyEmailModel(){ UserEmail = user.Email, UserName = user.UserName, PubId = pub.PublicationId, PubName = pub.Content };
                
                    _app.Model = info;
                    _app.Run();
                    
                }
            }
            else
            {
                Console.WriteLine(ModelState.Values);
            }
            return RedirectToAction("View", "Publication", new { id = model.PublicationId });


        }
    }
}