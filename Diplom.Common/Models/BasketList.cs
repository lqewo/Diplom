namespace Diplom.Common.Models
{
    public class BasketList
    {
        public int BasketListId { get; set; }

        public string Price { get; set; } // цена порции

        public string Name { get; set; } // название продукта
        public string ShortDescription { get; set; } // короткое оприсание
        public string Img { get; set; } // картинка продукта

        public int Quantity { get; set; } // количество
        public int BasketId { get; set; } // ид строчки в корзине

        public int OverallPrice { get; set; } // цена с учетом количества
        public int AllPrice { get; set; } // общая цена покупки

        public string Grams { get; set; } // кол-во грамм в порции
    }
}