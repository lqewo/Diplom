namespace Diplom.Common.Models
{
    public class UserResponse
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Year { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public SexType Sex { get; set; }
    }
}