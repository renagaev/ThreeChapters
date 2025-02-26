using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Bible_Structure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "group_title",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "testament",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("update books set testament = case when id <= 38 then 'Old' else 'New' end");
            migrationBuilder.Sql(@"update books set group_title = (case 
                                          when id between 0 and 4 then 'Пятикнижие'
                                          when id between 5 and 16 then 'Исторические книги'
                                          when id between 17 and 21 then 'Поэтические книги'
                                          when id between 22 and 26 then 'Пророки'
                                          when id between 27 and 38 then 'Малые пророки'
                                          when id between 39 and 42 then 'Евангелия'
                                          when id between 43 and 43 then 'История Церкви'
                                          when id between 44 and 64 then 'Послания'
                                          when id between 65 and 65 then 'Откровения' end)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "group_title",
                table: "books");

            migrationBuilder.DropColumn(
                name: "testament",
                table: "books");
        }
    }
}
