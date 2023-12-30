using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public List<string> Date { get; set; }
        public List<double> GraphListUSD { get; set; }
        public List<double> GraphListEUR { get; set; }
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double ConvertedNumber { get; set; }
        public CurrentRate CurrentRate { get; set; }
        public IEnumerable<SelectListItem> CurrencySelectList { get; set; }
    }
}
