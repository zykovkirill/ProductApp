using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;

        }

        #region Post
        // api/auth/register
        // [EnableCors]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при регистрации - {e}");
                return Problem("Ошибка при регистрации");
            }
        }

        //api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            //if (User.Identity != null)
            //{
            //    var authType = User.Identity.AuthenticationType;


            //    var isAuthenticated = User.Identity.IsAuthenticated;
            //    var name = User.Identity.Name;
            //    var claim = User.Claims;
            //}
            try
            {


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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при авторизации - {e}");
                return Problem("Ошибка при авторизации");
            }
        }

        //api/auth/ForgetPassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return NotFound();

                var result = await _userService.ForgetPsswordAsync(email);

                if (result.IsSuccess)
                    return Ok(result);// 200

                return BadRequest(result);// 400
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при смене пароля - {e}");
                return Problem("Ошибка при смене пароля ");
            }
        }

        //api/auth/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при сбросе пароля - {e}");
                return Problem("Ошибка при сбросе пароля");
            }
        }
        #endregion

        #region Get
        //api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при подтверждении почты - {e}");
                return Problem("Ошибка при подтверждении почты");
            }
        }
        #endregion
    }
}
