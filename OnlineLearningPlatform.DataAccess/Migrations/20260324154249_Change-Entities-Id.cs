using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearningPlatform.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntitiesId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentAnswerId",
                table: "StudentAnswers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Sessions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QuestionOptionId",
                table: "QuestionOptions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MaterialId",
                table: "Materials",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "Exams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExamAttemptId",
                table: "ExamAttempts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "Enrollments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Classrooms",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CheatLogId",
                table: "CheatLogs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AudioMediaId",
                table: "AudioMedias",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentAnswers",
                newName: "StudentAnswerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sessions",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Questions",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "QuestionOptions",
                newName: "QuestionOptionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Materials",
                newName: "MaterialId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Exams",
                newName: "ExamId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExamAttempts",
                newName: "ExamAttemptId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Enrollments",
                newName: "EnrollmentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Classrooms",
                newName: "ClassId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CheatLogs",
                newName: "CheatLogId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AudioMedias",
                newName: "AudioMediaId");
        }
    }
}
