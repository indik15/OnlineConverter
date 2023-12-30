using System.ComponentModel.DataAnnotations;

namespace OnlineConverter.Models
{
    public class EurGraph
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
    }
}
