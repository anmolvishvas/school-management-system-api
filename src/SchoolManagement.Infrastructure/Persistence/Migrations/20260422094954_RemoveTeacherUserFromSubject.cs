using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeacherUserFromSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Users_TeacherUserId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_TeacherUserId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "TeacherUserId",
                table: "Subjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherUserId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TeacherUserId",
                table: "Subjects",
                column: "TeacherUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Users_TeacherUserId",
                table: "Subjects",
                column: "TeacherUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
