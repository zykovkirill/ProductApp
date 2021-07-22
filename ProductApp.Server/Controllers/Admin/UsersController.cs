using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProductApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using ProductApp.Server.Services;
using System;

namespace CustomIdentityApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private IAdminService _adminService;
        private const int PAGE_SIZE = 10;

        public UsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Get
        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<IdentityUser>))]
        [HttpGet]
        public IActionResult Get(int page)

        {
            //  string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalUsers = 0;
            if (page == 0)
                page = 1;
            var users = _adminService.GetAllUsersAsync(PAGE_SIZE, page, out totalUsers);
            //TODO:totalPages - почему не используется?
            int totalPages = 0;
            if (totalUsers % PAGE_SIZE == 0)
                totalPages = totalUsers / PAGE_SIZE;
            else
                totalPages = (totalUsers / PAGE_SIZE) + 1;

            return Ok(new CollectionPagingResponse<IdentityUser>
            {
                Count = totalUsers,
                IsSuccess = true,
                Message = "Users received successfully!",
                OperationDate = DateTime.UtcNow,
                PageSize = PAGE_SIZE,
                Page = page,
                Records = users
            });
        }




        [ProducesResponseType(200, Type = typeof(OperationResponse<ChangeRoleViewModel>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ChangeRoleViewModel>))]
        [HttpGet("Edit")]
        public async Task<IActionResult> Get(string id)
        {
            // string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await _adminService.GetUserById(id);
            if (user == null)
                return BadRequest(new OperationResponse<ChangeRoleViewModel>
                {
                    IsSuccess = false,
                    Message = "Invalid operation",
                });

            return Ok(new OperationResponse<ChangeRoleViewModel>
            {
                Record = user,
                Message = "User retrieved successfully!",
                IsSuccess = true,
                OperationDate = DateTime.UtcNow
            });
        }


        #endregion

        #region Post 
       [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
       [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
       [HttpPost]
        public async Task<IActionResult> Post([FromForm] RegisterRequest model)
        {


            if (ModelState.IsValid)
            {
                UserManagerResponse result = await _adminService.CreateUserAsync(model);
                if (result.IsSuccess)
                    return Ok(result); // Код : 200

                return BadRequest(result);
            }

            return BadRequest("Одно или несколько свойств не прошли валидацию"); // Код : 400



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
                Message = "Not Valid",
                IsSuccess = false
            });
        }

        #endregion



    }
}