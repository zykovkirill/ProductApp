using ProductApp.Server.Models;
using ProductApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;
using ProductApp.Shared.Models;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]
    public class UserDataController : ControllerBase
    {
        private ApplicationDbContext _db;


        public UserDataController(ApplicationDbContext context)
        {
            _db = context;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserProfile>))]
        [HttpGet]
        public async Task<ActionResult<UserProfile>> Get()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value; // will give the user's userId
            //TODO: нужно ли тут подключать корзину, продукты,покупки??
            UserProfile dbUserData = await _db.UserProfiles.Include(p => p.UserCreatedProducts).Include(b => b.UserOrder).FirstOrDefaultAsync(i => i.UserId == userId);// Include(p=> p.Products) подключает продукты из БД!!!
            if (dbUserData == null)
                return BadRequest(new OperationResponse<UserProfile>
                {
                    IsSuccess = false,
                    Message = "Данные не найдены"
                });


            return Ok(new OperationResponse<UserProfile>
            {
                IsSuccess = true,
                Record = dbUserData,
                OperationDate = DateTime.Now

            });
        }
        #endregion

    }
}