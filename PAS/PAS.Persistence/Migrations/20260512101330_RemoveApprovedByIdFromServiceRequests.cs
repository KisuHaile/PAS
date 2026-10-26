using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApprovedByIdFromServiceRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[ServiceRequests]', N'ApprovedById') IS NOT NULL
BEGIN
    ALTER TABLE [ServiceRequests] DROP COLUMN [ApprovedById];
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[ServiceRequests]', N'ApprovedById') IS NULL
BEGIN
    ALTER TABLE [ServiceRequests] ADD [ApprovedById] uniqueidentifier NULL;
END
");
        }
    }
}
