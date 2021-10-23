﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int _pageSize = 10;

        public UserPurchasesController(IUserDataService userDataService, IConfiguration configuration)
        {
            _userDataService = userDataService;
            _configuration = configuration;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<UserOrder>))]
        [HttpGet]
        public IActionResult Get(int page)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalProducts = 0;
            if (page == 0)
                page = 1;
            var purchase = _userDataService.GetPurchase(_pageSize, page, userId, out totalProducts);

            int totalPages = 0;
            if (totalProducts % _pageSize == 0)
                totalPages = totalProducts / _pageSize;
            else
                totalPages = (totalProducts / _pageSize) + 1;

            return Ok(new CollectionPagingResponse<UserOrder>
            {
                Count = totalProducts,
                IsSuccess = true,
                Message = "Продукты переданы",
                OperationDate = DateTime.UtcNow,
                PageSize = _pageSize,
                Page = page,
                Records = purchase
            });
        }
        #endregion
    }
}
