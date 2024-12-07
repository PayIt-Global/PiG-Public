using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Data
{
    public class PciComplianceDbContext : DbContext
    {
        public PciComplianceDbContext(DbContextOptions<PciComplianceDbContext> options)
            : base(options)
        {
        }

        // Vendor Risk Management
        public DbSet<VendorEntity> Vendors { get; set; }
        public DbSet<VendorServiceEntity> VendorServices { get; set; }
        public DbSet<VendorContactEntity> VendorContacts { get; set; }
        public DbSet<VendorCertificationEntity> VendorCertifications { get; set; }
        public DbSet<VendorRiskScoreEntity> VendorRiskScores { get; set; }
        public DbSet<RiskFindingEntity> RiskFindings { get; set; }

        // Incident Response
        public DbSet<SecurityIncidentEntity> SecurityIncidents { get; set; }
        public DbSet<ContainmentActionEntity> ContainmentActions { get; set; }
        public DbSet<ForensicEvidenceEntity> ForensicEvidence { get; set; }
        public DbSet<IncidentTimelineEntity> IncidentTimelines { get; set; }
        public DbSet<TimelineEventEntity> TimelineEvents { get; set; }
        public DbSet<AutomatedResponseEntity> AutomatedResponses { get; set; }
        public DbSet<AutomatedActionEntity> AutomatedActions { get; set; }

        // Compliance Calendar
        public DbSet<ComplianceCalendarEntity> ComplianceCalendars { get; set; }
        public DbSet<ComplianceEventEntity> ComplianceEvents { get; set; }
        public DbSet<RecurringTaskEntity> RecurringTasks { get; set; }
        public DbSet<ComplianceDeadlineEntity> ComplianceDeadlines { get; set; }
        public DbSet<TaskDependencyEntity> TaskDependencies { get; set; }
        public DbSet<CalendarNotificationEntity> CalendarNotifications { get; set; }
        public DbSet<ComplianceWindowEntity> ComplianceWindows { get; set; }

        // Alerts
        public DbSet<AlertEntity> Alerts { get; set; }

        // Key Management
        public DbSet<EncryptionKeyEntity> EncryptionKeys { get; set; }
        public DbSet<KeyRotationEntity> KeyRotations { get; set; }
        public DbSet<ArchivedKeyVersionEntity> ArchivedKeyVersions { get; set; }

        // Audit Logging
        public DbSet<AuditLogEntryEntity> AuditLogs { get; set; }
        public DbSet<UserLocationHistoryEntity> UserLocationHistory { get; set; }
        public DbSet<ActivityPatternEntity> ActivityPatterns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Vendor Risk Management configurations
            modelBuilder.Entity<VendorEntity>()
                .HasMany(v => v.Services)
                .WithOne(s => s.Vendor)
                .HasForeignKey(s => s.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VendorEntity>()
                .HasMany(v => v.Contacts)
                .WithOne(c => c.Vendor)
                .HasForeignKey(c => c.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VendorEntity>()
                .HasMany(v => v.Certifications)
                .WithOne(c => c.Vendor)
                .HasForeignKey(c => c.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VendorEntity>()
                .HasOne(v => v.RiskScore)
                .WithOne(r => r.Vendor)
                .HasForeignKey<VendorRiskScoreEntity>(r => r.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Incident Response configurations
            modelBuilder.Entity<SecurityIncidentEntity>()
                .HasMany(i => i.ContainmentActions)
                .WithOne(a => a.Incident)
                .HasForeignKey(a => a.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SecurityIncidentEntity>()
                .HasMany(i => i.Evidence)
                .WithOne(e => e.Incident)
                .HasForeignKey(e => e.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SecurityIncidentEntity>()
                .HasOne(i => i.Timeline)
                .WithOne(t => t.Incident)
                .HasForeignKey<IncidentTimelineEntity>(t => t.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IncidentTimelineEntity>()
                .HasMany(t => t.Events)
                .WithOne(e => e.Timeline)
                .HasForeignKey(e => e.TimelineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Compliance Calendar configurations
            modelBuilder.Entity<ComplianceCalendarEntity>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Calendar)
                .HasForeignKey(e => e.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplianceCalendarEntity>()
                .HasMany(c => c.RecurringTasks)
                .WithOne(t => t.Calendar)
                .HasForeignKey(t => t.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplianceCalendarEntity>()
                .HasMany(c => c.Deadlines)
                .WithOne(d => d.Calendar)
                .HasForeignKey(d => d.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplianceCalendarEntity>()
                .HasMany(c => c.Notifications)
                .WithOne(n => n.Calendar)
                .HasForeignKey(n => n.CalendarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecurringTaskEntity>()
                .HasMany(t => t.Dependencies)
                .WithOne(d => d.Task)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlertEntity>(entity =>
            {
                entity.HasKey(e => e.AlertId);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Severity).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.TagsJson).HasColumnName("Tags");
                entity.Property(e => e.MetadataJson).HasColumnName("Metadata");
            });

            // Key Management configuration
            modelBuilder.Entity<EncryptionKeyEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.KeyMaterial).IsRequired();
                entity.Property(e => e.Algorithm).IsRequired();
                entity.Property(e => e.KeySize).IsRequired();
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.RotationStatus).IsRequired();
                entity.Property(e => e.TagsJson).HasColumnName("Tags");
                entity.Property(e => e.MetadataJson).HasColumnName("Metadata");
            });

            modelBuilder.Entity<KeyRotationEntity>(entity =>
            {
                entity.HasKey(e => e.RotationId);
                entity.Property(e => e.KeyId).IsRequired();
                entity.Property(e => e.OldVersion).IsRequired();
                entity.Property(e => e.NewVersion).IsRequired();
                entity.Property(e => e.NewKeyMaterial).IsRequired();
                entity.Property(e => e.InitiatedBy).IsRequired();
                entity.Property(e => e.InitiatedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.MetadataJson).HasColumnName("Metadata");

                entity.HasOne(e => e.Key)
                    .WithMany()
                    .HasForeignKey(e => e.KeyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ArchivedKeyVersionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KeyId).IsRequired();
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.KeyMaterial).IsRequired();
                entity.Property(e => e.ArchivedAt).IsRequired();

                entity.HasOne(e => e.Key)
                    .WithMany()
                    .HasForeignKey(e => e.KeyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Rotation)
                    .WithMany()
                    .HasForeignKey(e => e.RotationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Audit Logging configuration
            modelBuilder.Entity<AuditLogEntryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.ResourceId).IsRequired();
                entity.Property(e => e.ResourceType).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Sensitivity).IsRequired();
                entity.Property(e => e.MetadataJson).HasColumnName("Metadata");
            });

            modelBuilder.Entity<UserLocationHistoryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.FirstSeen).IsRequired();
                entity.Property(e => e.LastSeen).IsRequired();
                entity.Property(e => e.AccessCount).IsRequired();
                entity.Property(e => e.IsApproved).IsRequired();
            });

            modelBuilder.Entity<ActivityPatternEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.PatternType).IsRequired();
                entity.Property(e => e.PatternValue).IsRequired();
                entity.Property(e => e.Confidence).IsRequired();
                entity.Property(e => e.LastUpdated).IsRequired();
                entity.Property(e => e.MetadataJson).HasColumnName("Metadata");
            });
        }
    }
}
