using System;

namespace Gaia.IdP.Message.Responses
{
    public class ActivityLog
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public string ClientId { get; set; }
        public bool Succeed { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }

        public enum ActivityType {
            login = 0,
            logout = 1
        }
    }
}
