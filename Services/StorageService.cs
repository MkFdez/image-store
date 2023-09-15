using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public class StorageService : IStorageService
    {
        public delegate void ImageManager<T>(string path, T scaleOrName, string returnPath);

        public DownloadModel GenerateDownloadDirectory(PublicationToDownloadModel model, ImageManager<int> imageManager, string scale = "100")
        {
            string fileName = model.Path.Substring(model.Path.LastIndexOf("/") + 1);
            string pGuid = model.Guid;
            int intScale = int.Parse(scale);
            string guid = Guid.NewGuid().ToString();
            string newName = guid + fileName.Substring(fileName.LastIndexOf("."));
            imageManager(Path.Combine(HttpContext.Current.Server.MapPath("~/ImageVault/" + pGuid + "/"), fileName), intScale, Path.Combine(HttpContext.Current.Server.MapPath("~/TempData"), newName));
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(HttpContext.Current.Server.MapPath("~/TempData"), newName));
            return new DownloadModel() { File = fileBytes, FileName = newName };

        }

        public string NewImageDirectory(HttpPostedFileBase picture, int pId, string pGuid,ImageManager<int> scaler, ImageManager<string> watermarker)
        {
            Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/uploads"), pId.ToString()));
            Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/uploads/" + pId.ToString()), "FreeTrial"));
            Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/uploads/" + pId.ToString()), "LowRes"));
            Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath("~/ImageVault/"), pGuid));
            var fileName = Path.GetFileName(picture.FileName).Replace(" ", "");
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/uploads"), fileName);

            string fl = path.Substring(path.LastIndexOf("\\"));
            string[] split = fl.Split('\\');
            string newpath = split[1];
            string imagepath = "/uploads/" + pId.ToString() + "/LowRes/" + newpath;
            picture.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/ImageVault/" + pGuid + "/"), fileName));
            watermarker(Path.Combine(HttpContext.Current.Server.MapPath("~/ImageVault/" + pGuid), fileName), "my website", Path.Combine(HttpContext.Current.Server.MapPath("~/uploads/" + pId.ToString() + "/" + "FreeTrial/"), fileName));
            scaler(Path.Combine(HttpContext.Current.Server.MapPath("~/ImageVault/" + pGuid), fileName), 50, Path.Combine(HttpContext.Current.Server.MapPath("~/uploads/" + pId.ToString() + "/" + "LowRes/"), fileName));
            return imagepath;
        }

        public DownloadModel GetDownloadFreeDirectory(string path)
        {
            string fileName = path.Substring(path.LastIndexOf(@"/") + 1);
            path = path.Substring(0, path.LastIndexOf("/LowRes/"));
            string file = Path.Combine(Directory.GetFiles(HttpContext.Current.Server.MapPath("~" + path + "/FreeTrial/"), fileName));
            string guid = Guid.NewGuid().ToString();
            string newName = guid + fileName.Substring(fileName.LastIndexOf("."));

            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            return new DownloadModel() { File = fileBytes, FileName = newName };
        }
    }
}
