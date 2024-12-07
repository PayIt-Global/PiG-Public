using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptAplyApp.Infrastructure.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoTeams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    MinimumQuorum = table.Column<int>(nullable: false),
                    RequiresMajority = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Role = table.Column<string>(maxLength: 50, nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    LastAccessDate = table.Column<DateTime>(nullable: true),
                    RequiresMFA = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    BackupContact = table.Column<string>(maxLength: 256, nullable: true),
                    Department = table.Column<string>(maxLength: 100, nullable: true),
                    CertificationExpiry = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoTeamTeamMember",
                columns: table => new
                {
                    TeamsId = table.Column<int>(nullable: false),
                    MembersId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoTeamTeamMember", x => new { x.TeamsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_CryptoTeamTeamMember_CryptoTeams_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "CryptoTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoTeamTeamMember_TeamMembers_MembersId",
                        column: x => x.MembersId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CryptoKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Version = table.Column<string>(maxLength: 50, nullable: false),
                    KeyVaultUri = table.Column<string>(maxLength: 500, nullable: false),
                    Algorithm = table.Column<string>(maxLength: 50, nullable: false),
                    KeySizeInBits = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ActivationDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    LastRotationDate = table.Column<DateTime>(nullable: true),
                    NextRotationDate = table.Column<DateTime>(nullable: true),
                    RotationPeriodDays = table.Column<int>(nullable: false),
                    RetentionPeriodDays = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 100, nullable: true),
                    LastUsedDate = table.Column<DateTime>(nullable: true),
                    UsageCount = table.Column<int>(nullable: false),
                    RequiresDoubleAuth = table.Column<bool>(nullable: false),
                    Purpose = table.Column<string>(maxLength: 500, nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    ComplianceNotes = table.Column<string>(nullable: true),
                    IsHSMBacked = table.Column<bool>(nullable: false),
                    BackupLocation = table.Column<string>(maxLength: 500, nullable: true),
                    ParentKeyId = table.Column<int>(nullable: true),
                    ManagingTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoKeys_CryptoTeams_ManagingTeamId",
                        column: x => x.ManagingTeamId,
                        principalTable: "CryptoTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CryptoKeys_CryptoKeys_ParentKeyId",
                        column: x => x.ParentKeyId,
                        principalTable: "CryptoKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeyActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    CompletionDate = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(maxLength: 500, nullable: false),
                    Details = table.Column<string>(nullable: true),
                    RequiresQuorum = table.Column<bool>(nullable: false),
                    RequiredVotes = table.Column<int>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    Result = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    IsEmergency = table.Column<bool>(nullable: false),
                    CryptoKeyId = table.Column<int>(nullable: false),
                    CryptoTeamId = table.Column<int>(nullable: false),
                    InitiatorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyActions_CryptoKeys_CryptoKeyId",
                        column: x => x.CryptoKeyId,
                        principalTable: "CryptoKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeyActions_CryptoTeams_CryptoTeamId",
                        column: x => x.CryptoTeamId,
                        principalTable: "CryptoTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KeyActions_TeamMembers_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeyUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptoKeyId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Operation = table.Column<string>(maxLength: 50, nullable: false),
                    UserId = table.Column<string>(maxLength: 100, nullable: false),
                    Application = table.Column<string>(maxLength: 100, nullable: false),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: true),
                    WasSuccessful = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(maxLength: 500, nullable: true),
                    AdditionalData = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyUsageLogs_CryptoKeys_CryptoKeyId",
                        column: x => x.CryptoKeyId,
                        principalTable: "CryptoKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyActionAudits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyActionId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Event = table.Column<string>(maxLength: 100, nullable: false),
                    Details = table.Column<string>(maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(maxLength: 100, nullable: false),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyActionAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyActionAudits_KeyActions_KeyActionId",
                        column: x => x.KeyActionId,
                        principalTable: "KeyActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyActionVotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyActionId = table.Column<int>(nullable: false),
                    TeamMemberId = table.Column<int>(nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    VoteDate = table.Column<DateTime>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(maxLength: 500, nullable: true),
                    WasMFAUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyActionVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyActionVotes_KeyActions_KeyActionId",
                        column: x => x.KeyActionId,
                        principalTable: "KeyActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyActionVotes_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoKeys_ManagingTeamId",
                table: "CryptoKeys",
                column: "ManagingTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoKeys_Name",
                table: "CryptoKeys",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoKeys_ParentKeyId",
                table: "CryptoKeys",
                column: "ParentKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTeams_Name",
                table: "CryptoTeams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTeamTeamMember_MembersId",
                table: "CryptoTeamTeamMember",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActionAudits_KeyActionId",
                table: "KeyActionAudits",
                column: "KeyActionId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActions_CryptoKeyId",
                table: "KeyActions",
                column: "CryptoKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActions_CryptoTeamId",
                table: "KeyActions",
                column: "CryptoTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActions_InitiatorId",
                table: "KeyActions",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActionVotes_KeyActionId",
                table: "KeyActionVotes",
                column: "KeyActionId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyActionVotes_TeamMemberId",
                table: "KeyActionVotes",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyUsageLogs_CryptoKeyId",
                table: "KeyUsageLogs",
                column: "CryptoKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_Email",
                table: "TeamMembers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMembers",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoTeamTeamMember");

            migrationBuilder.DropTable(
                name: "KeyActionAudits");

            migrationBuilder.DropTable(
                name: "KeyActionVotes");

            migrationBuilder.DropTable(
                name: "KeyUsageLogs");

            migrationBuilder.DropTable(
                name: "KeyActions");

            migrationBuilder.DropTable(
                name: "CryptoKeys");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "CryptoTeams");
        }
    }
}
