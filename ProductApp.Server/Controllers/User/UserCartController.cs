using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]

    public class UserCartController : ControllerBase
    {
        //TODO: Разделить IProductsService на несколько интерфейсов
        private readonly IUserDataService _userDataService;
        private readonly ILogger<UserCartController> _logger;
        public UserCartController(IUserDataService userDataService,  ILogger<UserCartController> logger)
        {
            _userDataService = userDataService;
            _logger = logger;
        }

        #region Get    

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserOrder>))]
        [HttpGet("GetProductFromCart")]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userOrder = await _userDataService.GetProductsFromCart(userId);
            if (userOrder == null)
                return
                     Ok(new OperationResponse<UserOrder>
                     {
                         IsSuccess = false,
                         Message = $"Корзина пуста",
                         OperationDate = DateTime.UtcNow,
                         Record = userOrder
                     });
            return Ok(new OperationResponse<UserOrder>
            {
                IsSuccess = true,
                Message = $"Продукты из корзины",
                OperationDate = DateTime.UtcNow,
                Record = userOrder
            });
        }

        #endregion

        #region Post 

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserOrder>))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JToken json)
        {
            try
            {
                //TODO: Можно динамически https://stackoverflow.com/questions/54158740/using-inherited-classes-in-net-web-api-post-put-method
                //TODO: Создать дерево ролевой модели
                UserOrder model = json.ToObject<UserOrder>();
                if (model.Products.Any())
                {

                    //if (string.IsNullOrEmpty(model.UserId))
                        model.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    //else
                    //{
                    //    //TODO: сделать лог
                    //}
                    var purchase = await _userDataService.AddOrderAsync(model);
                    return Ok(new OperationResponse<UserOrder>
                    {
                        IsSuccess = true,
                        Message = $"Корзина сохранена!",
                        OperationDate = DateTime.UtcNow,
                        Record = purchase
                    });
                }
                else
                {
                    return BadRequest(new OperationResponse<UserOrder>
                    {
                        IsSuccess = false,
                        Message = $"Отсутствуют продукты",
                        OperationDate = DateTime.UtcNow,

                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при добавлении продукта  - {e}");
                return Problem("Ошибка при добавлении продукта");
    }
}

        #endregion

        #region Put 
        #endregion

        #region Delete

        #endregion 


    }
}