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
   public interface IUserService
    {

        Task<UserManagerResponse> RegisterUserAsync(RegisterRequest model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPsswordAsync(string email);

        Task<UserManagerResponse> ResetPsswordAsync(ResetPasswordViewModel model);
    }


    public class UserService : IUserService
    {

        private UserManager<IdentityUser> _userManager;
        //private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private ApplicationDbContext _db;

        public UserService(UserManager<IdentityUser> userManager,/* RoleManager<IdentityRole> roleManager,*/ IConfiguration configuration, IMailService mailService, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            //_roleManager = roleManager;
        }


        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest model)
        {
            if (model == null)
                throw new NullReferenceException("Модель регистрации не должна быть равна нулю");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Пароли не совподают",
                    IsSuccess = false,
                };

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {

                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
 
                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                // TODO: Иногда возникает ошибка с подтверждением 
                try
                {
                    await _mailService.SendEmailAsync(identityUser.Email, "Подтвердите свой email", "<h1>Добро пожаловать</h1>" + $"<p>Пожалуйста подтвердите свою электронную почту <a href = '{url}'>Нажмите сюда</a></p>");
                }
                catch
                {
                    Console.WriteLine("Возникло исключение при отправки сообщения  подтверждения электронной почты !");
                }


                // TODO: Может создать метод? Переименовать таблицу USERDATA в UserProfile
                UserProfile userData = new UserProfile
                {
                    UserId = identityUser.Id,
                    FirstName = model.FirstName, 
                    LastName = model.LastName
                };
                UserCart userCart = new UserCart
                {
                    UserId = identityUser.Id
                };
                await _db.UserProfiles.AddAsync(userData);
                await _db.UserCart.AddAsync(userCart);
                await _db.SaveChangesAsync();
                // TODO: Переименовать IUserService 
                // TODO: Отправлять подтверждение Email
                return new UserManagerResponse
                {
                    Message = "Пользователь успешно создан",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "Пользователь не был создан",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description).ToArray()

            };
        }


        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "Данный Email не соответствует не одному пользователю",
                    IsSuccess = false,

                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new UserManagerResponse
                {
                    Message = "Неправильный пароль",
                    IsSuccess = false,
                };
            // TODO: добавить инициализация ролей при создании БД мкрипт создающий пользователя и роль 
            //await _roleManager.CreateAsync(new IdentityRole("Admin"));            
            //await _userManager.AddToRoleAsync(user, "Admin");
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == null)
                role = "user";
           
            var claims = new[]
            {
                new Claim("Email",model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            //TODO: стоит поправить профиль в разоре и извлекать его не через запрос а из dirictory и убрать или объеденить LocalUserInfo
            var data = _db.UserProfiles.FirstOrDefault(d => d.UserId == user.Id);

            Dictionary<string, string> userinfo = new Dictionary<string, string>
            {
                { "Email", user.Email },
                { "FirstName", data.FirstName },
                { "LastName", data.LastName },
                { ClaimTypes.NameIdentifier, user.Id },
                //TODO:  { ClaimTypes.Role, role} - правильно ли передавать в таком виде лучше парсить из token
                { ClaimTypes.Role, role}
            };

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo,
                UserInfo = userinfo        
            };

        }



        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Пользователь не найден"
                };


            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Электронная почта подтверждена!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Электронная почта не подтверждена!",
                Errors = result.Errors.Select(e => e.Description).ToArray()
            };

        }

        public async Task<UserManagerResponse> ForgetPsswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Не найден пользователь с данной электронной почтой",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Сброс пароля", "<h1>Следуйте инструкциям для сброса пароля</h1>" + $"<p>Сброс пароля<a href ='{url}'>Нажмите сюда</a></p>");


            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Ссылка для сброса пароля отправлена"
            };

        }

        public async Task<UserManagerResponse> ResetPsswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Не найден пользователь с данной электронной почтой",
                };
            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Пароли не совподают",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Пароль изменён"
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Что-то пашло не так",
                Errors = result.Errors.Select(e => e.Description).ToArray(),
            };
        }
    }
}
