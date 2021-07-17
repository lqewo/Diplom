using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        //private readonly IWebHostEnvironment _appEnvironment;
        private readonly IHostingEnvironment _appEnvironment;

        public ContentController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _db = context;
            _appEnvironment = appEnvironment;
        }

        // Получение всех записей
        [HttpGet("contentGet")]
        public async Task<IActionResult> Get()
        {
            var contents = await _db.Contents.ToArrayAsync();
            foreach(var content in contents)
            {
                content.Img = string.Format("http://192.168.0.4:5002/images/{0}", content.Img);
            }

            return Ok(contents);
        }

        // Динамическое получение записей
        [HttpGet("contentTake")]
        public async Task<IActionResult> Take(int skip)
        {
            const int take = 5;
            var contents = await _db.Contents.ToArrayAsync();
            if (!contents.Any())
            {
                return Ok();
            }
            var sortList = contents.OrderByDescending(x => x.Date).ToList(); ;
            foreach (var content in sortList)
            {
                content.Img = string.Format("http://192.168.0.4:5002/images/{0}", content.Img);
            }

            return Ok(sortList.Skip(skip).Take(take));
        }

        //добавить новость 
        [HttpPost("contentAdd")]
        [Authorize]
        public async Task<IActionResult> ContentAdd([FromBody] Content data)
        {
            var content = new Content
            {
                Name = data.Name,
                Text = data.Text,
                Img = data.Img,
                Date = data.Date,
            };

            await _db.Contents.AddAsync(content); // добавить запись
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        // удаление новости
        [HttpPost("contentDell")]
        [Authorize]
        public async Task<IActionResult> DeleteFromBasket(Content del)
        {
            var content = _db.Contents.FirstOrDefault(x => x.ContentId == del.ContentId);
            var imgName = content.Img;
            _db.Contents.RemoveRange(content);
            await _db.SaveChangesAsync(); // сохранить запись

            var uploadLocationn = Path.Combine(_appEnvironment.WebRootPath, "images", $"{del.Img}");
            System.IO.File.Delete($"{_appEnvironment.WebRootPath}/images/{imgName}");

            return Ok();
        }

        // загрузка изображения на сервер
        [HttpPost("contentUpload")]
        public async Task<IActionResult> UploadPost(IFormFile file1)
        {
            const int MaxUploadSize = 1024 * 1024 * 2; // 2мб
            var uploadLocation = Path.Combine(_appEnvironment.WebRootPath, "images");

            var uploadedFileName = file1.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault(); // название и расширение файла
            var ext = Path.GetExtension(uploadedFileName).ToLowerInvariant(); // расширение файла
            var guid = Guid.NewGuid();
            var fileName = $"{guid}{ext}"; // новое имя файла

            if (file1.Length > 0)
            {
                if (file1.Length > MaxUploadSize)
                {
                    return BadRequest("Размер файла слишком большой");
                }
                using (var stream = new FileStream(Path.Combine(uploadLocation, fileName), FileMode.Create))
                {
                    await file1.CopyToAsync(stream);
                    
                }
                // удаление картинки
                //var x = Path.GetDirectoryName(fileName);
                //var uploadLocationn = Path.Combine(_appEnvironment.WebRootPath, "images", $"{fileName}");
                //System.IO.File.Delete($"{_appEnvironment.WebRootPath}/images/{fileName}");
            }
            return Ok(fileName);
        }

        //изменение новости с картинкой
        [HttpPost("contentUpdateImg")]
        [Authorize]
        public async Task<IActionResult> ContentUpdateImg([FromBody] Content bodyImg)
        {
            var content =  _db.Contents.FirstOrDefault(x => x.ContentId == bodyImg.ContentId);
            if (content is null)
            {
                return BadRequest("Запись не найден");
            }

            content.Img = bodyImg.Img;
            content.Name = bodyImg.Name;
            content.Text = bodyImg.Text;

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        //изменение новости без картинки
        [HttpPost("contentUpdate")]
        [Authorize]
        public async Task<IActionResult> ContentUpdate([FromBody] Content bodyImg)
        {
            var content = _db.Contents.FirstOrDefault(x => x.ContentId == bodyImg.ContentId);
            if (content is null)
            {
                return BadRequest("Запись не найден");
            }

            content.Name = bodyImg.Name;
            content.Text = bodyImg.Text;

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        //получение статистики
        [HttpGet("statistiksGet")]
        [Authorize]
        public async Task<IActionResult> GetStatistiks()
        {

        var baskets = await _db.Baskets.Where(x => x.OrderId != null).ToArrayAsync();
            // все заказы
            var result = from b in baskets
                         join aditionMenu in _db.AdditionMenus on b.AdditionMenuId equals
                                 aditionMenu.AdditionMenuId //название новая таблица поле из первой таблицы поле из новой таблицы
                         join product in _db.Products on aditionMenu.ProductId equals product.ProductId
                         join order in _db.Orders on b.OrderId equals order.OrderId
                         join user in _db.Users on order.UserId equals user.Id
                         select new OrderStatistik
                         {
                             Price = aditionMenu.Price,
                             Name = product.Name,
                             Quantity = b.Quantity,
                             BasketId = b.BasketId,

                             OverallPrice = b.Quantity * Convert.ToInt32(aditionMenu.Price),

                             ProductId = product.ProductId,
                             LeadTime = order.LeadTime,
                             Status = order.Status,
                             TotalPrice = order.TotalPrice,
                             Year = user.Year,
                             Sex = user.Sex,

                         };
            var orderMount = result.Where(x => x.LeadTime > DateTime.Now.AddMonths(-1) && x.Status == StatusType.Completed);
            var orderYear = result.Where(x => x.LeadTime > DateTime.Now.AddYears(-1) && x.Status == StatusType.Completed);
            var orderAll = result.Where(x => x.Status == StatusType.Completed);
            var orders = result.ToArray().GroupBy(x => x.ProductId).OrderBy(x => x.Count());

            if (orderMount == null || orderYear == null || orderAll == null)
            {
                return BadRequest("Недостаточно данных");
            }

            var populerEatMount = orderMount.Last().Name; // самое популярное блюдо за месяц
            var populerEatYear = orderYear.Last().Name; // самое популярное блюдо за год

            var revenueMount = orderMount.Sum(x => x.OverallPrice); // доход за месяц
            var revenueYear = orderYear.Sum(x => x.OverallPrice); // доход за год

            var averageCheck = orderAll.Average(x => x.TotalPrice); // средний чек заказа

            var averageYear = orderAll.Average(x => x.Year); // средний возраст покупателя
            var countSexFemale = orderMount.Count(x => x.Sex == SexType.Female); // кол-во женьщин покупателей за месяц
            var countSexMale = orderMount.Count(x => x.Sex == SexType.Male); // кол-во мужчинпокупателей за месяц

            var userCount = _db.Users.Count(); // общее кол-во пользователей

            var data = new Statistiks
            {
                populerEatMount = populerEatMount,
                populerEatYear = populerEatYear,
                revenueMount = revenueMount,
                revenueYear = revenueYear,
                averageCheck = Convert.ToInt32(averageCheck),
                averageYear = Convert.ToInt32(averageYear),
                countSexFemale = countSexFemale,
                countSexMale = countSexMale,
                userCount = userCount,

            };
            return Ok(data);
        }

    }
}