using System;
using Microsoft.AspNetCore.Identity;

namespace Gaia.IdP.DomainModel.Models
{
    public class AradUser : IdentityUser
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime CreationTime { get; private set; }
        
        public string Image { get; set; }
        
        public DateTime? Birthdate { get; set; }
        
        public string NationalId { get; set; }
        
        public string FatherName { get; set; }
        
        public string RegisteredCity { get; set; }
        
        public string LivingCity { get; set; }
        
        public string JobStatus { get; set; }
        
        public string Job { get; set; }

        public string LastLoginOriginIPs { get; set; }
    }
}
