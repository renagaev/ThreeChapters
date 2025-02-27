using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_MemberFrom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "member_from",
                table: "participants",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
            
            migrationBuilder.Sql("with dates as (select participant_id, min(date) as min_date from read_entries group by participant_id) update participants set member_from = min_date from dates where id = dates.participant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "member_from",
                table: "participants");
        }
    }
}
