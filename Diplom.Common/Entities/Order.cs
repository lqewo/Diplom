using System;
using Diplom.Common.Models;

namespace Diplom.Common.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } // ид клиента
        public DateTime OrderTime { get; set; } // дата и время оформления 
        public DateTime LeadTime { get; set; } //дата и время к которому нужно выполнить заказ
        public float TotalPrice { get; set; } //общая цена заказа
        public string Comment { get; set; } //комментарий пользователя 
        public PaymentType TypePayment { get; set; } // тип оплаты
        public StatusType Status { get; set; } // статус заказа
        
        public virtual SiteUser User { get; set; }
    }
}