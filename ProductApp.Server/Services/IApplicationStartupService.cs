using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApp.Server.Services
{
    public interface IApplicationStartupService
    {
        Task CreateAdminUserAsync(RegisterRequest model);
        Task<bool> CreateDefaultProductTypeAsync(string userId);
    }


    public class ApplicationStartupService : IApplicationStartupService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProductsService _productsService;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IApplicationStartupService> _logger;
        //TODO : вынести в конфигурацию или ресурсы
        private const string RoleAdmin = "Admin";
        private const string RoleUser = "User";
        private const string TypeToys = "Игрушки";
        private const string TypeChevrons = "Шевроны";
        private const string TypeBeads = "Бисер";


        public ApplicationStartupService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, ILogger<IApplicationStartupService> logger, IProductsService productsService)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _productsService = productsService;
        }


        #region Create Admin User(Fist Load Application)
        public async Task CreateAdminUserAsync(RegisterRequest model)
        {
            try
            {

                var identityUser = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.FirstName
                };

                await _roleManager.CreateAsync(new IdentityRole(RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(RoleUser));
                var result = await _userManager.CreateAsync(identityUser, model.Password);
                await _userManager.AddToRoleAsync(identityUser, RoleAdmin);
                if (result.Succeeded)
                {

                    UserProfile userProfile = new UserProfile
                    {
                        UserId = identityUser.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EditedUser = identityUser.Id
                    };

                    await _db.UserProfiles.AddAsync(userProfile);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Администратор добавлен ");
                }
                else
                {
                    _logger.LogWarning("Администратор не был добавлен ");
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Администратор не был добавлен, произошла ошибка - {e} ");
            }
        }

        public async Task<bool> CreateDefaultProductTypeAsync(string userId)
        {
            try
            {
                //TODO: изображения по умолчанию + пользователь который создал 
                var productsType = new List<ProductType>();
                productsType.Add(new ProductType() { TypeName = TypeToys , EditedUser = userId});
                productsType.Add(new ProductType() { TypeName = TypeChevrons, EditedUser = userId });
                productsType.Add(new ProductType() { TypeName = TypeBeads, EditedUser = userId });

                var result = await _productsService.AddProductsTypeAsync(productsType);

                if (result)   
                    _logger.LogInformation("Типы продуктов добавлены ");

                else
                    _logger.LogWarning("Типы продуктов не  добавлены ");

                return result;
            }

            catch (Exception e)
            {   
                _logger.LogWarning($"Типы продуктов не были добавлены, произошла ошибка - {e} ");
                return false;
            }
        }

        #endregion
    }
}
