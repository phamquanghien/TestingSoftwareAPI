using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    /// <inheritdoc />
    public partial class Create_Entity_Subject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "SubjectExams");

            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "StudentExams");

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectName = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectExams_SubjectCode",
                table: "SubjectExams",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExams_SubjectCode",
                table: "StudentExams",
                column: "SubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_SubjectCode",
                table: "ExamResults",
                column: "SubjectCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Subjects_SubjectCode",
                table: "ExamResults",
                column: "SubjectCode",
                principalTable: "Subjects",
                principalColumn: "SubjectCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentExams_Subjects_SubjectCode",
                table: "StudentExams",
                column: "SubjectCode",
                principalTable: "Subjects",
                principalColumn: "SubjectCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectExams_Subjects_SubjectCode",
                table: "SubjectExams",
                column: "SubjectCode",
                principalTable: "Subjects",
                principalColumn: "SubjectCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Subjects_SubjectCode",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentExams_Subjects_SubjectCode",
                table: "StudentExams");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectExams_Subjects_SubjectCode",
                table: "SubjectExams");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_SubjectExams_SubjectCode",
                table: "SubjectExams");

            migrationBuilder.DropIndex(
                name: "IX_StudentExams_SubjectCode",
                table: "StudentExams");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_SubjectCode",
                table: "ExamResults");

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "SubjectExams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "StudentExams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
