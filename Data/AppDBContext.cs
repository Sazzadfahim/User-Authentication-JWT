using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Models;

namespace UserAuthentication.Data
{
    public class AppDBContext: IdentityDbContext<User>
    {

        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
            
        }   

        //public DbSet<User> Users { get; set; }
    }
}
