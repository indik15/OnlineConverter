using System.ComponentModel.DataAnnotations;

namespace OnlineConverter.Models
{
    public class CurrentRate
    {
        [Key]
        public int Id { get; set; }
        public bool EurIsUpdated { get; set; }
        public bool UsdIsUpdated { get; set; }
    }
}
