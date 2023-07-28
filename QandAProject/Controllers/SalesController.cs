using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using DataAccess;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace QandAProject.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        // GET: Sales
        public ActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetDailySales()
        {
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
            List<ForChartModel> data = await EFDataAccess.GetSalesHistory();
            IEnumerable<decimal> amountList = data.Select(x => x.Price);
            IEnumerable<string> dateList = data.Select(x => $"{months[x.Date.Month-1]} {x.Date.Day}");
            var json = JsonConvert.SerializeObject(new { values = amountList, dates = dateList });
            return Json(json);
        }
        public ActionResult Enday()
        {
            EFDataAccess.Enday();
            return View();
        }
    }
}