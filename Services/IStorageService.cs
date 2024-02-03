using Models;
using System.Web;

namespace Services
{
    public interface IStorageService
    {
        DownloadModel GenerateDownloadDirectory(PublicationToDownloadModel model, StorageService.ImageManager<int> imageManager, string scale = "100");
        DownloadModel GetDownloadFreeDirectory(string imagepath);
        string NewImageDirectory(HttpPostedFileBase picture, int pId, string pGuid, StorageService.ImageManager<int> scaler, StorageService.ImageManager<string> watermarker);
    }
}