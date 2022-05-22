using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPay.API.Migrations
{
    public partial class AddAccessRightType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AccessRights",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "AccessRights");
        }
    }
}
