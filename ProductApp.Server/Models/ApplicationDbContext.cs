using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductApp.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductApp.Shared.Models.UserData;

namespace ProductApp.Server.Models
{
    public class ApplicationDbContext :IdentityDbContext
    {
        //TODO: Записи в БД НАСЛЕДУЙ от RECORD
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserCreatedProduct> UserCreatedProducts { get; set; }

        public DbSet<UserOrder> UserOrders  { get; set; }

        public DbSet<OrderHistory> PurchasesHistorys { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<UserOrderProduct> UserOrderProducts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
