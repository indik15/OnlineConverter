using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public bool EurIsUpdated { get; set; }
        public bool UsdIsUpdated { get; set; }
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double ConvertedNumber { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<SelectListItem> CurrencySelectList { get; set; }
    }
}
