using Microsoft.EntityFrameworkCore;

namespace TestingSoftwareAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
