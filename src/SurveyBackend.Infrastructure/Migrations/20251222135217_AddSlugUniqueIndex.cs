using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{

    public partial class AddSlugUniqueIndex : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Survey_Slug",
                table: "Survey",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Survey_Slug",
                table: "Survey");
        }
    }
}
