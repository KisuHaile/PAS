using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialStable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TinNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiresInspection = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemMasters_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PropertyCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyTypes_PropertyCategories_PropertyCategoryId",
                        column: x => x.PropertyCategoryId,
                        principalTable: "PropertyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SafetyBoxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalShelves = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyBoxes_PropertyLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "PropertyLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ShelfLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aisle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rack = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShelfNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QRCodeValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BinType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShelfLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShelfLocations_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SafetyBoxShelves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SafetyBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyBoxShelves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyBoxShelves_SafetyBoxes_SafetyBoxId",
                        column: x => x.SafetyBoxId,
                        principalTable: "SafetyBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approvers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvers_ApprovalWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "ApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Approvers_UserLogins_UserId",
                        column: x => x.UserId,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTrails_UserLogins_UserId",
                        column: x => x.UserId,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisposalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisposedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisposalMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisposalRecords_UserLogins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DisposalRecords_UserLogins_DisposedById",
                        column: x => x.DisposedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_UserLogins_UserId",
                        column: x => x.UserId,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceivingNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GRNNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryNoteNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceivedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceivingNotes_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceivingNotes_UserLogins_ReceivedById",
                        column: x => x.ReceivedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_ApprovalStatuses_ApprovalStatusId",
                        column: x => x.ApprovalStatusId,
                        principalTable: "ApprovalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_UserLogins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_UserLogins_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ItemMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShelfLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryStocks_ShelfLocations_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_ShelfLocations_ShelfLocationId",
                        column: x => x.ShelfLocationId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReturnMaterialRequestNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReturnNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnMaterialRequestNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_PropertyLocations_SourceLocationId",
                        column: x => x.SourceLocationId,
                        principalTable: "PropertyLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_ShelfLocations_SourceShelfId",
                        column: x => x.SourceShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_UserLogins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnMaterialRequestNotes_UserLogins_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransferRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferRecords_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRecords_PropertyLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "PropertyLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRecords_PropertyLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "PropertyLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRecords_ShelfLocations_FromShelfId",
                        column: x => x.FromShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferRecords_ShelfLocations_ToShelfId",
                        column: x => x.ToShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferRecords_UserLogins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferRecords_UserLogins_InitiatedById",
                        column: x => x.InitiatedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PropertyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SafetyBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SafetyBoxShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_PropertyCategories_PropertyCategoryId",
                        column: x => x.PropertyCategoryId,
                        principalTable: "PropertyCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Properties_PropertyLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "PropertyLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_SafetyBoxShelves_SafetyBoxShelfId",
                        column: x => x.SafetyBoxShelfId,
                        principalTable: "SafetyBoxShelves",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Properties_SafetyBoxes_SafetyBoxId",
                        column: x => x.SafetyBoxId,
                        principalTable: "SafetyBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InspectionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceivingNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    DeviationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionLogs_ReceivingNotes_ReceivingNoteId",
                        column: x => x.ReceivingNoteId,
                        principalTable: "ReceivingNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionLogs_UserLogins_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceivingNoteItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceivingNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceivingNoteItems_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceivingNoteItems_ReceivingNotes_ReceivingNoteId",
                        column: x => x.ReceivingNoteId,
                        principalTable: "ReceivingNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SR_Details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SRId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedQty = table.Column<int>(type: "int", nullable: false),
                    IssuedQty = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ItemMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SR_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SR_Details_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SR_Details_ItemMasters_ItemMasterId",
                        column: x => x.ItemMasterId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SR_Details_ServiceRequests_SRId",
                        column: x => x.SRId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SR_Details_ShelfLocations_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StoreIssueVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SRId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SIVNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreIssueVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreIssueVouchers_ServiceRequests_SRId",
                        column: x => x.SRId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreIssueVouchers_UserLogins_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "UserLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockLedgers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityChange = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    InventoryStockId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceivingNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLedgers_InventoryStocks_InventoryStockId",
                        column: x => x.InventoryStockId,
                        principalTable: "InventoryStocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockLedgers_ItemMasters_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLedgers_ReceivingNotes_ReceivingNoteId",
                        column: x => x.ReceivingNoteId,
                        principalTable: "ReceivingNotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockLedgers_ShelfLocations_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "ShelfLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedEntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceivingNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_ReceivingNotes_ReceivingNoteId",
                        column: x => x.ReceivingNoteId,
                        principalTable: "ReceivingNotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentAttachments_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStatuses_StatusName",
                table: "ApprovalStatuses",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflows_WorkflowName",
                table: "ApprovalWorkflows",
                column: "WorkflowName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvers_UserId",
                table: "Approvers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvers_WorkflowId_ApprovalLevel",
                table: "Approvers",
                columns: new[] { "WorkflowId", "ApprovalLevel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_ActionDate",
                table: "AuditTrails",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_EntityId",
                table: "AuditTrails",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_EntityName_EntityId",
                table: "AuditTrails",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_UserId",
                table: "AuditTrails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_ApprovedById",
                table: "DisposalRecords",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_DisposalDate",
                table: "DisposalRecords",
                column: "DisposalDate");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_DisposedById",
                table: "DisposalRecords",
                column: "DisposedById");

            migrationBuilder.CreateIndex(
                name: "IX_DisposalRecords_ItemId",
                table: "DisposalRecords",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_FileName",
                table: "DocumentAttachments",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_PropertyId",
                table: "DocumentAttachments",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_ReceivingNoteId",
                table: "DocumentAttachments",
                column: "ReceivingNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_RelatedEntityId_RelatedEntityName",
                table: "DocumentAttachments",
                columns: new[] { "RelatedEntityId", "RelatedEntityName" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAttachments_ServiceRequestId",
                table: "DocumentAttachments",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLogs_InspectorId",
                table: "InspectionLogs",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLogs_ReceivingNoteId",
                table: "InspectionLogs",
                column: "ReceivingNoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_ItemId_ShelfId",
                table: "InventoryStocks",
                columns: new[] { "ItemId", "ShelfId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_ItemMasterId",
                table: "InventoryStocks",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_ShelfId",
                table: "InventoryStocks",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_ShelfLocationId",
                table: "InventoryStocks",
                column: "ShelfLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_CategoryId",
                table: "ItemMasters",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_ItemName",
                table: "ItemMasters",
                column: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_SKU",
                table: "ItemMasters",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SentDate",
                table: "Notifications",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionName",
                table: "Permissions",
                column: "PermissionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LocationId",
                table: "Properties",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_PropertyCategoryId",
                table: "Properties",
                column: "PropertyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_PropertyTypeId",
                table: "Properties",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_SafetyBoxId",
                table: "Properties",
                column: "SafetyBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_SafetyBoxShelfId",
                table: "Properties",
                column: "SafetyBoxShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_SerialNumber",
                table: "Properties",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_TagNumber",
                table: "Properties",
                column: "TagNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCategories_Name",
                table: "PropertyCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLocations_Name",
                table: "PropertyLocations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTypes_Name",
                table: "PropertyTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTypes_PropertyCategoryId",
                table: "PropertyTypes",
                column: "PropertyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNoteItems_ItemId",
                table: "ReceivingNoteItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNoteItems_ReceivingNoteId_ItemId",
                table: "ReceivingNoteItems",
                columns: new[] { "ReceivingNoteId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNotes_GRNNumber",
                table: "ReceivingNotes",
                column: "GRNNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNotes_ReceivedById",
                table: "ReceivingNotes",
                column: "ReceivedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNotes_Status",
                table: "ReceivingNotes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNotes_SupplierId",
                table: "ReceivingNotes",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_ApprovedById",
                table: "ReturnMaterialRequestNotes",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_ItemId",
                table: "ReturnMaterialRequestNotes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_RequestDate",
                table: "ReturnMaterialRequestNotes",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_RequestedById",
                table: "ReturnMaterialRequestNotes",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_SourceLocationId",
                table: "ReturnMaterialRequestNotes",
                column: "SourceLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_SourceShelfId",
                table: "ReturnMaterialRequestNotes",
                column: "SourceShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterialRequestNotes_SupplierId",
                table: "ReturnMaterialRequestNotes",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyBoxes_BoxNumber",
                table: "SafetyBoxes",
                column: "BoxNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyBoxes_LocationId",
                table: "SafetyBoxes",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyBoxShelves_SafetyBoxId_ShelfNumber",
                table: "SafetyBoxShelves",
                columns: new[] { "SafetyBoxId", "ShelfNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ApprovalStatusId",
                table: "ServiceRequests",
                column: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ApprovedById",
                table: "ServiceRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_RequesterId",
                table: "ServiceRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_SRNumber",
                table: "ServiceRequests",
                column: "SRNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_Status",
                table: "ServiceRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfLocations_WarehouseId_Aisle_Rack_ShelfNumber",
                table: "ShelfLocations",
                columns: new[] { "WarehouseId", "Aisle", "Rack", "ShelfNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SR_Details_ItemId",
                table: "SR_Details",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SR_Details_ItemMasterId",
                table: "SR_Details",
                column: "ItemMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_SR_Details_ShelfId",
                table: "SR_Details",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_SR_Details_SRId",
                table: "SR_Details",
                column: "SRId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_CreatedDate",
                table: "StockLedgers",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_InventoryStockId",
                table: "StockLedgers",
                column: "InventoryStockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_ItemId",
                table: "StockLedgers",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_ReceivingNoteId",
                table: "StockLedgers",
                column: "ReceivingNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_ReferenceId",
                table: "StockLedgers",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_ShelfId",
                table: "StockLedgers",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIssueVouchers_IssuedById",
                table: "StoreIssueVouchers",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIssueVouchers_SIVNumber",
                table: "StoreIssueVouchers",
                column: "SIVNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreIssueVouchers_SRId",
                table: "StoreIssueVouchers",
                column: "SRId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierName",
                table: "Suppliers",
                column: "SupplierName");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TinNumber",
                table: "Suppliers",
                column: "TinNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_ApprovedById",
                table: "TransferRecords",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_FromLocationId",
                table: "TransferRecords",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_FromShelfId",
                table: "TransferRecords",
                column: "FromShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_InitiatedById",
                table: "TransferRecords",
                column: "InitiatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_ItemId",
                table: "TransferRecords",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_ToLocationId",
                table: "TransferRecords",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_ToShelfId",
                table: "TransferRecords",
                column: "ToShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRecords_TransferDate",
                table: "TransferRecords",
                column: "TransferDate");

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

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_LocationCode",
                table: "Warehouses",
                column: "LocationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_WarehouseName",
                table: "Warehouses",
                column: "WarehouseName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approvers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropTable(
                name: "DisposalRecords");

            migrationBuilder.DropTable(
                name: "DocumentAttachments");

            migrationBuilder.DropTable(
                name: "InspectionLogs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "ReceivingNoteItems");

            migrationBuilder.DropTable(
                name: "ReturnMaterialRequestNotes");

            migrationBuilder.DropTable(
                name: "SR_Details");

            migrationBuilder.DropTable(
                name: "StockLedgers");

            migrationBuilder.DropTable(
                name: "StoreIssueVouchers");

            migrationBuilder.DropTable(
                name: "TransferRecords");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflows");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "InventoryStocks");

            migrationBuilder.DropTable(
                name: "ReceivingNotes");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "PropertyTypes");

            migrationBuilder.DropTable(
                name: "SafetyBoxShelves");

            migrationBuilder.DropTable(
                name: "ItemMasters");

            migrationBuilder.DropTable(
                name: "ShelfLocations");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "ApprovalStatuses");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "PropertyCategories");

            migrationBuilder.DropTable(
                name: "SafetyBoxes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "PropertyLocations");
        }
    }
}
