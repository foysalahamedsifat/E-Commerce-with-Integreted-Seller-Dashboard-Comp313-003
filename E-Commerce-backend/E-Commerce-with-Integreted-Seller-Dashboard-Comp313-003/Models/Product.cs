using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Backend.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int Stock { get; set; }

    }
}

