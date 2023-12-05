using System.ComponentModel.DataAnnotations;

namespace OnlineConverter.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
    }
}
