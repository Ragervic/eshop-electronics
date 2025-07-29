using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestP.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? OrderId { get; set; }
        public Guid? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Price of the product at the time of the order ( for historical accuracy)
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
        // [NotMapped]
        // public decimal TotalPrice => UnitPrice * Quantity;
        public Order Order { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}