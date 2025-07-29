using Microsoft.EntityFrameworkCore;

namespace TestP.Models
{
    public class OrderEntities
    {
        [Owned]
        public class CustomerAddress
        {
            public string? FullName { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Email { get; set; }
            public string? StreetAddress { get; set; }
            public string? City { get; set; }
            public string? PostalCode { get; set; }
        }
        [Owned]
        public class DeliveryDetails
        {
            public string? DeliveryMethod { get; set; }
            public string? PickupStationName { get; set; }
            public string? PickupStationAddress { get; set; }
            public DateTime? DeliveryDateStart { get; set; }
            public DateTime? DeliveryDateEnd { get; set; }
        }
        [Owned]

        public class PaymentDetails
        {
            public string? PaymentMethodType { get; set; }
        }

    }
}