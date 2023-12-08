using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineConverter.API;
using OnlineConverter.Data;
using OnlineConverter.Models;
using System.Diagnostics;

namespace OnlineConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var getRequest = new GetRequest("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            await getRequest.RunAsync();

            var currencyJsons = JsonConvert.DeserializeObject<List<CurrencyJson>>(getRequest.Response);

            foreach(var obj in currencyJsons)
            {
                var currency = new Currency
                {
                    Name = obj.Name,
                    Code = obj.Code,
                    Price = obj.Price
                };
                _db.Currencies.Add(currency);
            }

            await _db.SaveChangesAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
