using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPay.API.Migrations
{
    public partial class AccessRightsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessRights_Users_UserId",
                table: "AccessRights");

            migrationBuilder.DropIndex(
                name: "IX_AccessRights_UserId",
                table: "AccessRights");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AccessRights");

            migrationBuilder.CreateTable(
                name: "AccessRightUser",
                columns: table => new
                {
                    AccessRightsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsersId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRightUser", x => new { x.AccessRightsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AccessRightUser_AccessRights_AccessRightsId",
                        column: x => x.AccessRightsId,
                        principalTable: "AccessRights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRightUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessRightUser_UsersId",
                table: "AccessRightUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRightUser");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AccessRights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_UserId",
                table: "AccessRights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessRights_Users_UserId",
                table: "AccessRights",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
