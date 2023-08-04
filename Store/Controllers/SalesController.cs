using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using DataAccess;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Services;

namespace Store.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public IServicePack ServicePack;
        public SalesController(IServicePack servicePack)
        {
            ServicePack = servicePack;
        }
        // GET: Sales
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public async  Task<JsonResult> GetTransactionsData(Pagination pagination)
        {
            var response = await ServicePack.GetTransactions(pagination);
            return Json(response);
        }

            [HttpPost]
        public async Task<JsonResult> GetDailySales(int month, int year)
        {
            
            List<ForChartModel> data = await ServicePack.GetSalesHistory(month, year);
            IEnumerable<decimal> amountList = data.Select(x => x.Price);
            IEnumerable<string> dateList = data.Select(x => $"{months[x.Date.Month-1]} {x.Date.Day}");
            var json = JsonConvert.SerializeObject(new { values = amountList, dates = dateList });
            return Json(json);
        }
        [HttpPost]
        public async Task<JsonResult> GetMonthySales(int year)
        {
            List<ForBarChartModel> data = await ServicePack.GetMonthlyHistory(year);
            IEnumerable<decimal> amountList = data.Select(x => x.Price);
            IEnumerable<string> dateList = data.Select(x => $"{months[x.Month - 1]}");
            var json = JsonConvert.SerializeObject(new { values = amountList, months = dateList });
            return Json(json);
        }
        public ActionResult Enday()
        {
            ServicePack.Enday();
            return View();
        }
        public ActionResult EndMonth()
        {
            ServicePack.EndMonth();
            return View();
        }
    }
}