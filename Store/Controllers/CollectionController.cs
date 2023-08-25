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

        [HttpPost]
        public async Task<ActionResult> Create(string name)
        {
            try
            {
                await servicePack.CreateCollection(name);
            }
            catch
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
                            
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }


       
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await servicePack.DeleteCollection(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Add(int publicationid, int collectionid)
        {
            try
            {
                await servicePack.MoveToCollection(collectionid, publicationid);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> Publications(int collectionid)
        {

            var data = await servicePack.GetCollectionPublication(collectionid);
         
            return Json(JsonConvert.SerializeObject(data));
        }     

        public async Task<ActionResult> DropPublication(int publicationid)
        {
            try
            {
                await servicePack.NoCollection(publicationid);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }
    }
}
