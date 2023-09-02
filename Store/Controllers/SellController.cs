using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Store.Models;
using DataAccess;
using Aplications;
using Autofac;
using Dependencies;
using Models;
using Microsoft.AspNet.Identity;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using Services;

namespace Store.Controllers
{
    public class SellController : Controller
    {
        public IServicePack ServicePack;
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("SellLogger");

        public SellController(IServicePack servicePack)
        {
            ServicePack = servicePack;
        }
        // GET: Sell
        public IApp _app { get; set; }
        public SellController(IApp app)
        {
            _app = app;
        }
        [HttpPost]
        public async Task<ActionResult> BuyImage(int pubId, string image, string name)
        {
            try
            {
                var publication = await ServicePack.GetPublication(pubId);
                long price = (long)(publication.Publication.Price * 100);
                if (price == 0)
                {
                    return RedirectToAction("Success", new { puid = pubId });
                }
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

                          UnitAmount = price,
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
                    SuccessUrl = domain + "/success?puid=" + pubId,
                    CancelUrl = domain + "/cancel",
                };
                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                //
                return new HttpStatusCodeResult(303);
            }catch(Exception ex)
            {
                log.Error("Error opening creating stripe session", ex);
                return new HttpStatusCodeResult(400);
            }
        }

        
        public ActionResult BuyImage(BuyImageModel model)
        {
            if (ModelState.IsValid)
            {
                

                using (var context = new Project1DBEntities())
                {
                    User user = context.Users.FirstOrDefault(x => x.Id == model.UserId);
                    Publication pub = context.Publications.FirstOrDefault(y => y.PublicationId == model.PublicationId);
                    user.SalesHistories.Add(new SalesHistory()
                    {
                        AspNetUser = user,
                        Date = DateTime.Now,
                        Publication = pub,
                        Amount = pub.Price
                    }
                        );
                    context.DailySales.Add(new DailySale()
                    {
                        AspNetUser = pub.User,
                        TotalAmount = pub.Price,
                        Date = DateTime.Now,
                    });
                    context.SaveChanges();
                    BuyEmailModel info = new BuyEmailModel(){ UserEmail = user.Email, UserName = user.UserName, PubId = pub.PublicationId, PubName = pub.Content };
                
                   
                    
                }
            }
            else
            {
                log.Info("Model State Not valid");
            }
            return RedirectToAction("View", "Publication", new { id = model.PublicationId });


        }
        
        public ActionResult Success(int puid)
        {
            using (var context = new Project1DBEntities())
            {
                int userId = User.Identity.GetUserId<int>();
                User user = context.Users.FirstOrDefault(x => x.Id == userId);
                Publication pub = context.Publications.FirstOrDefault(y => y.PublicationId == puid);
                pub.Downloads++;
                BuyEmailModel info = new BuyEmailModel() { UserEmail = user.Email, UserName = user.UserName, PubId = pub.PublicationId, PubName = pub.Content };
                user.SalesHistories.Add(new SalesHistory()
                {
                    AspNetUser = user,
                    Date = DateTime.Now,
                    Publication = pub,
                    Amount = pub.Price
                }
                         );
                context.DailySales.Add(new DailySale()
                {
                    AspNetUser = pub.User,
                    TotalAmount = pub.Price,
                    Date = DateTime.Now,
                });
                _app.Model = info;
                _app.Run();
                context.SaveChanges();
                return View();
            }
        }
        public ActionResult Cancel()
        {
            return View();
        }
    }
}