using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductTypesController> _logger;

        public ProductTypesController(IProductsService productsService, IConfiguration configuration, ILogger<ProductTypesController> logger)
        {
            _productsService = productsService;
            _configuration = configuration;
            _logger = logger;
        }



        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionResponse<ProductType>))]
        [HttpGet]
        //TODO : Раскомментировать 
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Get()
        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //TODO: сделать единый сервис со страницами, отдельный метод или класс с привязкой типа объекта

            try
            {
                var productTypes = await _productsService.GetProductTypesAsync();
                if (productTypes != null)
                    return Ok(new CollectionResponse<ProductType>
                    {
                        Count = productTypes.Count(),
                        IsSuccess = true,
                        Message = "Типы продуктов переданы",
                        OperationDate = DateTime.UtcNow,
                        Records = productTypes
                    });
                //TODO: запись в лог
                return BadRequest(new OperationResponse<ProductType>
                {
                    Message = "Что-то пошло не так",
                    IsSuccess = false,
                    OperationDate = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при запросе типов продуктов - {e}");
                return Problem("Ошибка при запросе типов  продуктов");
            }
        }


        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductType>))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] ProductType model)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model.EditedUser = userId;
                var prod = await _productsService.AddProductTypeAsync(model);
                return Ok(new OperationResponse<ProductType>
                {
                    Message = "Тип продукта создан",
                    IsSuccess = true,
                    OperationDate = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при создании типа продуктов - {e}");
                return Problem("Ошибка при создании типа продуктов");
            }

        }
        #endregion

        #region Put 


        #endregion

        #region Delete
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductType>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var getOld = await _productsService.GetProductById(id);
                if (getOld == null)
                    return NotFound();

                var deletedProduct = await _productsService.DeleteProductAsync(id);
                if (deletedProduct == null)
                    return BadRequest(new OperationResponse<ProductType>
                    {
                        Message = "Что-то пошло не так",
                        IsSuccess = false
                    });
                return Ok(new OperationResponse<Product>
                {
                    IsSuccess = true,
                    Message = $"{getOld.Name} продукт успешно удален!",
                    Record = deletedProduct
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при удалении типа продуктов - {e}");
                return Problem("Ошибка при удалении типа продуктов");
            }
        }

        #endregion 


    }
}