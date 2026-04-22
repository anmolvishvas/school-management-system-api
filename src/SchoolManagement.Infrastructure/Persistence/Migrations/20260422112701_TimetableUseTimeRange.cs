using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TimetableUseTimeRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimetableEntries_Class_Section_DayOfWeek_HourNumber",
                table: "TimetableEntries");

            migrationBuilder.DropIndex(
                name: "IX_TimetableEntries_TeacherId_DayOfWeek_HourNumber",
                table: "TimetableEntries");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "TimetableEntries",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "TimetableEntries",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.Sql("""
                UPDATE [TimetableEntries]
                SET
                    [StartTime] = CAST(DATEADD(minute, ([HourNumber] - 1) * 60, CAST('07:30:00' AS datetime)) AS time),
                    [EndTime] = CAST(DATEADD(minute, [HourNumber] * 60, CAST('07:30:00' AS datetime)) AS time)
                WHERE [HourNumber] > 0;
                """);

            migrationBuilder.DropColumn(
                name: "HourNumber",
                table: "TimetableEntries");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntries_Class_Section_DayOfWeek_StartTime_EndTime",
                table: "TimetableEntries",
                columns: new[] { "Class", "Section", "DayOfWeek", "StartTime", "EndTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntries_TeacherId_DayOfWeek_StartTime_EndTime",
                table: "TimetableEntries",
                columns: new[] { "TeacherId", "DayOfWeek", "StartTime", "EndTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimetableEntries_Class_Section_DayOfWeek_StartTime_EndTime",
                table: "TimetableEntries");

            migrationBuilder.DropIndex(
                name: "IX_TimetableEntries_TeacherId_DayOfWeek_StartTime_EndTime",
                table: "TimetableEntries");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TimetableEntries");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TimetableEntries");

            migrationBuilder.AddColumn<int>(
                name: "HourNumber",
                table: "TimetableEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntries_Class_Section_DayOfWeek_HourNumber",
                table: "TimetableEntries",
                columns: new[] { "Class", "Section", "DayOfWeek", "HourNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntries_TeacherId_DayOfWeek_HourNumber",
                table: "TimetableEntries",
                columns: new[] { "TeacherId", "DayOfWeek", "HourNumber" },
                unique: true);
        }
    }
}
