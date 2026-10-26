using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Column may already exist (e.g., added manually or by a previous migration).
            // Make this migration idempotent for SQL Server.
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[AspNetUsers]', N'PhotoUrl') IS NULL
BEGIN
    ALTER TABLE [AspNetUsers] ADD [PhotoUrl] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[AspNetUsers]', N'PhotoUrl') IS NOT NULL
BEGIN
    ALTER TABLE [AspNetUsers] DROP COLUMN [PhotoUrl];
END
");
        }
    }
}
