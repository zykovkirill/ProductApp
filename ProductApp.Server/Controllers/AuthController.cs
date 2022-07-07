using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApp.Server.Models;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System.Threading.Tasks;

namespace ProductApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private IUserService _userService;
        private IConfiguration _configuration;


        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

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
    }
}
