using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Column_ExamResult_ReviewScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReviewScore",
                table: "ExamResults",
                type: "TEXT",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReviewScore",
                table: "ExamResults",
                type: "TEXT",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 5,
                oldNullable: true);
        }
    }
}
