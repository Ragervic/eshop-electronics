using MudBlazor;

namespace TestP.Services
{
    public class BreadCrumbService
    {
        public event Action? OnChange;
        private List<BreadcrumbItem> _breadCrumbs = new();
        public List<BreadcrumbItem> GetItems() => _breadCrumbs;
        private void NotifyStateChanged() => OnChange?.Invoke();
        public void Clear()
        {
            _breadCrumbs.Clear();
            NotifyStateChanged();
        }
        public void AddItem(string text, string? href = null, bool disabled = false, object? icon = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text cannot be null or empty", nameof(text));
            }
            if (string.IsNullOrEmpty(href))
            {
                throw new ArgumentException("Href cannot be null or empty", nameof(href));
            }

            _breadCrumbs.Add(new BreadcrumbItem(text, href, disabled, (string?)icon));
            NotifyStateChanged();
        }
        public void AddItems(IEnumerable<BreadcrumbItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), "Breadcrumb items cannot be null");
            }

            _breadCrumbs.AddRange(items);
            NotifyStateChanged();
        }

    }
}