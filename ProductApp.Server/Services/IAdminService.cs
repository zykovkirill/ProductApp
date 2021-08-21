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

namespace ProductApp.Server.Services
{
   public interface IAdminService
    {


        IEnumerable<IdentityUser> GetAllUsersAsync(int pageSize, int pageNumber, out int totalUsers);
        Task<UserManagerResponse> CreateUserAsync(RegisterRequest model);
        Task<ChangeRoleViewModel> GetUserById(string id);
        Task<UserManagerResponse> EditUserById(ChangeRoleViewModel model);
        Task<UserManagerResponse> DeleteUserById(string id);
    }


    public class AdminService : IAdminService
    {

        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private ApplicationDbContext _db;

        public AdminService(UserManager<IdentityUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;

        }

        //TODO: сделать асинхронно
        public  IEnumerable<IdentityUser> GetAllUsersAsync(int pageSize, int pageNumber, out int totalUsers)
        {
            // total prod 
            var allUsers =  _db.Users;

            totalUsers = allUsers.Count();

            var users = allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();

            return users;
        }



        public async Task<UserManagerResponse> CreateUserAsync(RegisterRequest model)
        {
            var allUsers = _db.Users;
            var user = new IdentityUser { Email = model.Email, UserName = model.FirstName };
            var result = await _userManager.CreateAsync(user, model.Password);
     
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Пользователь создан",
                    IsSuccess = true,
                };
            }
            else
            {
                return new UserManagerResponse
                {
                    Message = "Пользователь не создан",
                    IsSuccess = false,
                    Errors = result.Errors.Select(r => r.Description).ToArray()
                };
            }

           

        }

    
        public async Task<ChangeRoleViewModel> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.Select(r=>r.Name).ToList();
                List<UserRoleViewModel> roles = new List<UserRoleViewModel>();
                foreach (var all in allRoles) {
                    var result = userRoles.FirstOrDefault(u => u == all);
                    if(result == null)
                        roles.Add(new UserRoleViewModel(false, all));
                    else
                        roles.Add(new UserRoleViewModel(true, all));
                }
                //TODO: OperationResponse<T> сделать во всех ответах
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    Roles = roles
                };
                return model;
            }
                return null;

        }

        public async Task<UserManagerResponse> EditUserById(ChangeRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);



            if (user == null)


                return new UserManagerResponse
                {
                    Message = "Пользователь не найден",
                    IsSuccess = false,
                };

                user.Email = model.UserEmail;
                user.UserName = model.UserEmail;


            // получем список ролей пользователя
            var userRoles = await _userManager.GetRolesAsync(user);
            // получаем все роли
            //var allRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            var rolesAdd = model.Roles.Where(r => r.IsOn == true).Select(n => n.RoleName);
            var roleRemoved = model.Roles.Where(r => r.IsOn == true).Select(n => n.RoleName);
            var addedRoles = rolesAdd.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(roleRemoved);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);




            var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = "Пользователь отредактирован",
                        IsSuccess = true,
                    };
                }
                else
                {
                    return new UserManagerResponse
                    {
                        Message = "Пользователь не отредактирован",
                        IsSuccess = false,
                        Errors = result.Errors.Select(r => r.Description).ToArray()
                    };
                }
            

        }
        public async Task<UserManagerResponse> DeleteUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "Пользователь не найден",
                    IsSuccess = false,
                };

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Пользователь удален",
                    IsSuccess = true,
                };
            }
            else
            {
                return new UserManagerResponse
                {
                    Message = "Пользователь не удален",
                    IsSuccess = false,
                    Errors = result.Errors.Select(r => r.Description).ToArray()
                };
            }

        }


    }
    }

