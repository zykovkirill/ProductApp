using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public ProductTypesController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }

      

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionResponse<ProductType>))]
        [HttpGet]
        //TODO : Раскомментировать 
        [Authorize(Roles = "Admin, User")]
        public IActionResult Get()
        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //TODO: сделать единый сервис со страницами, отдельный метод или класс с привязкой типа объекта

            try
            {
                var productTypes = _productsService.GetProductTypesAsync();
                return Ok(new CollectionResponse<ProductType>
                {
                    Count = productTypes.Count(),
                    IsSuccess = true,
                    Message = "Типы продуктов переданы",
                    OperationDate = DateTime.UtcNow,
                    Records = productTypes
                });
            }
            catch
            {
                //TODO: запись в лог
                return BadRequest(new OperationResponse<ProductType>
                {
                    Message = "Что-то пошло не так",
                    IsSuccess = false,
                    OperationDate = DateTime.UtcNow
                });
            }
        }


        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductType>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductType>))]
        [HttpPost]
        //TODO: раскомментировать
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] ProductType model)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            model.EditedUser = userId;
            var prod = await _productsService.AddProductTypeAsync(model);
            if (!prod)
                return BadRequest(new OperationResponse<ProductType>
                {
                    IsSuccess = false,
                    Message = "Тип продукта не создан",
                    OperationDate = DateTime.UtcNow
                });

            return Ok(new OperationResponse<ProductType>
            {
                Message = "Тип продукта создан",
                IsSuccess = true,
                OperationDate = DateTime.UtcNow
            });

        }
        #endregion

        #region Put 
       
   
        #endregion

        #region Delete
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var getOld = await _productsService.GetProductById(id);
            if (getOld == null)
                return NotFound();

            var deletedProduct = await _productsService.DeleteProductAsync(id);

            return Ok(new OperationResponse<Product>
            {
                IsSuccess = true,
                Message = $"{getOld.Name} продукт успешно удален!",
                Record = deletedProduct
            });
        }

        #endregion 


    }
}