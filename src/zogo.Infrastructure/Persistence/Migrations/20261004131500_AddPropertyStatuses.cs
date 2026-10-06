using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zogo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""PropertyStatuses"" (
                    ""Id"" smallint NOT NULL,
                    ""Code"" character varying(50) NOT NULL,
                    ""Name"" character varying(100) NOT NULL,
                    ""IsActive"" boolean NOT NULL DEFAULT true,
                    CONSTRAINT ""PK_PropertyStatuses"" PRIMARY KEY (""Id"")
                );

                INSERT INTO ""PropertyStatuses"" (""Id"", ""Code"", ""Name"", ""IsActive"")
                VALUES 
                    (1, 'DRAFT', 'Draft', true),
                    (2, 'PUBLISHED', 'Published', true),
                    (3, 'UNDER_REVIEW', 'Under Review', true),
                    (4, 'PENDING_APPROVAL', 'Pending Approval', true),
                    (5, 'SOLD', 'Sold', true),
                    (6, 'RENTED', 'Rented', true),
                    (7, 'SUSPENDED', 'Suspended', true),
                    (8, 'INACTIVE', 'Inactive', true)
                ON CONFLICT (""Id"") DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PropertyStatuses");
        }
    }
}
