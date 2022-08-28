using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;

namespace ProductApp.Server.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //TODO: Записи в БД НАСЛЕДУЙ от RECORD и не забывай при изменении записи менять поле EditUser, мб нужно обертку сделать какую-нибудь
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserCreatedProduct> UserCreatedProducts { get; set; }

        public DbSet<UserOrder> UserOrders { get; set; }

        public DbSet<OrderHistory> PurchasesHistorys { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductInfo> ProductInfos { get; set; }

        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserOrderProduct> UserOrderProducts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
