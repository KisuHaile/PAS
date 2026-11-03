using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatusIdToServiceRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[ServiceRequests]', N'ApprovalStatusId') IS NULL
BEGIN
    ALTER TABLE [ServiceRequests] ADD [ApprovalStatusId] uniqueidentifier NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_ServiceRequests_ApprovalStatusId'
      AND object_id = OBJECT_ID(N'[ServiceRequests]')
)
BEGIN
    CREATE INDEX [IX_ServiceRequests_ApprovalStatusId] ON [ServiceRequests] ([ApprovalStatusId]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ServiceRequests_ApprovalStatuses_ApprovalStatusId'
)
BEGIN
    ALTER TABLE [ServiceRequests]
    ADD CONSTRAINT [FK_ServiceRequests_ApprovalStatuses_ApprovalStatusId]
    FOREIGN KEY ([ApprovalStatusId]) REFERENCES [ApprovalStatuses]([Id]) ON DELETE NO ACTION;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_ServiceRequests_ApprovalStatuses_ApprovalStatusId'
)
BEGIN
    ALTER TABLE [ServiceRequests] DROP CONSTRAINT [FK_ServiceRequests_ApprovalStatuses_ApprovalStatusId];
END

IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_ServiceRequests_ApprovalStatusId'
      AND object_id = OBJECT_ID(N'[ServiceRequests]')
)
BEGIN
    DROP INDEX [IX_ServiceRequests_ApprovalStatusId] ON [ServiceRequests];
END

IF COL_LENGTH(N'[ServiceRequests]', N'ApprovalStatusId') IS NOT NULL
BEGIN
    ALTER TABLE [ServiceRequests] DROP COLUMN [ApprovalStatusId];
END
");
        }
    }
}
