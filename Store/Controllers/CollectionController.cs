using Models;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Store.Controllers
{
    public class CollectionController : Controller
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("CollectionLogger");
        IServicePack servicePack;
        public CollectionController(IServicePack service)
        {
            servicePack = service;
        }
        [HttpPost]
        public async Task<ActionResult> Index(Pagination pagination)
        {
            var data = await servicePack.GetCollections(pagination);

            return Json(data);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(string name)
        {
            try
            {
                await servicePack.CreateCollection(name);
            }
            catch(Exception ex)
            {
                log.Error($"Error creating collection with name {name}", ex);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
                            
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await servicePack.DeleteCollection(id);
            }
            catch(Exception ex)
            {
                log.Error($"error while deleting collection with collection id:{id}", ex);
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Add(int publicationid, int collectionid)
        {
            try
            {
                await servicePack.MoveToCollection(collectionid, publicationid);
            }
            catch(Exception ex)
            {
                log.Error($"Error adding the publicationid: {publicationid} to collectionid: {collectionid}", ex);
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [Authorize]
        public async Task<ActionResult> Publications(int collectionid)
        {

            var data = await servicePack.GetCollectionPublication(collectionid);
         
            return Json(JsonConvert.SerializeObject(data));
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> DropPublication(int publicationid)
        {
            try
            {
                await servicePack.NoCollection(publicationid);
            }
            catch(Exception ex)
            {
                log.Error($"Error removing publicationid:{publicationid} from its collection", ex);
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }

        [Authorize]
        public async Task<ActionResult> More(int collectionid, int publicationid)
        {
            var data = await servicePack.MoreInCollection(collectionid, publicationid);
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
