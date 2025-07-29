using TestP.Data;
using TestP.Models;
using Microsoft.EntityFrameworkCore;

namespace TestP.Services
{
    public class SiteSettingsService : ISiteSettingsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SiteSettingsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<SiteConfiguration> GetSiteSettingsAsync()
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var siteSettings = await context.SiteSettingsTable.FirstOrDefaultAsync();

                if (siteSettings != null)
                {
                    return siteSettings;
                }

                return new SiteConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading site settings: {ex.Message}");
                return new SiteConfiguration();
            }
        }

        public async Task<SiteConfiguration> SaveSiteSettingsAsync(SiteConfiguration settings)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                var existingSettings = await context.SiteSettingsTable.FirstOrDefaultAsync();

                if (existingSettings != null)
                {
                    existingSettings.ContactEmail = settings.ContactEmail;
                    existingSettings.ContactPhone = settings.ContactPhone;
                    existingSettings.CompanyAddress = settings.CompanyAddress;
                    existingSettings.CompanyLogo = settings.CompanyLogo;
                    existingSettings.WebsiteBanner = settings.WebsiteBanner;
                    existingSettings.ShippingPolicyDescription = settings.ShippingPolicyDescription;
                    existingSettings.StandardShippingCost = settings.StandardShippingCost;
                    existingSettings.ExpressShippingCost = settings.ExpressShippingCost;
                    existingSettings.DefaultTaxRate = settings.DefaultTaxRate;
                    existingSettings.PricingRulesNotes = settings.PricingRulesNotes;

                    context.SiteSettingsTable.Update(existingSettings);
                    await context.SaveChangesAsync();
                    return existingSettings;
                }
                else
                {
                    // Create new settings
                    var newSettings = new SiteSettings
                    {
                        ContactEmail = settings.ContactEmail,
                        ContactPhone = settings.ContactPhone,
                        CompanyAddress = settings.CompanyAddress,
                        CompanyLogo = settings.CompanyLogo,
                        WebsiteBanner = settings.WebsiteBanner,
                        ShippingPolicyDescription = settings.ShippingPolicyDescription,
                        StandardShippingCost = settings.StandardShippingCost,
                        ExpressShippingCost = settings.ExpressShippingCost,
                        DefaultTaxRate = settings.DefaultTaxRate,
                        PricingRulesNotes = settings.PricingRulesNotes
                    };

                    context.SiteSettingsTable.Add(newSettings);
                    await context.SaveChangesAsync();
                    return newSettings;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving site settings: {ex.Message}");
                throw;
            }
        }
    }
}