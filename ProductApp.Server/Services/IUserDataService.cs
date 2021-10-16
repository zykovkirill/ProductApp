using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;

namespace ProductApp.Server.Services
{
    public interface IUserDataService
    {
        Task<UserOrder> AddOrderAsync(UserOrder model);
        Task<UserOrder> GetProductsFromCart(string userId);
        IEnumerable<UserOrder> GetPurchase(int pageSize, int pageNumber, string userId, out int totalProducts);
    }
    public class UserDataService : IUserDataService
    {
        private readonly ApplicationDbContext _db;
        public UserDataService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<UserOrder> AddOrderAsync(UserOrder model)
        {
            //TODO: Логика с присваиванием модели не самый лучший вариант, мб оcтавить несколько ордеров со статусом корзина
            try
            {
                if (model.Status == OrderStatus.Cart)
                {
                    var order = await _db.UserOrders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == model.UserId && u.Status == OrderStatus.Cart);
                    if (order != null)
                    {
                        order.TotalSum = model.TotalSum;
                        order.Products = model.Products;
                        order.ProductCount = model.ProductCount;
                        _db.UserOrders.Update(order);
                    }
                    else
                        await _db.UserOrders.AddAsync(model);
                }
                else if (model.Status == OrderStatus.Buy)
                {
                    var order = await _db.UserOrders.AsNoTracking().FirstOrDefaultAsync(u => u.Id == model.Id);
                    if (order != null)
                        _db.UserOrders.Update(model);
                    else
                        await _db.UserOrders.AddAsync(model);


                    //TODO: Может быть возвращать статусы ок или не ок
                    //TODO: Task завернуть в исключения и логировать
                    //TODO: Использовать Update везде где меняем данные
                }
                var history = new OrderHistory()
                {
                    IdOrder = model.Id,
                    Status = model.Status
                };
                await _db.PurchasesHistorys.AddAsync(history);
                await _db.SaveChangesAsync();
            }
            catch
            {
                return null;
            }
            return model;
        }
        public async Task<UserOrder> GetProductsFromCart(string userId)
        {
            //TODO: AsNoTracking добавить туда где данные не изменяются
            var cart = await _db.UserOrders.Include(p => p.Products).AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.Status == OrderStatus.Cart);
            return cart;
        }
        public IEnumerable<UserOrder> GetPurchase(int pageSize, int pageNumber, string userId, out int totalProducts)
        {
            //TODO: IsDeleted - Нужно добавить? смотри GetAllUserProductsAsync УБРАТЬ USERPROFILE или ПРОВОДИТЬ СРАВНЕНИЕ ПО НЕМУ А НЕ ПО o.UserId == userId
            var allProducts = _db.UserOrders.Where(o => o.Status == OrderStatus.Buy && o.UserId == userId).AsNoTracking();

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            return prod;
        }
    }
}
