using Microsoft.EntityFrameworkCore.Migrations;

namespace masterInfrastructure.Migrations
{
    public partial class AddkeyAspNetUsersGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsersGroups",
                table: "AspNetUsersGroups",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsersGroups",
                table: "AspNetUsersGroups");
        }
    }
}
