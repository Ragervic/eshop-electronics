using System.ComponentModel.DataAnnotations;

namespace TestP.Models
{
    public class SiteConfiguration
    {
        public string ContactEmail { get; set; } = "support@ecomadmin.com";
        public string ContactPhone { get; set; } = "+254 712 345 689";
        public string CompanyAddress { get; set; } = "123 Lenana Street,Jumuia, Kilimani, Nairobi, 12345";
        public string? CompanyLogo { get; set; }
        public string? WebsiteBanner { get; set; }
        public string ShippingPolicyDescription { get; set; } = "Provide clear and concise information about your shipping methods, delivery times, and costs. Include details on international shipping, tracking, and any restrictions.";
        public decimal StandardShippingCost { get; set; } = 5.00M;
        public decimal ExpressShippingCost { get; set; } = 15.00M;
        public decimal DefaultTaxRate { get; set; } = 7.5M;
        public string PricingRulesNotes { get; set; } = "Define any specific pricing rules, discounts, or special conditions. E.g., \"All prices include VAT unless otherwise stated.\"";
    }

    public class SiteSettings : SiteConfiguration
    {
        public int Id { get; set; }
    }
}