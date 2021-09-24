using Microsoft.EntityFrameworkCore.Migrations;

namespace masterInfrastructure.Migrations
{
    public partial class addmenuitemsgroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItemUserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    IdMenuItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUserGroup = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemUserGroups", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemUserGroups");
        }
    }
}
