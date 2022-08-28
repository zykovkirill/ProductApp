using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApp.Server.Models;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System;
using System.Threading.Tasks;

namespace ProductApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IApplicationStartupService _applicationStartupService;

        public AuthController(IUserService userService, IApplicationStartupService applicationStartupService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _applicationStartupService = applicationStartupService;

        }

        #region Post
        // api/auth/register
        // [EnableCors]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess)
                    return Ok(result); // Код : 200

                return BadRequest(result);
            }

            return BadRequest("Одно или несколько свойств не прошли валидацию"); // Код : 400

        }

        //api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (User.Identity != null)
            {
                var authType = User.Identity.AuthenticationType;


                var isAuthenticated = User.Identity.IsAuthenticated;
                var name = User.Identity.Name;
                var claim = User.Claims;
            }
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    // await _mailService.SendEmailAsync(model.Email,"Привет", "<h1>Привет</h1>");
                    return Ok(result);
                }
                return BadRequest(result);

            }

            return BadRequest("Одно или несколько свойств не прошли валидацию"); // Код : 400
        }

        //api/auth/ForgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _userService.ForgetPsswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);// 200

            return BadRequest(result);// 400
        }

        //api/auth/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPsswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);

            }
            return BadRequest("Одно или несколько свойств не валидны");
        }
        #endregion

        #region Get
        //api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }
            return BadRequest(result);
        }
        ////api/auth/CreateAdminUser
        //[HttpGet("CreateAdminUser")]
        //public async Task<IActionResult> CreateAdminUser()
        //{
        //    try
        //    {
        //        //TODO: вынести в конфигурацию  в скрипт развертывания
        //        var config = _configuration.GetSection("FistStartSettings");
        //        Boolean.TryParse(config["IsNeedCreateAdminUser"], out bool IsNeedCreateAdminUser);
        //        if (IsNeedCreateAdminUser)
        //        {
        //            var model = new RegisterRequest()
        //            {
        //                Email = config["Email"],
        //                Password = config["Password"],
        //                ConfirmPassword = config["ConfirmPassword"],
        //                FirstName = config["FirstName"],
        //                LastName = config["LastName"]
        //            };

        //            await _applicationStartupService.CreateAdminUserAsync(model);
        //            return Ok("Пользователь-администратор создан");
        //        }
        //        return BadRequest("В настройках выключено создание пользователя-администратора ");
        //    }
        //    catch
        //    {
        //        return BadRequest("Произошла ошибка, обратитесь к администратору");
        //    }
        //}
        #endregion
    }
}
