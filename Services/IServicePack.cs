using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services
{
    public interface IServicePack
    {
        Task AddComment(Comment comment);
        Task AddPublication(Publication publication, int[] categories);
        void Enday();
        void EndMonth();
        Task<Dictionary<int, string>> GetCategories();
        Task<List<CommentModel>> GetComments(int actual, int pubId);
        Task<string> GetImagePath(int id);
        Task<List<ForBarChartModel>> GetMonthlyHistory(int year);
        Task<ProfileViewModel> GetProfile();
        Task<ProfileViewModel> GetProfile(int userid);
        Task<ExtendedPublicationVM> GetPublication(int id);
        Task<PublicationToDownloadModel> GetPublicationToDownload(int pubId);
        Task<List<ForChartModel>> GetSalesHistory(int month, int year);
        Task<List<PublicationViewModel>> GetSomePublications(int actualPage, Expression<Func<Publication, bool>> predicate);
        Task<DTResponse> GetTransactions(Pagination pagination);
        Task<DTResponse> GetPublicationsForDatatable(Pagination pagination);
        Task<bool> HasPublication(int pubId);
        Task<int> PublicationCount(Expression<Func<Publication, bool>> predicate);
        Task UpdateProfilePicture(string path);

        List<ManagePublicationsModel> GetForManage();
        void UpdateSocialMedia(SocialMedia social);
        Task<TemporalViewModel> GetCreatorPubliactions(string username);
    }
}