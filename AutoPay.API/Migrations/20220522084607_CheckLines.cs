using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPay.API.Migrations
{
    public partial class CheckLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SettlementChecks_SettlementCheckId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SettlementCheckId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SettlementCheckId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "CheckLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Count = table.Column<float>(type: "REAL", nullable: false),
                    SettlementCheckId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckLine_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckLine_SettlementChecks_SettlementCheckId",
                        column: x => x.SettlementCheckId,
                        principalTable: "SettlementChecks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckLine_ProductId",
                table: "CheckLine",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLine_SettlementCheckId",
                table: "CheckLine",
                column: "SettlementCheckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckLine");

            migrationBuilder.AddColumn<Guid>(
                name: "SettlementCheckId",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SettlementCheckId",
                table: "Products",
                column: "SettlementCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SettlementChecks_SettlementCheckId",
                table: "Products",
                column: "SettlementCheckId",
                principalTable: "SettlementChecks",
                principalColumn: "Id");
        }
    }
}
