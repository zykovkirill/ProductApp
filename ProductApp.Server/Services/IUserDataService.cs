using Microsoft.EntityFrameworkCore;
using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Server.Services
{
    public interface IUserDataService
    {
        Task<UserOrder> AddOrderAsync(UserOrder model);
        Task<UserOrder> GetProductsFromCart(string userId);
        Task<(int, IEnumerable<UserOrder>)> GetPurchase(int pageSize, int pageNumber, string userId);
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
                Status = model.Status,
                EditedUser = model.EditedUser
            };
            await _db.PurchasesHistorys.AddAsync(history);
            await _db.SaveChangesAsync();

            return model;
        }
        public async Task<UserOrder> GetProductsFromCart(string userId)
        {
            //TODO: AsNoTracking добавить туда где данные не изменяются
            var cart = await _db.UserOrders.Include(p => p.Products).AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.Status == OrderStatus.Cart);
            return cart;
        }
        public async Task<(int, IEnumerable<UserOrder>)> GetPurchase(int pageSize, int pageNumber, string userId)
        {
            //TODO: IsDeleted - Нужно добавить? смотри GetAllUserProductsAsync УБРАТЬ USERPROFILE или ПРОВОДИТЬ СРАВНЕНИЕ ПО НЕМУ А НЕ ПО o.UserId == userId
            var allProducts = _db.UserOrders.Where(o => o.Status == OrderStatus.Buy && o.UserId == userId).AsNoTracking();

            var prod = await allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (allProducts.Count(), prod);
        }
    }
}
