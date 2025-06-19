using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Admin_Flag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                table: "participants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_admin",
                table: "participants");
        }
    }
}
