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
using DataAccess.Models;
using Microsoft.AspNet.Identity;
using Stripe;
using Stripe.Checkout;

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
        [HttpPost]
        public ActionResult BuyImage(int pubId, string image, string name)
        {
            image = "https://localhost:44307" + image;
            StripeConfiguration.ApiKey = "sk_test_51Mr0ToLe6PSFHqVPPNKxsAFQFCSPhDLAVRcXklrag36qXfzNfrRyhifFhhaEII0s6CjP2qzUCaEQDq30wx69EyWQ00hL9klgeQ";
            var domain = "https://localhost:44307/sell";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                      PriceData = new SessionLineItemPriceDataOptions
                      {
                    
                          UnitAmount = 30000,
                          Currency = "usd",
                          ProductData = new SessionLineItemPriceDataProductDataOptions
                          {
                              Name = name,
                              Images = new List<string>(){  "https://th.bing.com/th/id/OIG.wRYxTlsIMpBdRssC7263?w=270&h=270&c=6&r=0&o=5&dpr=1.3&pid=ImgGn" },
                          },
                      },                    
                    Quantity = 1,
                   
                    
                  },
                },

                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            //BuyImageModel model = new BuyImageModel() { UserId = User.Identity.GetUserId<int>(), PublicationId = pubId };
            return new HttpStatusCodeResult(303);
        }

        
        public ActionResult BuyImage(BuyImageModel model)
        {
            if (ModelState.IsValid)
            {
                

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
        
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Cancel()
        {
            return View();
        }
    }
}