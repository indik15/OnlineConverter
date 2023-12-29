using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineConverter.API;
using OnlineConverter.Data;
using OnlineConverter.Models;
using OnlineConverter.Models.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private double _usdPrice;
        private double _eurPrice;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            #region Робота з БД
            //Отримання даних з API
            var getRequest = new GetRequest("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            await getRequest.RunAsync();

            //Конвертація API
            var currencyJsons = JsonConvert.DeserializeObject<List<CurrencyJson>>(getRequest.Response);

            //Додавання гривні
            CurrencyJson addUAH = new CurrencyJson
            {
                Name = "Українська гривня",
                Code = "UAH",
                Price = 1
            };
            currencyJsons.Add(addUAH);

            //Витяг поточних даних з БД
            Dictionary<string, Currency> dbcurrency = _db.Currencies.ToDictionary(c => c.Code);


            foreach (var obj in currencyJsons)
            {
                //Перебір даних
                if (obj.Code == "XDR" || obj.Code == "RUB" || obj.Code == "XAU"
                    || obj.Code == "XAG" || obj.Code == "XPT" || obj.Code == "XPD")
                {
                    continue;
                }

                //Скорочуємо назви
                if (obj.Code == "ILS")
                {
                    obj.Name = "Новий ізраїл. шекель";
                }
                else if (obj.Code == "AZN")
                {
                    obj.Name = "Азербайджан. манат";
                }
                else if (obj.Code == "TWD")
                {
                    obj.Name = "Новий тайван. долар";
                }
                else if (obj.Code == "TMT")
                {
                    obj.Name = "Туркмен. новий манат";
                }

                //Додаємо в блок "актуального курсу" курс долара та євро
                if (obj.Code == "USD")
                {
                    _usdPrice = Math.Round(obj.Price, 2);
                }
                else if (obj.Code == "EUR")
                {
                    _eurPrice = Math.Round(obj.Price, 2);
                }

                //Якщо в БД пусто перезаписуємо її
                if (!_db.Currencies.Any())
                {
                    var currency = new Currency
                    {
                        Name = obj.Name,
                        Code = obj.Code,
                        Price = obj.Price
                    };
                    _db.Currencies.Add(currency);
                }

                else
                {
                    //Оновлюємо дані якщо потрібно
                    if (dbcurrency.Count != 0 && dbcurrency.TryGetValue(obj.Code, out Currency currency))
                    {
                        if (obj.Price != currency.Price)
                        {
                            CurrentRate currentRate = _db.CurrentRates.FirstOrDefault();
                            if(currentRate == null)
                            {
                                currentRate = new CurrentRate();
                                _db.CurrentRates.Add(currentRate);
                            }
                            //Встановлюємо флажок в залежності від зміни даних
                            if(obj.Code == "USD" || obj.Code == "EUR")
                            {
                                if (obj.Code == "USD")
                                {
                                    if (obj.Price < currency.Price)
                                    {
                                        currentRate.UsdIsUpdated = false;
                                    }
                                    else if (obj.Price > currency.Price)
                                    {
                                        currentRate.UsdIsUpdated = true;
                                    }
                                }

                                else if (obj.Code == "EUR")
                                {
                                    if (obj.Price < currency.Price)
                                    {
                                        currentRate.EurIsUpdated = false;
                                    }
                                    else if (obj.Price > currency.Price)
                                    {
                                        currentRate.EurIsUpdated = true;
                                    }
                                }
                                currentRate.Id = 1;
                                _db.CurrentRates.Update(currentRate);
                            }

                            currency.Price = obj.Price;
                            _db.Currencies.Update(currency);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }


            await _db.SaveChangesAsync();
            #endregion

            //Передача списку назв валют та їх Id в select
            CurrencyVM currencyVM = new CurrencyVM
            {
                CurrentRate = _db.CurrentRates.FirstOrDefault(),
                Usd = _usdPrice,
                Eur = _eurPrice,
                CurrencySelectList =  _db.Currencies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };



            return View(currencyVM);
        }
        [HttpPost]
        public async Task<JsonResult> Index(int firstCurrencyId, int secondCurrencyId, string inputNum)
        {
            //Передача списку назв валют та їх Id в select
            CurrencyVM currencyVM = new CurrencyVM
            {
                CurrencySelectList = _db.Currencies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            
            //Додали функціонал що приймає як "." так і ","
            double convertNum;
            if (double.TryParse(inputNum,
                    NumberStyles.Number,
                    new NumberFormatInfo()
                    {
                        NumberDecimalSeparator = ".",
                        NumberGroupSeparator = ""
                    },
                    out convertNum) ||
                    double.TryParse(inputNum,
                    NumberStyles.Number,
                    new NumberFormatInfo()
                    {
                        NumberDecimalSeparator = ",",
                        NumberGroupSeparator = ""
                    },
                    out convertNum))
            {

            }

            if (firstCurrencyId == secondCurrencyId)
            {
                currencyVM.ConvertedNumber = convertNum;
                return Json(new
                {
                    convertedNumber = currencyVM.ConvertedNumber,
                });
            }

            //приймаємо 2 курса валют по Id
            double[] obj = await _db.Currencies
                .Where(u => u.Id == firstCurrencyId || u.Id == secondCurrencyId)
                .OrderBy(u => u.Id == firstCurrencyId ? 0 : 1)
                .Select(u => u.Price)
                .ToArrayAsync();

            
            //Формула конвертації
            if (obj.Length == 2 && obj[0] != 0 && obj[1] != 0)
            {               
                double firstInUAH = 0.0d;
                firstInUAH += convertNum * obj[0];
                currencyVM.ConvertedNumber = Math.Round(firstInUAH / obj[1], 4);
            }

            return Json(new
            {
                convertedNumber = currencyVM.ConvertedNumber,
            });
        }


        [HttpPost]
        public async Task<IActionResult> DbClear()
        {
            _db.Currencies.RemoveRange(_db.Currencies);

            await _db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT (Currencies, RESEED, 0);");
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
