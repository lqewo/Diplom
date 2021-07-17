using System.Collections.Generic;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteUser>>> Get()
        {
            return await _context.Users.ToArrayAsync();
        }

        //Востановить пароль
        //[HttpPost("PasswordEdit")]
        //public async Task<ActionResult> EditPassword(string password, string newPassword)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//найти id пользователя по токену
        //    SiteUser existedUser = await _userManager.FindByIdAsync(userId); //найти строчку с пользователем
        //    if (existedUser != null)
        //    {
        //    }
        //}
    }
}