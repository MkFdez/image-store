using System;
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
        public IServicePack ServicePack;
        public PublicationController(IServicePack servicePack)
        {
            ServicePack = servicePack;
        }
        public ActionResult Test()//controller created just for testing new ui elements without messing everything out
        {
            return View();
        }


        // GET: Publication            
        public async Task<ActionResult> Index(string category = "", string search = "", bool personalPage = false)
        {
            using (var context = new Project1DBEntities())
            {
                Dictionary<int, string> categories = await ServicePack.GetCategories();
               
                ViewBag.Categories = categories;
                ViewBag.Search = search;
                ViewBag.MyGallery = personalPage.ToString();
                ViewBag.Title = "Publications";
                ViewBag.Category = category; 
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

                    ViewBag.Comments = 10;
                    ViewBag.imgWidth = MkImage.GetWidth(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid + "/"), publication.Filename));
                    ViewBag.imgHeight = MkImage.GetHeight(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid + "/"), publication.Filename));

                    return View(publication.Publication);
                }
                catch
                {
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
                    string fileName = p.Path.Substring(p.Path.LastIndexOf("/") + 1);
                    string pGuid = p.Guid;
                    int intScale = int.Parse(scale);
                    string guid = Guid.NewGuid().ToString();
                    string newName = guid + fileName.Substring(fileName.LastIndexOf("."));
                    ImageManager<int> myDelegate = new ImageManager<int>(MkImage.Resize);
                    myDelegate(Path.Combine(Server.MapPath("~/ImageVault/" + pGuid + "/"), fileName), intScale, Path.Combine(Server.MapPath("~/TempData"), newName));
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/TempData"), newName));
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, newName);
                }
                catch
                {
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
                    string fileName = path.Substring(path.LastIndexOf(@"/") + 1);
                    path = path.Substring(0, path.LastIndexOf("/LowRes/"));
                    string file = Path.Combine(Directory.GetFiles(Server.MapPath("~" + path + "/FreeTrial/"), fileName));
                    string guid = Guid.NewGuid().ToString();
                    string newName = guid + fileName.Substring(fileName.LastIndexOf("."));

                    byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, newName);
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
        }
        // GET: Publication/Create
        [Authorize]
        public async Task<ActionResult> Create()
        {
          
            Dictionary<int, string> categories = await ServicePack.GetCategories();               
            ViewBag.Categories = categories;
          
            return View();
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
                    int userId = User.Identity.GetUserId<int>(); ;
                    
                        
                        var random = new Random();
                        userId = random.Next(10, 18);
                        Publication publication = new Publication
                        {
                            UserId = userId,
                            Content = model.Content,
                            HeaderPath = "temp",
                            DateOfCreated = DateTime.Now,
                            Guid = Guid.NewGuid().ToString(),
                            Price = price,
                            Previous_Price = price,
                            For_Sale = false


                        };
                       
                        int[] categories = new int[0];
                        
                       if (collection["AreChecked"] != null)
                       {
                           categories = Array.ConvertAll(collection["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString()));
                       }

                       if (model.Picture != null && model.Picture.ContentLength > 0)
                       {
                           Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads"), publication.PublicationId.ToString()));
                           Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString()), "FreeTrial"));
                           Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString()), "LowRes"));
                           Directory.CreateDirectory(Path.Combine(Server.MapPath("~/ImageVault/"), publication.Guid));
                           var fileName = Path.GetFileName(model.Picture.FileName).Replace(" ", "");
                           var path = Path.Combine(Server.MapPath("~/uploads"), fileName);

                           string fl = path.Substring(path.LastIndexOf("\\"));
                           string[] split = fl.Split('\\');
                           string newpath = split[1];
                           string imagepath = "/uploads/" + publication.PublicationId.ToString() + "/LowRes/" + newpath;
                           model.Picture.SaveAs(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid + "/"), fileName));
                           publication.HeaderPath = imagepath;
                           ImageManager<string> myDelegate = new ImageManager<string>(MkImage.setWatermarkText);
                           myDelegate(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid), fileName), "my website", Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString() + "/" + "FreeTrial/"), fileName));
                           ImageManager<int> myDelegate2 = new ImageManager<int>(MkImage.Resize);
                           myDelegate2(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid), fileName), 50, Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString() + "/" + "LowRes/"), fileName));

                       }
                       
                        await ServicePack.AddPublication(publication, categories);

                    
                }
                return RedirectToAction("Index");
            }
            return View();
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

        
        
        public async Task<ActionResult> ChangePage(int actualPage = 1, FormCollection c = null, string search = "", bool personalPage = false, string category = "")
        {
            List<PublicationViewModel> model = new List<PublicationViewModel>();
            using (var context = new Project1DBEntities())
            {
                var userId = HttpContext.User.Identity.GetUserId<int>();
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate1;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate2;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate3;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate;
                if (personalPage) { predicate = x => true; } else { predicate = x => x.StatusId == 0; } 
                if (search != "") { predicate2 = x => x.Content.Contains(search); } else { predicate2 = x => true; }
                var a = c["AreChecked"] != null ? Array.ConvertAll(c["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString())).ToList() : new List<int>() { };
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
                model = await ServicePack.GetSomePublications(actualPage, predicate1);
                TempData["Count"] = await ServicePack.PublicationCount(predicate1);
            }
            

            var realList = new PublicationsListViewModel(model);
            ViewBag.Search = search;
            ViewBag.MyGallery = personalPage;
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
