using Chat_Authentication;
using DataAccess;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Store.Controllers
{
    public class ChatController : Controller
    {
        private readonly IServicePack servicePack;
        public ChatController(IServicePack service) 
        {
            servicePack = service;
        }
        public async  Task<ActionResult> Index(string receiver = "")
        {

            string token = await servicePack.AddToken();
            ChatModel chat = new ChatModel() { Token = token };
            if(receiver != "") chat.Receiver = receiver;
            return View(chat);
        }
        // GET: Chat
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            try
            {
                string token = await servicePack.AddToken();
                    return Json(new { token = token });
            }
            catch (Exception e)
            {
                var err = e;
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
    }
}