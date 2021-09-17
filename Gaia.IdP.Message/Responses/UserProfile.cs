using System;

namespace Gaia.IdP.Message.Responses
{
    public class UserProfile
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public DateTime? Birthdate { get; set; }
        public string NationalId { get; set; }
        public string FatherName { get; set; }
    }
}
