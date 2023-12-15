using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public bool isCanConverted { get; set; } = true;
        public double convertedNumber { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<SelectListItem> CurrencySelectList { get; set; }
    }
}
