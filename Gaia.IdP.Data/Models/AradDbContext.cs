using Gaia.IdP.DomainModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gaia.IdP.Data.Models
{
    public class AradDbContext : IdentityDbContext<AradUser>
    {
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public AradDbContext(DbContextOptions<AradDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AradDbContext).Assembly);
        }
    }
}
