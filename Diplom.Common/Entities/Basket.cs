namespace Diplom.Common.Entities
{
    public class Basket
    {
        public int BasketId { get; set; }
        public string UserId { get; set; } // ид клиента
        public int AdditionMenuId { get; set; } // ид выбранного блюда
        public int Quantity { get; set; } // количество
        public int? OrderId { get; set; } // ид заказа
        
        public virtual Order Order { get; set; }
        public virtual SiteUser User { get; set; }
    }
}