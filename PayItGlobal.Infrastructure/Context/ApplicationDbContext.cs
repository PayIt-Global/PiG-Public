using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Infrastructure.Identity;

namespace PayItGlobal.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUser, AspNetRole, Guid, AspNetUserClaim, IdentityUserRole<Guid>, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser entity
            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.ToTable("asp_net_users", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                // No need to explicitly set default value for UUID; ensure your database is configured correctly
                // Other properties remain unchanged
            });

            // Other entity configurations remain the same as your previous setup
            // No explicit call to HasPostgresExtension is needed
        }
    }
}
