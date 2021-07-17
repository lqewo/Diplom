using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _appEnvironment;

        public ProductController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _db = context;
            _appEnvironment = appEnvironment;
        }

        //получение записей меню
        [HttpGet("productGet")]
        public async Task<IActionResult> Get(MenuType type)
        {
            var menus = await _db.Products.Where(x => x.Type == type).ToArrayAsync();
            foreach(var menu in menus)
            {
                menu.Img = string.Format("http://192.168.0.4:5002/images/{0}", menu.Img);
            }

            return Ok(menus);
        }

        //получение всех записей меню
        [HttpGet("productAllGet")]
        public async Task<ActionResult> GetAll()
        {
            var products = await _db.Products.ToArrayAsync();
            foreach(var product in products)
            {
                product.Img = string.Format("http://192.168.0.4:5002/images/{0}", product.Img);
            }

            return Ok(products);
        }

        // Динамическое получение записей меню
        [HttpGet("productTake")]
        public async Task<ActionResult> TakeProduct(int skip, MenuType type)
        {
            const int take = 5;
            var products = await _db.Products.Where(x => x.Type == type).ToArrayAsync();
            if (!products.Any())
            {
                return Ok();
            }

            var sortList = products.OrderBy(x => x.Name).ToList(); ;
            foreach (var content in sortList)
            {
                content.Img = string.Format("http://192.168.0.4:5002/images/{0}", content.Img);
            }

            return Ok(sortList.Skip(skip).Take(take));
        }

        //получение расширенных данных выбраной записи
        [HttpGet("productAdditionGet")]
        public async Task<ActionResult> Addition(int menuId)
        {
            var additions = await _db.AdditionMenus.Where(x => x.ProductId == menuId).ToArrayAsync();
            return Ok(additions);
        }

        //добавить Продукт 
        [HttpPost("productAdd")]
        [Authorize]
        public async Task<IActionResult> ProductAdd([FromBody] Product data)
        {
            var product = await _db.Products.AddAsync(data); // добавить запись
            await _db.SaveChangesAsync(); // сохранить запись
            var productId = product.Entity.ProductId; //получить Id сохраненой записи
            return Ok(productId);
        }
        //добавить расширеную запись продукта 
        [HttpPost("productAdditionAdd")]
        [Authorize]
        public async Task<IActionResult> ProductAdditionAdd([FromBody] AdditionMenu data)
        {
            await _db.AdditionMenus.AddAsync(data); // добавить запись
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        // удаление продукта
        [HttpPost("productDell")]
        [Authorize]
        public async Task<IActionResult> DeleteFromProduct(Product del)
        {
            // удаляем запись в продуктах
            var product = _db.Products.FirstOrDefault(x => x.ProductId == del.ProductId);
            var imgName = product.Img;
            var productId = product.ProductId;
            _db.Products.RemoveRange(product);
            await _db.SaveChangesAsync(); // сохранить запись

            // удаляем изображение продукта
            var uploadLocationn = Path.Combine(_appEnvironment.WebRootPath, "images", $"{imgName}");
            System.IO.File.Delete($"{_appEnvironment.WebRootPath}/images/{imgName}");

            // удаляем расширеные записи продукта
            var additionMenu = _db.AdditionMenus.Where(x => x.ProductId == productId);
            _db.AdditionMenus.RemoveRange(additionMenu);
            await _db.SaveChangesAsync(); // сохранить запись

            return Ok();
        }

        //изменение продукта с картинкой
        [HttpPost("productUpdateImg")]
        [Authorize]
        public async Task<IActionResult> ProductUpdateImg([FromBody] Product bodyImg)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == bodyImg.ProductId);
            if (product is null)
            {
                return BadRequest("Запись не найден");
            }
            product.Name = bodyImg.Name;
            product.ShortDescription = bodyImg.ShortDescription;
            product.LongDescription = bodyImg.LongDescription;
            product.Img = bodyImg.Img;

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();

        }

        //изменение продукта
        [HttpPost("productUpdate")]
        [Authorize]
        public async Task<IActionResult> ProductUpdate([FromBody] Product bodyImg)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == bodyImg.ProductId);
            if (product is null)
            {
                return BadRequest("Запись не найден");
            }
            product.Name = bodyImg.Name;
            product.ShortDescription = bodyImg.ShortDescription;
            product.LongDescription = bodyImg.LongDescription;

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();

        }
        //изменение расширеных записей
        [HttpPost("productAdditionUpdate")]
        [Authorize]
        public async Task<IActionResult> ProductAdditionUpdate([FromBody] AdditionMenu body)
        {
            var addition = _db.AdditionMenus.FirstOrDefault(x => x.ProductId == body.ProductId);
            if (addition is null)
            {
                return BadRequest("Запись не найден");
            }
            addition.Calories = body.Calories;
            addition.Grams = body.Grams;
            addition.Price = body.Price;

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }
        // удаление расширеной записи
        [HttpPost("productAdditionDell")]
        [Authorize]
        public async Task<IActionResult> DeleteFromProductAddition(AdditionMenu del)
        {
            // удаляем запись в продуктах
            var addition = _db.AdditionMenus.FirstOrDefault(x => x.AdditionMenuId == del.AdditionMenuId);
            
            _db.AdditionMenus.RemoveRange(addition);
            await _db.SaveChangesAsync(); // сохранить запись

            return Ok();
        }

    }
}