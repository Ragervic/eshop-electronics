using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestP.Data;
using TestP.Models; // Ensure this namespace is correct

namespace TestP.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public Guid? ProductId { get; set; }
        public virtual ApplicationUser User { get; set; } = default!;

        public virtual Product? Product { get; set; }

        // Snapshot of product details at the time of adding to cart
        //Client-side storage
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }


        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }

        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;

    }

}