using System;

namespace Diplom.Common.Entities
{
    public class Content
    {
        public int ContentId { get; set; }
        public string Name { get; set; } // заголовок записи
        public string Text { get; set; } // текст записи
        public string Img { get; set; } // картинка записи
        public DateTime Date { get; set; } // даты добавления
    }
}