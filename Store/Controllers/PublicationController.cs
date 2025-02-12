﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text.Json;
using System.Web.Mvc;
using Store.Models;
using System.IO;
using MyFilter;
using MkImages;
using Microsoft.AspNet.Identity;
using DataAccess;
using System.Threading.Tasks;
using Models;
using Services;
using System.Diagnostics;

namespace Store.Controllers
{
    delegate void ImageManager<T>(string path, T scaleOrName, string returnPath);

    public class PublicationController : Controller
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("PublicationLogger");
        public IStorageService storageService;
        public IServicePack ServicePack;
        public PublicationController(IServicePack servicePack, IStorageService _storageService)
        {
            ServicePack = servicePack;
            storageService = _storageService;
        }
        public ActionResult Test()//controller created just for testing new ui elements without messing everything out
        {
            return View();
        }


        // GET: Publication            
        public async Task<ActionResult> Index(string category = "", string search = "", bool personalPage = false, bool onlyForMe = false)
        {
            using (var context = new Project1DBEntities())
            {
                Dictionary<int, string> categories = await ServicePack.GetCategories();
               
                ViewBag.Categories = categories;
                ViewBag.Search = search;
                ViewBag.MyGallery = personalPage.ToString();
                ViewBag.Title = "Publications";
                ViewBag.Category = category;
                ViewBag.OrderBy = OrderByModel.DateDesc;
                ViewBag.OnlyForMe = onlyForMe;
            }
            return View();
        }

        // GET: Publication/Details/5
        public async Task<ActionResult> View(int id)
        {
            using(var context = new Project1DBEntities())
            {
                try
                {
                    var publication = await ServicePack.GetPublication(id);
                    if(publication.Publication.OnlyFor != null && publication.Publication.OnlyFor != HttpContext.User.Identity.GetUserId<int>())
                    {
                        log.Warn($"User {HttpContext.User.Identity.GetUserId<int>()} tried to open publication {id}");
                        return RedirectToAction("Index");
                    }
                    ViewBag.Comments = 10;
                    ViewBag.imgWidth = MkImage.GetWidth(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid + "/"), publication.Filename));
                    ViewBag.imgHeight = MkImage.GetHeight(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid + "/"), publication.Filename));

                    return View(publication.Publication);
                }
                catch(Exception ex)
                {
                    log.Error("Error opening publication", ex);
                    return RedirectToAction("Index");
                }
            }
            
        }
        [Authorize]
        
        public async Task<ActionResult> Download(string scale, string pubid )
        {
            int id = int.Parse(pubid);
            int userid = User.Identity.GetUserId<int>();
            using (var context = new Project1DBEntities())
            {
                try
                {
                    if (await ServicePack.HasPublication(id) == false)
                    {
                        return RedirectToAction("View", "Publication", new { id = id });
                    }
                    var p = await ServicePack.GetPublicationToDownload(id);
                    var directory = storageService.GenerateDownloadDirectory(p, MkImage.Resize, scale);
                    return File(directory.File, System.Net.Mime.MediaTypeNames.Application.Octet, directory.FileName);
                }
                catch(Exception ex)
                {
                    log.Error($"Error downloading image with pubblicationid:{pubid} at scale:{scale}", ex);
                    return RedirectToAction("Index");
                }
            }
        }

        

        public async Task<ActionResult> DownloadFreeTry(string id)
        {
            using (var context = new Project1DBEntities())
            {
                try
                {
                    int puid = int.Parse(id);
                    string path = await ServicePack.GetImagePath(puid);
                    var directory = storageService.GetDownloadFreeDirectory(path);
                    return File(directory.File, System.Net.Mime.MediaTypeNames.Application.Octet, directory.FileName);
                }
                catch(Exception ex)
                {
                    log.Error($"Error downloading watermarked image with pubblicationid:{id}", ex);
                    return RedirectToAction("Index");
                }
            }
        }
        // GET: Publication/Create
        [Authorize]
        public async Task<ActionResult> Create(string onlyFor = null)
        {
          
            Dictionary<int, string> categories = await ServicePack.GetCategories();               
            ViewBag.Categories = categories; 
            return View(new PublicationCreateViewModel() { OnlyFor = onlyFor});
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Like(int publicationid, bool like)
        {
            try
            {
                await ServicePack.Like(like, publicationid);
            }catch(Exception e)
            {
                log.Error($"Error {e} with like action in publication {publicationid}");
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            return new HttpStatusCodeResult(200);
        }

        // POST: Publication/Create
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(PublicationCreateViewModel model,  FormCollection collection)
        {
            if(ModelState.IsValid)
            {

                using (var context = new Project1DBEntities())
                {
                   
                    decimal price = decimal.Parse(model.Price);
                    int userId = User.Identity.GetUserId<int>();
                    Publication publication = new Publication
                    {
                        UserId = userId,
                        Content = model.Content,
                        HeaderPath = "temp",
                        DateOfCreated = DateTime.Now,
                        Guid = Guid.NewGuid().ToString(),
                        Price = price,
                        Previous_Price = price,
                        For_Sale = false,
                        OnlyFor = context.Users.FirstOrDefault(x => x.UserName == model.OnlyFor).Id

                    };
                       
                    int[] categories = new int[0];
                        
                    if (collection["AreChecked"] != null)
                    {
                        categories = Array.ConvertAll(collection["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString()));
                    }
                    try
                    {
                        if (model.Picture != null && model.Picture.ContentLength > 0)
                        {
                            
                            publication.HeaderPath = storageService.NewImageDirectory(model.Picture, publication.PublicationId, publication.Guid, MkImage.Resize, MkImage.setWatermarkText);
                        }

                        await ServicePack.AddPublication(publication, categories);
                    }catch(Exception ex)
                    {
                        log.Error($"Error adding storing new image {model.Picture.FileName}", ex);
                    }
                    
                }
                return Json(new { redirect = Url.Action("Index") });
            }
            else
            {
                log.Info("Model State Not valid");
            }
            return Json(new { error = new { } });
        }

        // GET: Publication/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Publication/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Publication/Delete/5
        public void DeleteTempData(string path, string scale)
        {
            string fileName = path.Substring(path.LastIndexOf(@"/") + 1);
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/TempData"), scale + fileName));
        }

        
        
        public async Task<ActionResult> ChangePage(int actualPage = 1, FormCollection c = null, string search = "", bool personalPage = false, string category = "", OrderByModel ob = OrderByModel.DateDesc, List<int> categories = null, bool onlyForMe = false)
        {
            List<PublicationViewModel> model = new List<PublicationViewModel>();
            var dfh = c["order"];
            List<int> a = categories != null ? categories : new List<int>();
            using (var context = new Project1DBEntities())
            {
                var userId = HttpContext.User.Identity.GetUserId<int>();
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate1;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate2;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate3;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate;
                System.Linq.Expressions.Expression<Func<Publication, dynamic>> order;
                if (personalPage) { predicate = x => true; } else { predicate = x => x.StatusId == 0; } 
                if (search != "") { predicate2 = x => x.Content.Contains(search); } else { predicate2 = x => true; }
                a = c["AreChecked"] != null ? Array.ConvertAll(c["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString())).ToList() : a;
                predicate2 = predicate.And(predicate2);
                if (category == "")
                {
                    if (System.Web.HttpContext.Current.User.IsInRole("Standard") || !HttpContext.User.Identity.IsAuthenticated)
                    {

                        if (c["AreChecked"] != null)
                        {
                            predicate1 = x => x.StatusId == 0 && a.Any(y => x.Categories.Any(z => z.CategoryId == y));

                        }
                        else
                        {
                            predicate1 = x => x.StatusId == 0;
                        }
                        predicate1 = predicate1.And(predicate2);

                    }
                    else
                    {
                        if (c["AreChecked"] != null)
                        {
                            predicate1 = x => a.Any(y => x.Categories.Any(z => z.CategoryId == y));
                        }
                        else
                        {
                            predicate1 = x => true;
                        }
                        predicate1 = predicate1.And(predicate2);

                    }
                    if (personalPage)
                    {


                        int id = User.Identity.GetUserId<int>();
                        System.Linq.Expressions.Expression<Func<Publication, bool>> predicate4 = x => x.SalesHistories.Any(y => y.UserId == id);
                        predicate1 = predicate1.And(predicate4);


                    }
                }
                else
                {
                    int catId = int.Parse(category);
                    predicate1 = x => x.StatusId == 0 && x.Categories.Any(z => z.CategoryId == catId);
                }
                if (c["MinPrice"] != null)
                {
                    if (c["MinPrice"] != "" && c["MaxPrice"] != "")
                    {
                        int min = int.Parse(c["MinPrice"]);
                        int max = int.Parse(c["MaxPrice"]);
                        predicate3 = x => x.Price >= min && x.Price <= max;
                        predicate1 = predicate1.And(predicate3);
                    }
                    else if (c["MinPrice"] != "")
                    {
                        int min = int.Parse(c["MinPrice"]);
                        predicate3 = x => x.Price >= min;
                        predicate1 = predicate1.And(predicate3);

                    }
                    else if (c["MaxPrice"] != "")
                    {
                        int max = int.Parse(c["MaxPrice"]);
                        predicate3 = x => x.Price <= max;
                        predicate1 = predicate1.And(predicate3);

                    }
                }
                OrderByModel orderby = ob;

                if(c["order"]!= null)
                {
                    orderby = (OrderByModel)Enum.Parse(typeof(OrderByModel), c["order"]);
                }
                System.Linq.Expressions.Expression<Func<Publication, bool>> _onlyForMe = x => x.OnlyFor == HttpContext.User.Identity.GetUserId<int>();

                if (!onlyForMe)
                {
                    _onlyForMe=  x => x.OnlyFor == null;
                }
                predicate1 = predicate1.And(_onlyForMe);

                ViewBag.OrderBy = orderby;
                
                model = await ServicePack.GetSomePublications(actualPage, predicate1, orderby);
                TempData["Count"] = await ServicePack.PublicationCount(predicate1);
            }
            

            var realList = new PublicationsListViewModel(model);
            ViewBag.Search = search;
            ViewBag.Categories = a;
            ViewBag.MyGallery = personalPage;
            ViewBag.OnlyForMe = onlyForMe;
            TempData["ActualPage"] = actualPage ;
            return PartialView("~/Views/Publication/_PublicationLists.cshtml", realList);
        }

        public ActionResult Manage()
        {
            var model = ServicePack.GetForManage();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int pubId)
        {
            bool ok = ServicePack.DeletePublication(pubId);
            if (!ok)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
    }

     


}
