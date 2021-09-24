using Microsoft.EntityFrameworkCore.Migrations;

namespace masterInfrastructure.Migrations
{
    public partial class UpdateUserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idCustomer",
                table: "AspNetUsersGroups",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idCustomer",
                table: "AspNetUsersGroups");
        }
    }
}
