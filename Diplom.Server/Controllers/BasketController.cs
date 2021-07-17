using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Diplom.Common.Entities;
using Diplom.Common.Models;
using Diplom.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BasketController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpPost("basketAdd")]
        [Authorize]
        public async Task<IActionResult> AddToBasket([FromBody] Basket data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену

            //проверяет есть ли уже эта запись в корзине
            var existedUser = _db.Baskets.Any(x => x.UserId == userId && x.OrderId == null && x.AdditionMenuId == data.AdditionMenuId);
            if(existedUser)
            {
                return BadRequest("Запись уже добавлена в корзину");
            }

            data.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену
            var basket = new Basket
            {
                UserId = data.UserId,
                AdditionMenuId = data.AdditionMenuId,
                Quantity = data.Quantity,
                OrderId = data.OrderId
            };

            await _db.Baskets.AddAsync(basket); // добавить запись
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        //public int BasketListId { get; set; }

        //public string Price { get; set; } // цена порции

        //public string Name { get; set; } // название продукта
        //public string ShortDescription { get; set; } // короткое оприсание
        //public string Img { get; set; } // картинка продукта

        //public int Quantity { get; set; } // колличество
        //public int BasketId { get; set; } // ид строчки в корзине

        //public int OverallPrice { get; set; } //цена с учетом количества
        //public int AllPrice { get; set; } //общая цена покупки
        [HttpGet("basketGet")]
        [Authorize]
        public async Task<IActionResult> GetUserBasket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену

            //все записи пользователя которые должны отобразаться в корзине
            var baskets = await _db.Baskets.Where(x => x.UserId == userId && x.OrderId == null).ToArrayAsync();
            var result = from b in baskets
                         join aditionMenu in _db.AdditionMenus on b.AdditionMenuId equals
                                 aditionMenu.AdditionMenuId //название новая таблица поле из первой таблицы поле из новой таблицы
                         join product in _db.Products on aditionMenu.ProductId equals product.ProductId
                         select new BasketList
                         {
                             Price = aditionMenu.Price,
                             Name = product.Name,
                             ShortDescription = product.ShortDescription,
                             Img = string.Format("http://192.168.0.4:5002/images/{0}", product.Img),
                             Quantity = b.Quantity,
                             BasketId = b.BasketId,
                             OverallPrice = b.Quantity * Convert.ToInt32(aditionMenu.Price),
                         };
            return Ok(result);
        }

        //получение корзины выбраной записи
        [HttpGet("basketOneGet")]
        [Authorize]
        public async Task<IActionResult> BasketOneGet(int orderOne)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену

            //все записи пользователя которые должны отобразаться в корзине
            var baskets = await _db.Baskets.Where(x => x.OrderId == orderOne)
                                   .ToArrayAsync(); //записи в корзине относящиеся к выбраному заказу
            var result = from b in baskets
                         join aditionMenu in _db.AdditionMenus on b.AdditionMenuId equals
                                 aditionMenu.AdditionMenuId //название новая таблица поле из первой таблицы поле из новой таблицы
                         join product in _db.Products on aditionMenu.ProductId equals product.ProductId
                         select new BasketList
                         {
                             Price = aditionMenu.Price,
                             Name = product.Name,
                             ShortDescription = product.ShortDescription,
                             Img = string.Format("http://192.168.0.4:5002/images/{0}", product.Img),
                             Quantity = b.Quantity,
                             BasketId = b.BasketId,
                             OverallPrice = b.Quantity * Convert.ToInt32(aditionMenu.Price),
                             Grams = aditionMenu.Grams,
                         };
            return Ok(result);
        }

        [HttpPost("basketDell")]
        [Authorize]
        public async Task<IActionResult> DeleteFromBasket(BasketList del)
        {
            var basket = _db.Baskets.FirstOrDefault(x => x.BasketId == del.BasketId);
            _db.Baskets.RemoveRange(basket);
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        [HttpPost("basketOrderDell")]
        [Authorize]
        public async Task<IActionResult> BasketOrderDell(BasketList del, int orderId)
        {
            var basket = _db.Baskets.FirstOrDefault(x => x.BasketId == del.BasketId);
            _db.Baskets.RemoveRange(basket);
            await _db.SaveChangesAsync(); // сохранить запись
            
            var orderExistBasket = _db.Baskets.Any(x => x.OrderId == orderId); //есть ли продукты в заказе если нет то удалить заказ
            if(!orderExistBasket)
            {
                var orderDel = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
                _db.Orders.Remove(orderDel);
                await _db.SaveChangesAsync(); // сохранить запись
                return Ok(del.Name);
            }

            return Ok();
        }

        [HttpPost("basketOneAdd")]
        [Authorize]
        public async Task<IActionResult> basketOneAdd(BasketList del)
        {
            var basket = _db.Baskets.FirstOrDefault(x => x.BasketId == del.BasketId);
            if(basket.Quantity >= 10)
            {
                return BadRequest("Максимальное количество");
            }

            basket.Quantity++;
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        [HttpPost("basketOneDell")]
        [Authorize]
        public async Task<IActionResult> basketOneDell(BasketList del)
        {
            var basket = _db.Baskets.FirstOrDefault(x => x.BasketId == del.BasketId);
            if(basket.Quantity <= 1)
            {
                return BadRequest("Достигнуто минимальное количество");
            }

            basket.Quantity--;
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }
    }
}