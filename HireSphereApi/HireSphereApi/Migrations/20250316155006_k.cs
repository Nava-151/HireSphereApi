using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireSphereApi.Migrations
{
    /// <inheritdoc />
    public partial class k : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AIResponse");

            migrationBuilder.DropColumn(
                name: "Links",
                table: "AIResponse");

            migrationBuilder.DropColumn(
                name: "RemoteWork",
                table: "AIResponse");

            migrationBuilder.RenameColumn(
                name: "WorkPlace",
                table: "AIResponse",
                newName: "Education");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Education",
                table: "AIResponse",
                newName: "WorkPlace");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AIResponse",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Links",
                table: "AIResponse",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "RemoteWork",
                table: "AIResponse",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
