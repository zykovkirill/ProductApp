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
        // Сделать region для продуктов, продуктов пользователя 
        IEnumerable<Product> GetAllProductsAsync(int pageSize, int pageNumber, out int totalProducts);
        IEnumerable<Product> SearchProductsAsync(string query, int pageSize, int pageNumber, out int totalProducts);
        IEnumerable<Product> FilterProductsAsync(string filter, int pageSize, int pageNumber, out int totalProducts);
        Task<Product> AddProductAsync(string name, string description, int price, int type, string imagePath);
        Task<Product> EditProductAsync(string id, string newName, string description, int price, int type, string newImagePath);
        Task<UserCreatedProduct> AddUserProductAsync(UserCreatedProduct model, string userId);
        Task<Product> DeleteProductAsync(string id);
        Task<Product> GetProductById(string id);
        Product GetProductByName(string name);
        IEnumerable<UserCreatedProduct> GetAllUserProductsAsync(int pageSize, int pageNumber, string userId, out int totalProducts);
        Task<UserCreatedProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath);
        Task<UserCreatedProduct> GetUserProductById(string id);
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
            //TODO: ПЕРЕДАВАТЬ Модель и вставить её в Update()
           // _db.Products.Update(prod);
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
            //TODO:сделать отдельный сервис для сохранения и работы с  изображениями
            
            var profile = await _db.UserProfiles.Include(p => p.UserCreatedProducts).FirstOrDefaultAsync(pr => pr.UserId == userId);
            profile.UserCreatedProducts.Add(model);
            //await _db.UserProducts.AddAsync(model);
            await _db.SaveChangesAsync();

            return model;
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
            //TODO: ПЕРЕДАВАТЬ Модель и вставить её в Update()
            // _db.UserCreatedProducts.Update(prod);

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
    }

}

