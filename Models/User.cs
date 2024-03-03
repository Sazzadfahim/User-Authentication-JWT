using Microsoft.AspNetCore.Identity;

namespace UserAuthentication.Models
{
    public class User: IdentityUser
    {
        public string FullName { get; set; }


    }
}
