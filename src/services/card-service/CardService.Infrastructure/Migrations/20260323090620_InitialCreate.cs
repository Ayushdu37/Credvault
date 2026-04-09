using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardIssuers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CardLength = table.Column<int>(type: "int", nullable: false),
                    BinPrefixes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardIssuers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaskedNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardNumberHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpiryMonth = table.Column<int>(type: "int", nullable: false),
                    ExpiryYear = table.Column<int>(type: "int", nullable: false),
                    IssuerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    BillingCycleStartDay = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCards_CardIssuers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "CardIssuers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CardIssuers",
                columns: new[] { "Id", "BinPrefixes", "CardLength", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000001"), "4", 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Visa" },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000002"), "51,52,53,54,55,2221-2720", 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mastercard" },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000003"), "34,37", 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Amex" },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000004"), "60,65,81,82,508", 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RuPay" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardIssuers_Name",
                table: "CardIssuers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_IssuerId",
                table: "CreditCards",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId_CardNumberHash",
                table: "CreditCards",
                columns: new[] { "UserId", "CardNumberHash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "CardIssuers");
        }
    }
}
