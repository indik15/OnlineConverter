using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public Currency Currency { get; set; }

        public IEnumerable<SelectListItem> SelectListItem { get; set; }
    }
}
