using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Data
{
    public class CryptAplyDbContext : DbContext
    {
        public CryptAplyDbContext(DbContextOptions<CryptAplyDbContext> options)
            : base(options)
        {
        }

        public DbSet<CryptoTeam> CryptoTeams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<CryptoKey> CryptoKeys { get; set; }
        public DbSet<KeyAction> KeyActions { get; set; }
        public DbSet<KeyActionVote> KeyActionVotes { get; set; }
        public DbSet<KeyActionAudit> KeyActionAudits { get; set; }
        public DbSet<KeyUsageLog> KeyUsageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CryptoTeam configuration
            modelBuilder.Entity<CryptoTeam>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(100);
            });

            // TeamMember configuration
            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                
                // Many-to-Many relationship with CryptoTeam
                entity.HasMany(e => e.Teams)
                      .WithMany(e => e.Members);
            });

            // CryptoKey configuration
            modelBuilder.Entity<CryptoKey>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Version).IsRequired().HasMaxLength(50);
                entity.Property(e => e.KeyVaultUri).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Algorithm).IsRequired().HasMaxLength(50);
                
                // Self-referencing relationship for derived keys
                entity.HasOne(e => e.ParentKey)
                      .WithMany(e => e.DerivedKeys)
                      .HasForeignKey(e => e.ParentKeyId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship with managing team
                entity.HasOne(e => e.ManagingTeam)
                      .WithMany(e => e.ManagedKeys)
                      .HasForeignKey(e => e.ManagingTeamId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // KeyAction configuration
            modelBuilder.Entity<KeyAction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
                
                entity.HasOne(e => e.CryptoKey)
                      .WithMany(e => e.Actions)
                      .HasForeignKey(e => e.CryptoKeyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CryptoTeam)
                      .WithMany(e => e.KeyActions)
                      .HasForeignKey(e => e.CryptoTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Initiator)
                      .WithMany(e => e.InitiatedActions)
                      .HasForeignKey(e => e.InitiatorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // KeyActionVote configuration
            modelBuilder.Entity<KeyActionVote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Comment).HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);

                entity.HasOne(e => e.KeyAction)
                      .WithMany(e => e.Votes)
                      .HasForeignKey(e => e.KeyActionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TeamMember)
                      .WithMany(e => e.Votes)
                      .HasForeignKey(e => e.TeamMemberId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // KeyActionAudit configuration
            modelBuilder.Entity<KeyActionAudit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Event).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Details).HasMaxLength(1000);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);

                entity.HasOne(e => e.KeyAction)
                      .WithMany(e => e.AuditTrail)
                      .HasForeignKey(e => e.KeyActionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // KeyUsageLog configuration
            modelBuilder.Entity<KeyUsageLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Operation).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Application).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.ErrorMessage).HasMaxLength(500);
                entity.Property(e => e.AdditionalData).HasMaxLength(1000);

                entity.HasOne(e => e.CryptoKey)
                      .WithMany(e => e.UsageLogs)
                      .HasForeignKey(e => e.CryptoKeyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
