using DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using DataAccess.Models;
using System.Linq.Expressions;

namespace DataAccess
{
    public static class EFDataAccess
    {
        public static async Task AddComment(Comment comment)
        {
            using (var context = new Project1DBEntities())
            {
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }
        }
        public static async Task<List<CommentModel>> GetComments(int actual, int pubId)
            {
                using (var context = new Project1DBEntities())
                {
                    List<CommentModel> comments = new List<CommentModel>();

                    foreach (var c in context.Comments.Where(x => x.PublicationId == pubId).Select(x => new { x.CommentId, x.Content, x.User.UserName, x.DateOfCreated}).OrderBy(x => x.CommentId).Skip(actual).Take(10))
                    {
                        comments.Add(new CommentModel()
                        {
                            Content = c.Content,
                            DateOfCreated = c.DateOfCreated,
                            UserName = c.UserName,
                        });
                    }
                    return comments;
                }
           }


        public static async Task<Dictionary<int, string>> GetCategories()
        {
            using(var context = new Project1DBEntities())
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

        public static async Task<ExtendedPublicationVM> GetPublication(int id)
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
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //int userid = User.Identity.GetUserId<int>();
                    if (await EFDataAccess.HasPublication(id))
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
                    DateOfCreated = temp.DateOfCreated.ToShortDateString(),
                    headerPath = temp.HeaderPath,
                    Content = temp.Content,
                    Comments = comments.Take(10).ToList(),
                    Categories = temp.Categories.Select(x => x.CategoryName).ToList(),
                    Status = temp.Status.Description,
                    isBuyed = isBuyed,
                };

                var toReturn = new ExtendedPublicationVM(publication, temp.Guid, fileName);
                return toReturn;
            }
        }

        public static async Task<bool> HasPublication(int pubId)
        {
            using(var context = new Project1DBEntities())
            {
                int userid = HttpContext.Current.User.Identity.GetUserId<int>();
                return context.Users.Select(x => new { x.UserId, PurPublications = x.PurPublication.Select(p => p.PublicationId) }).FirstOrDefault(x => x.UserId == userid).PurPublications.Any(x => x == pubId);
            }
        }

        public static async Task<PublicationToDownloadModel> GetPublicationToDownload(int pubId)
        {
            using(var context = new Project1DBEntities())
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
        public static async Task<string> GetImagePath(int id)
        {
            using (var context = new Project1DBEntities())
            {
                string path = context.Publications.Select(x => new { x.PublicationId,  x.HeaderPath }).FirstOrDefault(x => x.PublicationId == id).HeaderPath;
                return path;
            }
        }
        public static async Task AddPublication(Publication publication, int[] categories)
        {
            using(var context = new Project1DBEntities())
            {
                context.Categories.Where(x => categories.Contains(x.CategoryId)).ToList().ForEach(y => publication.Categories.Add(y));
                context.Publications.Add(publication);
                context.SaveChanges();
            }

        }

        public static async Task<List<PublicationViewModel>> GetSomePublications(int actualPage, Expression<Func<Publication, bool>> predicate)
        {
            using(var context = new Project1DBEntities())
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
                    Status = x.Status.Description
                }).OrderBy(x => x.PublicationId).Skip((actualPage - 1) * 9).Take(9);
                foreach (var x in publications)
                {
                    result.Add(new PublicationViewModel
                    {
                        id = x.PublicationId,
                        headerPath = x.HeaderPath,
                        Content = x.Content,
                        User = x.UserName,
                        DateOfCreated = x.DateOfCreated.ToShortDateString(),
                        Status = x.Status,
                    });
                }
                return result;
            }
        }
    }
}
