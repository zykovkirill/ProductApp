using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RolesController> _logger;
        // UserManager<IdentityUser> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager/*, UserManager<IdentityUser> userManager*/, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
            // _userManager = userManager;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionResponse<RoleViewModel>))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                ////  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //int totalUsers = 0;
                var roles = await _roleManager.Roles.ToListAsync();
                if (!roles.Any())
                    return NotFound();
                var roleModels = new List<RoleViewModel>();
                foreach (var role in roles)
                {

                    var roleModel = new RoleViewModel(role.Id, role.Name);
                    roleModels.Add(roleModel);
                }
                var totalRoles = roles.Count;
                return Ok(new CollectionResponse<RoleViewModel>
                {
                    Count = totalRoles,
                    IsSuccess = true,
                    Message = "Users received successfully!",
                    OperationDate = DateTime.UtcNow,
                    Records = roleModels
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при получении ролей - {e}");
                return Problem("Ошибка при получении ролей");
            }

        }

        #endregion

        #region Post

        [ProducesResponseType(200, Type = typeof(BaseAPIResponse))]
        [ProducesResponseType(400, Type = typeof(BaseAPIResponse))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RoleViewModel model)
        {
            try
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return Ok(new BaseAPIResponse
                    {
                        IsSuccess = true,
                        Message = "Роль добавлена"
                    });
                }
                else
                {
                    _logger.LogError($" Ошибка при создании роли {result.Errors.FirstOrDefault().Description}");
                    return BadRequest(new BaseAPIResponse
                    {
                        IsSuccess = false,
                        Message = "Ошибка при создании роли,менеджер ролей вернул отрицательный результат"
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка при создании роли - {e}");
                return Problem("Ошибка при создании роли");
            }

        }
        #endregion
        #region Delete

        [ProducesResponseType(200, Type = typeof(BaseAPIResponse))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {

                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    IdentityResult result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return Ok(new BaseAPIResponse
                        {
                            IsSuccess = true,
                            Message = "Роль удалена"
                        });
                    }
                    else
                    {
                        _logger.LogError($"Ошибка при удалении роли - {result.Errors.FirstOrDefault().Description}");;
                        return BadRequest(new BaseAPIResponse
                        {
                            IsSuccess = false,
                            Message = "Ошибка при создании роли,менеджер ролей вернул отрицательный результат"
                        });
                    }
                }

                return BadRequest(new BaseAPIResponse
                {
                    IsSuccess = false,
                    Message = "Роль не найдена"
                });
            }
            catch(Exception e)
            {
                _logger.LogError($"Ошибка при удалении роли - {e}");
                return Problem("Ошибка при удалении роли");
            }
        }
        #endregion

    }


}
