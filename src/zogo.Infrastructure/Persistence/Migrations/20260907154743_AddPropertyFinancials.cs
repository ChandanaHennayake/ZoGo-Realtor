using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace zogo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyFinancials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartmentTypes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Condominiums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DistrictId = table.Column<short>(type: "smallint", nullable: true),
                    DeveloperName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TotalFloors = table.Column<int>(type: "integer", nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condominiums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Condominiums_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FurnishingTypes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnishingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFinancials",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaintenanceFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaintenanceFeePeriod = table.Column<short>(type: "smallint", nullable: true),
                    SinkingFundAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SinkingFundPeriod = table.Column<short>(type: "smallint", nullable: true),
                    BillsUpToDate = table.Column<bool>(type: "boolean", nullable: true),
                    HasOutstandingCharges = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OutstandingDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFinancials", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_PropertyFinancials_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ViewTypes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApartmentDetails",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApartmentTypeId = table.Column<short>(type: "smallint", nullable: true),
                    FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    UnitNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsCornerUnit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FloorAreaSqFt = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Bedrooms = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    MasterBedrooms = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Bathrooms = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    AttachedBathrooms = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Balconies = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    FurnishingTypeId = table.Column<short>(type: "smallint", nullable: true),
                    ViewTypeId = table.Column<short>(type: "smallint", nullable: true),
                    HasMaidRoom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasMaidBathroom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasLaundryArea = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasStorageRoom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasWalkInCloset = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasDriversRoom = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentDetails", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_ApartmentDetails_ApartmentTypes_ApartmentTypeId",
                        column: x => x.ApartmentTypeId,
                        principalTable: "ApartmentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartmentDetails_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartmentDetails_FurnishingTypes_FurnishingTypeId",
                        column: x => x.FurnishingTypeId,
                        principalTable: "FurnishingTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartmentDetails_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartmentDetails_ViewTypes_ViewTypeId",
                        column: x => x.ViewTypeId,
                        principalTable: "ViewTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentDetails_ApartmentTypeId",
                table: "ApartmentDetails",
                column: "ApartmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentDetails_CondominiumId",
                table: "ApartmentDetails",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentDetails_FurnishingTypeId",
                table: "ApartmentDetails",
                column: "FurnishingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentDetails_ViewTypeId",
                table: "ApartmentDetails",
                column: "ViewTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Condominiums_DistrictId",
                table: "Condominiums",
                column: "DistrictId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentDetails");

            migrationBuilder.DropTable(
                name: "PropertyFinancials");

            migrationBuilder.DropTable(
                name: "ApartmentTypes");

            migrationBuilder.DropTable(
                name: "Condominiums");

            migrationBuilder.DropTable(
                name: "FurnishingTypes");

            migrationBuilder.DropTable(
                name: "ViewTypes");
        }
    }
}
