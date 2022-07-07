using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        RoleManager<IdentityRole> _roleManager;
        // UserManager<IdentityUser> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager/*, UserManager<IdentityUser> userManager*/)
        {
            _roleManager = roleManager;
            // _userManager = userManager;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionResponse<RoleViewModel>))]
        [HttpGet]
        public IActionResult Get()

        {
            ////  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //int totalUsers = 0;

            var roles = _roleManager.Roles.ToList();
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

        #endregion

        #region Post

        [ProducesResponseType(200, Type = typeof(BaseAPIResponse))]
        [ProducesResponseType(400, Type = typeof(BaseAPIResponse))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RoleViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Name))
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
                    return BadRequest(new BaseAPIResponse
                    {
                        IsSuccess = false,
                        Message = result.Errors.FirstOrDefault().Description
                    });
                }
            }
            return BadRequest(new BaseAPIResponse
            {
                IsSuccess = false,
                Message = "Роль не может быть пустой"
            });
        }
        #endregion
        #region Delete

        [ProducesResponseType(200, Type = typeof(BaseAPIResponse))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
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
                    return BadRequest(new BaseAPIResponse
                    {
                        IsSuccess = false,
                        Message = result.Errors.FirstOrDefault().Description
                    });
                }
            }

            return BadRequest(new BaseAPIResponse
            {
                IsSuccess = false,
                Message = "Роль не найдена"
            });
        }

        #endregion
    }
}