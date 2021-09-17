using System;

namespace Gaia.IdP.DomainModel.Models
{
    public class ActivityLog
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public AradUser User { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public string ClientId { get; set; }
        public bool Succeed { get; set; }
        public string ErrorMessage { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }

        public enum ActivityType {
            login = 0,
            logout = 1
        }
    }
}
