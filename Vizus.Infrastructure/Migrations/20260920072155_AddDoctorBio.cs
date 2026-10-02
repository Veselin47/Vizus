using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vizus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorBio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Doctors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Doctors");
        }
    }
}
