using System.ComponentModel.DataAnnotations;

namespace OnlineConverter.Models
{
    public class UsdGraph
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public string Date { get; set; }

    }
}
