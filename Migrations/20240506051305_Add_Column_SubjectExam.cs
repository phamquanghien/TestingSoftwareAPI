using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_SubjectExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterCandidatesAbsentStatus",
                table: "SubjectExams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchingTestScoreStatus",
                table: "SubjectExams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterCandidatesAbsentStatus",
                table: "SubjectExams");

            migrationBuilder.DropColumn(
                name: "MatchingTestScoreStatus",
                table: "SubjectExams");
        }
    }
}
