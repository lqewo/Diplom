using Diplom.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Diplom.Common.Entities
{
    public class SiteUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Year { get; set; }

        public SexType Sex { get; set; }
    }
}