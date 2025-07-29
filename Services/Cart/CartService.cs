using Microsoft.EntityFrameworkCore;
using TestP.Data;
using TestP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace TestP.Services
{
    public class CartService
    {
        private const string AnonCartKey = "AnonUser";

        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ISnackbar _snackbar;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        private List<CartItem> _cartItems = new();
        private bool _isUserAuthenticated = false;

        public event Action? OnCartChange;
        private void NotifyCartChange() => OnCartChange?.Invoke();

        public CartService(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            ISnackbar snackbar,
            UserManager<ApplicationUser> userManager,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage,
            NavigationManager navigationManager)
        {
            _dbFactory = dbFactory;
            _snackbar = snackbar;
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _navigationManager = navigationManager;

            // Subscribe to authentication state changes to merge carts
            _authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateChanged;
        }

        public decimal GetSubtotal()
        {
            return _cartItems.Sum(item => item.Quantity * item.UnitPrice);
        }
        public int GetTotalItems()
        {
            return _cartItems.Sum(item => item.Quantity);
        }

        private async void AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            var authState = await task;
            var user = authState.User;
            var newAuthStatus = user.Identity?.IsAuthenticated == true;

            if (newAuthStatus && !_isUserAuthenticated)
            {
                await MergeAnonymousCartToDatabase();
            }

            _isUserAuthenticated = newAuthStatus;
            await LoadCartForCurrentUserAsync(); // Reload cart based on new auth status
        }

        public IReadOnlyList<CartItem> GetCartItems() => _cartItems.AsReadOnly();

        public async Task LoadCartForCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isUserAuthenticated = user.Identity?.IsAuthenticated == true;

            _cartItems.Clear();

            if (_isUserAuthenticated)
            {
                var appUser = await _userManager.GetUserAsync(user);
                if (appUser != null)
                {
                    using (var context = _dbFactory.CreateDbContext())
                    {
                        _cartItems = await context.CartItems
                            .Where(ci => ci.UserId == appUser.Id)
                            .Include(ci => ci.Product)
                            .ToListAsync();
                    }
                }
            }
            else
            {
                // Load from local storage for anonymous users
                var anonymousCart = await _localStorage.GetItemAsync<List<CartItem>>(AnonCartKey);
                if (anonymousCart != null)
                {
                    // Anonymous cart only stores IDs. No need to fetch product details.
                    using (var context = _dbFactory.CreateDbContext())
                    {
                        var productIds = anonymousCart.Select(ci => ci.ProductId).ToList();
                        var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

                        foreach (var anonymousItem in anonymousCart)
                        {
                            var product = products.FirstOrDefault(p => p.Id == anonymousItem.ProductId);
                            if (product != null)
                            {
                                // Reconstruct CartItem with product details 
                                _cartItems.Add(new CartItem
                                {
                                    Id = anonymousItem.Id, //same Id for removal logic
                                    ProductId = anonymousItem.ProductId,
                                    Quantity = anonymousItem.Quantity,
                                    UnitPrice = anonymousItem.UnitPrice,
                                    ProductName = product.Name,
                                    ProductImageUrl = product.ImageUrl,
                                    Product = product // Attach the full product object for display
                                });
                            }
                        }
                    }
                }
            }
            OnCartChange?.Invoke();
        }

        private async Task SaveAnonymousCartAsync()
        {
            var serializableCart = _cartItems.Select(ci => new CartItem
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                ProductName = ci.ProductName,
                ProductImageUrl = ci.ProductImageUrl
            }).ToList();
            await _localStorage.SetItemAsync(AnonCartKey, serializableCart);
        }

        public async Task AddToCart(Product product, int quantity = 1)
        {

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isUserAuthenticated = user.Identity?.IsAuthenticated == true;

            // Find existing item in the current in-memory cart
            var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _snackbar.Add($"Increased quantity of {product.Name} to {existingItem.Quantity}", Severity.Info);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    Id = Guid.NewGuid(), // Give it an ID immediately
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    Product = product // Attach product for in-memory use
                };
                _cartItems.Add(newCartItem);
                _snackbar.Add($"{product.Name} added to cart!", Severity.Success);
            }

            if (_isUserAuthenticated)
            {
                // Save to database
                var appUser = await _userManager.GetUserAsync(user);
                if (appUser == null)
                {
                    _snackbar.Add("User not found. Cannot update persistent cart.", Severity.Error);
                    return;
                }
                using (var context = _dbFactory.CreateDbContext())
                {
                    var existingDbItem = await context.CartItems
                        .FirstOrDefaultAsync(item => item.UserId == appUser.Id && item.ProductId == product.Id);

                    if (existingDbItem != null)
                    {
                        existingDbItem.Quantity = existingItem?.Quantity ?? quantity;
                        context.CartItems.Update(existingDbItem);
                    }
                    else
                    {
                        context.CartItems.Add(new CartItem
                        {
                            Id = existingItem?.Id ?? Guid.NewGuid(),
                            UserId = appUser.Id,
                            ProductId = product.Id,
                            Quantity = existingItem?.Quantity ?? quantity,
                            UnitPrice = product.Price
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                await SaveAnonymousCartAsync();
            }

            OnCartChange?.Invoke();
        }

        public async Task RemoveFromCart(Guid cartItemId)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isUserAuthenticated = user.Identity?.IsAuthenticated == true;

            var itemToRemove = _cartItems.FirstOrDefault(item => item.Id == cartItemId);
            if (itemToRemove == null) return;

            _cartItems.Remove(itemToRemove);
            _snackbar.Add("Item removed from cart.", Severity.Info);

            if (_isUserAuthenticated)
            {
                using (var context = _dbFactory.CreateDbContext())
                {
                    var dbItemToRemove = await context.CartItems.FindAsync(cartItemId);
                    if (dbItemToRemove != null)
                    {
                        context.CartItems.Remove(dbItemToRemove);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                await SaveAnonymousCartAsync();
            }
            OnCartChange?.Invoke();
        }

        public async Task UpdateCartItemQuantity(Guid cartItemId, int newQuantity)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isUserAuthenticated = user.Identity?.IsAuthenticated == true;

            var itemToUpdate = _cartItems.FirstOrDefault(item => item.Id == cartItemId);
            if (itemToUpdate == null) return;

            if (newQuantity <= 0)
            {
                await RemoveFromCart(cartItemId);
                return;
            }

            itemToUpdate.Quantity = newQuantity;
            _snackbar.Add("Cart item quantity updated.", Severity.Info);

            if (_isUserAuthenticated)
            {
                using (var context = _dbFactory.CreateDbContext())
                {
                    var dbItemToUpdate = await context.CartItems.FindAsync(cartItemId);
                    if (dbItemToUpdate != null)
                    {
                        dbItemToUpdate.Quantity = newQuantity;
                        context.CartItems.Update(dbItemToUpdate);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                await SaveAnonymousCartAsync();
            }
            OnCartChange?.Invoke();
        }

        public async Task ClearCart()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            _isUserAuthenticated = user.Identity?.IsAuthenticated == true;

            _cartItems.Clear();
            _snackbar.Add("Cart cleared!", Severity.Info);

            if (_isUserAuthenticated)
            {
                var appUser = await _userManager.GetUserAsync(user);
                if (appUser != null)
                {
                    using (var context = _dbFactory.CreateDbContext())
                    {
                        var userCartItems = await context.CartItems.Where(ci => ci.UserId == appUser.Id).ToListAsync();
                        context.CartItems.RemoveRange(userCartItems);
                        await context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                await _localStorage.RemoveItemAsync(AnonCartKey);
            }
            OnCartChange?.Invoke();
        }

        // --- New Method to Merge Anonymous Cart to Database on Login ---
        private async Task MergeAnonymousCartToDatabase()
        {
            var anonymousCartFromStorage = await _localStorage.GetItemAsync<List<CartItem>>(AnonCartKey);

            if (anonymousCartFromStorage == null || !anonymousCartFromStorage.Any())
            {
                return;
            }

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var appUser = await _userManager.GetUserAsync(user);

            if (appUser == null) return;

            using (var context = _dbFactory.CreateDbContext())
            {
                var existingUserCart = await context.CartItems
                    .Where(ci => ci.UserId == appUser.Id)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var anonymousItem in anonymousCartFromStorage)
                {
                    var existingDbItem = existingUserCart.FirstOrDefault(dbItem => dbItem.ProductId == anonymousItem.ProductId);

                    if (existingDbItem != null)
                    {
                        existingDbItem.Quantity += anonymousItem.Quantity;
                        context.CartItems.Update(existingDbItem);
                    }
                    else
                    {
                        context.CartItems.Add(new CartItem
                        {
                            UserId = appUser.Id,
                            ProductId = anonymousItem.ProductId,
                            Quantity = anonymousItem.Quantity,
                            UnitPrice = anonymousItem.UnitPrice
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            // After merging, clear the anonymous cart from local storage
            await _localStorage.RemoveItemAsync(AnonCartKey);
            _snackbar.Add("Your cart was merged!", Severity.Info);
        }

        // Dispose method to unsubscribe from AuthenticationStateProvider
        public void Dispose()
        {
            _authenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateChanged;
        }
    }
}