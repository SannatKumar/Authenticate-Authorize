using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceButtonBackend.Migrations
{
    /// <inheritdoc />
    public partial class UserPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "v_user_permission",
                columns: table => new
                {
                    PageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Read = table.Column<int>(type: "int", nullable: false),
                    Create = table.Column<int>(type: "int", nullable: false),
                    Update = table.Column<int>(type: "int", nullable: false),
                    Delete = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "v_user_permission");
        }
    }
}
