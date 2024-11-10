using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations
{
    /// <inheritdoc />
    public partial class CheckoutModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckoutModelId",
                table: "ShoppingCartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckoutModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_CheckoutModelId",
                table: "ShoppingCartItems",
                column: "CheckoutModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_CheckoutModels_CheckoutModelId",
                table: "ShoppingCartItems",
                column: "CheckoutModelId",
                principalTable: "CheckoutModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_CheckoutModels_CheckoutModelId",
                table: "ShoppingCartItems");

            migrationBuilder.DropTable(
                name: "CheckoutModels");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_CheckoutModelId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "CheckoutModelId",
                table: "ShoppingCartItems");
        }
    }
}
