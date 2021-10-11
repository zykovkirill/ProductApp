using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;
using System.IO;

namespace ProductApp.Server.Services
{
    //TODO: Обобщить интерфейс
    public interface IProductsService
    {
        // Сделать region для продуктов, продуктов пользователя и продуктов в корзине
        // TODO: Убрать лишнюю реализацию методов или перенести в другой интерфейс
        IEnumerable<Product> GetAllProductsAsync(int pageSize, int pageNumber, out int totalProducts);
        UserImageRequest GetImageAsync(string imgID);
        IEnumerable<Product> SearchProductsAsync(string query, int pageSize, int pageNumber, out int totalProducts);
        IEnumerable<Product> FilterProductsAsync(string filter, int pageSize, int pageNumber, out int totalProducts);
        Task<Product> AddProductAsync(string name, string description, int price, int type, string imagePath);
        Task<Product> EditProductAsync(string id, string newName, string description, int price, int type, string newImagePath);
        Task<Product> AddProdToCartAsync(int count, string Prodid, string userId);
        Task<UserCreatedProduct> AddUserProdToCartAsync(int count, string Prodid, string userId);
        Task<UserCreatedProduct> AddUserProductAsync(UserCreatedProduct model, string userId);
        Task<Product> DeleteProductAsync(string id);
        Task<Product> GetProductById(string id);
        Task<UserOrderProduct> DeleteProductFromCartById(string id);
        Product GetProductByName(string name);
        Task<UserOrder> GetProductsFromCart(string userId);
        IEnumerable<UserCreatedProduct> GetAllUserProductsAsync(int pageSize, int pageNumber, string userId, out int totalProducts);
        Task<UserCreatedProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath);
        Task<UserCreatedProduct> GetUserProductById(string id);

        Task<UserOrder> BuyProductsAsync(UserOrder model);
    }

    public class ProductsService : IProductsService
    {

        private readonly ApplicationDbContext _db;
        public ProductsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product> AddProductAsync(string name, string description, int price, int type, string imagePath)
        {
            var product = new Product
            {
                CoverPath = imagePath,
                Name = name,
                Description = description,
                Price = price,
                ProductType = type
            };

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<Product> AddProdToCartAsync(int count, string prodId, string userId)
        {
            var prod = await _db.Products.FindAsync(prodId);
            if (prod == null || prod.IsDeleted)
                return null;
            UserOrder userOrder;
            //TODO: Сравнить JOIN c INCLUDE
            userOrder = await _db.UserOrders.Include(p => p.Products).FirstOrDefaultAsync(o => o.UserId == userId && o.Status == Status.Cart);
            if (userOrder == null)
            {
                userOrder = new UserOrder()
                {
                    UserId = userId,
                    Status = Status.Cart
                };
                await _db.UserOrders.AddAsync(userOrder);
                var history = new OrderHistory()
                {
                    Status = Status.Cart,
                    IdOrder = userOrder.Id
                };
                await _db.PurchasesHistorys.AddAsync(history);
            }   
            var prodDB = userOrder.Products.FirstOrDefault(p => p.ProductId == prod.Id);

            if (prodDB != null)
                prodDB.ProductCount += count;
            else
            {
                UserOrderProduct userProductInCart = new UserOrderProduct()
                {
                    ProductId = prod.Id,
                    ProductCount = count,
                    ProductName = prod.Name,
                    ProductPrice = prod.Price,
                    ProductCoverPath = prod.CoverPath

                };
                userOrder.Products.Add(userProductInCart);
            }

            await _db.SaveChangesAsync();

            return prod;
        }
        //TODO: Настройки выносить в файл конфигурации для того чтобы не пересобирать
        public async Task<UserCreatedProduct> AddUserProdToCartAsync(int count, string prodId, string userId)
        {
            var prod = await _db.UserCreatedProducts.FindAsync(prodId);
            if (prod != null)
            {
                var toy = await _db.Products.FindAsync(prod.ToyProductId);
                var chevron = await _db.Products.FindAsync(prod.ChevronProductId);
                if (toy != null && chevron != null)
                {
                    if (prod == null || prod.IsDeleted)
                        return null;
                    UserOrder userOrder;
                    userOrder = await _db.UserOrders.Include(p => p.Products).FirstOrDefaultAsync(o => o.UserId == userId && o.Status == Status.Cart);
                    if (userOrder == null)
                    {
                        userOrder = new UserOrder()
                        {
                            UserId = userId,
                            Status = Status.Cart
                        };
                        await _db.UserOrders.AddAsync(userOrder);
                        var history = new OrderHistory()
                        {
                            Status = Status.Cart,
                            IdOrder = userOrder.Id
                        };
                        await _db.PurchasesHistorys.AddAsync(history);
                    }
                    var prodDB = userOrder.Products.FirstOrDefault(p => p.ProductId == prod.Id);

                    if (prodDB != null)
                        prodDB.ProductCount += count;
                    else
                    {

                        UserOrderProduct userProductInCart = new UserOrderProduct()
                        {
                            ProductId = prod.Id,
                            ProductCount = count,
                            ProductName = prod.Name,
                            //TODO: Добавить коэффициент стоимости зависящий от сложности изготовления и дополнительных рассходов(сделать отдельный класс для рассчета этого коэффициента)
                            ProductPrice = toy.Price + chevron.Price,
                            ProductCoverPath = prod.CoverPath
                        };
                        userOrder.Products.Add(userProductInCart);

                    }

                    await _db.SaveChangesAsync();
                }
            }
            return prod;
        }

        public async Task<UserOrder> GetProductsFromCart(string userId)
        {
            //TODO: AsNoTracking добавить туда где данные не изменяются
            var cart = await _db.UserOrders.Include(p => p.Products).AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.Status == Status.Cart);
            return cart;
        }


        public async Task<Product> DeleteProductAsync(string id)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod.IsDeleted)
                return null;

            prod.IsDeleted = true;
            prod.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return prod;
        }

        public async Task<Product> EditProductAsync(string id, string newName, string description, int price, int type, string newImagePath)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod.IsDeleted)
                return null;

            prod.Name = newName;
            prod.Description = description;
            prod.Price = price;
            prod.ProductType = type;
            if (newImagePath != null)
                prod.CoverPath = newImagePath;
            prod.ModifiedDate = DateTime.Now;

            await _db.SaveChangesAsync();
            return prod;
        }
        //TODO :Сделать асинхронно 
        public IEnumerable<Product> GetAllProductsAsync(int pageSize, int pageNumber, out int totalProducts)
        {
              var allProducts = _db.Products.Where(p => !p.IsDeleted).AsNoTracking();

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return prod;
        }

        //TODO :Сделать асинхронно 
        public IEnumerable<UserCreatedProduct> GetAllUserProductsAsync(int pageSize, int pageNumber, string userId, out int totalProducts)
        {
            var profile =  _db.UserProfiles.Include(p => p.UserCreatedProducts).AsNoTracking().FirstOrDefault(pr => pr.UserId == userId);
            var allProducts = profile.UserCreatedProducts.Where(p => !p.IsDeleted);

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            return prod;
        }

        public async Task<Product> GetProductById(string id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
                return null;

            return product;
        }

        public async Task<UserOrderProduct> DeleteProductFromCartById(string id)
        {
            UserOrderProduct product = _db.UserOrderProducts.FirstOrDefault(x => x.Id == id);
            if (product == null)
                return null;
            //TODO: Помечать как удаленные только Продукты, продукты пользователя, пользователей
            _db.UserOrderProducts.Remove(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public Product GetProductByName(string name)
        {
            var prod = _db.Products.SingleOrDefault(p => p.Name == name);
            if (prod == null || prod.IsDeleted)
                return null;

            return prod;
        }

        public IEnumerable<Product> SearchProductsAsync(string query, int pageSize, int pageNumber, out int totalProducts)
        {
            //total products 
            var allProducts = _db.Products.Where(p => !p.IsDeleted && (p.Description.Contains(query) || p.Name.Contains(query))).AsNoTracking();

            totalProducts = allProducts.Count();

            var products = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();


            return products;
        }
        /// <summary>
        /// Возвращает продукты с учетом фильтра по типам 
        /// </summary>
        /// <param name="filter">тип</param>
        /// <param name="pageSize">размер страницы</param>
        /// <param name="pageNumber">номер страницы</param>
        /// <param name="totalProducts">общее количество продуктов</param>
        /// <returns></returns>
        public IEnumerable<Product> FilterProductsAsync(string filter, int pageSize, int pageNumber, out int totalProducts)
        {
            List<Product> allProducts = new List<Product>();
            if (!String.IsNullOrWhiteSpace(filter))
            {
                string[] words = filter.Split(',');
                foreach (var i in words)
                {
                    allProducts.AddRange(_db.Products.Where(p => !p.IsDeleted && p.ProductType == Int32.Parse(i)).AsNoTracking().ToList());
                
                };
            }
            else
                allProducts = (_db.Products.Where(p => !p.IsDeleted)).ToList();

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return prod;
        }
        /// <summary>
        /// Добавляет продукт созданный пользователем
        /// </summary>
        /// <param name="model">Продукт пользователя</param>
        /// <returns></returns>
        public async Task<UserCreatedProduct> AddUserProductAsync(UserCreatedProduct model, string userId)
        {
            //TODO:сделать отдельный сервис для сохранения изображения
            
            var profile = await _db.UserProfiles.Include(p => p.UserCreatedProducts).FirstOrDefaultAsync(pr => pr.UserId == userId);
            profile.UserCreatedProducts.Add(model);
            //await _db.UserProducts.AddAsync(model);
            await _db.SaveChangesAsync();

            return model;
        }

        //TODO: Удалить
        public UserImageRequest GetImageAsync(string imgID)
        {
            //var prod = _db.UserCreatedProducts.FirstOrDefault(p => p.Id == imgID);
            //MemoryStream destination = new MemoryStream();
            //using (var file = File.OpenRead(@"c:\3.png"))
            //{
            //    file.CopyTo(destination);
            //};
            //var model = new UserImageRequest() { Data = destination };
            return null;
        }

        public async Task<UserCreatedProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath)
        {
            var prod = await _db.UserCreatedProducts.FindAsync(id);
            if (prod.IsDeleted)
                return null;

            prod.Name = newName;
            prod.ChevronProductId = chevronProductIdiption;
            prod.ToyProductId = toyProductId;
            prod.X = x;
            prod.Y = y;
            prod.Size = size;
            if (newImagePath != null)
                prod.CoverPath = newImagePath;
            prod.ModifiedDate = DateTime.Now;
           // _db.UserProducts.Update(prod);
            await _db.SaveChangesAsync();
            return prod;
        }

        public async Task<UserCreatedProduct> GetUserProductById(string id)
        {
            var product = await _db.UserCreatedProducts.FindAsync(id);
            if (product == null || product.IsDeleted)
                return null;

            return product;
        }

        public async Task<UserOrder> BuyProductsAsync(UserOrder model)
        {
            var order = await _db.UserOrders.FindAsync(model.Id);
            order.Status = Status.Buy;
            //order.ProductCount = model.ProductCount;
            //order.Products = model.Products;
            //order.TotalSum = model.TotalSum;
            //order.UserId = order.UserId;
            var history = new OrderHistory()
            {
                IdOrder = model.Id,
                Status = Status.Buy
            };
            //TODO: Task завернуть в исключения и логировать
            //TODO: Использовать Update везде где меняем данные
            await _db.PurchasesHistorys.AddAsync(history);
           // _db.UserOrders.Update(order);
            await _db.SaveChangesAsync();
            return order;
        }

    }

}

