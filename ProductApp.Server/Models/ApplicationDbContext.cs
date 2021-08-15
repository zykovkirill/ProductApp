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

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserProduct> UserProducts { get; set; }

        public DbSet<UserPurchase> UserPurchases { get; set; }

        public DbSet<UserCart> UserCart  { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<UserProductBuy> UserProductBuy { get; set; }

        public DbSet<UserProductInCart> UserProductInCarts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        //public UsersContext(DbContextOptions<UsersContext> options)
        //    : base(options)
        //{
        //    Database.EnsureCreated();
        //}
    }
}
