using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations.VictuZ_Lars_DbMigrations
{
    /// <inheritdoc />
    public partial class StoreUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Store",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Store",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Store");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Store",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
