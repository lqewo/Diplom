using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<SiteUser> _userManager; // манагер для управления пользователями

        public ReviewController(ApplicationDbContext context, UserManager<SiteUser> userManager)
        {
            _userManager = userManager;
            _db = context;
        }

        //добавить отзыв пользователя
        [HttpPost("reviewAdd")]
        [Authorize]
        public async Task<IActionResult> ReviewAdd([FromBody] Review data)
        {
            data.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену
            await _db.Reviews.AddAsync(data); // добавить запись
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }
        //получить отзывы пользователей
        [HttpGet("reviewGet")]
        public async Task<IActionResult> Get()
        {
            var reviews = await _db.Reviews.Include(x => x.User).Select(x =>
            new ReviewShow()
            {
                //ReviewId = x.ReviewId,
                Text = x.Text,
                Rating = x.Rating,
                Date = x.Date,
                FirstName = x.User.FirstName
                }).ToArrayAsync();

            if (!reviews.Any())
            {
                return Ok();
            }
            return Ok(reviews);
        }
        // Динамическое получение записей
        [HttpGet("reviewTake")]
        public async Task<IActionResult> Take(int skip)
        {
            const int take = 5;
            var reviews = await _db.Reviews.Include(x => x.User).Select(x =>
            new ReviewShow()
            {
                //ReviewId = x.ReviewId,
                Text = x.Text,
                Rating = x.Rating,
                Date = x.Date,
                FirstName = x.User.FirstName
            }).ToArrayAsync();

            if (!reviews.Any())
            {
                return Ok();
            }
            var sortList = reviews.OrderByDescending(x => x.Date).ToList();

            return Ok(sortList.Skip(skip).Take(take));
        }


        ////получить отзывы пользователей
        //[HttpGet("reviewGet")]
        //public async Task<IActionResult> Get()
        //{
        //    var reviews = await _db.Reviews.ToArrayAsync();

        //    foreach (var name in reviews)
        //    {
        //        var user = await _userManager.FindByIdAsync(name.UserId);
        //        name.FirstName = user.FirstName; 
        //    }
        //    return Ok(reviews);
        //}
    }
}