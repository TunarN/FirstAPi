using Microsoft.AspNetCore.Identity;

namespace FirstApi.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
