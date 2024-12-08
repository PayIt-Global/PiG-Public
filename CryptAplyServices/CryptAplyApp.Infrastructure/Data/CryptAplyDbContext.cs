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

        // Security Policy Entities
        public DbSet<SecurityPolicy> SecurityPolicies { get; set; }
        public DbSet<SecurityControl> SecurityControls { get; set; }
        public DbSet<PolicyCompliance> PolicyCompliances { get; set; }
        public DbSet<PolicyException> PolicyExceptions { get; set; }
        public DbSet<ControlAssessment> ControlAssessments { get; set; }

        // ML Model Entities
        public DbSet<MLModel> MLModels { get; set; }
        public DbSet<ModelPrediction> ModelPredictions { get; set; }
        public DbSet<ModelTrainingHistory> ModelTrainingHistories { get; set; }
        public DbSet<ModelFeature> ModelFeatures { get; set; }

        // Emergency Protocol Entities
        public DbSet<EmergencyProtocol> EmergencyProtocols { get; set; }
        public DbSet<EmergencyIncident> EmergencyIncidents { get; set; }
        public DbSet<IncidentAction> IncidentActions { get; set; }
        public DbSet<ProtocolTest> ProtocolTests { get; set; }

        // Hardware Security Entities
        public DbSet<HSMDevice> HSMDevices { get; set; }
        public DbSet<HSMPartition> HSMPartitions { get; set; }
        public DbSet<HSMKey> HSMKeys { get; set; }
        public DbSet<HSMHealthLog> HSMHealthLogs { get; set; }
        public DbSet<HSMOperationLog> HSMOperationLogs { get; set; }
        public DbSet<KeyBackup> KeyBackups { get; set; }

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

            // Security Policy configurations
            modelBuilder.Entity<SecurityPolicy>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Settings).HasColumnType("jsonb");
            });

            modelBuilder.Entity<SecurityControl>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Implementation).HasMaxLength(1000);
            });

            modelBuilder.Entity<PolicyCompliance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PolicyId).IsRequired();
                entity.Property(e => e.ControlId).IsRequired();
                entity.Property(e => e.ComplianceStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ComplianceNotes).HasMaxLength(500);

                entity.HasOne(e => e.SecurityPolicy)
                      .WithMany(e => e.Compliances)
                      .HasForeignKey(e => e.PolicyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SecurityControl)
                      .WithMany(e => e.Compliances)
                      .HasForeignKey(e => e.ControlId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PolicyException>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PolicyId).IsRequired();
                entity.Property(e => e.ControlId).IsRequired();
                entity.Property(e => e.ExceptionReason).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ExceptionNotes).HasMaxLength(1000);

                entity.HasOne(e => e.SecurityPolicy)
                      .WithMany(e => e.Exceptions)
                      .HasForeignKey(e => e.PolicyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SecurityControl)
                      .WithMany(e => e.Exceptions)
                      .HasForeignKey(e => e.ControlId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ControlAssessment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ControlId).IsRequired();
                entity.Property(e => e.AssessmentStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AssessmentNotes).HasMaxLength(500);

                entity.HasOne(e => e.SecurityControl)
                      .WithMany(e => e.Assessments)
                      .HasForeignKey(e => e.ControlId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ML Model configurations
            modelBuilder.Entity<MLModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ModelPath).HasMaxLength(1000);
                entity.Property(e => e.Hyperparameters).HasColumnType("jsonb");
            });

            modelBuilder.Entity<ModelPrediction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Input).HasColumnType("jsonb");
                entity.Property(e => e.Output).HasColumnType("jsonb");
                entity.Property(e => e.FeedbackNotes).HasMaxLength(500);

                entity.HasOne(e => e.MLModel)
                      .WithMany(e => e.Predictions)
                      .HasForeignKey(e => e.ModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ModelTrainingHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModelId).IsRequired();
                entity.Property(e => e.TrainingStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TrainingNotes).HasMaxLength(500);

                entity.HasOne(e => e.MLModel)
                      .WithMany(e => e.TrainingHistory)
                      .HasForeignKey(e => e.ModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ModelFeature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModelId).IsRequired();
                entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FeatureType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FeatureDescription).HasMaxLength(500);

                entity.HasOne(e => e.MLModel)
                      .WithMany(e => e.Features)
                      .HasForeignKey(e => e.ModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Emergency Protocol configurations
            modelBuilder.Entity<EmergencyProtocol>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Procedures).HasMaxLength(2000);
                entity.Property(e => e.ResponsibleTeam).HasMaxLength(100);
                entity.Property(e => e.EscalationPath).HasMaxLength(500);
            });

            modelBuilder.Entity<EmergencyIncident>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IncidentId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AffectedSystems).HasMaxLength(500);
                entity.Property(e => e.ImpactAssessment).HasMaxLength(1000);
                entity.Property(e => e.ResolutionSteps).HasMaxLength(2000);
                entity.Property(e => e.Metadata).HasColumnType("jsonb");

                entity.HasOne(e => e.EmergencyProtocol)
                      .WithMany(e => e.Incidents)
                      .HasForeignKey(e => e.ProtocolId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<IncidentAction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IncidentId).IsRequired();
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ActionNotes).HasMaxLength(500);

                entity.HasOne(e => e.EmergencyIncident)
                      .WithMany(e => e.Actions)
                      .HasForeignKey(e => e.IncidentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProtocolTest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProtocolId).IsRequired();
                entity.Property(e => e.TestStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TestNotes).HasMaxLength(500);

                entity.HasOne(e => e.EmergencyProtocol)
                      .WithMany(e => e.Tests)
                      .HasForeignKey(e => e.ProtocolId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Hardware Security configurations
            modelBuilder.Entity<HSMDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirmwareVersion).HasMaxLength(50);
                entity.Property(e => e.Configuration).HasColumnType("jsonb");
            });

            modelBuilder.Entity<HSMPartition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Policies).HasColumnType("jsonb");

                entity.HasOne(e => e.HSMDevice)
                      .WithMany(e => e.Partitions)
                      .HasForeignKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HSMKey>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KeyId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Label).HasMaxLength(100);
                entity.Property(e => e.Attributes).HasColumnType("jsonb");

                entity.HasOne(e => e.HSMPartition)
                      .WithMany(e => e.Keys)
                      .HasForeignKey(e => e.PartitionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HSMHealthLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired();
                entity.Property(e => e.HealthStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HealthNotes).HasMaxLength(500);

                entity.HasOne(e => e.HSMDevice)
                      .WithMany(e => e.HealthLogs)
                      .HasForeignKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HSMOperationLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired();
                entity.Property(e => e.OperationType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.OperationNotes).HasMaxLength(500);

                entity.HasOne(e => e.HSMDevice)
                      .WithMany(e => e.OperationLogs)
                      .HasForeignKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<KeyBackup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KeyId).IsRequired();
                entity.Property(e => e.BackupLocation).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BackupNotes).HasMaxLength(500);

                entity.HasOne(e => e.HSMKey)
                      .WithMany(e => e.Backups)
                      .HasForeignKey(e => e.KeyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
