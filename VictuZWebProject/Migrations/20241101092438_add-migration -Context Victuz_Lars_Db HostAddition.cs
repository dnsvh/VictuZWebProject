using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations
{
    /// <inheritdoc />
    public partial class addmigrationContextVictuz_Lars_DbHostAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Activity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "Activity");
        }
    }
}
