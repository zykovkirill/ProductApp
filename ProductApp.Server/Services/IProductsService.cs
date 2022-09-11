using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Server.Services
{
    //TODO: Обобщить интерфейс, разделить принцип  SOLID
    public interface IProductsService
    {
        // Сделать region для продуктов, продуктов пользователя 
        Task<(int, IEnumerable<Product>)> GetAllProductsAsync(int pageSize, int pageNumber);
        Task<IEnumerable<ProductType>> GetProductTypesAsync();
        Task<(int, IEnumerable<Product>)> SearchProductsAsync(string query, int pageSize, int pageNumber);
        Task<(int,IEnumerable<Product>)> FilterProductsAsync(string filter, int pageSize, int pageNumber);
        Task<Product> AddProductAsync(string name, string description, int price, string type, string imagePath, string editedUser);
        Task<Product> EditProductAsync(string id, string newName, string description, int price, string type, string newImagePath);
        Task<UserCreatedProduct> AddUserProductAsync(UserCreatedProduct model, string userId);
        Task<ProductInfo> AddCommentAsync(string id, Comment comment);
        Task<ProductInfo> AddRatingAsync(string id, Rating comment);
        Task<bool> AddProductTypeAsync(ProductType model);
        Task<bool> AddProductsTypeAsync(List<ProductType> models);
        Task<Product> DeleteProductAsync(string id);
        Task<Product> GetProductById(string id);
        Task<ProductInfo> GetProductInfoById(string id);
        Task<Product> GetProductByName(string name);
        Task <(int,IEnumerable<UserCreatedProduct>)> GetAllUserProductsAsync(int pageSize, int pageNumber, string userId);
        Task<UserCreatedProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath);
        Task<UserCreatedProduct> GetUserProductById(string id);
    }

    public class ProductsService : IProductsService
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<IApplicationStartupService> _logger;
        public ProductsService(ApplicationDbContext db, ILogger<IApplicationStartupService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Product> AddProductAsync(string name, string description, int price, string productTypeId, string imagePath, string editedUser)
        {
            var productType = await _db.ProductTypes.FindAsync(productTypeId);
            var product = new Product
            {
                CoverPath = imagePath,
                Name = name,
                Description = description,
                Price = price,
                ProductType = productType,
                EditedUser = editedUser
            };
            var productInfo = new ProductInfo()
            {

                EditedUser = editedUser
            };
            productInfo.ProductId = product.Id;
            await _db.ProductInfos.AddAsync(productInfo);
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

        public async Task<Product> EditProductAsync(string id, string newName, string description, int price, string productTypeId, string newImagePath)
        {
          
                var productType = await _db.ProductTypes.FindAsync(productTypeId);
                var prod = await _db.Products.FindAsync(id);
                if (prod.IsDeleted)
                    return null;

                prod.Name = newName;
                prod.Description = description;
                prod.Price = price;
                prod.ProductType = productType;
                if (newImagePath != null)
                    prod.CoverPath = newImagePath;
                prod.ModifiedDate = DateTime.UtcNow;
                //TODO: ПЕРЕДАВАТЬ Модель и вставить её в Update()
                // _db.Products.Update(prod);
                await _db.SaveChangesAsync();
                return prod;
        }
        //TODO :Сделать асинхронно используя кортежи
        public async Task<(int, IEnumerable<Product>)> GetAllProductsAsync(int pageSize, int pageNumber)
        {
            var allProducts = _db.Products.Include(p => p.ProductType).Where(p => !p.IsDeleted).AsNoTracking();

            var prod = await allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (allProducts.Count(), prod);
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            return await _db.ProductTypes.Where(p => !p.IsDeleted).AsNoTracking().ToListAsync();
        }

        public async Task<(int, IEnumerable<UserCreatedProduct>)> GetAllUserProductsAsync(int pageSize, int pageNumber, string userId)
        {
            var profile = await _db.UserProfiles.Include(p => p.UserCreatedProducts).AsNoTracking().FirstOrDefaultAsync(pr => pr.UserId == userId);
            var allProducts = profile.UserCreatedProducts.Where(p => !p.IsDeleted);

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            return(allProducts.Count(), prod);
        }

        public async Task<Product> GetProductById(string id)
        {
            var product = await _db.Products.Include(p => p.ProductType).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null || product.IsDeleted)
                return null;

            return product;
        }

        public async Task<ProductInfo> GetProductInfoById(string id)
        {
            var productInfo = await _db.ProductInfos.Include(p => p.Comments).AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (productInfo == null || productInfo.IsDeleted)
                return null;

            return productInfo;
        }

        public async Task<ProductInfo> AddCommentAsync(string id, Comment comment)
        {
            var productInfo = await _db.ProductInfos.Include(p => p.Comments).FirstOrDefaultAsync(p => p.ProductId == id);
            if (productInfo == null || productInfo.IsDeleted)
                return null;
            productInfo.Comments.Add(comment);
            _db.SaveChanges();
            return productInfo;
        }
        public async Task<ProductInfo> AddRatingAsync(string id, Rating rating)
        {
            var productInfo = await _db.ProductInfos.Include(p => p.Ratings).FirstOrDefaultAsync(p => p.ProductId == id);
            if (productInfo == null || productInfo.IsDeleted)
                return null;
            var oldRating = productInfo.Ratings.FirstOrDefault(r => r.UserId == rating.UserId);
            if (oldRating == null)
                productInfo.Ratings.Add(rating);
            else
            {
                oldRating.ProductRating = rating.ProductRating;
                //TODO: Изменять дату  где меняются данные 
                oldRating.ModifiedDate = DateTime.UtcNow;
            }
            _db.SaveChanges();
            return productInfo;
        }
        public async Task<Product> GetProductByName(string name)
        {
            var prod = await _db.Products.SingleOrDefaultAsync(p => p.Name == name);
            if (prod == null || prod.IsDeleted)
                return null;
            return prod;
        }

        //TODO: применить кортеж чтобы сделать асинхронныим 
        public async Task<(int, IEnumerable<Product>)> SearchProductsAsync(string query, int pageSize, int pageNumber)
        {
            var allProducts = _db.Products.Where(p => !p.IsDeleted && (p.Description.Contains(query) || p.Name.Contains(query))).AsNoTracking();

            var products = await allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (allProducts.Count(), products);
        }
        /// <summary>
        /// Возвращает продукты с учетом фильтра по типам 
        /// </summary>
        /// <param name="type">тип</param>
        /// <param name="pageSize">размер страницы</param>
        /// <param name="pageNumber">номер страницы</param>
        /// <param name="totalProducts">общее количество продуктов</param>
        /// <returns></returns>
        /// TODO: ПЕРЕДЕЛАТЬ ФИЛДЬТР НА ФРОНТЕ
        public async Task<(int,IEnumerable<Product>)> FilterProductsAsync(string type, int pageSize, int pageNumber)
        {
            if (!String.IsNullOrWhiteSpace(type))
            {
                List<Product> allProducts = new List<Product>();
                string[] words = type.Split(',');
                foreach (var i in words)
                {
                   allProducts.AddRange( _db.Products.Include(p => p.ProductType).Where(p => !p.IsDeleted && p.ProductType.Id == i).AsNoTracking());
                };
                return (allProducts.Count(), allProducts);
            }
            var prod = await (_db.Products.Where(p => !p.IsDeleted)).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (prod.Count(), prod);
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
            //TODO : Проверить использование сервисом возвращаемых значений - может проще bool сделать а на вход модель добавления 
            return model;
        }

        public async Task<UserCreatedProduct> EditUserProductAsync(string id, string newName, string chevronProductIdiption, string toyProductId, float x, float y, float size, string newImagePath)
        {
            var prod = await _db.UserCreatedProducts.FindAsync(id);
            if (prod.IsDeleted)
                return null;

            prod.Name = newName;
          //  prod.ChevronProductId = chevronProductIdiption;
            //prod.ToyProductId = toyProductId;
            //prod.X = x;
            //prod.Y = y;
            //prod.Size = size;
            if (newImagePath != null)
                prod.CoverPath = newImagePath;
            prod.ModifiedDate = DateTime.UtcNow;
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

        public async Task<bool> AddProductTypeAsync(ProductType model)
        {
            await _db.ProductTypes.AddAsync(model);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddProductsTypeAsync(List<ProductType> models)
        {
            //При первом добавлении типов user
            await _db.ProductTypes.AddRangeAsync(models);
            var user = await _db.Users.FirstOrDefaultAsync();
            await _db.SaveChangesAsync();
            return true;
        }
    }

}

