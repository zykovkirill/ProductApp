using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]
    public class UserProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private const int PageSize = 10;
        private readonly ILogger<UserProductsController> _logger;

        private readonly List<string> _allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };
        public UserProductsController(IProductsService productsService, IConfiguration configuration, ILogger<UserProductsController> logger)
        {
            _productsService = productsService;
            _configuration = configuration;
            _logger = logger;
        }
        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<UserCreatedProduct>))]
        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (page == 0)
                page = 1;
            (var totalProducts, var products) = await _productsService.GetAllUserProductsAsync(PageSize, page, userId);

            int totalPages = 0;
            if (totalProducts % PageSize == 0)
                totalPages = totalProducts / PageSize;
            else
                totalPages = (totalProducts / PageSize) + 1;
            //TODO: Протестировать логи заключить их в try
            _logger.LogInformation("Продукты переданы");
            return Ok(new CollectionPagingResponse<UserCreatedProduct>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = "Продукты переданы",
                OperationDate = DateTime.UtcNow,
                PageSize = PageSize,
                Page = page,
                Records = products
            });
        }

        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserCreatedProduct>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<UserCreatedProduct>))]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UserProductRequest model)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string url = $"{_configuration["AppUrl"]}UsersImages/default.jpg";
            string fullPath = null;
            if (model.CoverFile != null)
            {
                string extension = Path.GetExtension(model.CoverFile.FileName);

                if (!_allowedExtensions.Contains(extension))
                    return BadRequest(new OperationResponse<UserCreatedProduct>
                    {
                        Message = "Данный тип изображения не поддерживается",
                        IsSuccess = false,
                    });

                if (model.CoverFile.Length > 1000000)
                    return BadRequest(new OperationResponse<UserCreatedProduct>
                    {
                        Message = "Изображение не должно быть больше  10 мб",
                        IsSuccess = false,
                    });

                string newFileName = $"UsersImages/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }

            var userProd = new UserCreatedProduct
            {
                BaseProductId = model.ToyProductId,
                CoverPath = url,
                Name = model.FileName

            };
            userProd.IncludedProducts.Add(new IncludedProduct
            {
                ProductID = model.ChevronProductId,
                Size = float.Parse(model.Size, CultureInfo.InvariantCulture.NumberFormat),
                X = float.Parse(model.X, CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(model.Y, CultureInfo.InvariantCulture.NumberFormat)
            });
            var addedProduct = await _productsService.AddUserProductAsync(userProd, userId);
            if (addedProduct != null)
            {
                if (fullPath != null)
                {
                    using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await model.CoverFile.CopyToAsync(fs);
                    }
                }

                return Ok(new OperationResponse<UserCreatedProduct>
                {
                    IsSuccess = true,
                    Message = $"{addedProduct.Id} продукт успешно добавлен!",
                    Record = addedProduct
                });

            }
            return BadRequest(new OperationResponse<UserCreatedProduct>
            {
                Message = "Что-то пошло не так",
                IsSuccess = false
            });
        }
        #endregion

        #region Put 
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserCreatedProduct>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<UserCreatedProduct>))]
        [HttpPut]
        //TODO: попробовать типизировать все в единый запрос Put Post Get Delete <Product> <UserProduct> 
        public async Task<IActionResult> Put([FromForm] UserProductRequest model)
        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/default.jpg";
            string fullPath = null;
            if (model.Id == null)
                return BadRequest(new OperationResponse<UserCreatedProduct>
                {
                    Message = "Не найден продукт",
                    IsSuccess = false,
                });
            // Check the file 
            if (model.CoverFile != null)
            {
                string extension = Path.GetExtension(model.CoverFile.FileName);

                if (!_allowedExtensions.Contains(extension))
                    return BadRequest(new OperationResponse<UserCreatedProduct>
                    {
                        Message = "Данный тип изображения не поддерживается",
                        IsSuccess = false,
                    });

                if (model.CoverFile.Length > 500000)
                    return BadRequest(new OperationResponse<UserCreatedProduct>
                    {
                        Message = "Изображение не должно быть больше  5 мб",
                        IsSuccess = false,
                    });

                string newFileName = $"Images/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }
            var oldProduct = await _productsService.GetUserProductById(model.Id);
            if (fullPath == null)
                url = oldProduct.CoverPath;

            var editedProduct = await _productsService.EditUserProductAsync(model.Id, model.FileName, model.ChevronProductId, model.ToyProductId, float.Parse(model.X), float.Parse(model.Y), float.Parse(model.Size), url);

            if (editedProduct != null)
            {
                if (fullPath != null)
                {
                    using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await model.CoverFile.CopyToAsync(fs);
                    }
                }

                return Ok(new OperationResponse<UserCreatedProduct>
                {
                    IsSuccess = true,
                    Message = $"{editedProduct.Name} продукт успешно отредактирован!",
                    Record = editedProduct
                });
            }


            return BadRequest(new OperationResponse<UserCreatedProduct>
            {
                Message = "Что-то пошло не так",
                IsSuccess = false
            });

        }
        #endregion

    }
}