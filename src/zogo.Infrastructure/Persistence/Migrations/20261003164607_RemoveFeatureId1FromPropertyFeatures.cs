using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace zogo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFeatureId1FromPropertyFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyFeatures_Features_FeatureId1",
                table: "PropertyFeatures");

            migrationBuilder.DropIndex(
                name: "IX_PropertyFeatures_FeatureId1",
                table: "PropertyFeatures");

            migrationBuilder.DropColumn(
                name: "FeatureId1",
                table: "PropertyFeatures");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "FeatureId1",
                table: "PropertyFeatures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFeatures_FeatureId1",
                table: "PropertyFeatures",
                column: "FeatureId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyFeatures_Features_FeatureId1",
                table: "PropertyFeatures",
                column: "FeatureId1",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
