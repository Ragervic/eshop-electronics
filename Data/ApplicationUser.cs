using Microsoft.AspNetCore.Identity;

namespace TestP.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }


    //Customer
    public string? ShippingAddress { get; set; }
    [PersonalData]
    public string? PreferredDeliveryMethod { get; set; }
    [PersonalData]
    public string? PreferredPickupStationName { get; set; }
    [PersonalData]
    public string? PreferredPickupStationAddress { get; set; }
    [PersonalData]
    public string? PreferredPaymentMethodType { get; set; }
}

