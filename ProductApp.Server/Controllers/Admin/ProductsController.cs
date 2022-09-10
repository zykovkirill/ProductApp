using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductsController> _logger;
        private const int PageSize = 10;
        public ProductsController(IProductsService productsService, IConfiguration configuration, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _configuration = configuration;
            _logger = logger;
        }

        private readonly List<string> allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]

        //TODO: переделать на async Task<IActionResult>
        public async Task<IActionResult> Get(int page)
        {
            try
            {
                // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //TODO: сделать единый сервис со страницами, отдельный метод или класс с привязкой типа объекта
                if (page == 0)
                    page = 1;

                //TODO : Логика исключений будет лежать полностью на сервисе если null возвращает то возвращаем badREquest
                (int totalProducts, var products) = await _productsService.GetAllProductsAsync(PageSize, page);
                if (!products.Any())
                    return BadRequest(new CollectionPagingResponse<Product>
                    {

                        IsSuccess = false,
                        Message = "Не найдено продуктов",
                        OperationDate = DateTime.UtcNow,

                    });
                int totalPages = 0;
                if (totalProducts % PageSize == 0)
                    totalPages = totalProducts / PageSize;
                else
                    totalPages = (totalProducts / PageSize) + 1;

                return Ok(new CollectionPagingResponse<Product>
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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при запросе продуктов - {e}");
                return Problem("Ошибка при запросе продуктов");
            }
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet("search")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Get(string query, int page)
        {
            try
            {
                //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (page == 0)
                    page = 1;
                (var totalProducts, var products) = await _productsService.SearchProductsAsync(query, PageSize, page);
                if (!products.Any())
                    return BadRequest(new CollectionPagingResponse<Product>
                    {

                        IsSuccess = false,
                        Message = "Не найдено продуктов",
                        OperationDate = DateTime.UtcNow,

                    });
                int totalPages = 0;
                if (totalProducts % PageSize == 0)
                    totalPages = totalProducts / PageSize;
                else
                    totalPages = (totalProducts / PageSize) + 1;

                return Ok(new CollectionPagingResponse<Product>
                {
                    Count = totalProducts,
                    IsSuccess = true,
                    Message = $"Продукты по запросу '{query}' ",
                    OperationDate = DateTime.UtcNow,
                    PageSize = PageSize,
                    Page = page,
                    Records = products
                });
            }
            catch (Exception e)
            {
                //TODO: логи записать в словарь(нужно для локализации)
                _logger.LogError($"Ошибка при запросе продуктов с применением поиска - {e}");
                return Problem("Ошибка при запросе продуктов с применением поиска");
            }

        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Product>))]
        [HttpGet("filter")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Get(int page, string filter)
        {
            try
            {
                //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int totalProducts = 0;
                if (page == 0)
                    page = 1;
                (totalProducts, var products) = await _productsService.FilterProductsAsync(filter, PageSize, page);
                if (!products.Any())
                    return BadRequest(new CollectionPagingResponse<Product>
                    {

                        IsSuccess = false,
                        Message = "Не найдено продуктов",
                        OperationDate = DateTime.UtcNow,

                    });
                int totalPages = 0;
                if (totalProducts % PageSize == 0)
                    totalPages = totalProducts / PageSize;
                else
                    totalPages = (totalProducts / PageSize) + 1;

                return Ok(new CollectionPagingResponse<Product>
                {
                    Count = totalProducts,
                    IsSuccess = true,
                    Message = $"Продукты по запросу '{filter}' ",
                    OperationDate = DateTime.UtcNow,
                    PageSize = PageSize,
                    Page = page,
                    Records = products
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при запросе продуктов с применением фильтра - {e}");
                return Problem("Ошибка при запросе продуктов с применением фильтра");
            }
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpGet("Edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
            //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var prod = await _productsService.GetProductById(id);
                if (prod == null)
                    return BadRequest(new OperationResponse<Product>
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
            catch(Exception e)
            {
                _logger.LogError($"Ошибка при получении продукта по id - {e}");
                return Problem("Ошибка при получении продукта по id");
            }
        }

        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm] ProductRequestServer model)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

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

                await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    if (fullPath != null)
                    {
                        var img = Image.Load(model.CoverFile.OpenReadStream());
                        //System.Drawing.Image img = System.Drawing.Image.FromStream(model.CoverFile.OpenReadStream());
                        ////TODO: сделать глобальные настройки разрешения
                        if (img.Width != 1000 && img.Height != 1000)
                        {
                            return BadRequest(new OperationResponse<Product>
                            {
                                Message = "Изображение  должно иметь разрешение 1000 x 1000 ",
                                IsSuccess = false,
                            });
                        }
                    }
                    var addedProduct = await _productsService.AddProductAsync(model.Name, model.Description, model.Price, model.ProductType, url, userId);

                    await model.CoverFile.CopyToAsync(fs);
                    return Ok(new OperationResponse<Product>
                    {
                        IsSuccess = true,
                        Message = $"{addedProduct.Name} продукт успешно добавлен!",
                        Record = addedProduct
                    });

                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при добавлении  продукта - {e}");
                return Problem("Ошибка при добавлении  продукта ");
            }
        }
        #endregion

        #region Put 
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<Product>))]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromForm] ProductRequestServer model)
        {
            //TODO: во всех методах Put где не наследуется RECORD нужно менять Id пользователя изменившего запись, может создать базовый класс контроллера и включить в него логику по изменению ID string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
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
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при редактировании  продукта - {e}");
                return Problem("Ошибка при добавлении  продукта ");
            }
        }
        #endregion

        #region Delete
        [ProducesResponseType(200, Type = typeof(OperationResponse<Product>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //TODO: Проверять удаленные сущности перед запросом в сервисах возможно вынести в базовый класс или сделать REPOSITORY - там всю логику по взаимодействию с сущностями описать
                var getOld = await _productsService.GetProductById(id);
                if (getOld == null)
                    return NotFound();

                var deletedProduct = await _productsService.DeleteProductAsync(id);
                if (deletedProduct == null)
                    return BadRequest(new OperationResponse<Product>
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
                _logger.LogError($"Ошибка при удалении  продукта - {e}");
                return Problem("Ошибка при удалении  продукта ");
            }
        }

        #endregion 


    }
}