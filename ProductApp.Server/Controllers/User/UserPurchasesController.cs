using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Server.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = ("Admin"))]
    public class UserPurchasesController : Controller
    {
        //TODO: Разделить IProductsService на несколько интерфейсов
        private readonly IPurchasesService _purchasesService;
        private readonly IConfiguration _configuration;
        private const int PAGE_SIZE = 10;

        public UserPurchasesController(IPurchasesService purchasesService, IConfiguration configuration)
        {
            _purchasesService = purchasesService;
            _configuration = configuration;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<UserOrder>))]
        [HttpGet]
        public IActionResult Get(int page)
        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var purchase = _purchasesService.GetPurchase(PAGE_SIZE, page, out totalProducts);

            int totalPages = 0;
            if (totalProducts % PAGE_SIZE == 0)
                totalPages = totalProducts / PAGE_SIZE;
            else
                totalPages = (totalProducts / PAGE_SIZE) + 1;

            return Ok(new CollectionPagingResponse<UserOrder>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = "Продукты переданы",
                OperationDate = DateTime.UtcNow,
                PageSize = PAGE_SIZE,
                Page = page,
                Records = purchase
            });
        }
        #endregion
    }
}
