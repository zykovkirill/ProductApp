using Blazored.SessionStorage;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.WebClient
{
    /// <summary>
    /// Вспомогательный класс для работы с сессией
    /// </summary>
    public static class SessionUtils
    {
        private const string _cartSessionStorageName = "CartSessionStorage";
        private const string _productSessionStorageName = "ProductSessionStorage";
        private static bool _isCartSessionExists = false;

        #region Работа с сессией корзины
        public async static Task<List<UserOrderProduct>> InitOrGetCartSessionStorageAsync(ISessionStorageService sessionStorage, List<UserOrderProduct> userOrderProducts = null )
        {
            if(userOrderProducts != null && userOrderProducts.Any())
            {
                await sessionStorage.SetItemAsync(_cartSessionStorageName, userOrderProducts);
            }
            if (!_isCartSessionExists)
            {
                await sessionStorage.SetItemAsync(_cartSessionStorageName, new List<UserOrderProduct>());
                _isCartSessionExists = true;
            }
            return await sessionStorage.GetItemAsync<List<UserOrderProduct>>(_cartSessionStorageName);
        }

        public async static Task AddProductToCartSessionStorageAsync(ISessionStorageService sessionStorage, BaseProduct selectedProduct, int count)
        {
            var cartSessionStorage = await InitOrGetCartSessionStorageAsync(sessionStorage);
            var isProductExists = cartSessionStorage.Any(c => c.ProductId == selectedProduct.Id);
            if (!isProductExists)
            {
                var userOrderProduct = new UserOrderProduct(selectedProduct, count);
                userOrderProduct.Count = count;
                cartSessionStorage.Add(userOrderProduct);
                await sessionStorage.SetItemAsync(_cartSessionStorageName, cartSessionStorage);
            }
            else
            {
                var productCountable = cartSessionStorage.FirstOrDefault(c => c.ProductId == selectedProduct.Id);
                productCountable.Count = productCountable.Count + count;
                await sessionStorage.SetItemAsync(_cartSessionStorageName, cartSessionStorage);
            }
        }
        public async static Task<List<UserOrderProduct>> DeleteProductFromCartSessionStorageAsync(ISessionStorageService sessionStorage, UserOrderProduct productCountable)
        {
            var cartSessionStorage = await sessionStorage.GetItemAsync<List<UserOrderProduct>>(_cartSessionStorageName);
            cartSessionStorage.Remove(cartSessionStorage.FirstOrDefault(c => c.Id == productCountable.Id));
            await sessionStorage.SetItemAsync(_cartSessionStorageName, cartSessionStorage);
            return cartSessionStorage;
        }
        public async static Task<List<UserOrderProduct>> ChangeProductFromCartSessionStorageAsync(ISessionStorageService sessionStorage, int value, string id)
        {
            var cartSessionStorage = await InitOrGetCartSessionStorageAsync(sessionStorage);
            var product = cartSessionStorage.FirstOrDefault(c => c.Id == id);
            if (value != 0)
            {
                product.Count = value;
                await sessionStorage.SetItemAsync(_cartSessionStorageName, cartSessionStorage);
                return cartSessionStorage;
            }
            else
            {
                cartSessionStorage.Remove(product);
                await sessionStorage.SetItemAsync(_cartSessionStorageName, cartSessionStorage);
                return await DeleteProductFromCartSessionStorageAsync(sessionStorage, product);
            }
        }
        public async static Task ClearCartSessionStorageAsync(ISessionStorageService sessionStorage)
        {
            await sessionStorage.RemoveItemAsync(_cartSessionStorageName);
            _isCartSessionExists = false;
        }
        #endregion
    }
}
