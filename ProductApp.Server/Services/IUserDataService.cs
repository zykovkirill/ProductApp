using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Server.Services
{
    public interface IUserDataService
    {

        IEnumerable<Product> GetAllProductsAsync(int pageSize, int pageNumber, out int totalProducts);
        IEnumerable<Product> SearchProductsAsync(string query, int pageSize, int pageNumber, out int totalProducts);
        Task<Product> AddProductToCartAsync(Product prod, int count);
        Task<Product> EditProductAsync(string id, string newName, string description, int price, string newImagePath);
        Task<Product> DeleteProductAsync(string id);
        Task<Product> GetProductById(string id);
        Product GetProductByName(string name); 
    }

    public class UserDataService : IUserDataService
    {

        private readonly ApplicationDbContext _db;
        public UserDataService(ApplicationDbContext db)
        {
            _db = db;
        }

        //TODO: Удалить может бы 
        public async Task<Product> AddProductToCartAsync(Product prod,  int count)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //await _db.Products.AddAsync(product);
            //await _db.SaveChangesAsync();

            return null;
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

        public async Task<Product> EditProductAsync(string id, string newName, string description, int price, string newImagePath)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod.IsDeleted)
                return null;

            prod.Name = newName;
            prod.Description = description;
            prod.Price = price;
            if (newImagePath != null)
                prod.CoverPath = newImagePath;
            prod.ModifiedDate = DateTime.Now;

            await _db.SaveChangesAsync();
            return prod;
        }

        public IEnumerable<Product> GetAllProductsAsync(int pageSize, int pageNumber, out int totalProducts)
        {
            // total prod 
            var allProducts = _db.Products.Where(p => !p.IsDeleted);
          
            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return prod; 
        }

        public async Task<Product> GetProductById(string id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product.IsDeleted)
                return null;

            return product;
        }

        public Product GetProductByName(string name)
        {
            var prod = _db.Products.SingleOrDefault(p => p.Name == name);
            if (prod.IsDeleted)
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


    }
}
