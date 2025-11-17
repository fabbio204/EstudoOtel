using EstudoOtel.Models;
using Microsoft.EntityFrameworkCore;

namespace EstudoOtel.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Pessoa> Pessoas { get; set; }
    }
}
