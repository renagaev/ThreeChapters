using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Avatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_path",
                table: "participants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_path",
                table: "participants");
        }
    }
}
