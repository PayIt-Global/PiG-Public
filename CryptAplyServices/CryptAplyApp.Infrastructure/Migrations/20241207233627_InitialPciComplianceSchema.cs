using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptAplyApp.Infrastructure.Migrations
{
    public partial class InitialPciComplianceSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Vendor Risk Management
            migrationBuilder.CreateTable(
                name: "VendorProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OnboardingDate = table.Column<DateTime>(nullable: false),
                    LastAssessmentDate = table.Column<DateTime>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Metadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorProfiles", x => x.Id);
                });

            // Encryption Keys
            migrationBuilder.CreateTable(
                name: "EncryptionKeys",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    KeyMaterial = table.Column<byte[]>(nullable: false),
                    Algorithm = table.Column<string>(nullable: false),
                    KeySize = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastRotatedAt = table.Column<DateTime>(nullable: true),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RotationStatus = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Metadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptionKeys", x => x.Id);
                });

            // Key Rotations
            migrationBuilder.CreateTable(
                name: "KeyRotations",
                columns: table => new
                {
                    RotationId = table.Column<string>(nullable: false),
                    KeyId = table.Column<string>(nullable: false),
                    OldVersion = table.Column<int>(nullable: false),
                    NewVersion = table.Column<int>(nullable: false),
                    NewKeyMaterial = table.Column<byte[]>(nullable: false),
                    InitiatedBy = table.Column<string>(nullable: false),
                    InitiatedAt = table.Column<DateTime>(nullable: false),
                    CompletedAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Metadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyRotations", x => x.RotationId);
                    table.ForeignKey(
                        name: "FK_KeyRotations_EncryptionKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "EncryptionKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Archived Key Versions
            migrationBuilder.CreateTable(
                name: "ArchivedKeyVersions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    KeyId = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    KeyMaterial = table.Column<byte[]>(nullable: false),
                    ArchivedAt = table.Column<DateTime>(nullable: false),
                    ArchivedBy = table.Column<string>(nullable: true),
                    RotationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedKeyVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedKeyVersions_EncryptionKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "EncryptionKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArchivedKeyVersions_KeyRotations_RotationId",
                        column: x => x.RotationId,
                        principalTable: "KeyRotations",
                        principalColumn: "RotationId",
                        onDelete: ReferentialAction.SetNull);
                });

            // Audit Logs
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Action = table.Column<string>(nullable: false),
                    ResourceId = table.Column<string>(nullable: false),
                    ResourceType = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Sensitivity = table.Column<int>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    SessionId = table.Column<string>(nullable: true),
                    WasSuccessful = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Metadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            // User Location History
            migrationBuilder.CreateTable(
                name: "UserLocationHistory",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    FirstSeen = table.Column<DateTime>(nullable: false),
                    LastSeen = table.Column<DateTime>(nullable: false),
                    AccessCount = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApprovedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocationHistory", x => x.Id);
                });

            // Activity Patterns
            migrationBuilder.CreateTable(
                name: "ActivityPatterns",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    PatternType = table.Column<string>(nullable: false),
                    PatternValue = table.Column<string>(nullable: false),
                    Confidence = table.Column<double>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Metadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPatterns", x => x.Id);
                });

            // Alerts
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Metadata = table.Column<string>(nullable: true),
                    SlackMessageTs = table.Column<string>(nullable: true),
                    AcknowledgedBy = table.Column<string>(nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(nullable: true),
                    ResolvedBy = table.Column<string>(nullable: true),
                    ResolvedAt = table.Column<DateTime>(nullable: true),
                    Resolution = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_ArchivedKeyVersions_KeyId",
                table: "ArchivedKeyVersions",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedKeyVersions_RotationId",
                table: "ArchivedKeyVersions",
                column: "RotationId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyRotations_KeyId",
                table: "KeyRotations",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ResourceId",
                table: "AuditLogs",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocationHistory_UserId_Location",
                table: "UserLocationHistory",
                columns: new[] { "UserId", "Location" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPatterns_UserId_PatternType",
                table: "ActivityPatterns",
                columns: new[] { "UserId", "PatternType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Timestamp",
                table: "Alerts",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Status",
                table: "Alerts",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "VendorProfiles");
            migrationBuilder.DropTable(name: "ArchivedKeyVersions");
            migrationBuilder.DropTable(name: "KeyRotations");
            migrationBuilder.DropTable(name: "EncryptionKeys");
            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.DropTable(name: "UserLocationHistory");
            migrationBuilder.DropTable(name: "ActivityPatterns");
            migrationBuilder.DropTable(name: "Alerts");
        }
    }
}
