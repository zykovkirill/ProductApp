using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;

namespace ProductApp.Server.Services
{
   public interface IPurchasesService
    {

      IEnumerable<UserPurchase> GetPurchase(int pageSize, int pageNumber, out int totalProducts);

    }


    public class PurchasesService : IPurchasesService
    {

        private readonly ApplicationDbContext _db;

        public PurchasesService(ApplicationDbContext db)
        {
            _db = db;
        }


        //TODO :Сделать асинхронно 
        public IEnumerable<UserPurchase> GetPurchase(int pageSize, int pageNumber, out int totalProducts)
        {
            //TODO: IsDeleted - Нужно добавить? смотри GetAllUserProductsAsync
            var allProducts = _db.UserPurchases;

            totalProducts = allProducts.Count();

            var prod = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return prod;
        }



    }
}
