using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductInfoController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductInfoController> _logger;
        public ProductInfoController(IProductsService productsService, IConfiguration configuration, ILogger<ProductInfoController> logger)
        {
            _productsService = productsService;
            _configuration = configuration;
            _logger = logger;
        }


        #region Get

        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpGet("info")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Get(string id)
        {
            // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var productInfo = await _productsService.GetProductInfoById(id);
                if (productInfo == null)
                    return BadRequest(new OperationResponse<ProductInfo>
                    {
                        IsSuccess = false,
                        Message = "Информация о продукте не найдена",
                    });

                return Ok(new OperationResponse<ProductInfo>
                {
                    Record = productInfo,
                    Message = "Информация о продукте передана",
                    IsSuccess = true,
                    OperationDate = DateTime.UtcNow
                });
            }
            catch(Exception e)
            {
                _logger.LogError($"Ошибка при запросе информации о продукте {id} - {e}");
                return Problem("Ошибка при запросе информации о продукте");
            }
        }

        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpPost("comment")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Post([FromBody] BaseBuffer<Comment> model)
        {
            try
            {
                // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var productInfo = await _productsService.AddCommentAsync(model.Id, model.Entity);
                if (productInfo != null)
                    return Ok(new OperationResponse<ProductInfo>
                    {
                        IsSuccess = true,
                        Message = $"Комментарий  успешно добавлен!",
                        Record = productInfo
                    });

                return BadRequest(new OperationResponse<ProductInfo>
                {
                    Message = "Что-то пошло не так",
                    IsSuccess = false
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при добавлении комментария о продукте - {e}");
                return Problem("Ошибка при добавлении комментария о продукте");
            }

        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpPost("rating")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Post([FromBody] BaseBuffer<Rating> model)
        {
            try
            {
                // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model.Entity.UserId = userId;
                var productInfo = await _productsService.AddRatingAsync(model.Id, model.Entity);
                if (productInfo != null)
                    return Ok(new OperationResponse<ProductInfo>
                    {
                        IsSuccess = true,
                        Message = $"Рейтинг  успешно добавлен!",
                        Record = productInfo
                    });


                return BadRequest(new OperationResponse<ProductInfo>
                {
                    Message = "Что-то пошло не так",
                    IsSuccess = false
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при добавлении рейтинга о продукте - {e}");
                return Problem("Ошибка при добавлении рейтинга о продукте");
            }

        }
        #endregion
        //TODO: Дописать изменение комментария 
        #region Put 
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpPut]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Put([FromForm] ProductRequestServer model)
        {
            return Problem("Метод не реализован");
        }
        #endregion
        //TODO: Дописать удаление комментария 
        #region Delete
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            return Problem("Метод не реализован");
        }

        #endregion


    }
}