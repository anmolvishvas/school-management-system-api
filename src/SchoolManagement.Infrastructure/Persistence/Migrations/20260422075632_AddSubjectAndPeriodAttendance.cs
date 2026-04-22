using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectAndPeriodAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeacherUserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_TeacherUserId",
                        column: x => x.TeacherUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodAttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherUserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    HourNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Section = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MarkedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodAttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodAttendanceRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PeriodAttendanceRecords_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PeriodAttendanceRecords_Users_MarkedByUserId",
                        column: x => x.MarkedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PeriodAttendanceRecords_Users_TeacherUserId",
                        column: x => x.TeacherUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAttendanceRecords_MarkedByUserId",
                table: "PeriodAttendanceRecords",
                column: "MarkedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAttendanceRecords_StudentId_Date_HourNumber_SubjectId",
                table: "PeriodAttendanceRecords",
                columns: new[] { "StudentId", "Date", "HourNumber", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAttendanceRecords_SubjectId",
                table: "PeriodAttendanceRecords",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAttendanceRecords_TeacherUserId",
                table: "PeriodAttendanceRecords",
                column: "TeacherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TeacherUserId",
                table: "Subjects",
                column: "TeacherUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodAttendanceRecords");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
