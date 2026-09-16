using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zogo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyLegalDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyLegalDetails",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnershipType = table.Column<short>(type: "smallint", nullable: true),
                    HasMortgage = table.Column<bool>(type: "boolean", nullable: false),
                    MortgageProvider = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HasLegalIssues = table.Column<bool>(type: "boolean", nullable: false),
                    LegalIssueDescription = table.Column<string>(type: "text", nullable: true),
                    LegalVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyLegalDetails", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_PropertyLegalDetails_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyLegalDetails");
        }
    }
}
