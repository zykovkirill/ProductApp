using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Models;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, User")]
    public class UserDataController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserDataController> _logger;
        public UserDataController(ApplicationDbContext context, ILogger<UserDataController> logger)
        {
            _db = context;
            _logger = logger;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserProfile>))]
        [HttpGet]
        public async Task<ActionResult<UserProfile>> Get()
        {

            try
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
                    OperationDate = DateTime.UtcNow,
                    Message = "Данные переданы"

                });
            }
            catch(Exception e)
            {
                _logger.LogError($"Ошибка при данных пользователя - {e}");
                return Problem("Ошибка при данных пользователя");
            }
        }
        #endregion

    }
}