using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom.Common.Models
{
    public class ReviewShow
    {
        public int ReviewShowId { get; set; }
        public string Text { get; set; } // текст отзыва
        public int Rating { get; set; } // выставленный рейтинг заведению
        public DateTime Date { get; set; } // дата оставления отзыва
        public string FirstName { get; set; } // имя пользователя
    }
}
