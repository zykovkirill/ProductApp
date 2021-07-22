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
    [Authorize]
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
            UserProfile dbUserData = await _db.UserDatas.Include(p => p.UserProducts).Include(b => b.UserPurchases).FirstOrDefaultAsync(i => i.UserId == userId);// Include(p=> p.Products) подключает продукты из БД!!!
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

        //// GET api/Products/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<UserProduct>> Get(int id)
        //{


        //    UserProduct prod = await _db.UserProducts.FirstOrDefaultAsync(x => x.Id == id);
        //    if (prod == null)
        //        return NotFound();
        //    return new ObjectResult(prod);
        //}

        // POST api/Products
        //[HttpPost]
        //public async Task<ActionResult<UserProduct>> Post(UserProduct product)
        //{
        //    if (product == null)
        //    {
        //        return BadRequest();
        //    }

        //    _db.UserProducts.Add(product);
        //    await _db.SaveChangesAsync();
        //    return Ok(product);
        //}

        //// PUT api/Products/
        //[HttpPut]
        //public async Task<ActionResult<UserProduct>> Put(UserProduct product)
        //{
        //    if (product == null)
        //    {
        //        return BadRequest();
        //    }
        //    if (!_db.UserProducts.Any(x => x.Id == product.Id))
        //    {
        //        return NotFound();
        //    }

        //    _db.Update(product);
        //    await _db.SaveChangesAsync();
        //    return Ok(product);
        //}

        //// DELETE api/Products/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<UserProduct>> Delete(int id)
        //{
        //    UserProduct product = _db.UserProducts.FirstOrDefault(x => x.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _db.UserProducts.Remove(product);
        //    await _db.SaveChangesAsync();
        //    return Ok(product);
        //}
    }
}