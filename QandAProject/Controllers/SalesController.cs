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
        private string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        // GET: Sales
        public ActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetDailySales()
        {
            
            List<ForChartModel> data = await EFDataAccess.GetSalesHistory();
            IEnumerable<decimal> amountList = data.Select(x => x.Price);
            IEnumerable<string> dateList = data.Select(x => $"{months[x.Date.Month-1]} {x.Date.Day}");
            var json = JsonConvert.SerializeObject(new { values = amountList, dates = dateList });
            return Json(json);
        }
        [HttpPost]
        public async Task<JsonResult> GetMonthySales()
        {
            List<ForBarChartModel> data = await EFDataAccess.GetMonthlyHistory();
            IEnumerable<decimal> amountList = data.Select(x => x.Price);
            IEnumerable<string> dateList = data.Select(x => $"{months[x.Month - 1]}");
            var json = JsonConvert.SerializeObject(new { values = amountList, months = dateList });
            return Json(json);
        }
        public ActionResult Enday()
        {
            EFDataAccess.Enday();
            return View();
        }
        public ActionResult EndMonth()
        {
            EFDataAccess.EndMonth();
            return View();
        }
    }
}