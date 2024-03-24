using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_Exam_IsAutoGenRegistrationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoGenRegistrationCode",
                table: "Exam",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoGenRegistrationCode",
                table: "Exam");
        }
    }
}
