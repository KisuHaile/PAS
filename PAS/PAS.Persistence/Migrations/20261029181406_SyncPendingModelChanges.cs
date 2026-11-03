using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyTypes_PropertyCategories_PropertyCategoryId",
                table: "PropertyTypes");

            migrationBuilder.DropIndex(
                name: "IX_PropertyTypes_PropertyCategoryId",
                table: "PropertyTypes");

            migrationBuilder.DropColumn(
                name: "PropertyCategoryId",
                table: "PropertyTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropertyCategoryId",
                table: "PropertyTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTypes_PropertyCategoryId",
                table: "PropertyTypes",
                column: "PropertyCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyTypes_PropertyCategories_PropertyCategoryId",
                table: "PropertyTypes",
                column: "PropertyCategoryId",
                principalTable: "PropertyCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
