using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductApp.Server.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]
    public class UserPurchasesController : Controller
    {
        //TODO: Разделить IProductsService на несколько интерфейсов
        private readonly IUserDataService _userDataService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserPurchasesController> _logger;
        private const int _pageSize = 10;

        public UserPurchasesController(IUserDataService userDataService, IConfiguration configuration, ILogger<UserPurchasesController> logger)
        {
            _userDataService = userDataService;
            _configuration = configuration;
            _logger = logger;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<UserOrder>))]
        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (page == 0)
                    page = 1;
                (var totalPurchase, var purchase) = await _userDataService.GetPurchase(_pageSize, page, userId);

                int totalPages = 0;
                if (totalPurchase % _pageSize == 0)
                    totalPages = totalPurchase / _pageSize;
                else
                    totalPages = (totalPurchase / _pageSize) + 1;

                return Ok(new CollectionPagingResponse<UserOrder>
                {
                    Count = totalPurchase,
                    IsSuccess = true,
                    Message = "Заказы переданы",
                    OperationDate = DateTime.UtcNow,
                    PageSize = _pageSize,
                    Page = page,
                    Records = purchase
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при получении заказов  - {e}");
                return Problem("Ошибка при получении заказов");
            }
        }
        #endregion
    }
}
