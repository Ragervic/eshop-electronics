using System;
using static TestP.Models.OrderEntities;
namespace TestP.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFees { get; set; }
        public string? Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public CustomerAddress? CustomerAddress { get; set; }
        public DeliveryDetails? DeliveryDetails { get; set; }
        public PaymentDetails? PaymentDetails { get; set; }
    }

}