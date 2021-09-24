using Microsoft.EntityFrameworkCore.Migrations;

namespace masterInfrastructure.Migrations
{
    public partial class addinactivetousermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "inactive",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inactive",
                table: "AspNetUsers");
        }
    }
}
