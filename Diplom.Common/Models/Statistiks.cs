using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom.Common.Models
{
    public class Statistiks
    {
        public string populerEatMount { get; set; } // самое популярное блюдо за месяц
        public string populerEatYear { get; set; } // самое популярное блюдо за год

        public int revenueMount { get; set; } // доход за месяц
        public int revenueYear { get; set; } // доход за год

        public int averageCheck { get; set; } // средний чек заказа

        public int averageYear { get; set; } // средний возраст покупателя
        public int countSexFemale { get; set; } // кол-во женьщин покупателей за месяц
        public int countSexMale { get; set; } // кол-во мужчинпокупателей за месяц

        public int userCount { get; set; } // общее кол-во пользователей
    }
}
