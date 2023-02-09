using Microsoft.EntityFrameworkCore;

using Boticario.WebApp.Models;

namespace Boticario.WebApp.Data
{
    public class AppDbContext : DbContext
    {
        protected AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Repos> Repositories { get; set; }
    }
}
