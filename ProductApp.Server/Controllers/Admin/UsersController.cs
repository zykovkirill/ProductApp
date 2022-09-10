using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {

        private IAdminService _adminService;
        private const int _pageSize = 10;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAdminService adminService, ILogger<UsersController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<IdentityUser>))]
        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            try
            {
                //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (page == 0)
                    page = 1;
                (var totalUsers, var users) = await _adminService.GetAllUsersAsync(_pageSize, page);
                if (!users.Any())
                    return BadRequest(new CollectionPagingResponse<IdentityUser>
                    {
                        IsSuccess = false,
                        Message = "Пользователи не найдены!",
                        OperationDate = DateTime.UtcNow,

                    });

                int totalPages = 0;
                if (totalUsers % _pageSize == 0)
                    totalPages = totalUsers / _pageSize;
                else
                    totalPages = (totalUsers / _pageSize) + 1;

                return Ok(new CollectionPagingResponse<IdentityUser>
                {
                    Count = totalUsers,
                    IsSuccess = true,
                    Message = "Пользователи переданы!",
                    OperationDate = DateTime.UtcNow,
                    PageSize = _pageSize,
                    Page = page,
                    Records = users
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при получении пользователей - {e}");
                return Problem("Ошибка при получении пользователей");
            }
        }




        [ProducesResponseType(200, Type = typeof(OperationResponse<ChangeRoleViewModel>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ChangeRoleViewModel>))]
        [HttpGet("Edit")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var user = await _adminService.GetUserById(id);
                if (user == null)
                    return BadRequest(new OperationResponse<ChangeRoleViewModel>
                    {
                        IsSuccess = false,
                        Message = "Пользователь не найден",
                    });

                return Ok(new OperationResponse<ChangeRoleViewModel>
                {
                    Record = user,
                    Message = "Пользователь передан",
                    IsSuccess = true,
                    OperationDate = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при получении пользователя по id - {e}");
                return Problem("Ошибка при получении пользователя по id ");
            }
        }


        #endregion

        #region Post 
        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserManagerResponse result = await _adminService.CreateUserAsync(model);
                    if (result.IsSuccess)
                        return Ok(result);

                    return BadRequest(result);
                }

                return BadRequest("Одно или несколько свойств не прошли валидацию"); // Код : 400
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при создании пользователя  - {e}");
                return Problem("Ошибка при создании пользователя по id ");
            }
        }

        #endregion

        #region Put

        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        [HttpPut]
        public async Task<IActionResult> EditAsync([FromBody] ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _adminService.EditUserById(model);
                if (result.IsSuccess)
                    return Ok(result); // Код : 200

                return BadRequest(result);
            }

            return BadRequest("Одно или несколько свойств не прошли валидацию"); // Код : 400
        }
        #endregion

        #region Delete
        [ProducesResponseType(200, Type = typeof(OperationResponse<IdentityUser>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var result = await _adminService.DeleteUserById(id);
                    if (result.IsSuccess)
                    {
                        return Ok(new OperationResponse<IdentityUser>
                        {
                            IsSuccess = true,
                            Message = $"{id} has been deleted !",
                        });
                    }
                    else
                    {
                        return BadRequest(new OperationResponse<IdentityUser>
                        {
                            IsSuccess = false,
                            Message = result.Errors.FirstOrDefault(),
                        });
                    }
                }
                return BadRequest(new OperationResponse<IdentityUser>
                {
                    Message = "Одно или несколько свойств не прошли валидацию",
                    IsSuccess = false
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при создании пользователя  - {e}");
                return Problem("Ошибка при создании пользователя по id ");
            }
        }

        #endregion

    }
}