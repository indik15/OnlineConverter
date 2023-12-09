using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OnlineConverter.API;
using OnlineConverter.Data;
using OnlineConverter.Models;
using OnlineConverter.Models.ViewModel;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var getRequest = new GetRequest("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            await getRequest.RunAsync();

            var currencyJsons = JsonConvert.DeserializeObject<List<CurrencyJson>>(getRequest.Response);

            foreach (var obj in currencyJsons)
            {
                if(obj.Code == "XDR" || obj.Code == "RUB" || obj.Code == "XAU"
                    || obj.Code == "XAG" || obj.Code == "XPT" || obj.Code == "XPD")
                {
                    continue;
                }
                else
                {
                    var currency = new Currency
                    {
                        Name = obj.Name,
                        Code = obj.Code,
                        Price = obj.Price
                    };
                    _db.Currencies.Add(currency);
                }              
            }

            await _db.SaveChangesAsync();


            CurrencyVM currencyVM = new CurrencyVM
            {
                Currency = new Currency(),
                CurrencySelectList =  _db.Currencies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Price.ToString()
                })
            };

            return View(currencyVM);
        }
        [HttpPost]
        public IActionResult Index(double firstNum, double secondNum)
        {
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DbClear()
        {
            _db.Currencies.RemoveRange(_db.Currencies);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
