using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionHistories",
                columns: table => new
                {
                    ActionHistoryID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Detail = table.Column<string>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionHistories", x => x.ActionHistoryID);
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExamCode = table.Column<string>(type: "TEXT", nullable: false),
                    ExamName = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatePerson = table.Column<string>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: true),
                    Status = table.Column<bool>(type: "INTEGER", nullable: true),
                    StartRegistrationCode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.ExamId);
                });

            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    ExamResultID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExamCodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ExamResult1 = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    ExamResult2 = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    AverageScore = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    ReviewScore = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExamBag = table.Column<int>(type: "INTEGER", nullable: false),
                    IsReview = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.ExamResultID);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    BirthDay = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentCode);
                });

            migrationBuilder.CreateTable(
                name: "SubjectExams",
                columns: table => new
                {
                    SubjectExamID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnterCandidatesAbsent = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserEnterCandidatesAbsent = table.Column<string>(type: "TEXT", nullable: false),
                    IsMatchingTestScore = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserMatchingTestScore = table.Column<string>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubjectName = table.Column<string>(type: "TEXT", nullable: false),
                    ExamBag = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDownloadFileDiem = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectExams", x => x.SubjectExamID);
                });

            migrationBuilder.CreateTable(
                name: "StudentExams",
                columns: table => new
                {
                    StudentExamID = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubjectCode = table.Column<string>(type: "TEXT", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectName = table.Column<string>(type: "TEXT", nullable: false),
                    TestDay = table.Column<string>(type: "TEXT", nullable: true),
                    TestRoom = table.Column<string>(type: "TEXT", nullable: true),
                    LessonStart = table.Column<string>(type: "TEXT", nullable: false),
                    LessonNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExamBag = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExams", x => x.StudentExamID);
                    table.ForeignKey(
                        name: "FK_StudentExams_Students_StudentCode",
                        column: x => x.StudentCode,
                        principalTable: "Students",
                        principalColumn: "StudentCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationCodes",
                columns: table => new
                {
                    RegistrationCodeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegistrationCodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentExamID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCodes", x => x.RegistrationCodeID);
                    table.ForeignKey(
                        name: "FK_RegistrationCodes_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationCodes_StudentExams_StudentExamID",
                        column: x => x.StudentExamID,
                        principalTable: "StudentExams",
                        principalColumn: "StudentExamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCodes_ExamId",
                table: "RegistrationCodes",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCodes_StudentExamID",
                table: "RegistrationCodes",
                column: "StudentExamID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExams_StudentCode",
                table: "StudentExams",
                column: "StudentCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionHistories");

            migrationBuilder.DropTable(
                name: "ExamResults");

            migrationBuilder.DropTable(
                name: "RegistrationCodes");

            migrationBuilder.DropTable(
                name: "SubjectExams");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "StudentExams");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
