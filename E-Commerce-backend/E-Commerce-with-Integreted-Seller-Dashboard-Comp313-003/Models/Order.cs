using JWTAuthentication.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_Backend.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

