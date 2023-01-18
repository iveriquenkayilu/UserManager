using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagerService.Repository.Migrations
{
    public partial class modified_typo_login_sessions_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "LoginSessions",
                newName: "IpAdress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAdress",
                table: "LoginSessions",
                newName: "IpAddress");
        }
    }
}
