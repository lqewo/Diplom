using Diplom.Common.Models;

namespace Diplom.Common.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } // название продукта
        public string ShortDescription { get; set; } // короткое описание
        public string LongDescription { get; set; } //длинное описание
        public string Img { get; set; } // картинка продукта
        public MenuType Type { get; set; } //тип продукта (еда или напиток)
    }
}