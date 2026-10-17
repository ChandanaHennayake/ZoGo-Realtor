using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zogo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentProfiles_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentDistricts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DistrictId = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentDistricts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentDistricts_AgentProfiles_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentDistricts_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AgentProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentNotifications_AgentProfiles_AgentProfileId",
                        column: x => x.AgentProfileId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentNotifications_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentNotifications_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInterests_Status",
                table: "PropertyInterests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerPropertyInteractions_CurrentStatus",
                table: "BuyerPropertyInteractions",
                column: "CurrentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDistricts_AgentId",
                table: "AgentDistricts",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentDistricts_AgentId_DistrictId",
                table: "AgentDistricts",
                columns: new[] { "AgentId", "DistrictId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentDistricts_DistrictId",
                table: "AgentDistricts",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotifications_AgentProfileId",
                table: "AgentNotifications",
                column: "AgentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotifications_AgentProfileId_PropertyId_BuyerId",
                table: "AgentNotifications",
                columns: new[] { "AgentProfileId", "PropertyId", "BuyerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotifications_BuyerId",
                table: "AgentNotifications",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotifications_CreatedAt",
                table: "AgentNotifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotifications_PropertyId",
                table: "AgentNotifications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_CityId",
                table: "AgentProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_IsActive",
                table: "AgentProfiles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_UserId",
                table: "AgentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO ""Roles"" (""Code"", ""Name"", ""IsActive"")
                VALUES 
                    ('AGT', 'Agent', true),
                    ('ADM', 'Admin', true)
                ON CONFLICT (""Code"") DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentDistricts");

            migrationBuilder.DropTable(
                name: "AgentNotifications");

            migrationBuilder.DropTable(
                name: "AgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_PropertyInterests_Status",
                table: "PropertyInterests");

            migrationBuilder.DropIndex(
                name: "IX_BuyerPropertyInteractions_CurrentStatus",
                table: "BuyerPropertyInteractions");
        }
    }
}

