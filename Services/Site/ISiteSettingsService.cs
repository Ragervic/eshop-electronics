using TestP.Models;

namespace TestP.Services
{
    public interface ISiteSettingsService
    {
        Task<SiteConfiguration> GetSiteSettingsAsync();
        Task<SiteConfiguration> SaveSiteSettingsAsync(SiteConfiguration settings);
    }
}