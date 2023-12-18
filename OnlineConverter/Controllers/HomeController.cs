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
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //��������� ����� � API
            var getRequest = new GetRequest("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            await getRequest.RunAsync();

            //����������� API
            var currencyJsons = JsonConvert.DeserializeObject<List<CurrencyJson>>(getRequest.Response);

            //��������� ������
            CurrencyJson addUAH = new CurrencyJson
            {
                Name = "��������� ������",
                Code = "UAH",
                Price = 1
            };
            currencyJsons.Add(addUAH);

            //����� �������� ����� � ��
            Dictionary<string, Currency> dbcurrency = _db.Currencies.ToDictionary(c=>c.Code);

            
            foreach (var obj in currencyJsons)
            {
                //������ �����
                if (obj.Code == "XDR" || obj.Code == "RUB" || obj.Code == "XAU"
                    || obj.Code == "XAG" || obj.Code == "XPT" || obj.Code == "XPD")
                {
                    continue;
                }

                //���� � �� ����� ������������ ��
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
                    //��������� ���� ���� �������
                    if ( dbcurrency.Count !=0 && dbcurrency.TryGetValue(obj.Code, out Currency currency))
                    {
                        if(obj.Price != currency.Price)
                        {
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

            //�������� ������ ���� ����� �� �� Id � select
            CurrencyVM currencyVM = new CurrencyVM
            {
                Currency = new Currency(),
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
            //������ ���������� �� ������ �� "." ��� � ","
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
            //�������� 2 ����� ����� �� Id
            double[] obj = await _db.Currencies
                .Where(u => u.Id == firstCurrencyId || u.Id == secondCurrencyId)
                .OrderBy(u => u.Id == firstCurrencyId ? 0 : 1)
                .Select(u => u.Price)
                .ToArrayAsync();

            //�������� ������ ���� ����� �� �� Id � select
            CurrencyVM currencyVM = new CurrencyVM
            {
                Currency = new Currency(),
                CurrencySelectList = _db.Currencies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            //������� �����������
            if (obj.Length == 2 && obj[0] != 0 && obj[1] != 0)
            {
                double firstInUAH = 0.0d;
                firstInUAH += convertNum * obj[0];
                currencyVM.convertedNumber += Math.Round(firstInUAH / obj[1], 4);
            }
            else
            {
                //���� � ����� ������� ��������� ���� �������� �������
                currencyVM.isCanConverted = false;
            }

            return Json(new
            {
                convertedNumber = currencyVM.convertedNumber,
                isCanConverted = currencyVM.isCanConverted
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
