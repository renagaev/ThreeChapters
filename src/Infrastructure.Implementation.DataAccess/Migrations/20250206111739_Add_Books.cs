using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Books : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "books",
                columns: new[] { "id", "chapters_count", "title", "title_variants" },
                values: new object[,]
                {
                    { 65, 22, "Откровение", new[] { "Апокалипсис", "Откровение Иоанна" } },
                    { 64, 13, "Евреям", new[] { "Евреям" } },
                    { 63, 1, "Филимону", new[] { "Филимону" } },
                    { 62, 3, "Титу", new[] { "Титу" } },
                    { 61, 4, "2-Тимофею", new[] { "Второе Тимофею", "2 Тимофею" } },
                    { 60, 6, "1-Тимофею", new[] { "Первое Тимофею", "1 Тимофею" } },
                    { 59, 3, "2-Фессалоникийцам", new[] { "Второе Фессалоникийцам", "2 Фессалоникийцам" } },
                    { 58, 5, "1-Фессалоникийцам", new[] { "Первое Фессалоникийцам", "1 Фессалоникийцам" } },
                    { 57, 4, "Колоссянам", new[] { "Колоссянам" } },
                    { 56, 4, "Филиппийцам", new[] { "Филиппийцам" } },
                    { 55, 6, "Ефесянам", new[] { "Ефесянам" } },
                    { 54, 6, "Галатам", new[] { "Галатам" } },
                    { 53, 13, "2-Коринфянам", new[] { "Второе Коринфянам", "2 Коринфянам" } },
                    { 52, 16, "1-Коринфянам", new[] { "Первое Коринфянам", "1 Коринфянам" } },
                    { 51, 16, "Римлянам", new[] { "Римлянам" } },
                    { 50, 1, "Иуды", new[] { "Послание Иуды" } },
                    { 49, 1, "3-Иоанна", new[] { "Третье Иоанна", "3 Иоанна" } },
                    { 48, 1, "2-Иоанна", new[] { "Второе Иоанна", "2 Иоанна" } },
                    { 47, 5, "1-Иоанна", new[] { "Первое Иоанна", "1 Иоанна" } },
                    { 46, 3, "2-Петра", new[] { "Второе Петра", "2 Петра" } },
                    { 45, 5, "1-Петра", new[] { "Первое Петра", "1 Петра" } },
                    { 44, 5, "Иакова", new[] { "Послание Иакова" } },
                    { 43, 28, "Деяния", new[] { "Деяния Апостолов" } },
                    { 42, 21, "Иоанна", new[] { "Иоана" } },
                    { 41, 24, "Луки", new[] { "Луки" } },
                    { 40, 16, "Марка", new[] { "Марка" } },
                    { 39, 28, "Матфея", new[] { "Матвея" } },
                    { 38, 4, "Малахия", new[] { "Малахии" } },
                    { 37, 14, "Захария", new[] { "Захарии" } },
                    { 36, 2, "Аггей", new[] { "Аггея" } },
                    { 35, 3, "Софония", new[] { "Софонии" } },
                    { 34, 3, "Аввакум", new[] { "Аввакума" } },
                    { 33, 3, "Наум", new[] { "Наума" } },
                    { 32, 7, "Михей", new[] { "Михея" } },
                    { 31, 4, "Иона", new[] { "Ионы" } },
                    { 30, 1, "Авдий", new[] { "Авдия" } },
                    { 29, 9, "Амос", new[] { "Амоса" } },
                    { 28, 3, "Иоиль", new[] { "Иоиля" } },
                    { 27, 14, "Осия", new[] { "Осии" } },
                    { 26, 12, "Даниил", new[] { "Даниила" } },
                    { 25, 48, "Иезекииль", new[] { "Иезекииля" } },
                    { 24, 5, "Плач Иеремии", new string[0] },
                    { 23, 52, "Иеремия", new[] { "Иеремии" } },
                    { 22, 66, "Исаия", new[] { "Исайя" } },
                    { 21, 8, "Песни Песней", new[] { "Песнь Соломона" } },
                    { 20, 12, "Екклесиаст", new[] { "Екклесиаста", "Экклезиаст" } },
                    { 19, 31, "Притчи", new[] { "Притчи Соломона" } },
                    { 18, 150, "Псалтирь", new[] { "Псалмы", "Псалом" } },
                    { 17, 42, "Иов", new[] { "Иова" } },
                    { 16, 10, "Есфирь", new[] { "Есфири" } },
                    { 15, 13, "Неемия", new[] { "Неемии" } },
                    { 14, 10, "Ездра", new[] { "Ездры" } },
                    { 13, 36, "2-Паралипоменон", new[] { "Вторая Паралипоменон", "2 Пар", "2 Паралипоменон" } },
                    { 12, 29, "1-Паралипоменон", new[] { "Первая Паралипоменон", "1 Пар", "1 Паралипоменон" } },
                    { 11, 25, "4-Царств", new[] { "Четвёртая Царств", "4 Царств" } },
                    { 10, 22, "3-Царств", new[] { "Третья Царств", "3 Царств" } },
                    { 9, 24, "2-Царств", new[] { "Вторая Царств", "2 Царств" } },
                    { 8, 31, "1-Царств", new[] { "Первая Царств", "1 Царств" } },
                    { 7, 4, "Руфь", new[] { "Руфи" } },
                    { 6, 21, "Судей", new[] { "Книга Судей" } },
                    { 5, 24, "Иисус Навин", new[] { "Иисуса Навина", "Иисуса Навин" } },
                    { 4, 34, "Второзаконие", new string[0] },
                    { 3, 36, "Числа", new string[0] },
                    { 2, 27, "Левит", new string[0] },
                    { 1, 40, "Исход", new string[0] },
                    { 0, 50, "Бытие", new string[0] },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
