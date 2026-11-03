using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[FK_Approvers_UserLogins_UserId]', N'F') IS NOT NULL ALTER TABLE [Approvers] DROP CONSTRAINT [FK_Approvers_UserLogins_UserId];
IF OBJECT_ID(N'[FK_AuditTrails_UserLogins_UserId]', N'F') IS NOT NULL ALTER TABLE [AuditTrails] DROP CONSTRAINT [FK_AuditTrails_UserLogins_UserId];
IF OBJECT_ID(N'[FK_DisposalRecords_UserLogins_ApprovedById]', N'F') IS NOT NULL ALTER TABLE [DisposalRecords] DROP CONSTRAINT [FK_DisposalRecords_UserLogins_ApprovedById];
IF OBJECT_ID(N'[FK_DisposalRecords_UserLogins_DisposedById]', N'F') IS NOT NULL ALTER TABLE [DisposalRecords] DROP CONSTRAINT [FK_DisposalRecords_UserLogins_DisposedById];
IF OBJECT_ID(N'[FK_InspectionLogs_UserLogins_InspectorId]', N'F') IS NOT NULL ALTER TABLE [InspectionLogs] DROP CONSTRAINT [FK_InspectionLogs_UserLogins_InspectorId];
IF OBJECT_ID(N'[FK_Notifications_UserLogins_UserId]', N'F') IS NOT NULL ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_UserLogins_UserId];
IF OBJECT_ID(N'[FK_Permissions_Roles_RoleId]', N'F') IS NOT NULL ALTER TABLE [Permissions] DROP CONSTRAINT [FK_Permissions_Roles_RoleId];
IF OBJECT_ID(N'[FK_ReceivingNotes_UserLogins_ReceivedById]', N'F') IS NOT NULL ALTER TABLE [ReceivingNotes] DROP CONSTRAINT [FK_ReceivingNotes_UserLogins_ReceivedById];
IF OBJECT_ID(N'[FK_ReturnMaterialRequestNotes_UserLogins_ApprovedById]', N'F') IS NOT NULL ALTER TABLE [ReturnMaterialRequestNotes] DROP CONSTRAINT [FK_ReturnMaterialRequestNotes_UserLogins_ApprovedById];
IF OBJECT_ID(N'[FK_ReturnMaterialRequestNotes_UserLogins_RequestedById]', N'F') IS NOT NULL ALTER TABLE [ReturnMaterialRequestNotes] DROP CONSTRAINT [FK_ReturnMaterialRequestNotes_UserLogins_RequestedById];
IF OBJECT_ID(N'[FK_ServiceRequests_UserLogins_ApprovedById]', N'F') IS NOT NULL ALTER TABLE [ServiceRequests] DROP CONSTRAINT [FK_ServiceRequests_UserLogins_ApprovedById];
IF OBJECT_ID(N'[FK_ServiceRequests_UserLogins_RequesterId]', N'F') IS NOT NULL ALTER TABLE [ServiceRequests] DROP CONSTRAINT [FK_ServiceRequests_UserLogins_RequesterId];
IF OBJECT_ID(N'[FK_StoreIssueVouchers_UserLogins_IssuedById]', N'F') IS NOT NULL ALTER TABLE [StoreIssueVouchers] DROP CONSTRAINT [FK_StoreIssueVouchers_UserLogins_IssuedById];
IF OBJECT_ID(N'[FK_TransferRecords_UserLogins_ApprovedById]', N'F') IS NOT NULL ALTER TABLE [TransferRecords] DROP CONSTRAINT [FK_TransferRecords_UserLogins_ApprovedById];
IF OBJECT_ID(N'[FK_TransferRecords_UserLogins_InitiatedById]', N'F') IS NOT NULL ALTER TABLE [TransferRecords] DROP CONSTRAINT [FK_TransferRecords_UserLogins_InitiatedById];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferRecords_ApprovedById' AND object_id = OBJECT_ID(N'[TransferRecords]')) DROP INDEX [IX_TransferRecords_ApprovedById] ON [TransferRecords];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TransferRecords_InitiatedById' AND object_id = OBJECT_ID(N'[TransferRecords]')) DROP INDEX [IX_TransferRecords_InitiatedById] ON [TransferRecords];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StoreIssueVouchers_IssuedById' AND object_id = OBJECT_ID(N'[StoreIssueVouchers]')) DROP INDEX [IX_StoreIssueVouchers_IssuedById] ON [StoreIssueVouchers];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequests_ApprovedById' AND object_id = OBJECT_ID(N'[ServiceRequests]')) DROP INDEX [IX_ServiceRequests_ApprovedById] ON [ServiceRequests];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceRequests_RequesterId' AND object_id = OBJECT_ID(N'[ServiceRequests]')) DROP INDEX [IX_ServiceRequests_RequesterId] ON [ServiceRequests];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReturnMaterialRequestNotes_ApprovedById' AND object_id = OBJECT_ID(N'[ReturnMaterialRequestNotes]')) DROP INDEX [IX_ReturnMaterialRequestNotes_ApprovedById] ON [ReturnMaterialRequestNotes];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReturnMaterialRequestNotes_RequestedById' AND object_id = OBJECT_ID(N'[ReturnMaterialRequestNotes]')) DROP INDEX [IX_ReturnMaterialRequestNotes_RequestedById] ON [ReturnMaterialRequestNotes];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReceivingNotes_ReceivedById' AND object_id = OBJECT_ID(N'[ReceivingNotes]')) DROP INDEX [IX_ReceivingNotes_ReceivedById] ON [ReceivingNotes];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_InspectionLogs_InspectorId' AND object_id = OBJECT_ID(N'[InspectionLogs]')) DROP INDEX [IX_InspectionLogs_InspectorId] ON [InspectionLogs];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DisposalRecords_ApprovedById' AND object_id = OBJECT_ID(N'[DisposalRecords]')) DROP INDEX [IX_DisposalRecords_ApprovedById] ON [DisposalRecords];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DisposalRecords_DisposedById' AND object_id = OBJECT_ID(N'[DisposalRecords]')) DROP INDEX [IX_DisposalRecords_DisposedById] ON [DisposalRecords];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditTrails_UserId' AND object_id = OBJECT_ID(N'[AuditTrails]')) DROP INDEX [IX_AuditTrails_UserId] ON [AuditTrails];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Approvers_UserId' AND object_id = OBJECT_ID(N'[Approvers]')) DROP INDEX [IX_Approvers_UserId] ON [Approvers];

IF COL_LENGTH(N'[DisposalRecords]', N'ApprovedById') IS NOT NULL ALTER TABLE [DisposalRecords] DROP COLUMN [ApprovedById];

IF OBJECT_ID(N'[UserLogins]', N'U') IS NOT NULL DROP TABLE [UserLogins];
IF OBJECT_ID(N'[Roles]', N'U') IS NOT NULL DROP TABLE [Roles];
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "DisposalRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogins_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogins_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_ApprovedById",
                table: "TransferRecords",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_InitiatedById",
                table: "TransferRecords",
                column: "InitiatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIssueVouchers_IssuedById",
                table: "StoreIssueVouchers",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ApprovedById",
                table: "ServiceRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_RequesterId",
                table: "ServiceRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_ApprovedById",
                table: "ReturnMaterialRequestNotes",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_RequestedById",
                table: "ReturnMaterialRequestNotes",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNotes_ReceivedById",
                table: "ReceivingNotes",
                column: "ReceivedById");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLogs_InspectorId",
                table: "InspectionLogs",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_ApprovedById",
                table: "DisposalRecords",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_DisposedById",
                table: "DisposalRecords",
                column: "DisposedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_UserId",
                table: "AuditTrails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvers_UserId",
                table: "Approvers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_AspNetUserId",
                table: "UserLogins",
                column: "AspNetUserId",
                unique: true,
                filter: "[AspNetUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_EmployeeId",
                table: "UserLogins",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_RoleId",
                table: "UserLogins",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_Username",
                table: "UserLogins",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvers_UserLogins_UserId",
                table: "Approvers",
                column: "UserId",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrails_UserLogins_UserId",
                table: "AuditTrails",
                column: "UserId",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DisposalRecords_UserLogins_ApprovedById",
                table: "DisposalRecords",
                column: "ApprovedById",
                principalTable: "UserLogins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DisposalRecords_UserLogins_DisposedById",
                table: "DisposalRecords",
                column: "DisposedById",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionLogs_UserLogins_InspectorId",
                table: "InspectionLogs",
                column: "InspectorId",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserLogins_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                table: "Permissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingNotes_UserLogins_ReceivedById",
                table: "ReceivingNotes",
                column: "ReceivedById",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnMaterialRequestNotes_UserLogins_ApprovedById",
                table: "ReturnMaterialRequestNotes",
                column: "ApprovedById",
                principalTable: "UserLogins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnMaterialRequestNotes_UserLogins_RequestedById",
                table: "ReturnMaterialRequestNotes",
                column: "RequestedById",
                principalTable: "UserLogins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_UserLogins_ApprovedById",
                table: "ServiceRequests",
                column: "ApprovedById",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_UserLogins_RequesterId",
                table: "ServiceRequests",
                column: "RequesterId",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreIssueVouchers_UserLogins_IssuedById",
                table: "StoreIssueVouchers",
                column: "IssuedById",
                principalTable: "UserLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRecords_UserLogins_ApprovedById",
                table: "TransferRecords",
                column: "ApprovedById",
                principalTable: "UserLogins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRecords_UserLogins_InitiatedById",
                table: "TransferRecords",
                column: "InitiatedById",
                principalTable: "UserLogins",
                principalColumn: "Id");
        }
    }
}
