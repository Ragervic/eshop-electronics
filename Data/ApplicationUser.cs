using Microsoft.AspNetCore.Identity;

namespace TestP.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
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

