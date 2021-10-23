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

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private const int _pageSize = 10;
        public ProductsController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }

        private readonly List<string> allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Get(int page)
        {
          //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
          //TODO: сделать единый сервис со страницами, отдельный метод или класс с привязкой типа объекта
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var products = _productsService.GetAllProductsAsync(_pageSize, page, out totalProducts);

            int totalPages = 0;
            if (totalProducts % _pageSize == 0)
                totalPages = totalProducts / _pageSize;
            else
                totalPages = (totalProducts / _pageSize) + 1;

            return Ok(new CollectionPagingResponse<Product>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = "Продукты переданы",
                OperationDate = DateTime.UtcNow,
                PageSize = _pageSize,
                Page = page,
                Records = products
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet("search")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Get(string query, int page)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var products = _productsService.SearchProductsAsync(query, _pageSize, page, out totalProducts);

            int totalPages = 0;
            if (totalProducts % _pageSize == 0)
                totalPages = totalProducts / _pageSize;
            else
                totalPages = (totalProducts / _pageSize) + 1;

            return Ok(new CollectionPagingResponse<Product>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = $"Продукты по запросу '{query}' ",
                OperationDate = DateTime.UtcNow,
                PageSize = _pageSize,
                Page = page,
                Records = products
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet("filter")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Get(int page, string filter)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var products = _productsService.FilterProductsAsync(filter, _pageSize, page, out totalProducts);

            int totalPages = 0;
            if (totalProducts % _pageSize == 0)
                totalPages = totalProducts / _pageSize;
            else
                totalPages = (totalProducts / _pageSize) + 1;

            return Ok(new CollectionPagingResponse<Product>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = $"Продукты по запросу '{filter}' ",
                OperationDate = DateTime.UtcNow,
                PageSize = _pageSize,
                Page = page,
                Records = products
            });
        }


        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpGet("Edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
           // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var prod = await _productsService.GetProductById(id);
            if (prod == null)
                return BadRequest(new OperationResponse<string>
                {
                    IsSuccess = false,
                    Message = "Продукт не найден",
                });

            return Ok(new OperationResponse<Product>
            {
                Record = prod,
                Message = "Продукт передан",
                IsSuccess = true,
                OperationDate = DateTime.UtcNow
            });
        }

        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm] ProductRequestServer model)
        {
           // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/default.jpg";
            string fullPath = null;
            // Check the file 
            if (model.CoverFile != null)
            {
                string extension = Path.GetExtension(model.CoverFile.FileName);

                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new OperationResponse<Product>
                    {
                        Message = "Данный тип изображения не поддерживается",
                        IsSuccess = false,
                    });

                if (model.CoverFile.Length > 5000000)
                    return BadRequest(new OperationResponse<Product>
                    {
                        Message = "Изображение не должно быть больше  5 мб",
                        IsSuccess = false,
                    });

                string newFileName = $"Images/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }

            using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                if (fullPath != null)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(model.CoverFile.OpenReadStream());
                    //TODO: сделать глобальные настройки разрешения
                    if (img.Width != 1000 && img.Height != 1000)
                    {
                        return BadRequest(new OperationResponse<Product>
                        {
                            Message = "Изображение  должно иметь разрешение 1000 x 1000 ",
                            IsSuccess = false,
                        });
                    }
                }
                var addedProduct = await _productsService.AddProductAsync(model.Name, model.Description, model.Price, model.ProductType, url);

                if (addedProduct != null)
                {
                    await model.CoverFile.CopyToAsync(fs);
                    return Ok(new OperationResponse<Product>
                    {
                        IsSuccess = true,
                        Message = $"{addedProduct.Name} продукт успешно добавлен!",
                        Record = addedProduct
                    });

                }
            }

            return BadRequest(new OperationResponse<Product>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromForm] ProductRequestServer model)
        {
          //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/default.jpg";
            string fullPath = null;
            // Check the file 
            if (model.CoverFile != null)
            {
                string extension = Path.GetExtension(model.CoverFile.FileName);

                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new OperationResponse<Product>
                    {
                        Message = "Данный тип изображения не поддерживается",
                        IsSuccess = false,
                    });

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

            var editedProduct = await _productsService.EditProductAsync(model.Id, model.Name, model.Description, model.Price, model.ProductType, url);

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