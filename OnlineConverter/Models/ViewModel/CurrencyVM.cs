using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double ConvertedNumber { get; set; }
        public CurrentRate CurrentRate { get; set; }
        public IEnumerable<SelectListItem> CurrencySelectList { get; set; }
    }
}
