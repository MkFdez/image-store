
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
                var temp = context.Publications.FirstOrDefault(x => x.PublicationId == id);
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
                    Categories = temp.Categories.Select(x => x.CategoryName).ToList(),
                    Status = temp.Status.Description,
                    isBuyed = isBuyed,
                    ProfilePicture = temp.User.ProfilePicture.Image,
                    Price = Convert.ToDecimal(String.Format("{0:0.00}", temp.Price)),
                    Downloads = temp.Downloads,
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
                var temp = context.Publications.Select(x => new { x.PublicationId, x.Guid, x.HeaderPath }).FirstOrDefault(x => x.PublicationId == pubId);
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
                string path = context.Publications.Select(x => new { x.PublicationId, x.HeaderPath }).FirstOrDefault(x => x.PublicationId == id).HeaderPath;
                return path;
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
                var temp = context.Users.Select(x => new { x.Id, x.UserName, Picture = x.ProfilePicture.Image, x.Email, x.SalesHistories, x.Publications }).First(x => x.Id == userId);
                return new ProfileViewModel() { Email = temp.Email, ProfilePicture = temp.Picture, UserName = temp.UserName, PostedPicture = null, GalleryCount = temp.SalesHistories.Count(), PostedImages = temp.Publications.Count() };
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
                var data = context.SalesHistories.Where(x => x.UserId == userid).OrderByDescending(x => x.Date).Take(pagination.data.length).Skip(pagination.data.start).Select(x => new { publication = x.Publication.Content, date = x.Date, amount = x.Amount });
                int count = context.SalesHistories.Count();
                DTResponse response = new DTResponse();
                response.data = JsonConvert.SerializeObject(data);
                response.recordsFiltered = data.Count();
                response.recordsTotal = count;
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
    }
}
