using JWTAuthentication.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Backend.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}

