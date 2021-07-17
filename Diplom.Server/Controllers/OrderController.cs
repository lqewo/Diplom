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
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext context)
        {
            _db = context;
        }

        //Оформить заказ
        [HttpPost("orderAdd")]
        [Authorize]
        public async Task<IActionResult> MakeOrder([FromBody] Order data)
        {
            data.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //найти id пользователя по токену

            // чтобы подсчитать цену заказа
            var baskets = await _db.Baskets.Where(x => x.UserId == data.UserId && x.OrderId == null).ToArrayAsync();
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
            var TotalPrice = result.Sum(xx => xx.OverallPrice);

            var order = await _db.Orders.AddAsync(new Order
            {
                UserId = data.UserId,
                OrderTime = data.OrderTime,
                LeadTime = data.LeadTime,
                TotalPrice = TotalPrice,
                Comment = data.Comment,
                TypePayment = data.TypePayment,
                Status = data.Status,
            }); // добавить запись и выбрать Id это записи

            await _db.SaveChangesAsync(); // сохранить запись
            var orderId = order.Entity.OrderId; //получить Id  сохраненой записи

            //Привязать продукты из корзины к текущему заказу
            var basketUpdate = await _db.Baskets.Where(x => x.UserId == data.UserId && x.OrderId == null).ToArrayAsync();
            foreach(var menu in basketUpdate)
            {
                menu.OrderId = orderId;
            }

            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();

            //var orderId = _db.Orders.Add(new Order
            //{
            //    UserId = data.UserId,
            //    OrderTime = data.OrderTime,
            //    LeadTime = data.LeadTime,
            //    TotalPrice = data.TotalPrice,
            //    Comment = data.Comment,
            //    TypePayment = data.TypePayment,
            //    Status = data.Status,
            //}).Entity.OrderId;
        }

        //Получить список заказов пользователя
        [HttpGet("orderGet")]
        [Authorize]
        public IActionResult GetUserOrders()
        {
            //найти id пользователя по токену
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            //все заказы пользователя
            var order = _db.Orders.Where(x => x.UserId == userId).Select(x =>
            new OrderList()
            {
                OrderListId = x.OrderId,
                OrderId = x.OrderId,
                OrderTime = x.OrderTime,
                LeadTime = x.LeadTime,
                TotalPrice = x.TotalPrice,
                Comment = x.Comment,
                TypePayment = x.TypePayment,
                Status = x.Status,
            });
            return Ok(order);
        }

        //Удалить выбраный заказ
        [HttpPost("orderDell")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            //найти id пользователя по токену
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var baskets = await _db.Baskets.Where(x => x.UserId == userId && x.OrderId == orderId).ToArrayAsync();
            _db.Baskets.RemoveRange(baskets);
            await _db.SaveChangesAsync(); // сохранить запись

            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync(); // сохранить запись
            return Ok();
        }

        [HttpGet("orderOneGet")]
        public IActionResult Get(int orderOne)
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderOne);
            //var orderr = new OrderList()
            //{
            //    OrderTime = order.OrderTime,
            //    LeadTime = order.LeadTime,
            //    TotalPrice = order.TotalPrice,
            //    Comment = order.Comment,
            //    TypePayment = order.TypePayment,
            //    Status = order.Status,
            //};
            return Ok(order);
        }

        [HttpPost("orderUpdate")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder([FromBody] Order data)
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == data.OrderId);
            order.Comment = data.Comment;
            order.LeadTime = data.LeadTime;
            order.Status = data.Status;
            order.TypePayment = data.TypePayment;

            await _db.SaveChangesAsync(); // сохранить изменения

            return Ok();
        }

        [HttpPost("orderUpdatePrice")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderPrice(int orderId)
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);

            //найти id пользователя по токену
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // чтобы подсчитать цену заказа
            var baskets = await _db.Baskets.Where(x => x.UserId == userId && x.OrderId == orderId).ToArrayAsync();
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
            var TotalPrice = result.Sum(xx => xx.OverallPrice);

            order.TotalPrice = TotalPrice;

            await _db.SaveChangesAsync(); // сохранить изменения

            return Ok();
        }

        //Получить список заказов для работников
        [HttpGet("orderWorkerGet")]
        [Authorize]
        public IActionResult GetWorkerOrders(int skip)
        {
            const int take = 5;
            var time = DateTime.Now;

            //все заказы 
            var order = _db.Orders.Where(x => x.LeadTime >= time).Select(x =>
            new OrderList()
            {
                OrderListId = x.OrderId,
                OrderId = x.OrderId,
                OrderTime = x.OrderTime,
                LeadTime = x.LeadTime,
                TotalPrice = x.TotalPrice,
                Comment = x.Comment,
                TypePayment = x.TypePayment,
                Status = x.Status,
            });
            if (!order.Any())
            {
                return Ok();
            }
            var sortList = order.OrderBy(x => x.LeadTime).ToList();
            return Ok(sortList.Skip(skip).Take(take));
        }

        // получение одного заказа для работника
        [HttpGet("orderWorkerOneGet")]
        public IActionResult WorkerGet(int orderOne)
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderOne);
            var user = _db.Users.FirstOrDefault(x => x.Id == order.UserId);

            var orderr = new OrderDetailWorker()
            {
                OrderTime = order.OrderTime,
                LeadTime = order.LeadTime,
                TotalPrice = order.TotalPrice,
                Comment = order.Comment,
                TypePayment = order.TypePayment,
                Status = order.Status,

                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
            };
            return Ok(orderr);
        }
        // обновление статуса заказа
        [HttpPost("orderUpdateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatusOrder(int orderId, StatusType status)
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
            order.Status = status;

            await _db.SaveChangesAsync(); // сохранить изменения

            return Ok();
        }
    }
}