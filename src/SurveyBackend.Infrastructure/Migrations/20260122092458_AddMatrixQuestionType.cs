using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatrixQuestionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MatrixExplanationLabel",
                table: "Question",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixScale1Label",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixScale2Label",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixScale3Label",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixScale4Label",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatrixScale5Label",
                table: "Question",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MatrixShowExplanation",
                table: "Question",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "AnswerOption",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScaleValue",
                table: "AnswerOption",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatrixExplanationLabel",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixScale1Label",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixScale2Label",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixScale3Label",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixScale4Label",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixScale5Label",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MatrixShowExplanation",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "AnswerOption");

            migrationBuilder.DropColumn(
                name: "ScaleValue",
                table: "AnswerOption");
        }
    }
}
