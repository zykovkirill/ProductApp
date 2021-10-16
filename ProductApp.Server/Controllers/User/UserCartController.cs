using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProductApp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using ProductApp.Shared.Models;
using ProductApp.Server.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using ProductApp.Shared.Models.UserData;
using Newtonsoft.Json.Linq;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]

    public class UserCartController : ControllerBase
    {
        //TODO: Разделить IProductsService на несколько интерфейсов
        private readonly IUserDataService _userDataService;
        private readonly IConfiguration _configuration;
        public UserCartController(IUserDataService userDataService, IConfiguration configuration)
        {
            _userDataService = userDataService;
            _configuration = configuration;
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
        public async Task<IActionResult> Post([FromBody] JObject json)
        {
            //TODO: Можно динамически https://stackoverflow.com/questions/54158740/using-inherited-classes-in-net-web-api-post-put-method
            UserOrder model = json.ToObject<UserOrder>();
            if (model.Products.Any())
            {
                if (string.IsNullOrEmpty(model.UserId))
                    model.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var purchase = await _userDataService.AddOrderAsync(model);
                if (purchase == null)
                    return
                        BadRequest(new OperationResponse<UserOrder>
                        {
                            IsSuccess = false,
                            Message = $"Ошибка при формирование заказа",
                            OperationDate = DateTime.UtcNow,

                        });
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
               return  BadRequest(new OperationResponse<UserOrder>
                {
                    IsSuccess = false,
                    Message = $"Отсутствуют продукты",
                    OperationDate = DateTime.UtcNow,

                });
            }
        }

        #endregion

        #region Put 
        #endregion

        #region Delete

        #endregion 


    }
}