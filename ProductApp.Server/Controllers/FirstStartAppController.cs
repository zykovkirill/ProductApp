using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApp.Server.Models;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirstStartAppController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IApplicationStartupService _applicationStartupService;

        public FirstStartAppController(IApplicationStartupService applicationStartupService, IConfiguration configuration)
        {
            _configuration = configuration;
            _applicationStartupService = applicationStartupService;

        }

        #region Get
        //api/FirstStartApp/CreateAdminUser
        [HttpGet("CreateAdminUser")]
        public async Task<IActionResult> CreateAdminUser()
        {
            try
            {
                //TODO: вынести в конфигурацию  в скрипт развертывания
                var config = _configuration.GetSection("FistStartSettings");
                Boolean.TryParse(config["IsNeedCreateAdminUser"], out bool IsNeedCreateAdminUser);
                if (IsNeedCreateAdminUser)
                {
                    var model = new RegisterRequest()
                    {
                        Email = config["Email"],
                        Password = config["Password"],
                        ConfirmPassword = config["ConfirmPassword"],
                        FirstName = config["FirstName"],
                        LastName = config["LastName"]
                    };

                    await _applicationStartupService.CreateAdminUserAsync(model);
                    return Ok("Пользователь-администратор создан");
                }
                return BadRequest("В настройках выключено создание пользователя-администратора ");
            }
            catch
            {
                return BadRequest("Произошла ошибка, обратитесь к администратору");
            }
        }


        //api/FirstStartApp/CreateProductType
        [HttpGet("CreateProductType")]
        public async Task<IActionResult> CreateProductType()
        {
            try
            {
                //TODO: вынести в конфигурацию  в скрипт развертывания
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var config = _configuration.GetSection("FistStartSettings");
                Boolean.TryParse(config["IsNeedCreateDefaultProductType"], out bool IsNeedCreateAdminUser);
                if (IsNeedCreateAdminUser)
                {
                    var result = await _applicationStartupService.CreateDefaultProductTypeAsync(userId);
                    if (result)
                        return Ok("Типы Продуктов по умолчанию добавлены");
                    else
                        return BadRequest("Произошла ошибка при добавлении");
                }
                return BadRequest("В настройках выключено создание типов продуктов по умолчанию ");
            }
            catch
            {
                return BadRequest("Произошла ошибка, обратитесь к администратору");
            }
        }

        #endregion
    }
}
