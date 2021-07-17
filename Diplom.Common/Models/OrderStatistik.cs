using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom.Common.Models
{
    public class OrderStatistik
    {
        public int OrderStatistikId { get; set; }

        public string Price { get; set; } // цена порции

        public string Name { get; set; } // название продукта
        public string ShortDescription { get; set; } // короткое оприсание
        public string Img { get; set; } // картинка продукта

        public int Quantity { get; set; } // количество
        public int BasketId { get; set; } // ид строчки в корзине

        public int OverallPrice { get; set; } // цена с учетом количества
        public int AllPrice { get; set; } // общая цена покупки

        public string Grams { get; set; } // кол-во грамм в порции

        public int ProductId { get; set; }
        public DateTime LeadTime { get; set; } //дата и время к которому нужно выполнить заказ
        public StatusType Status { get; set; } // статус заказа
        public float TotalPrice { get; set; } //общая цена заказа
        public int Year { get; set; } // возраст покупателя
        public SexType Sex { get; set; } // пол покупателя
    }
}
