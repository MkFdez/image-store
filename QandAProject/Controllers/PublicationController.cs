using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text.Json;
using System.Web.Mvc;
using DataRepository;
using QandAProject.Models;
using System.IO;
using MyFilter;
using MkImages;
using Microsoft.AspNet.Identity;

namespace QandAProject.Controllers
{
    delegate void ImageManager<T>(string path, T scaleOrName, string returnPath);
    public class PublicationController : Controller
    {
        // GET: Publication            
        public ActionResult Index(string search = "", bool personalPage = false)
        {
            using (var context = new Project1DBEntities())
            {
                var categories = context.Categories;
                Dictionary<int, string> values = new Dictionary<int, string>();
                foreach (var cat in categories)
                {
                    values.Add(cat.CategoryId, cat.CategoryName);
                    
                }
                ViewBag.Categories = values;
                ViewBag.Search = search;
                ViewBag.MyGallery = personalPage.ToString();
            }
            return View();
        }

        // GET: Publication/Details/5
        public ActionResult View(int id)
        {
            using(var context = new Project1DBEntities())
            {
                
                var temp = context.Publications.FirstOrDefault(x => x.PublicationId == id);
                var comments = new List<CommentModel>();
                temp.Comments1.ToList().ForEach(x => comments.Add(new CommentModel()
                {
                    DateOfCreated = x.DateOfCreated,
                    UserName = x.User.UserName,
                    Content = x.Content,

                }));
                bool isBuyed = false;
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated )
                {
                    int userid = User.Identity.GetUserId<int>();
                    if (context.Users.FirstOrDefault(x => x.UserId == userid).PurPublication.Any(x => x.PublicationId == id)){
                        isBuyed = true;
                    }
                }
                string path = Server.MapPath("~" + temp.HeaderPath);
                string fileName = path.Substring(path.LastIndexOf(@"\") + 1);
                var publication = new PublicationViewModel()
                {
                    id = id,
                    User = temp.User.UserName,
                    DateOfCreated = temp.DateOfCreated.ToShortDateString(),
                    headerPath = temp.HeaderPath,
                    Content = temp.Content,
                    Comments = comments.Take(10).ToList(),
                    Categories = temp.Categories.Select(x => x.CategoryName).ToList(),
                    Status = temp.Status.Description,
                    isBuyed = isBuyed,
                };

                ViewBag.Comments = 10;
                ViewBag.imgWidth = MkImage.GetWidth(Path.Combine(Server.MapPath("~/ImageVault/" + temp.Guid + "/"), fileName));
                ViewBag.imgHeight = MkImage.GetHeight(Path.Combine(Server.MapPath("~/ImageVault/" + temp.Guid + "/"), fileName));

                return View(publication);
            }
            
        }
        [Authorize]
        
        public ActionResult Download(string scale, string pubid )
        {
            int id = int.Parse(pubid);
            int userid = User.Identity.GetUserId<int>();
            using (var context = new Project1DBEntities())
            {

                if (!context.Users.FirstOrDefault(x => x.UserId == userid).PurPublication.Any(x => x.PublicationId == id))
                {
                    return RedirectToAction("View", "Publication", new { id = id });
                }

                var p = context.Publications.FirstOrDefault(x => x.PublicationId == id);
                string fileName = p.HeaderPath.Substring(p.HeaderPath.LastIndexOf("/")+1);
                string pGuid = p.Guid; 
                int intScale = int.Parse(scale);
                string guid = Guid.NewGuid().ToString();
                string newName = guid + fileName.Substring(fileName.LastIndexOf("."));
                ImageManager<int> myDelegate = new ImageManager<int>(MkImage.Resize);
                myDelegate(Path.Combine(Server.MapPath("~/ImageVault/" + pGuid + "/"), fileName), intScale, Path.Combine(Server.MapPath("~/TempData"), newName));
                byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/TempData"), newName));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, newName);
            }
        }

        

        public FileResult DownloadFreeTry(string id)
        {
            using (var context = new Project1DBEntities())
            {
                int puid = int.Parse(id);
                string path = context.Publications.FirstOrDefault(x => x.PublicationId == puid).HeaderPath;
                string fileName = path.Substring(path.LastIndexOf(@"/") + 1);
                path = path.Substring(0, path.LastIndexOf("/LowRes/"));
                string file = Path.Combine(Directory.GetFiles(Server.MapPath("~" + path + "/FreeTrial/"), fileName));
                string guid = Guid.NewGuid().ToString();
                string newName = guid + fileName.Substring(fileName.LastIndexOf("."));
               
                byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, newName);
            }
        }
        // GET: Publication/Create
        [Authorize]
        public ActionResult Create()
        {
            using(var context = new Project1DBEntities())
            {
                var categories = context.Categories;
                Dictionary<int, string> values = new Dictionary<int, string>();
                foreach (var cat in categories)
                {
                    values.Add(cat.CategoryId, cat.CategoryName);
                }
                ViewBag.Categories = values;
            }
            return View();
        }

        // POST: Publication/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(PublicationCreateViewModel model,  FormCollection collection)
        {
            if(ModelState.IsValid)
            {
               
                using (var context = new Project1DBEntities())
                {

                    int userId = User.Identity.GetUserId<int>(); ;
                    Publication publication = new Publication
                    {
                        UserId = userId,
                        Content = model.Content,
                        HeaderPath = "temp",
                        DateOfCreated = DateTime.Now,
                        Guid = Guid.NewGuid().ToString(),
                       
                    };
                    if (collection["aAreChecked"] != null)
                    {
                        var a = Array.ConvertAll(collection["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString())).ToList();
                        context.Categories.Where(x => a.Contains(x.CategoryId)).ToList().ForEach(y => publication.Categories.Add(y));
                    }
                    context.Publications.Add(publication);
                    context.SaveChanges();
                    if (model.Picture != null && model.Picture.ContentLength > 0)
                    {
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads"), publication.PublicationId.ToString()));
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads/"+ publication.PublicationId.ToString()), "FreeTrial"));
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/uploads/"+ publication.PublicationId.ToString()), "LowRes"));
                        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/ImageVault/"), publication.Guid));
                        var fileName = Path.GetFileName(model.Picture.FileName).Replace(" ", "");
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        
                        string fl = path.Substring(path.LastIndexOf("\\"));
                        string[] split = fl.Split('\\');
                        string newpath = split[1];
                        string imagepath = "/uploads/"+ publication.PublicationId.ToString()+"/LowRes/" + newpath;
                        model.Picture.SaveAs(Path.Combine(Server.MapPath("~/ImageVault/"+ publication.Guid + "/"), fileName));
                        publication.HeaderPath = imagepath;
                        context.SaveChanges();
                        ImageManager<string> myDelegate = new ImageManager<string>(MkImage.setWatermarkText);
                        myDelegate(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid), fileName), "my website", Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString()+"/"+ "FreeTrial/"), fileName));
                        ImageManager<int> myDelegate2 = new ImageManager<int>(MkImage.Resize);
                        myDelegate2(Path.Combine(Server.MapPath("~/ImageVault/" + publication.Guid), fileName), 50, Path.Combine(Server.MapPath("~/uploads/" + publication.PublicationId.ToString() + "/" + "LowRes/"), fileName));
                    }

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

        // POST: Publication/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
        public ActionResult ChangePage(int actualPage = 1, FormCollection c = null, string search = "", bool personalPage = false)
        {
            List<PublicationViewModel> model = new List<PublicationViewModel>();
            using (var context = new Project1DBEntities())
            {
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate1;
                System.Linq.Expressions.Expression<Func<Publication, bool>> predicate2;
                if (search != "") { predicate2 = x => x.Content.Contains(search); } else { predicate2 = x => true; }

                

                var a = c["AreChecked"] != null ? Array.ConvertAll(c["AreChecked"].ToString().Split(','), x => int.Parse(x.ToString())).ToList() : new List<int>() { };


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
                if (!personalPage)
                {
                    foreach (var x in context.Publications.Where(predicate1).OrderBy(x => x.PublicationId).Skip((actualPage - 1) * 9).Take(9)) {
                        model.Add(new PublicationViewModel
                        {
                            id = x.PublicationId,
                            headerPath = x.HeaderPath,
                            Content = x.Content,
                            User = x.User.UserName,
                            DateOfCreated = x.DateOfCreated.ToShortDateString(),
                            Status = x.Status.Description,
                        });
                    }
                    TempData["Count"] = context.Publications.Count();
                }
                else
                {
                    
                    int id = User.Identity.GetUserId<int>();
                    System.Linq.Expressions.Expression<Func<Publication, bool>> predicate3 = x => x.OwnerUser.Any(y => y.UserId == id);
                    predicate1 = predicate1.And(predicate3);
                    context.Publications.Where(predicate1).OrderBy(x => x.PublicationId).Skip((actualPage - 1) * 9).Take(9).ToList().ForEach(x => model.Add(new PublicationViewModel
                    {
                        id = x.PublicationId,
                        headerPath = x.HeaderPath,
                        Content = x.Content,
                        User = x.User.UserName,
                        DateOfCreated = x.DateOfCreated.ToShortDateString(),
                        Status = x.Status.Description,
                    }));
                }
            }
            TempData["Count"] = model.Count;

            var realList = new PublicationsListViewModel(model);
            ViewBag.Search = search;
            ViewBag.MyGallery = personalPage;
            TempData["ActualPage"] = actualPage ;
            return PartialView("~/Views/Publication/_PublicationLists.cshtml", realList);
        }

        
    }
}
