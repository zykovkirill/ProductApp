using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        Task<UserProduct> AddUserProdToCartAsync(int count, string Prodid, string userId);
        Task<UserProduct> AddUserProductAsync(UserProduct model, string userId);
        Task<Product> DeleteProductAsync(string id);
        Task<Product> GetProductById(string id);
        Task<UserProductInCart> DeleteProductFromCartById(string id);
        Product GetProductByName(string name);
        Task<UserCart> GetProductsFromCart(string userId);
        IEnumerable<UserProduct> GetAllUserProductsAsync(int pageSize, int pageNumber, out int totalProducts);
        Task<UserProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath);
        Task<UserProduct> GetUserProductById(string id);

        Task<UserPurchase> BuyProductsAsync(IList<UserProductInCart> model);
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
        //TODO: МОЖЕТ СДЕЛАТЬ  ВСЕХ НАСЛЕДНИКАМИ RECORD
        public async Task<Product> AddProdToCartAsync(int count, string prodId, string userId)
        {
            var prod = await _db.Products.FindAsync(prodId);
            if (prod == null || prod.IsDeleted)
                return null;
            var cart = await _db.UserCart.Include(p => p.Products).FirstOrDefaultAsync(pr => pr.UserId == userId);
            var prodDB = cart.Products.FirstOrDefault(p => p.ProductId == prod.Id);

            if (prodDB != null)
                prodDB.ProductCount += count;
            else
            {
                UserProductInCart userProductInCart = new UserProductInCart()
                {
                    ProductId = prod.Id,
                    ProductCount = count,
                    ProductName = prod.Name,
                    ProductPrice = prod.Price,
                    ProductCoverPath = prod.CoverPath

                };
                cart.Products.Add(userProductInCart);
            }

            await _db.SaveChangesAsync();

            return prod;
        }
        //TODO: Настройки выносить в файл конфигурации для того чтобы не пересобирать
        //TODO: МОЖЕТ СДЕЛАТЬ  ВСЕХ НАСЛЕДНИКАМИ RECORD
        public async Task<UserProduct> AddUserProdToCartAsync(int count, string prodId, string userId)
        {
            var prod = await _db.UserProducts.FindAsync(prodId);
            if (prod != null)
            {
                var toy = await _db.Products.FindAsync(prod.ToyProductId);
                var chevron = await _db.Products.FindAsync(prod.ChevronProductId);
                if (toy != null && chevron != null)
                {
                    if (prod == null || prod.IsDeleted)
                        return null;
                    var cart = await _db.UserCart.Include(p => p.Products).FirstOrDefaultAsync(pr => pr.UserId == userId);
                    var prodDB = cart.Products.FirstOrDefault(p => p.ProductId == prod.Id);

                    if (prodDB != null)
                        prodDB.ProductCount += count;
                    else
                    {

                        UserProductInCart userProductInCart = new UserProductInCart()
                        {
                            ProductId = prod.Id,
                            ProductCount = count,
                            ProductName = prod.Name,
                            //TODO: Добавить коэффициент стоимости зависящий от сложности изготовления и доболнительных рассходов(сделать отдельный класс для рассчета этого коэффициента)
                            ProductPrice = toy.Price + chevron.Price,
                            ProductCoverPath = prod.CoverPath
                        };
                        cart.Products.Add(userProductInCart);

                    }

                    await _db.SaveChangesAsync();
                }
            }
            return prod;
        }

        public async Task<UserCart> GetProductsFromCart(string userId)
        {

            var cart = await _db.UserCart.Include(p => p.Products).FirstOrDefaultAsync(c => c.UserId == userId);
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
            // total prod 
            var allProducts = _db.Products.Where(p => !p.IsDeleted);

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return prod;
        }

        //TODO :Сделать асинхронно 
        public IEnumerable<UserProduct> GetAllUserProductsAsync(int pageSize, int pageNumber, out int totalProducts)
        {
            // total prod 
            var allProducts = _db.UserProducts.Where(p => !p.IsDeleted);

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

        public async Task<UserProductInCart> DeleteProductFromCartById(string id)
        {
            UserProductInCart product = _db.UserProductInCarts.FirstOrDefault(x => x.Id == id);
            if (product == null)
                return null;

            _db.UserProductInCarts.Remove(product);
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
            var allProducts = _db.Products.Where(p => !p.IsDeleted && (p.Description.Contains(query) || p.Name.Contains(query)));

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
                    allProducts.AddRange((_db.Products.Where(p => !p.IsDeleted && p.ProductType == Int32.Parse(i))).ToList());
                
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
        public async Task<UserProduct> AddUserProductAsync(UserProduct model, string userId)
        {
            //TODO:сделать отдельный сервис для сохранения изображения
            
            var profile = await _db.UserProfiles.Include(p => p.UserProducts).FirstOrDefaultAsync(pr => pr.UserId == userId);
            profile.UserProducts.Add(model);
            //await _db.UserProducts.AddAsync(model);
            await _db.SaveChangesAsync();

            return model;
        }

        public UserImageRequest GetImageAsync(string imgID)
        {
            var prod = _db.UserProducts.FirstOrDefault(p => p.Id == imgID);
            MemoryStream destination = new MemoryStream();
            using (var file = File.OpenRead(@"c:\3.png"))
            {
                file.CopyTo(destination);
            };
            var model = new UserImageRequest() { Data = destination };
            return model;
        }

        public async Task<UserProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath)
        {
            var prod = await _db.UserProducts.FindAsync(id);
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

            await _db.SaveChangesAsync();
            return prod;
        }

        public async Task<UserProduct> GetUserProductById(string id)
        {
            var product = await _db.UserProducts.FindAsync(id);
            if (product == null || product.IsDeleted)
                return null;

            return product;
        }

        public async Task<UserPurchase> BuyProductsAsync(IList<UserProductInCart> model)
        {
            //TODO: ВСЕ КРИВО!
            List<UserProductBuy> buy = new List<UserProductBuy>();
            foreach (var mbox in model)
            {
                var b = new UserProductBuy()
                {
                    ProductId = mbox.ProductId,
                    ProductName = mbox.ProductName,
                    ProductCount = mbox.ProductCount,
                    ProductCoverPath = mbox.ProductCoverPath,
                    ProductPrice = mbox.ProductPrice
                };
                buy.Add(b);
            }

            var purchase = new UserPurchase()
            {
                UserProductBuy = buy,
                Satus = ProductApp.Shared.Models.Status.Buy,
                PurchaseTime = DateTime.UtcNow

            };

            
             _db.UserProductInCarts.RemoveRange(model);
            await _db.UserProductBuy.AddRangeAsync(buy);
            await _db.UserPurchases.AddAsync(purchase);
            await _db.SaveChangesAsync();
            return purchase;
        }

    }

}

