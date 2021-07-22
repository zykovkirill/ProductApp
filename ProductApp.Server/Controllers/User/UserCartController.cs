﻿using System.Collections.Generic;
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

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //  [Authorize]
    [Authorize(Roles = ("Admin"))]
    // [Authorize(Policy = "RequireAdministratorRole")]
    // [Authorize(AuthenticationSchemes = { BasicAuthenticationHandler.AuthenticationScheme, BasicAuthenticationHandler.AuthenticationScheme + 2 }, Roles = "admin")]
    public class UserCartController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;

        private const int PAGE_SIZE = 10;
        public UserCartController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }

        private readonly List<string> allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };

        #region Get    
        //TODO: НАДО  сделать POST - данный вариант не безопасен
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserProduct>))]
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
                            Message = $"Errors",
                            OperationDate = DateTime.UtcNow,
                            Record = product
                        });
                return Ok(new OperationResponse<Product>
                {
                    IsSuccess = true,
                    Message = $"Products of  received successfully!",
                    OperationDate = DateTime.UtcNow,
                    Record = product
                });
            }
            if (classType == (int)ClassType.UserProduct)
            {
                var product = await _productsService.AddUserProdToCartAsync(count, id, userId);
                if (product == null)
                    return
                        BadRequest(new OperationResponse<UserProduct>
                        {
                            IsSuccess = false,
                            Message = $"Errors",
                            OperationDate = DateTime.UtcNow,
                            Record = product
                        });
                return Ok(new OperationResponse<UserProduct>
                {
                    IsSuccess = true,
                    Message = $"Products of  received successfully!",
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


        [ProducesResponseType(200, Type = typeof(OperationResponse<UserCart>))]
        [HttpGet("GetProductFromCart")]
        public async Task<IActionResult> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userCart = await _productsService.GetProductsFromCart(userId);
            if (userCart == null)
                return
                    BadRequest(new OperationResponse<UserCart>
                    {
                        IsSuccess = false,
                        Message = $"Errors",
                        OperationDate = DateTime.UtcNow,
                        Record = userCart
                    });
            return Ok(new OperationResponse<UserCart>
            {
                IsSuccess = true,
                Message = $"Products of  received successfully!",
                OperationDate = DateTime.UtcNow,
                Record = userCart
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


        #endregion

        #region Put 
        #endregion

        #region Delete

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserProductInCart>))]
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


            return Ok(new OperationResponse<UserProductInCart>
            {
                IsSuccess = true,
                Message = $"{getOld.ProductName} has been deleted successfully!",
                Record = getOld
            });
        }
        #endregion 


    }
}