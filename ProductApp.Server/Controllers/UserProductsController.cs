using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ProductApp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using ProductApp.Shared.Models.UserData;
using ProductApp.Shared.Models;
using ProductApp.Server.Services;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = ("Admin"))]
    public class UserProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private const int PAGE_SIZE = 10;

        private readonly List<string> allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };
        public UserProductsController(IProductsService productsService, IConfiguration configuration)
        {
            _productsService = productsService;
            _configuration = configuration;
        }
        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<UserCreatedProduct>))]
        [HttpGet]
        public IActionResult Get(int page)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var products = _productsService.GetAllUserProductsAsync(PAGE_SIZE, page, userId, out totalProducts);

            int totalPages = 0;
            if (totalProducts % PAGE_SIZE == 0)
                totalPages = totalProducts / PAGE_SIZE;
            else
                totalPages = (totalProducts / PAGE_SIZE) + 1;

            return Ok(new CollectionPagingResponse<UserCreatedProduct>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = "Продукты переданы",
                OperationDate = DateTime.UtcNow,
                PageSize = PAGE_SIZE,
                Page = page,
                Records = products
            });
        }

        //TODO:Этот метод используется?
        //[ProducesResponseType(200, Type = typeof(FileStreamResult))]
        //[HttpGet("img")]
        //public IActionResult Get(string imgID)
        //{
        //    var imgStream = _productsService.GetImageAsync(imgID);
        //    var image = System.IO.File.OpenRead("C:\\3.png");
        //    var model =  File(image, "image/png");
        //    return model;
        //    return Ok(new OperationResponse<FileStreamResult>
        //    {
        //        Record = model
        //    });
        //}
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

                if (!allowedExtensions.Contains(extension))
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
                ChevronProductId = model.ChevronProductId,
                ToyProductId = model.ToyProductId,
                CoverPath = url,
                Size = float.Parse(model.Size, CultureInfo.InvariantCulture.NumberFormat),
                X = float.Parse(model.X, CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(model.Y, CultureInfo.InvariantCulture.NumberFormat),
                Name = model.FileName
                
            };

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

                if (!allowedExtensions.Contains(extension))
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