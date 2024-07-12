using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Entities; // Assuming this namespace contains your custom entities
using PayItGlobal.Infrastructure.Identity; // Adjust namespace as necessary

namespace PayItGlobal.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUser, AspNetRole, int, AspNetUserClaim, IdentityUserRole<int>, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("asp_net_users", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserName).HasColumnName("user_name");
                entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");
                entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            });

            // IdentityRole entity
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.ToTable("asp_net_roles", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.NormalizedName).HasColumnName("normalized_name");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });

            // IdentityUserRole<string> entity
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("asp_net_user_roles", "core_identity");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
            });

            // IdentityUserClaim<string> entity
            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.ToTable("asp_net_user_claims", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ClaimType).HasColumnName("claim_type");
                entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });

            // IdentityUserLogin<string> entity
            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.ToTable("asp_net_user_logins", "core_identity");
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }); // Composite key
                entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
                entity.Property(e => e.ProviderKey).HasColumnName("provider_key");
                entity.Property(e => e.ProviderDisplayName).HasColumnName("provider_display_name");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            // IdentityUserToken<string> entity
            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.ToTable("asp_net_user_tokens", "core_identity");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Value).HasColumnName("value");
            });

            // IdentityRoleClaim<string> entity
            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.ToTable("asp_net_role_claims", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.ClaimType).HasColumnName("claim_type");
                entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });

            // RefreshToken entity (assuming you have a RefreshToken entity)
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens", "core_identity");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.Expires).HasColumnName("expires");
                entity.Property(e => e.Created).HasColumnName("created");
                entity.Property(e => e.CreatedByIp).HasColumnName("created_by_ip");
                entity.Property(e => e.Revoked).HasColumnName("revoked");
                entity.Property(e => e.RevokedByIp).HasColumnName("revoked_by_ip");
                entity.Property(e => e.ReplacedByToken).HasColumnName("replaced_by_token");
                entity.Property(e => e.ReasonRevoked).HasColumnName("reason_revoked");
            });
        }

    }
}
