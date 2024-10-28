using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations.VictuZ_Lars_DbMigrations
{
    /// <inheritdoc />
    public partial class StoreUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Store");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Store",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Store");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Store",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
