using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom.Common.Models
{
    public class OrderDetailWorker
    {
        public int OrderDetailWorkerId { get; set; }

        public int OrderId { get; set; }
        public string UserId { get; set; } // ид клиента
        public DateTime OrderTime { get; set; } // дата и время оформления 
        public DateTime LeadTime { get; set; } //дата и время к которому нужно выполнить заказ
        public float TotalPrice { get; set; } //общая цена заказа
        public string Comment { get; set; } //комментарий пользователя 
        public PaymentType TypePayment { get; set; } // тип оплаты
        public StatusType Status { get; set; } // статус заказа

        public string PhoneNumber { get; set; } // телефон клиента
        public string Email { get; set; } // email  клиента
    }
}
