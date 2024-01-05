using Microsoft.AspNetCore.Identity;

namespace asp_net_core_web_app_authentication_authorisation.Models
{
    public class ApplicationUser : IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String PassportNumber { get; set; }
        public Guid CustomerNumber { get; set; }
    }
}
