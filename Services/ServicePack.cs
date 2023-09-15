
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using DataAccess;
using Services;
using Models;


namespace Services
{
    public class ServicePack : IServicePack
    {
        public async Task AddComment(Comment comment)
        {
            using (var context = new Project1DBEntities())
            {
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }
        }
        public async Task<List<CommentModel>> GetComments(int actual, int pubId)
        {
            using (var context = new Project1DBEntities())
            {
                List<CommentModel> comments = new List<CommentModel>();

                foreach (var c in context.Comments.Where(x => x.PublicationId == pubId).Select(x => new { x.CommentId, x.Content, x.User.UserName, x.DateOfCreated, Picture = x.User.ProfilePicture.Image }).OrderBy(x => x.CommentId).Skip(actual).Take(10))
                {
                    comments.Add(new CommentModel()
                    {
                        Content = c.Content,
                        DaysSinceCreated = (DateTime.Now - c.DateOfCreated).Days,
                        UserName = c.UserName,
                        ProfilePicture = c.Picture,
                    });
                }
                return comments;
            }
        }


        public async Task<Dictionary<int, string>> GetCategories()
        {
            using (var context = new Project1DBEntities())
            {
                var categories = context.Categories;
                Dictionary<int, string> values = new Dictionary<int, string>();
                foreach (var cat in categories)
                {
                    values.Add(cat.CategoryId, cat.CategoryName);

                }
                return values;
            }

        }

        public async Task<ExtendedPublicationVM> GetPublication(int id)
        {
            using (var context = new Project1DBEntities())
            {
                var userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var temp = context.Publications.FirstOrDefault(x => x.PublicationId == id);
                if (temp.StatusId != 0 && !temp.SalesHistories.Any(x => x.UserId == userid)){
                    throw new Exception("Publication deleted");
                }
                var comments = new List<CommentModel>();
                temp.Comments1.ToList().ForEach(x => comments.Add(new CommentModel()
                {
                    DaysSinceCreated = (DateTime.Now - x.DateOfCreated).Days,
                    UserName = x.User.UserName,
                    Content = x.Content,
                    ProfilePicture = x.User.ProfilePicture.Image,
                }));
                bool isBuyed = false;
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //int userid = User.Identity.GetUserId<int>();
                    if (await HasPublication(id))
                    {
                        isBuyed = true;
                    }
                }
                string path = HttpContext.Current.Server.MapPath("~" + temp.HeaderPath);
                string fileName = path.Substring(path.LastIndexOf(@"\") + 1);
                var publication = new PublicationViewModel()
                {
                    id = id,
                    User = temp.User.UserName,
                    DaysSinceCreated = (DateTime.Now - temp.DateOfCreated).Days,
                    headerPath = temp.HeaderPath,
                    Content = temp.Content,
                    Comments = comments.Take(10).ToList(),
                    CommentTotal = comments.Count(),
                    Categories = temp.Categories.Select(x => x.CategoryName).ToList(),
                    Status = temp.Status.Description,
                    isBuyed = isBuyed,
                    ProfilePicture = temp.User.ProfilePicture.Image,
                    Price = Convert.ToDecimal(String.Format("{0:0.00}", temp.Price)),
                    Downloads = temp.Downloads,
                    inCollection = temp.CollectionId != null ? temp.Collection.Publications.Count() - 1 : 0,
                    CollectionId = temp.CollectionId,
                };

                var toReturn = new ExtendedPublicationVM(publication, temp.Guid, fileName);
                return toReturn;
            }
        }

        public async Task<bool> HasPublication(int pubId)
        {
            using (var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                return context.Users.Select(x => new { x.Id, PurPublications = x.SalesHistories.Select(p => p.PublicationId) }).FirstOrDefault(x => x.Id == userid).PurPublications.Any(x => x == pubId) || context.Users.Select(x => new { x.Id, Publication = x.Publications.Select(p => p.PublicationId) }).FirstOrDefault(x => x.Id == userid).Publication.Any(x => x == pubId);
            }
        }

        public async Task<PublicationToDownloadModel> GetPublicationToDownload(int pubId)
        {
            using (var context = new Project1DBEntities())
            {
                var userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var temp = context.Publications.Select(x => new { x.PublicationId, x.Guid, x.HeaderPath, x.SalesHistories, x.StatusId }).FirstOrDefault(x => x.PublicationId == pubId);
                if (temp.StatusId != 0 && !temp.SalesHistories.Any(x => x.UserId == userid))
                {
                    throw new Exception("Publication deleted");
                }
                var publication = new PublicationToDownloadModel()
                {
                    Guid = temp.Guid,
                    Path = temp.HeaderPath
                };
                return publication;
            }


        }
        public async Task<string> GetImagePath(int id)
        {
            using (var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var temp = context.Publications.Select(x => new { x.PublicationId, x.HeaderPath, x.SalesHistories, x.StatusId }).FirstOrDefault(x => x.PublicationId == id);
                if (temp.StatusId != 0 && !temp.SalesHistories.Any(x => x.UserId == userid))
                {
                    throw new Exception("Publication deleted");
                }
                return temp.HeaderPath;
            }
        }
        public async Task AddPublication(Publication publication, int[] categories)
        {
            using (var context = new Project1DBEntities())
            {
                context.Categories.Where(x => categories.Contains(x.CategoryId)).ToList().ForEach(y => publication.Categories.Add(y));
                context.Publications.Add(publication);
                context.SaveChanges();
            }

        }

        public async Task<ProfileViewModel> GetProfile()
        {
            using (var context = new Project1DBEntities())
            {
                int userId = HttpContext.Current.User.Identity.GetUserId<int>();
                var temp = context.Users.Select(x => new { x.Id, x.UserName, Picture = x.ProfilePicture.Image, x.Email, x.SalesHistories, x.Publications, x.SocialMedia }).First(x => x.Id == userId);
                return new ProfileViewModel() {
                    Email = temp.Email,
                    ProfilePicture = temp.Picture,
                    UserName = temp.UserName,
                    PostedPicture = null,                    
                    Instagram = temp.SocialMedia?.Instagram,
                    Facebook = temp.SocialMedia?.Facebook,
                    Website = temp.SocialMedia?.Website,
                    Twitter = temp.SocialMedia?.Twitter,
                    Pinterest = temp.SocialMedia?.Pinterest,
                };
            }
        }
        public async Task<ProfileViewModel> GetProfile(string username)
        {
            using (var context = new Project1DBEntities())
            {                
                var temp = context.Users.Select(x => new { x.Id, x.UserName, Picture = x.ProfilePicture.Image, x.Email, x.SalesHistories, x.Publications, x.SocialMedia }).First(x => x.UserName == username);
                return new ProfileViewModel()
                {
                    Email = temp.Email,
                    ProfilePicture = temp.Picture,
                    UserName = temp.UserName,
                    PostedPicture = null,
                    Instagram = temp.SocialMedia?.Instagram,
                    Facebook = temp.SocialMedia?.Facebook,
                    Website = temp.SocialMedia?.Website,
                    Twitter = temp.SocialMedia?.Twitter,
                    Pinterest = temp.SocialMedia?.Pinterest,
                };
            }
        }
        public async Task UpdateProfilePicture(string path)
        {
            using (var context = new Project1DBEntities())
            {
                try
                {
                    int userId = HttpContext.Current.User.Identity.GetUserId<int>();
                    var user = context.Users.First(x => x.Id == userId);
                    user.ProfilePicture.Image = path;
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }
        public async Task<int> PublicationCount(Expression<Func<Publication, bool>> predicate)
        {
            using (var context = new Project1DBEntities())
            {
                return context.Publications.Where(predicate).Count();

            }
        }
        public void Enday()
        {
            using (var context = new Project1DBEntities())
            {
                var usersIds = context.Users.Select(x => x.Id);
                var dayBefore = DateTime.Now.AddDays(-1);
                foreach (int id in usersIds)
                {
                    decimal amount = context.SalesHistories.Where(x => x.Date.Day == dayBefore.Day && x.Date.Month == dayBefore.Month && x.Date.Year == dayBefore.Year && x.Publication.UserId == id).Select(x => x.Amount).Sum() ?? 0;
                    context.DailySales.Add(new DailySale()
                    {
                        Date = DateTime.Now.AddDays(-1),
                        TotalAmount = amount,
                        UserId = id,
                    });
                }
                context.SaveChanges();
            }
        }
        public void EndMonth()
        {
            using (var context = new Project1DBEntities())
            {
                var userIds = context.Users.Select(x => x.Id);
                var date = DateTime.Now.AddMonths(-1);
                foreach (var id in userIds)
                {
                    var amount = context.SalesHistories.Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.UserId == id).Select(x => x.Amount).Sum() ?? 0;
                    context.MonthlySales.Add(new MonthlySale()
                    {
                        UserId = id,
                        Amount = amount,
                        Month = date.Month,
                        Year = date.Year,
                    });

                }
                context.SaveChanges();

            }
        }
        public async Task<List<ForChartModel>> GetSalesHistory(int month, int year)
        {
            int userId = HttpContext.Current.User.Identity.GetUserId<int>();
            using (var context = new Project1DBEntities())
            {
                if (month == -1 || year == -1)
                {
                    DateTime efectiveDate = DateTime.Now;
                    year = efectiveDate.Year;
                    month = efectiveDate.Month;
                }
                List<ForChartModel> data = context.DailySales.Where(x => x.Date.Month == month && x.Date.Year == year && x.UserId == userId).Select(x => new ForChartModel() { Date = x.Date, Price = x.TotalAmount }).ToList();
                return data;
            }

        }

        public async Task<List<ForBarChartModel>> GetMonthlyHistory(int year)
        {
            int userId = HttpContext.Current.User.Identity.GetUserId<int>();
            if (year == -1)
            {
                year = DateTime.Now.Year;
            }
            using (var context = new Project1DBEntities())
            {

                List<ForBarChartModel> data = context.MonthlySales.Where(x => x.Year == year && x.UserId == userId).Select(x => new ForBarChartModel() { Month = x.Month, Price = x.Amount }).ToList();
                return data;

            }


        }

        public async Task<List<PublicationViewModel>> GetSomePublications(int actualPage, Expression<Func<Publication, bool>> predicate)
        {
            using (var context = new Project1DBEntities())
            {
                List<PublicationViewModel> result = new List<PublicationViewModel>();
                var publications = context.Publications.Where(predicate).Select(x =>
                new
                {
                    x.PublicationId,
                    x.User.UserName,
                    Comments = x.Comments1.Select(c => new { c.User.UserName, c.DateOfCreated, c.Content }),
                    x.Content,
                    x.Categories,
                    x.HeaderPath,
                    x.DateOfCreated,
                    Status = x.Status.Description,
                    x.Price,
                }).OrderBy(x => x.PublicationId).Skip((actualPage - 1) * 9).Take(9);
                foreach (var x in publications)
                {
                    result.Add(new PublicationViewModel
                    {
                        id = x.PublicationId,
                        headerPath = x.HeaderPath,
                        Content = x.Content,
                        User = x.UserName,
                        DaysSinceCreated = (DateTime.Now - x.DateOfCreated).Days,
                        Status = x.Status,
                        Price = Convert.ToDecimal(String.Format("{0:0.00}", x.Price)),
                    });
                }
                return result;
            }
        }

        public async Task<DTResponse> GetTransactions(Pagination pagination)
        {
            using (var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var data = context.SalesHistories.Where(x => x.UserId == userid).OrderByDescending(x => x.Date).Skip(pagination.data.start).Take(pagination.data.length).Select(x => new { publication = x.Publication.Content, date = x.Date, amount = x.Amount });
                int count = context.SalesHistories.Where(x => x.Publication.UserId == userid).Count();
                DTResponse response = new DTResponse();
                response.data = JsonConvert.SerializeObject(data);
                response.recordsFiltered = count;
                response.recordsTotal = count;
                return response;

            }
        }
        public async Task<DTResponse> GetPublicationsForDatatable(Pagination pagination)
        {
            using (var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var data = context.Publications.Where(x => x.UserId == userid && x.StatusId == 0).OrderByDescending(x => x.DateOfCreated)
                    .Skip(pagination.data.start)
                    .Take(pagination.data.length)
                    .Select(x => new { image = x.HeaderPath, 
                        publication = x.Content, 
                        date = x.DateOfCreated, 
                        downloads = x.Downloads, 
                        actions = new { title = x.Content, image = x.HeaderPath, id = x.PublicationId }, 
                        collection = new {collection = x.Collection.Name, publicationid = x.PublicationId } });
                var collections = context.Collections.Where(x => x.UserId == userid).Select(x => new { collectionid = x.CollectionId, collection = x.Name }).ToList();
                var count = context.Publications.Where(x => x.UserId == userid && x.StatusId == 0).Count();
                DTResponse response = new DTResponse();
                response.data = JsonConvert.SerializeObject(data);
                response.recordsFiltered = count;
                response.recordsTotal = count;
                response.collections = JsonConvert.SerializeObject(collections);

                return response;

            }
        }

        public List<ManagePublicationsModel> GetForManage()
        {
            int userid = HttpContext.Current.User.Identity.GetUserId<int>();
            using(var context = new Project1DBEntities())
            {
                var data = context.Publications.Where(x => x.UserId == userid).Select(x => new ManagePublicationsModel() { Downloads = x.Downloads, Id = x.PublicationId, Image = x.HeaderPath, Name = x.Content, Price = x.Price}).ToList();
                return data;
            }
        }

        public async Task<ProfileViewModel> GetCreator(string username)
        {
            using(var context = new Project1DBEntities())
            {

                var creator = await  GetProfile(username);
                return creator;
            }
        }
        public async Task<List<SimplePublicationViewModel>> GetCreatorPubliactions(string username, int count)
        {
            using(var context = new Project1DBEntities())
            {
                var pubs = context.Users.FirstOrDefault(x => x.UserName == username).Publications
                    .OrderByDescending(x => x.DateOfCreated)
                    .Skip(count)
                    .Take(10)
                    .Select(x => new SimplePublicationViewModel() { PublicationId = x.PublicationId, Image = x.HeaderPath }).ToList();
                return pubs;
            }
        }

        public void UpdateSocialMedia(SocialMedia social)
        {
            using(var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                social.UserId = userid;
                var user = context.Users.FirstOrDefault(x => x.Id == userid);
                if(user.SocialMedia == null)
                {
                    user.SocialMedia = social;
                }
                else
                {
                    user.SocialMedia.Instagram = social.Instagram;
                    user.SocialMedia.Facebook = social.Facebook;
                    user.SocialMedia.Pinterest = social.Pinterest;
                    user.SocialMedia.Twitter = social.Twitter;
                    user.SocialMedia.Website = social.Website;
                }
                context.SaveChanges();
            }
        }
        public bool DeletePublication(int pubId)
        {
            using(var context = new Project1DBEntities())
            {
                int userId = HttpContext.Current.User.Identity.GetUserId<int>();
                var publication = context.Publications.FirstOrDefault(x => x.PublicationId == pubId);
                if(publication.UserId != userId)
                {
                    return false;
                }
                publication.StatusId = 2;
                context.SaveChanges();
                return true;
            }
        }

        public async Task CreateCollection(string name)
        {
            using(var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                if (context.Collections.Any(x => x.Name == name && x.UserId == userid))
                    throw new Exception("there is another collection with the same name");
                context.Collections.Add(new Collection() { Name = name, UserId = userid });
                context.SaveChanges();
            }
        }
        public async Task<DTResponse> GetCollections(Pagination pagination)
        {
            using(var context = new Project1DBEntities())
            {              
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var data = context.Collections.Where(x => x.UserId == userid).OrderBy(x => x.CollectionId).Skip(pagination.data.start).Take(pagination.data.length).Select(x => new { name = x.Name, count = x.Publications.Count, actions = new { collectionid = x.CollectionId, name = x.Name } });
                int count = context.Collections.Where(x => x.UserId == userid).Count();
                DTResponse response = new DTResponse();
                response.data = JsonConvert.SerializeObject(data);
                response.recordsFiltered = count;
                response.recordsTotal = count;
                return response;
            }
        }
        public async Task DeleteCollection(int collectionId)
        {
            using(var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var collection = context.Collections.First(x => x.CollectionId == collectionId);
                if (collection.UserId != userid)
                    throw new Exception("not authorized");
                foreach(var p in collection.Publications)
                {
                    p.Collection = null;
                }
                context.SaveChanges();
                context.Collections.Remove(collection);
                context.SaveChanges();
            }
        }

        public async Task MoveToCollection(int collectionid, int publicationid)
        {
            using(var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var publication = context.Publications.First(x => x.PublicationId == publicationid);
                if (publication.UserId != userid || context.Collections.First(x => x.CollectionId == collectionid).UserId != userid)
                    throw new Exception("unauthorized");
                publication.CollectionId = collectionid;
                context.SaveChanges();
            }
        }

        public async Task NoCollection(int publicationid)
        {
            using (var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                var publication = context.Publications.First(x => x.PublicationId == publicationid);
                if (publication.UserId != userid)
                    throw new Exception("unauthorized");
                publication.CollectionId = null;
                context.SaveChanges();
            }
        }

        public async Task<List<SimplePublicationViewModel>> GetCollectionPublication(int collectionid)
        {
            using(var context = new Project1DBEntities())
            {
                var data = context.Publications.Where(x => x.CollectionId == collectionid).Select(x => new SimplePublicationViewModel() { PublicationId = x.PublicationId, Image = x.HeaderPath, Name = x.Content }).ToList();
                return data;
            }
        }

        public async Task<List<SimplePublicationViewModel>> MoreInCollection(int collectionid, int publicationid)
        {
            using (var context = new Project1DBEntities())
            {
                return context.Collections.First(x => x.CollectionId == collectionid)
                    .Publications.Where(x => x.PublicationId != publicationid)
                    .Select(x => new SimplePublicationViewModel()
                    {
                        PublicationId = x.PublicationId,
                        Image = x.HeaderPath,
                    }).ToList();
            }
        }
        
    }
}
