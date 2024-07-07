using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Infrastructure.Identity;


namespace PayItGlobal.Infrastructure.Context
{
    // Adjust the generic type parameters to use int for the role key type
    public class ApplicationDbContext : IdentityDbContext<AspNetUser, AspNetRole, int, AspNetUserClaim, IdentityUserRole<int>, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customizing table names for ASP.NET Core Identity
            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.ToTable(name: "asp_net_users", schema: "core_identity");
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.ToTable(name: "asp_net_roles", schema: "core_identity");
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.ToTable(name: "asp_net_user_claims", schema: "core_identity");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable(name: "asp_net_user_roles", schema: "core_identity");
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.ToTable(name: "asp_net_user_logins", schema: "core_identity");
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.ToTable(name: "asp_net_role_claims", schema: "core_identity");
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.ToTable(name: "asp_net_user_tokens", schema: "core_identity");
            });

            // Example of configuring a unique index
            modelBuilder.Entity<AspNetUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Add any additional configuration as needed
        }
    }
}