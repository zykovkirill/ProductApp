using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private const int PageSize = 10;
        public ProductInfoController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }


        #region Get

        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpGet("info")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Get(string id)
        {
            // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var productInfo = await _productsService.GetProductInfoById(id);
            if (productInfo == null)
                return BadRequest(new OperationResponse<string>
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

        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpPost("comment")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Post([FromBody] BaseBuffer<Comment> model)
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

        [ProducesResponseType(200, Type = typeof(OperationResponse<ProductInfo>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ProductInfo>))]
        [HttpPost("rating")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Post([FromBody] BaseBuffer<Rating> model)
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
        #endregion

        #region Put 
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpPut]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Put([FromForm] ProductRequestServer model)
        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/default.jpg";
            string fullPath = null;
            // Check the file 
            if (model.CoverFile != null)
            {
                string extension = Path.GetExtension(model.CoverFile.FileName);

                //if (!allowedExtensions.Contains(extension))
                //    return BadRequest(new OperationResponse<Product>
                //    {
                //        Message = "Данный тип изображения не поддерживается",
                //        IsSuccess = false,
                //    });

                if (model.CoverFile.Length > 500000)
                    return BadRequest(new OperationResponse<Product>
                    {
                        Message = "Изображение не должно быть больше  5 мб",
                        IsSuccess = false,
                    });

                string newFileName = $"Images/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }
            var oldProduct = await _productsService.GetProductById(model.Id);
            if (fullPath == null)
                url = oldProduct.CoverPath;

            var editedProduct = await _productsService.EditProductAsync(model.Id, model.Name, model.Description, model.Price, model.ProductTypeId, url);

            if (editedProduct != null)
            {
                if (fullPath != null)
                {
                    using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await model.CoverFile.CopyToAsync(fs);
                    }
                }

                return Ok(new OperationResponse<Product>
                {
                    IsSuccess = true,
                    Message = $"{editedProduct.Name} продукт успешно отредактирован!",
                    Record = editedProduct
                });
            }


            return BadRequest(new OperationResponse<Product>
            {
                Message = "Что-то пошло не так",
                IsSuccess = false
            });

        }
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