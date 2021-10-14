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
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        public UserCartController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }

        #region Get    
        //TODO: НАДО  сделать POST - данный вариант не безопасен
        //TODO: Удалить данный метод
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserCreatedProduct>))]
        [HttpGet("addprod")]
        ///<summary>
        ///Добавляет продукты в корзину
        /// </summary>
        public async Task<IActionResult> Get(int count, string id, int classType)
        {

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (classType == (int)ClassType.Product)
            {
                //TODO: Может типизировать запрос????
                var product = await _productsService.AddProdToCartAsync(count, id, userId);
                if (product == null)
                    return
                        BadRequest(new OperationResponse<Product>
                        {
                            IsSuccess = false,
                            Message = $"Ошибка",
                            OperationDate = DateTime.UtcNow,
                            Record = product
                        });
                return Ok(new OperationResponse<Product>
                {
                    IsSuccess = true,
                    Message = $"Продукты добавлены в корзину",
                    OperationDate = DateTime.UtcNow,
                    Record = product
                });
            }
            if (classType == (int)ClassType.UserProduct)
            {
                var product = await _productsService.AddUserProdToCartAsync(count, id, userId);
                if (product == null)
                    return
                        BadRequest(new OperationResponse<UserCreatedProduct>
                        {
                            IsSuccess = false,
                            Message = $"Ошибки",
                            OperationDate = DateTime.UtcNow,
                            Record = product
                        });
                return Ok(new OperationResponse<UserCreatedProduct>
                {
                    IsSuccess = true,
                    Message = $"Продукты добавлены в корзину",
                    OperationDate = DateTime.UtcNow,
                    Record = product
                });
            }
            return
                   BadRequest(new BaseAPIResponse()
                   {
                       IsSuccess = false,
                       Message = $"Не найден тип"
                   });
        }


        [ProducesResponseType(200, Type = typeof(OperationResponse<UserOrder>))]
        [HttpGet("GetProductFromCart")]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userOrder = await _productsService.GetProductsFromCart(userId);
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
        ////TODO: Лучше сделать POST
        //[ProducesResponseType(200, Type = typeof(OperationResponse<UserProduct>))]
        //[HttpPost("adduserprod")]
        //public async Task<IActionResult> Post(int count, string id)
        //{
        //    string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var product = await _productsService.AddProdToCartAsync(count, id, userId);
        //    if (product == null)
        //        return
        //            BadRequest(new OperationResponse<Product>
        //            {
        //                IsSuccess = false,
        //                Message = $"Errors",
        //                OperationDate = DateTime.UtcNow,
        //                Record = product
        //            });
        //    return Ok(new OperationResponse<UserProduct>
        //    {
        //        IsSuccess = true,
        //        Message = $"Products of  received successfully!",
        //        OperationDate = DateTime.UtcNow,
        //      //  Record = product
        //    });
        //}

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
                var purchase = await _productsService.AddOrderAsync(model);
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

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserOrderProduct>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var getOld = await _productsService.DeleteProductFromCartById(id);
            if (getOld == null)
                return NotFound();

            // Remove the file 
            //string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", getOld.CoverPath.Replace(_configuration["AppUrl"], ""));
            //System.IO.File.Delete(fullPath);


            return Ok(new OperationResponse<UserOrderProduct>
            {
                IsSuccess = true,
                //Message = $"{getOld.ProductName} has been deleted successfully!",
                Record = getOld
            });
        }
        #endregion 


    }
}