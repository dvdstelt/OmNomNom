using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentInfo.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    CreditCardId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CardHolder = table.Column<string>(type: "TEXT", nullable: false),
                    CardType = table.Column<string>(type: "TEXT", nullable: false),
                    LastDigits = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<string>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.CreditCardId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_CreditCards_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCards",
                        principalColumn: "CreditCardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreditCardId",
                table: "Orders",
                column: "CreditCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CreditCards");
        }
    }
}
