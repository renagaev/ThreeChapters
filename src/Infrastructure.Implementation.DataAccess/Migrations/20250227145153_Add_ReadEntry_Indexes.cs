using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_ReadEntry_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_read_entries_date",
                table: "read_entries",
                column: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_read_entries_date",
                table: "read_entries");
        }
    }
}
