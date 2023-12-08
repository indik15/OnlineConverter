using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public Currency currency { get; set; }

        public IEnumerable<SelectListItem> selectCurrency { get; set; }
    }
}
