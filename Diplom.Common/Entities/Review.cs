using System;

namespace Diplom.Common.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Text { get; set; } // текст отзыва
        public int Rating { get; set; } // выставленный рейтинг заведению
        public DateTime Date { get; set; } // дата оставления отзыва
        public string UserId { get; set; } // ид пользователя
        
        public virtual SiteUser User { get; set; }
    }
}