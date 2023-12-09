using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineConverter.Models.ViewModel
{
    public class CurrencyVM
    {
        public int inputNum { get; set; }
        public string firstNum {  get; set; }
        public string secondNum { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<SelectListItem> CurrencySelectList { get; set; }
    }
}
