using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCH.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToStudentTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "dbo",
                table: "Teacher",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "dbo",
                table: "Student",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_UserId",
                schema: "dbo",
                table: "Teacher",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Student_UserId",
                schema: "dbo",
                table: "Student",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_User_UserId",
                schema: "dbo",
                table: "Student",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_User_UserId",
                schema: "dbo",
                table: "Teacher",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_User_UserId",
                schema: "dbo",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_User_UserId",
                schema: "dbo",
                table: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Teacher_UserId",
                schema: "dbo",
                table: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Student_UserId",
                schema: "dbo",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Student");
        }
    }
}
