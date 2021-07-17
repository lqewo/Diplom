using System;
using System.Collections.Generic;
using System.Text;

namespace Diplom.Common.Models
{
    public class WorkerList
    {
        public int WorkerListId { get; set; }

        public string UserName { get; set; } 
        public string LastName { get; set; } // имя
        public string FirstName { get; set; } // фамилия
        public string Year { get; set; } // возраст
        public string Email { get; set; } // почтовый ящик
        public string PhoneNumber { get; set; } // телефон

    }
}
